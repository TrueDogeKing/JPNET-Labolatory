module LibrarySystem.Core.Currencies

open System
open System.Net.Http
open System.Text.Json


let private client = new HttpClient(Timeout = TimeSpan.FromSeconds 15.0)

let fetchRate (code: string) : Async<Result<string * decimal, string>> =
    async {
        try
            let url = $"https://api.nbp.pl/api/exchangerates/rates/a/{code}/?format=json"
            let! json = client.GetStringAsync(url) |> Async.AwaitTask
            use doc = JsonDocument.Parse json
            let mid =
                doc.RootElement.GetProperty("rates").EnumerateArray()
                |> Seq.head
                |> fun r -> r.GetProperty("mid").GetDecimal()
            return Ok(code, mid)
        with ex ->
            return Error $"{code}: {ex.Message}"
    }

let fetchRates (codes: string list) : Async<Result<string * decimal, string>[]> =
    codes |> List.map fetchRate |> Async.Parallel

type CurrencyAnalyzer(rates: Map<string, decimal>) =
    member _.Rates = rates

    /// Kurs waluty w PLN albo None.
    member _.Rate(code: string) : decimal option = rates.TryFind code

    member _.ToPln(code: string, amount: decimal) : decimal option =
        rates.TryFind code |> Option.map (fun r -> r * amount)

    member _.MostExpensive() : string * decimal = rates |> Map.toList |> List.maxBy snd
    member _.Cheapest() : string * decimal = rates |> Map.toList |> List.minBy snd
    member _.Average() : decimal = rates |> Map.toSeq |> Seq.averageBy snd

    static member Load(codes: string list) : Async<Result<CurrencyAnalyzer, string>> =
        async {
            let! results = fetchRates codes
            let ok = results |> Array.choose (function Ok x -> Some x | Error _ -> None)
            let errs = results |> Array.choose (function Error e -> Some e | Ok _ -> None)
            if ok.Length = 0 then
                return Error(String.Join("; ", errs))
            else
                return Ok(CurrencyAnalyzer(Map.ofArray ok))
        }
