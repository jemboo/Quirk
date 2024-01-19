namespace Quirk.Core
open FSharp.UMX
open System.Text.RegularExpressions

[<Measure>] type noiseFraction
[<Measure>] type sorterPhenotypeCount
[<Measure>] type stageWeight
[<Measure>] type sorterSetPrunerId


type sorterEvalMode = 
    | DontCheckSuccess
    | CheckSuccess 
    | GetSortedSetCount

module SorterEvalMode =
    let fromString (sv: string) =
        match sv.ToLowerInvariant() with
        | "DontCheckSuccess" -> Ok DontCheckSuccess
        | "CheckSuccess" -> Ok CheckSuccess
        | "GetSortedSetCount" -> Ok GetSortedSetCount
        | _ -> Error "Invalid sorterEvalMode string"

    let toString (mode: sorterEvalMode) =
        match mode with
        | DontCheckSuccess -> "DontCheckSuccess"
        | CheckSuccess -> "CheckSuccess"
        | GetSortedSetCount -> "GetSortedSetCount"


type sorterSetPruneMethod = 
    | Whole
    | PhenotypeCap of int<sorterPhenotypeCount>
    | Shc


module SorterSetPruneMethod =

    let toReport (sspm:sorterSetPruneMethod) 
        =
        match sspm with
        | Whole -> "Whole"
        | PhenotypeCap sspc -> $"PhenotypeCap({sspc |> UMX.untag})"
        | Shc -> "Shc"


    let extractWordAndNumber (input: string) =
        let pattern = @"(\w+)\((\d+)\)"
        let matchResult = Regex.Match(input, pattern)
        if matchResult.Success then
            let word = matchResult.Groups.[1].Value
            let number = matchResult.Groups.[2].Value
            Some(word, int number)
        else
            None


    let fromReport (repVal:string)
        =
         match repVal with
         | "Whole" -> sorterSetPruneMethod.Whole |> Ok
         | "Shc" -> sorterSetPruneMethod.Shc |> Ok
         | _ -> 
            let er = extractWordAndNumber repVal
            match er with
            | None -> $"{repVal} not valid in SorterSetPruneMethod.fromReport" |> Error
            | Some (w, n) ->
                match w with
                | "PhenotypeCap" ->
                    n |> UMX.tag<sorterPhenotypeCount>
                      |> sorterSetPruneMethod.PhenotypeCap |> Ok
                | _ ->
                    $"{repVal} not valid in SorterSetPruneMethod.fromReport" |> Error



