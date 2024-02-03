namespace Quirk.SortingResults

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open Quirk.SortingOps
open System.Text.RegularExpressions

[<Measure>] type noiseFraction
[<Measure>] type sorterPhenotypeCount
// 1.0 is neutral, higher numbers emphasize stageCount
[<Measure>] type stageWeight
[<Measure>] type sorterSetAncestryId
[<Measure>] type sorterSetPrunerId
[<Measure>] type sorterPhenotypeId
[<Measure>] type sorterSetEvalId
[<Measure>] type switchesUsed
[<Measure>] type sorterFitness
[<Measure>] type selectionFraction


module NoiseFraction =
    let toFloat (nf: float<noiseFraction> option) =
        match nf with
        | Some v ->  v |> UMX.untag
        | None -> 0.0


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


module SorterPhenotypeId =

    let createFromSwitches (switches: seq<switch>) =
        switches
        |> Seq.map (fun sw -> sw :> obj)
        |> GuidUtils.guidFromObjs
        |> UMX.tag<sorterPhenotypeId>



type sorterSpeed = private { switchCt:int<switchCount>; stageCt:int<stageCount> }

module SorterSpeed =
    let create 
            (switchCt: int<switchCount>) 
            (stageCt: int<stageCount>) 
        =
        { switchCt = switchCt
          stageCt = stageCt }

    let getSwitchCount (sorterSpeedBn: sorterSpeed) = sorterSpeedBn.switchCt

    let getStageCount (sorterSpeedBn: sorterSpeed) = sorterSpeedBn.stageCt

    let toIndex (sorterSpeedBn: sorterSpeed) =
        let switchCtV = sorterSpeedBn.switchCt |> UMX.untag
        let stageCtV = sorterSpeedBn.stageCt |> UMX.untag
        ((switchCtV * (switchCtV + 1)) / 2) + stageCtV

    let fromIndex (index: int) =
        let indexFlt = (index |> float) + 1.0
        let p = (sqrt (1.0 + 8.0 * indexFlt) - 1.0) / 2.0
        let pfloor = Math.Floor(p)

        if (p = pfloor) then
            let stageCt = 1 |> (-) (int pfloor) |> UMX.tag<stageCount>
            let switchCt = 1 |> (-) (int pfloor) |> UMX.tag<switchCount>
            { 
              sorterSpeed.switchCt = switchCt
              stageCt = stageCt 
            }
        else
            let stageCt =
                (float index) - (pfloor * (pfloor + 1.0)) / 2.0 |> int |> UMX.tag<stageCount>
            let switchCt = 
                (int pfloor) |> UMX.tag<switchCount>
            { 
                sorterSpeed.switchCt = switchCt
                stageCt = stageCt 
            }


    let fromSorterOpOutput (sorterOpOutpt: sorterOpOutput) =
        let sortr = sorterOpOutpt |> SorterOpOutput.getSorter
        try
            let switchUseCts = 
                   sorterOpOutpt
                   |> SorterOpOutput.getSorterOpTracker
                   |> SorterOpTracker.getSwitchUseCounts

            let switchesUsd =
                    switchUseCts
                    |> SwitchUseCounts.getUsedSwitchesFromSorter sortr
                
            let usedSwitchCt = switchesUsd.Length |> UMX.tag<switchCount>
            let usedStageCt = (switchesUsd |> StageCover.getStageCount)
            let sortrPhenotypId = switchesUsd |> SorterPhenotypeId.createFromSwitches
            (create usedSwitchCt usedStageCt, sortrPhenotypId, switchUseCts) |> Ok
        with ex ->
            (sprintf "error in SorterSpeed.fromSorterOpOutput: %s (*89)" ex.Message)
            |> Result.Error


    let modifyForPrefix
            (ordr:int<order>)
            (tcAdded:int<stageCount>)
            (ss:sorterSpeed) 
        =
        let wcNew = 
            tcAdded 
            |> StageCount.toSwitchCount ordr
            |> SwitchCount.add (getSwitchCount ss)

        let tcNew = 
            tcAdded
            |> StageCount.add (getStageCount ss)

        create wcNew tcNew


    let report (sorterSpd : sorterSpeed option) =
        match sorterSpd with
        | Some ss -> sprintf "%d\t%d"
                         (ss |> getStageCount |> UMX.untag)
                         (ss |> getSwitchCount |> UMX.untag)
        | None -> "-\t-"

    let getProps (sorterSpd : sorterSpeed option) =
        match sorterSpd with
        | Some ss -> ((ss |> getStageCount |> UMX.untag |> string),
                      (ss |> getSwitchCount |> UMX.untag |> string))
        | None -> ("", "")

    let getStageCount0 (sorterSpd : sorterSpeed option) =
        match sorterSpd with
        | Some ss -> (ss |> getStageCount |> UMX.untag)
        | None -> 0


    let getSwitchCount0 (sorterSpd : sorterSpeed option) =
        match sorterSpd with
        | Some ss -> (ss |> getSwitchCount |> UMX.untag)
        | None -> 0


module SorterFitness =
    let fromSpeed 
            (stageWght:float<stageWeight>) 
            (sorterSpd:sorterSpeed) 
        = 
        (stageWght |> UMX.untag) /
        (sorterSpd |> SorterSpeed.getStageCount |> UMX.untag |> float)
        +
        1.0 /
        (sorterSpd |> SorterSpeed.getSwitchCount |> UMX.untag |> float)
        |> UMX.tag<sorterFitness>


