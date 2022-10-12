module Capstone2.Operations

open System
open Capstone2.Domain

let deposit amount account =
    {account with Balance = account.Balance + amount}
    
let withdraw amount account =
    if account.Balance < amount then account
    else {account with Balance = account.Balance - amount}
    
let auditAs operationName audit operation amount account =
    audit account $"{DateTime.Now}: Performing a %s{operationName} operation for £%M{amount}..."
    let updatedAccount = operation amount account
    
    let accountIsUnchanged = (updatedAccount = account)

    if accountIsUnchanged then audit account $"{DateTime.Now}: Transaction rejected!" 
    else audit account $"{DateTime.Now}: Transaction accepted! Balance is now £%M{updatedAccount.Balance}."

    updatedAccount
