module Capstone3.Auditing

open Capstone3.Operations
open Capstone3.Domain

/// Logs to the console
let printTransaction _ accountId transaction =
    printfn $"Account {accountId}: %s{transaction.Command} of %M{transaction.Amount} (approved: %b{transaction.Accepted})"

// Logs to both console and file system
let composedLogger = 
    let loggers =
        [ FileRepository.writeTransaction
          printTransaction ]
    fun accountId owner transaction ->
        loggers
        |> List.iter(fun logger -> logger accountId owner transaction)