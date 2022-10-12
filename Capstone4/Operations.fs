module Capstone4.Operations

open System
open Capstone4.Utils
open Capstone4.Domain

let classifyAccount account =
    if account.Balance >= 0M then InCredit ^ CreditAccount account
    else Overdrawn account

/// Withdraws an amount of an account (if there are sufficient funds)
let withdraw amount (CreditAccount account) =
    { account with Balance = account.Balance - amount }
    |> classifyAccount

let withdrawSafe amount ratedAccount =
    match ratedAccount with
    | InCredit account -> account |> withdraw amount
    | Overdrawn _ ->
        printfn "Your account is overdrawn - withdrawal rejected"
        ratedAccount

/// Deposits an amount into an account
let deposit amount account =
    let account =
        match account with
        | Overdrawn account -> account
        | InCredit (CreditAccount account) -> account

    { account with Balance = account.Balance + amount }
    |> classifyAccount

/// Runs some account operation such as withdraw or deposit with auditing.
let auditAs operationName audit operation amount account accountId owner =
    let updatedAccount = operation amount account

    let transaction =
        { Operation = operationName
          Amount = amount
          Timestamp = DateTime.UtcNow }

    audit accountId owner.Name transaction
    updatedAccount

/// Creates an account from a historical set of transactions
let loadAccount (owner, accountId, transactions) =
    let openingAccount =
        classifyAccount
            { AccountId = accountId
              Balance = 0M
              Owner = { Name = owner } }


    transactions
    |> Seq.sortBy (fun txn -> txn.Timestamp)
    |> Seq.fold
        (fun account txn ->
            match txn.Operation, account with
            | Deposit, _ -> account |> deposit txn.Amount
            | Withdraw, InCredit account -> account |> withdraw txn.Amount
            | Withdraw, Overdrawn _ -> account)
        openingAccount
