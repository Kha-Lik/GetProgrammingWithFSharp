module Capstone4.FileRepository

open Capstone4.Utils
open Capstone4.Domain
open System.IO
open System

let private accountsPath =
    let path = @"accounts"
    Directory.CreateDirectory path |> ignore
    path

let private tryFindAccountFolder owner =
    let folders = Directory.EnumerateDirectories(accountsPath, $"%s{owner}_*")

    match List.ofSeq folders with
    | [] -> None
    | _ -> Some
            ( let folder = Seq.head folders
              DirectoryInfo(folder).Name )

let private buildPath (owner, accountId: Guid) = $@"%s{accountsPath}\%s{owner}_{accountId}"

let loadTransactions (folder: string) =
    let owner, accountId =
        let parts = folder.Split '_'
        parts.[0], Guid.Parse parts.[1]

    owner,
    accountId,
    buildPath (owner, accountId)
    |> Directory.EnumerateFiles
    |> Seq.map (File.ReadAllText >> Transactions.deserialize)

/// Finds all transactions from disk for specific owner.
let tryFindTransactionsOnDisk owner =
    let folder = tryFindAccountFolder owner

    match folder with
    | Some name -> Some ^ loadTransactions name
    | _ -> None

/// Logs to the file system
let writeTransaction accountId owner transaction =
    let path = buildPath (owner, accountId)
    path |> Directory.CreateDirectory |> ignore

    let filePath = $"%s{path}/%d{transaction.Timestamp.ToFileTimeUtc()}.txt"

    let line = $"{transaction.Timestamp}***%A{transaction.Operation}***%M{transaction.Amount}"

    File.WriteAllText(filePath, line)
