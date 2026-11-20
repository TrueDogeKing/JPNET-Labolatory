namespace LibrarySystem.Core

open System
open System.IO
open LibrarySystem.Core.Builders


module LibraryOps =

    let getLoanHistory (dir: string) (patronId: int) : Result<Loan list, string> =
        Data.loadLoans (Path.Combine(dir, $"loans_{patronId}.json")) patronId
        |> Result.map (List.sortByDescending (fun l -> l.LoanDate))

    let isPatronForLongerThan (years: int) (now: DateTime) (patron: Czytelnik) : bool =
        patron.DataRejestracji.AddYears years <= now

    let addFine (amount: decimal<PLN>) (patron: Czytelnik) : Result<Czytelnik, string> =
        if amount <= 0m<PLN> then
            Error "Kara musi być dodatnia"
        else
            Ok { patron with Kary = patron.Kary + amount }

type Library(initial: Czytelnik list, dir: string) =
    let premiumDeposit = 200m<PLN>
    let minDeposit = 50m<PLN>
    let maxFinesToBorrow = 20m<PLN>
    let premiumYears = 2
    let premiumMinLoans = 3

    let mutable patrons = initial

    member private _.Replace(updated: Czytelnik) =
        patrons <- patrons |> List.map (fun p -> if p.Id = updated.Id then updated else p)

    member _.Patrons = patrons

    member _.Find(patronId: int) : Czytelnik option =
        patrons |> List.tryFind (fun p -> p.Id = patronId)

    member _.Loans(patronId: int) : Result<Loan list, string> =
        LibraryOps.getLoanHistory dir patronId

    member this.AddFine(patronId: int, amount: decimal<PLN>) : Result<Czytelnik, string> =
        result {
            let! p = this.Find patronId |> optionToResult "Nie znaleziono czytelnika"
            let! updated = LibraryOps.addFine amount p
            this.Replace updated
            return updated
        }

    member this.Promote(patronId: int) : Result<Czytelnik, string> =
        result {
            let! p = this.Find patronId |> optionToResult "Nie znaleziono czytelnika"
            do! (if p.Czlonkostwo = Premium then Error "Czytelnik jest już Premium" else Ok())
            do!
                (if LibraryOps.isPatronForLongerThan premiumYears DateTime.Now p then Ok()
                 else Error $"Staż krótszy niż {premiumYears} lata")
            do!
                (if p.Kaucja >= premiumDeposit then Ok()
                 else Error $"Kaucja {decimal p.Kaucja:F2} PLN jest niższa niż wymagane {decimal premiumDeposit:F2} PLN")
            do! (if p.Kary = 0m<PLN> then Ok() else Error "Czytelnik ma nieuregulowane kary")
            let! loans = this.Loans patronId
            do!
                (if loans.Length >= premiumMinLoans then Ok()
                 else Error $"Za mało wypożyczeń ({loans.Length} z {premiumMinLoans})")
            let updated = { p with Czlonkostwo = Premium }
            this.Replace updated
            return updated
        }

    member this.CanBorrow(patronId: int) : Result<unit, string> =
        result {
            let! p = this.Find patronId |> optionToResult "Nie znaleziono czytelnika"
            do! (if p.Kaucja >= minDeposit then Ok() else Error "Kaucja poniżej minimum 50 PLN")
            do!
                (if p.Czlonkostwo = Premium || p.Kary <= maxFinesToBorrow then Ok()
                 else Error "Kary przekraczają 20 PLN")
            return ()
        }
