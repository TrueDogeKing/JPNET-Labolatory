namespace LibrarySystem.Core

open System

// Plik sygnatur: to jest JEDYNA publiczna powierzchnia biblioteki.
// Wszystko, czego tu nie ma (np. progi awansu, funkcje pomocnicze), jest ukryte.

module LibraryOps =
    val getLoanHistory : dir:string -> patronId:int -> Result<Loan list, string>
    val isPatronForLongerThan : years:int -> now:DateTime -> patron:Czytelnik -> bool
    val addFine : amount:decimal<PLN> -> patron:Czytelnik -> Result<Czytelnik, string>

[<Class>]
type Library =
    new : initial:Czytelnik list * dir:string -> Library
    member Patrons : Czytelnik list
    member Find : patronId:int -> Czytelnik option
    member Loans : patronId:int -> Result<Loan list, string>
    member AddFine : patronId:int * amount:decimal<PLN> -> Result<Czytelnik, string>
    member Promote : patronId:int -> Result<Czytelnik, string>
    member CanBorrow : patronId:int -> Result<unit, string>
