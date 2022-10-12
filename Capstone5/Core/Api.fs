/// Provides access to the banking API.
module Capstone5.Api

open Capstone5.Domain
open Capstone5.FileRepository
open Capstone5.Operations
open Capstone5.Auditing
open System

/// Loads an account from disk. If no account exists, an empty one is automatically created.
let LoadAccount (customer: Customer) : RatedAccount =
    let tryLoadAccount = tryFindTransactionsOnDisk >> Option.map loadAccount
    match tryLoadAccount customer.Name with
    | Some value -> value
    | None -> InCredit(
        CreditAccount
            { Owner = customer
              AccountId = Guid.NewGuid()
              Balance = 0M }
            )

/// Deposits funds into an account.
let Deposit (amount: decimal) (customer: Customer) : RatedAccount =
    let account = LoadAccount customer
    let accountId = account.GetField (fun a -> a.AccountId)
    let owner = account.GetField(fun a -> a.Owner)
    auditAs "deposit" composedLogger deposit amount account accountId owner

/// Withdraws funds from an account that is in credit.
let Withdraw (amount: decimal) (customer: Customer) : RatedAccount =
    let account = LoadAccount customer
    match account with
    | InCredit (CreditAccount account as creditAccount) -> auditAs "withdraw" composedLogger withdraw amount creditAccount account.AccountId account.Owner
    | account -> account

/// Loads the transaction history for an owner. If no transactions exist, returns an empty sequence.
let LoadTransactionHistory (customer: Customer) : Transaction seq =
    match tryFindTransactionsOnDisk customer.Name with
    | Some (_, _, txns) -> txns
    | None -> Seq.empty
