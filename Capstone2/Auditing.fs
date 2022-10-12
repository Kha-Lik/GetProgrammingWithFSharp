module Capstone2.Auditing

open System
open System.IO
open Capstone2.Domain

let (^) f x = f x

let fileSystemAudit account message =
    let dir = Directory.CreateDirectory $"C:\\temp\\learnfs\\capstone2\\%s{account.Customer.Name}"
    let filename = $"%s{account.ID.ToString()}.txt"
    let filePath = $"%s{dir.FullName}\\%s{filename}"
    if (not ^ File.Exists filePath) then (File.Create filePath).Close()
    File.AppendAllText(filePath, $"%s{message}\n")
    
let console account message =
    Console.WriteLine $"Account %O{account.ID}: %s{message}"
