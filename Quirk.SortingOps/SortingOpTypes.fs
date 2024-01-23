namespace Quirk.SortingOps

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open SysExt


type sorterOpTrackMode =
    | SwitchUses
    | SwitchTrack


type switchOpMode =
    | Standard
    | BitStriped



type switchUseCounts = private { useCounts: int[] }

module SwitchUseCounts =

    let getUseCounters (switchUses: switchUseCounts) = switchUses.useCounts

    let init (switchCount: int<switchCount>) =
        { switchUseCounts.useCounts = Array.zeroCreate<int> (switchCount |> UMX.untag) }

    let make (useCounts: int[]) = { useCounts = useCounts }

    let getUsedSwitchCount (switchUseTrackStandard: switchUseCounts) =
        switchUseTrackStandard.useCounts
        |> Seq.filter ((<) 0)
        |> Seq.length
        |> UMX.tag<switchCount>


    let getUsedSwitchesFromSorter 
            (sortr: sorter) 
            (switchUseCnts: switchUseCounts) =
        switchUseCnts
        |> getUseCounters
        |> Seq.mapi (fun i w -> i, w)
        |> Seq.filter (fun t -> (snd t) > 0)
        |> Seq.map (fun t -> (sortr |> Sorter.getSwitches).[(fst t)])
        |> Seq.toArray
        
    let ofOption (switchUseCts: switchUseCounts option) =
        match switchUseCts with
        | Some suc -> suc |> getUseCounters
        | None -> [||]

