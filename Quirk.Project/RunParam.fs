﻿namespace Quirk.Project

open FSharp.UMX
open Quirk.Core


type runParamSet =
    | Sim of simParamSet
    | Report of reportParamSet


module RunParamSet =

    let toSimParamSet 
            (runParamSet:runParamSet)
        =
        match runParamSet with
        | Sim sps -> sps |> Ok
        | Report rps -> "runParamSet.Report given, Sim expected (*68)" |> Error

    let toReportParamSet 
            (runParamSet:runParamSet)
        =
        match runParamSet with
        | Sim sps -> "runParamSet.Sim given, Sim Report (*69)" |> Error
        | Report rps -> rps |> Ok



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
        | _ -> Error $"{qrm} not handled in RunParamSetType.fromString (*70)"

