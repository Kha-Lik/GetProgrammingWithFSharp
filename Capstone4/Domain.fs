namespace Capstone4.Domain

open System

type BankOperation =
    | Deposit
    | Withdraw

type Customer = { Name: string }

type Account =
    { AccountId: Guid
      Owner: Customer
      Balance: decimal }

type CreditAccount = CreditAccount of Account

type RatedAccount =
    | InCredit of CreditAccount
    | Overdrawn of Account
    member this.GetField getter =
        match this with
        | InCredit (CreditAccount account) -> getter account
        | Overdrawn account -> getter account

type Transaction =
    { Timestamp: DateTime
      Operation: BankOperation
      Amount: decimal }

module Transactions =
    /// Serializes a transaction
    let serialize transaction = $"{transaction.Timestamp}***%A{transaction.Operation}***%M{transaction.Amount}"

    let parseTransactionCommand command =
        match command with
        | "Deposit" -> Deposit
        | "Withdraw" -> Withdraw
        | _ -> failwith "Invalid command string"

    /// Deserializes a transaction
    let deserialize (fileContents: string) =
        let parts = fileContents.Split([| "***" |], StringSplitOptions.None)

        { Timestamp = DateTime.Parse parts.[0]
          Operation = parseTransactionCommand parts.[1]
          Amount = Decimal.Parse parts.[2] }
