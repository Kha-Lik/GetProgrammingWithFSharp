#load "Domain.fs"
#load "Operations.fs"
#load "Utils.fs"
#load "FileRepository.fs"
#load "Auditing.fs"


open System
open Capstone4.Operations
open Capstone4.Domain
open Capstone4.Utils
open Capstone4.Auditing
open Capstone4.FileRepository

type Command =
    | AccountCommand of BankOperation
    | Exit

let withdrawWithAudit =
    auditAs Withdraw composedLogger withdraw

let depositWithAudit =
    auditAs Deposit composedLogger deposit

let loadAccountFromDisk =
    findTransactionsOnDisk >> loadAccount

let tryParseCommand commandChar =
    match commandChar with
    | 'd' -> Some ^ AccountCommand Deposit
    | 'w' -> Some ^ AccountCommand Withdraw
    | 'x' -> Some Exit
    | _ -> None

let tryGetBankOperation command =
    match command with
    | AccountCommand operation -> Some operation
    | Exit -> None

let commands =
    [ 'd'; 'w'; 'z'; 's'; 'e'; 'd'; 'x' ]

let getAmount command =
    Console.WriteLine()
    Console.Write "Enter Amount: "
    command, Console.ReadLine() |> Decimal.Parse

let processCommand account (command, amount) =
    printfn ""

    let account =
        match command with
        | Deposit -> account |> depositWithAudit amount
        | Withdraw -> account |> withdrawWithAudit amount

    printfn $"Current balance is £%M{account.Balance}"
    account

let openingAccount =
    { Owner = { Name = "Kolia" }
      AccountId = Guid.NewGuid()
      Balance = 25M }

let closingAccount =
    commands
    |> Seq.choose tryParseCommand
    |> Seq.takeWhile (fun c -> c = Exit)
    |> Seq.choose tryGetBankOperation
    |> Seq.map getAmount
    |> Seq.fold processCommand openingAccount
