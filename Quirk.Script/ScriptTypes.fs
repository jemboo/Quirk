namespace Quirk.Script
open FSharp.UMX


[<Measure>] type reportName

type quirkScriptMode =
    | Sim
    | Report of string<reportName>


module QuirkScriptMode =

    let toString 
            (quirkProgramMode:quirkScriptMode)
        =
        match quirkProgramMode with
        | Sim -> "Sim"
        | Report rn -> $"Report {rn |> UMX.untag}"

    let fromString (qrm: string) =
        match qrm.Split() with
        | [| "Sim" |] -> quirkScriptMode.Sim |> Ok 
        | [| "Report"; rn |] -> rn |> UMX.tag<reportName> |> quirkScriptMode.Report |> Ok
        | _ -> Error $"{qrm} not handled in QuirkScriptMode.fromString"

