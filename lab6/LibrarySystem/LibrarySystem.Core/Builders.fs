module LibrarySystem.Core.Builders

/// result { } - pierwszy Error przerywa obliczenie (Railway Oriented Programming).
type ResultBuilder() =
    member _.Bind(m: Result<'a, 'e>, f: 'a -> Result<'b, 'e>) : Result<'b, 'e> =
        match m with
        | Ok x -> f x
        | Error e -> Error e

    member _.Return(x: 'a) : Result<'a, 'e> = Ok x
    member _.ReturnFrom(m: Result<'a, 'e>) : Result<'a, 'e> = m

/// maybe { } - pierwszy None przerywa obliczenie.
type MaybeBuilder() =
    member _.Bind(m: 'a option, f: 'a -> 'b option) : 'b option =
        match m with
        | Some x -> f x
        | None -> None

    member _.Return(x: 'a) : 'a option = Some x
    member _.ReturnFrom(m: 'a option) : 'a option = m

let result = ResultBuilder()
let maybe = MaybeBuilder()

/// Most Option -> Result: dokłada komunikat błędu dla None.
let optionToResult (err: 'e) (o: 'a option) : Result<'a, 'e> =
    match o with
    | Some x -> Ok x
    | None -> Error err
