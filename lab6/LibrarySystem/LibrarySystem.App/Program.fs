module Program

open System
open System.Globalization
open System.IO
open LibrarySystem.Core
open LibrarySystem.Core.Builders
open LibrarySystem.Core.Currencies


let dataDir = Path.Combine(AppContext.BaseDirectory, "data")

let mutable analyzer: CurrencyAnalyzer option = None

let pln (x: decimal<PLN>) : string =
    let v = decimal x
    $"{v:F2} PLN"

let readInt (prompt: string) : int option =
    printf "%s" prompt
    match Int32.TryParse(Console.ReadLine()) with
    | true, v -> Some v
    | _ -> None

let readMoney (prompt: string) : decimal<PLN> option =
    printf "%s" prompt
    match Decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture) with
    | true, v -> Some(v * 1.0m<PLN>)
    | _ -> None

let showResult (r: Result<string, string>) =
    match r with
    | Ok msg -> printfn "OK: %s" msg
    | Error e -> printfn "Błąd: %s" e

let describePatron (p: Czytelnik) : string =
    let kontakt =
        match p.Dane.Kontakt with
        | TylkoTelefon t -> $"tel. {t}"
        | TylkoAdres a -> $"adres {a}"
        | TelefonIAdres(t, a) -> $"tel. {t}, adres {a}"
    let od = p.DataRejestracji.ToString("yyyy-MM-dd")
    $"#{p.Id} {p.Dane.Imie} {p.Dane.Nazwisko} [{p.Czlonkostwo}] od {od}, kaucja {pln p.Kaucja}, kary {pln p.Kary}, {kontakt}"

let listPatrons (lib: Library) =
    lib.Patrons |> List.iter (describePatron >> printfn "%s")

let showLoans (lib: Library) =
    match readInt "Id czytelnika: " with
    | None -> printfn "Niepoprawne id"
    | Some id ->
        match lib.Loans id with
        | Error e -> printfn "Błąd: %s" e
        | Ok [] -> printfn "Brak wypożyczeń"
        | Ok loans ->
            for l in loans do
                let ret =
                    match l.ReturnDate with
                    | Some d -> "zwrot " + d.ToString("yyyy-MM-dd")
                    | None -> "NIEZWRÓCONA"
                printfn "  %s (%s, %s)" l.Title (l.LoanDate.ToString("yyyy-MM-dd")) ret

let addFine (lib: Library) =
    match readInt "Id czytelnika: ", readMoney "Kwota kary (PLN): " with
    | Some id, Some amount ->
        lib.AddFine(id, amount)
        |> Result.map (fun p -> $"Kary czytelnika #{p.Id}: {pln p.Kary}")
        |> showResult
    | _ -> printfn "Niepoprawne dane"

let promote (lib: Library) =
    match readInt "Id czytelnika: " with
    | Some id ->
        lib.Promote id
        |> Result.map (fun p -> $"{p.Dane.Imie} {p.Dane.Nazwisko} jest teraz {p.Czlonkostwo}")
        |> showResult
    | None -> printfn "Niepoprawne id"

let checkBorrow (lib: Library) =
    match readInt "Id czytelnika: " with
    | Some id -> lib.CanBorrow id |> Result.map (fun () -> "Może wypożyczać") |> showResult
    | None -> printfn "Niepoprawne id"

let fetchRatesMenu () =
    printfn "Pobieram kursy z NBP (równolegle)..."
    let codes = [ "EUR"; "USD"; "GBP"; "CHF"; "CZK"; "JPY" ]
    match CurrencyAnalyzer.Load codes |> Async.RunSynchronously with
    | Error e -> printfn "Błąd: %s" e
    | Ok a ->
        analyzer <- Some a
        for KeyValue(code, rate) in a.Rates do
            printfn "  1 %s = %M PLN" code rate
        let (maxC, maxR) = a.MostExpensive()
        let (minC, minR) = a.Cheapest()
        let avg = a.Average()
        printfn "Najdroższa: %s (%M), najtańsza: %s (%M), średni kurs: %M" maxC maxR minC minR avg

let finesInForeign (lib: Library) (patronId: int) : string option =
    maybe {
        let! a = analyzer
        let! p = lib.Find patronId
        let! eur = a.Rate("EUR")
        let! usd = a.Rate("USD")
        let e = Currency.plnToEur (Currency.eurRate eur) p.Kary
        let u = Currency.plnToUsd (Currency.usdRate usd) p.Kary
        let ev = decimal e
        let uv = decimal u
        return $"Kary {pln p.Kary} = {ev:F2} EUR = {uv:F2} USD"
    }

let finesMenu (lib: Library) =
    match readInt "Id czytelnika: " with
    | None -> printfn "Niepoprawne id"
    | Some id ->
        match finesInForeign lib id with
        | Some s -> printfn "%s" s
        | None -> printfn "Brak danych: pobierz najpierw kursy (opcja 6) lub podaj istniejące id"

let printMenu () =
    printfn ""
    printfn "=== System biblioteczny ==="
    printfn "1. Lista czytelników"
    printfn "2. Historia wypożyczeń"
    printfn "3. Dodaj karę"
    printfn "4. Awans na Premium"
    printfn "5. Sprawdź możliwość wypożyczenia"
    printfn "6. Pobierz kursy NBP"
    printfn "7. Kary w EUR/USD"
    printfn "0. Wyjście"
    printf "Wybór: "

let rec loop (lib: Library) =
    printMenu ()
    match Console.ReadLine() with
    | null
    | "0" -> printfn "Do widzenia"
    | "1" -> listPatrons lib; loop lib
    | "2" -> showLoans lib; loop lib
    | "3" -> addFine lib; loop lib
    | "4" -> promote lib; loop lib
    | "5" -> checkBorrow lib; loop lib
    | "6" -> fetchRatesMenu (); loop lib
    | "7" -> finesMenu lib; loop lib
    | _ -> printfn "Nieznana opcja"; loop lib

[<EntryPoint>]
let main _ =
    match Data.loadPatrons (Path.Combine(dataDir, "patrons.json")) with
    | Error e ->
        printfn "Błąd krytyczny: %s" e
        1
    | Ok(patrons, rejected) ->
        for r in rejected do
            printfn "Odrzucono: %s" r
        loop (Library(patrons, dataDir))
        0
