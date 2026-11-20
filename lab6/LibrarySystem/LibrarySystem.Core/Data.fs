module LibrarySystem.Core.Data

open System
open System.IO
open System.Text.Json
open LibrarySystem.Core.Builders


/// Zwraca Some tylko dla niepustego stringa; null/brak pola/pusty -> None.
let private optString (el: JsonElement) (name: string) : string option =
    match el.TryGetProperty(name) with
    | true, v when v.ValueKind = JsonValueKind.String ->
        let s = v.GetString()
        if String.IsNullOrWhiteSpace s then None else Some s
    | _ -> None

let private validatePhone (p: string) : Result<string, string> =
    if p.Length = 9 && Seq.forall Char.IsDigit p then Ok p
    else Error $"Niepoprawny telefon '{p}' (wymagane 9 cyfr)"

/// Składa unię Kontakt - brak obu pól to błąd walidacji.
let private validateContact (phone: string option) (address: string option) : Result<Kontakt, string> =
    match phone, address with
    | Some p, Some a -> Ok(TelefonIAdres(p, a))
    | Some p, None -> Ok(TylkoTelefon p)
    | None, Some a -> Ok(TylkoAdres a)
    | None, None -> Error "Wymagany telefon lub adres"

let private parsePatron (el: JsonElement) : Result<Czytelnik, string> =
    result {
        let pid = el.GetProperty("id").GetInt32()
        let! phone =
            match optString el "phone" with
            | Some p -> validatePhone p |> Result.map Some
            | None -> Ok None
        let! kontakt = validateContact phone (optString el "address")
        return
            { Id = pid
              Dane =
                { Imie = el.GetProperty("firstName").GetString()
                  Nazwisko = el.GetProperty("lastName").GetString()
                  Kontakt = kontakt }
              Czlonkostwo = if el.GetProperty("premium").GetBoolean() then Premium else Standard
              DataRejestracji = DateTime.Parse(el.GetProperty("registered").GetString())
              Kaucja = el.GetProperty("deposit").GetDecimal() * 1.0m<PLN>
              Kary = el.GetProperty("fines").GetDecimal() * 1.0m<PLN> }
    }

/// Wczytuje patrons.json. Zwraca (poprawni czytelnicy, komunikaty o odrzuconych).
let loadPatrons (path: string) : Result<Czytelnik list * string list, string> =
    try
        use doc = JsonDocument.Parse(File.ReadAllText path)
        let parsed =
            doc.RootElement.EnumerateArray()
            |> Seq.mapi (fun i el ->
                try
                    parsePatron el |> Result.mapError (fun e -> $"Rekord #{i + 1}: {e}")
                with ex ->
                    Error $"Rekord #{i + 1}: {ex.Message}")
            |> List.ofSeq
        let good = parsed |> List.choose (function Ok p -> Some p | Error _ -> None)
        let bad = parsed |> List.choose (function Error e -> Some e | Ok _ -> None)
        Ok(good, bad)
    with ex ->
        Error $"Nie udało się wczytać {path}: {ex.Message}"

/// Wczytuje loans_<id>.json. Brak pliku = brak wypożyczeń.
let loadLoans (path: string) (patronId: int) : Result<Loan list, string> =
    if not (File.Exists path) then
        Ok []
    else
        try
            use doc = JsonDocument.Parse(File.ReadAllText path)
            let loans =
                doc.RootElement.EnumerateArray()
                |> Seq.map (fun el ->
                    { LoanId = el.GetProperty("loanId").GetInt32()
                      PatronId = patronId
                      Title = el.GetProperty("title").GetString()
                      LoanDate = DateTime.Parse(el.GetProperty("loanDate").GetString())
                      ReturnDate = optString el "returnDate" |> Option.map DateTime.Parse })
                |> List.ofSeq
            Ok loans
        with ex ->
            Error $"Nie udało się wczytać {path}: {ex.Message}"
