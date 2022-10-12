module Capstone5.Auditing

open Capstone5.Domain

/// Logs to the console
let printTransaction _ accountId transaction =
    printfn $"Account {accountId}: %s{transaction.Operation} of %M{transaction.Amount}"

// Logs to both console and file system
let composedLogger =
    let loggers =
        [ FileRepository.writeTransaction
          printTransaction ]

    fun accountId owner transaction ->
        loggers
        |> List.iter (fun logger -> logger accountId owner transaction)
