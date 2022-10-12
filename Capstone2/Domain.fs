namespace Capstone2.Domain

open System

type Customer = {
    Name: string
    Age: int
}

type Account = {
    ID: Guid
    Balance: decimal
    Customer: Customer
}