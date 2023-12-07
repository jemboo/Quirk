namespace Quirk.Cfg.Core
open FSharp.UMX

[<Measure>] type replicaNumber
[<Measure>] type cfgModelParamName
[<Measure>] type cfgRunParamName

[<Measure>] type cfgModelParamSetId
[<Measure>] type cfgRunParamSetId

[<Measure>] type cfgPlexItemName
[<Measure>] type cfgPlexItemRank
[<Measure>] type cfgPlexName


type quirkRunMode =
    | Sim
    | Report


module QuirkRunMode =

    let toString 
            (quirkRunMode:quirkRunMode)
        =
        match quirkRunMode with
        | Sim -> "Sim"
        | Report -> "Report"


    let fromString
            (qrm:string)
        =
        match qrm with
        | "Sim" -> quirkRunMode.Sim |> Ok
        | "Report" -> quirkRunMode.Report |> Ok
        | _ -> $"{qrm} not handled in QuirkRunMode.fromString" |> Error




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

