open System
open Capstone3.Domain
open Capstone3.Auditing
open Capstone3.Operations
open Capstone3.FileRepository

let (^) f x = f x
    
let withdrawWithAudit = auditAs "withdraw" composedLogger withdraw
let depositWithAudit = auditAs "deposit" composedLogger deposit
let loadAccountFromDisk = findTransactionsOnDisk >> loadAccount
let isValidCommand command =
    let validCommands = ['d'; 'w'; 'x']
    List.contains command validCommands
let isStopCommand command = command = 'x'
let getAmount command =
    let prompt =
        Console.Write "\nEnter amount:"
        Console.ReadLine() |> decimal
    match command with
    | 'd' -> 'd', prompt
    | 'w' -> 'w', prompt
    | _ -> 'x', 0M
let processCommand account (command, amount) =
    match command with
    | 'd' -> account |> depositWithAudit amount
    | 'w' -> account |> withdrawWithAudit amount
    | _ -> account



[<EntryPoint>]
let main _ =
    let openingAccount =
        Console.Write "Please enter your name: "
        Console.ReadLine() |> loadAccountFromDisk
    
    printfn $"Current balance is £%M{openingAccount.Balance}"
    
    let consoleCommands =
        seq {
            while true do
                Console.Write "\n(d)eposit, (w)ithdraw or e(x)it:"
                yield Console.ReadKey().KeyChar
        }

    let processCommand account (command, amount) =
        printfn ""
        let account =
            if command = 'd' then account |> depositWithAudit amount
            else account |> withdrawWithAudit amount
        printfn $"Current balance is £%M{account.Balance}"
        account

    let closingAccount =
        consoleCommands
        |> Seq.filter isValidCommand
        |> Seq.takeWhile (not << isStopCommand)
        |> Seq.map getAmount
        |> Seq.fold processCommand openingAccount
    
    printfn ""
    printfn $"Closing Balance:\r\n %A{closingAccount}"
    Console.ReadKey() |> ignore

    0