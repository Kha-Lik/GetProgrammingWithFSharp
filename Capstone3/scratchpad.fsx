#load "Domain.fs"
#load "Auditing.fs"

open System
open Capstone3.Domain
open Capstone3.Auditing

let isValidCommand command =
    let validCommands = ['d'; 'w'; 'x']
    List.contains command validCommands
let isStopCommand command = command = 'x'
let getAmount command =
    match command with
    | 'd' -> 'd', 50M
    | 'w' -> 'w', 25M
    | _ -> 'x', 0M
let processCommand (account) (command, amount) =
    match command with
    | 'd' -> {account with Balance = account.Balance + amount}
    | 'w' -> {account with Balance = account.Balance - amount}
    | _ -> account

let openingAccount = {Customer = {Name = "Isaac"}; Balance = 0M; ID = Guid.Empty}
let account =
    let commands = ['d'; 'w'; 'z'; 'f'; 'd'; 'x'; 'w']
    commands
    |> Seq.filter isValidCommand
    |> Seq.takeWhile (not << isStopCommand)
    |> Seq.map getAmount
    |> Seq.fold processCommand openingAccount