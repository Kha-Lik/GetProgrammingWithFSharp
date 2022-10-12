namespace Capstone3.Domain

open System

type Customer = {
    Name: string
}

type Account = {
    ID: Guid
    Balance: decimal
    Customer: Customer
}

type Transaction = {
    Amount: decimal
    Command: string
    Timestamp: DateTime
    Accepted: bool
}

module TransactionOperations =
    /// Serializes a transaction
    let serialized transaction =
        $"{transaction.Timestamp}***%s{transaction.Command}***%M{transaction.Amount}***%b{transaction.Accepted}"
    
    /// Deserializes a transaction
    let deserialized (fileContents:string) =
        let parts = fileContents.Split([|"***"|], StringSplitOptions.None)
        { Timestamp = DateTime.Parse parts.[0]
          Command = parts.[1]
          Amount = Decimal.Parse parts.[2]
          Accepted = Boolean.Parse parts.[3] }