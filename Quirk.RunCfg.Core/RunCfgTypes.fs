namespace Quirk.RunCfg.Core
open FSharp.UMX

[<Measure>] type replicaNumber
[<Measure>] type quirkRunId

[<Measure>] type runParamName

[<Measure>] type cfgPlexItemName
[<Measure>] type cfgPlexItemRank
[<Measure>] type cfgPlexName


type quirkRunType =
    | Shc
    | Ga


module QuirkRunType =

    let toString 
            (quirkRunType:quirkRunType)
        =
        match quirkRunType with
        | Shc -> "Shc"
        | Ga -> "Ga"


    let fromString 
            (qrt:string)
        =
        match qrt with
        | "Shc" -> quirkRunType.Shc |> Ok
        | "Ga" -> quirkRunType.Ga |> Ok
        | _ -> $"{qrt} not handled in QuirkRunType.fromString" |> Error