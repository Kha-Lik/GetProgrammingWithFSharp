open System
open Capstone2.Domain
open Capstone2.Auditing
open Capstone2.Operations

printfn "Enter your name"
let name = Console.ReadLine()

printfn "Enter your age"
let age = Int32.Parse ^ Console.ReadLine()

printfn "Enter opening balance"
let balance = Decimal.Parse ^ Console.ReadLine()
    
let withdrawWithConsoleAudit = auditAs "Withdraw" console withdraw
let depositWithConsoleAudit = auditAs "Deposit" console deposit

let mutable account = {
    ID = Guid.NewGuid()
    Balance = balance
    Customer = {
        Name = name
        Age =  age
    }
}

while true do
    printfn "Action (x to close):"
    let action = Console.ReadLine()
    if action = "x" then Environment.Exit 0
    printfn "Amount:"
    let amount = Decimal.Parse ^ Console.ReadLine()
    account <-
        match action with
        | "deposit" ->  account |> depositWithConsoleAudit amount
        | "withdraw" ->  account |> withdrawWithConsoleAudit amount
        | _ -> account