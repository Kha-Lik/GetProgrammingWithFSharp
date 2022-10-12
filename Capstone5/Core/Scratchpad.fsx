#r "nuget: Newtonsoft.Json"

#load "Domain.fs"
#load "Operations.fs"

open System
open System.Text.Json
open Capstone5.Operations
open Capstone5.Domain

let txn = { Timestamp = DateTime.Now
            Operation = "Withdraw"
            Amount = 10M }

let serialized = JsonSerializer.Serialize txn
let deserialized = JsonSerializer.Deserialize<Transaction> serialized
