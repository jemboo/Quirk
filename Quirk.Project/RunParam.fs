namespace Quirk.Project

open FSharp.UMX
open Quirk.Core


//type scriptModelType =
//    | Ga
//    | Shc


//module ScriptModelType =

//    let toString 
//            (scriptModelType:scriptModelType)
//        =
//        match scriptModelType with
//        | Ga -> "Ga"
//        | Shc -> $"Shc"

//    let fromString 
//            (qrm: string) 
//        =
//        match qrm.Split() with
//        | [| "Ga" |] -> scriptModelType.Ga |> Ok 
//        | [| "Shc" |] -> scriptModelType.Shc |> Ok
//        | _ -> Error $"{qrm} not handled in ScriptModelType.fromString"




type runParamSet =
    | Sim of simParamSet
    | Report of reportParamSet



type runParamSetType =
    | Sim
    | Report

module RunParamSetType =

    let toString 
            (scriptParamSetType:runParamSetType)
        =
        match scriptParamSetType with
        | Sim -> "Run"
        | Report -> $"Report"

    let fromString 
            (qrm: string) 
        =
        match qrm.Split() with
        | [| "Run" |] -> runParamSetType.Sim |> Ok 
        | [| "Report" |] -> runParamSetType.Report |> Ok
        | _ -> Error $"{qrm} not handled in RunParamSetType.fromString"

