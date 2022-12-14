module Capstone3.FileRepository

open System
open System.IO
open Capstone3.Domain.TransactionOperations

let private accountsPath =
    let path = @"accounts"
    Directory.CreateDirectory path |> ignore
    path
let private findAccountFolder owner =    
    let folders = Directory.EnumerateDirectories(accountsPath, $"%s{owner}_*")
    if Seq.isEmpty folders then ""
    else
        let folder = Seq.head folders
        DirectoryInfo(folder).Name
let private buildPath(owner, accountId:Guid) = $@"%s{accountsPath}\%s{owner}_{accountId}"
let loadTransactions (folder:string) =
    let owner, accountId =
        let parts = folder.Split '_'
        parts.[0], Guid.Parse parts.[1]
    owner, accountId, buildPath(owner, accountId)
                      |> Directory.EnumerateFiles
                      |> Seq.map (File.ReadAllText >> deserialized)

/// Finds all transactions from disk for specific owner.
let findTransactionsOnDisk owner =
    let folder = findAccountFolder owner
    if String.IsNullOrEmpty folder then owner, Guid.NewGuid(), Seq.empty
    else loadTransactions folder

/// Logs to the file system
let writeTransaction accountId owner transaction =
    let path = buildPath(owner, accountId)    
    path |> Directory.CreateDirectory |> ignore
    let filePath = $"%s{path}/%d{DateTime.UtcNow.ToFileTimeUtc()}.txt"
    File.WriteAllText(filePath, serialized transaction)