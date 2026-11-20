namespace LibrarySystem.Core

open System

[<Measure>] type PLN
[<Measure>] type EUR
[<Measure>] type USD

/// Unia wymuszająca, że istnieje telefon LUB adres (albo oba).
/// Stan "brak kontaktu" jest po prostu niereprezentowalny.
type Kontakt =
    | TylkoTelefon of telefon: string
    | TylkoAdres of adres: string
    | TelefonIAdres of telefon: string * adres: string

type DaneKontaktowe =
    { Imie: string
      Nazwisko: string
      Kontakt: Kontakt }

type Czlonkostwo =
    | Standard
    | Premium

type Czytelnik =
    { Id: int
      Dane: DaneKontaktowe
      Czlonkostwo: Czlonkostwo
      DataRejestracji: DateTime
      Kaucja: decimal<PLN>
      Kary: decimal<PLN> }

type Loan =
    { LoanId: int
      PatronId: int
      Title: string
      LoanDate: DateTime
      ReturnDate: DateTime option }

/// Kursy NBP są podawane jako "ile PLN za 1 walutę", stąd jednostki PLN/EUR i PLN/USD.
module Currency =
    let eurRate (r: decimal) : decimal<PLN/EUR> = r * 1.0m<PLN/EUR>
    let usdRate (r: decimal) : decimal<PLN/USD> = r * 1.0m<PLN/USD>

    let plnToEur (rate: decimal<PLN/EUR>) (x: decimal<PLN>) : decimal<EUR> = x / rate
    let plnToUsd (rate: decimal<PLN/USD>) (x: decimal<PLN>) : decimal<USD> = x / rate
    let eurToPln (rate: decimal<PLN/EUR>) (x: decimal<EUR>) : decimal<PLN> = x * rate
    let usdToPln (rate: decimal<PLN/USD>) (x: decimal<USD>) : decimal<PLN> = x * rate
