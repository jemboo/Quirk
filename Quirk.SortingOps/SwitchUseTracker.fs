namespace Quirk.SortingOps

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open SysExt

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



type switchUseTrackerBitStriped = private { useFlags: uint64[] }

module SwitchUseTrackerBitStriped =

    let init (switchCount: int<switchCount>) =
        { switchUseTrackerBitStriped.useFlags = Array.zeroCreate<uint64> (switchCount |> UMX.untag) }

    let make (useFlags: uint64[]) = { useFlags = useFlags }

    let getUseFlags (switchUseTrackBitStriped: switchUseTrackerBitStriped) = switchUseTrackBitStriped.useFlags

    let getSwitchUseCounts (switchUseTrackBitStriped: switchUseTrackerBitStriped) =
        switchUseTrackBitStriped |> getUseFlags |> Array.map (fun l -> l.count |> int)
        |> SwitchUseCounts.make

    let getUsedSwitchCount (switchUseTrackBitStriped: switchUseTrackerBitStriped) =
        switchUseTrackBitStriped.useFlags
        |> Seq.filter ((<) 0uL)
        |> Seq.length
        |> UMX.tag<switchCount>



type switchUseTracker =
    | Standard of switchUseCounts
    | BitStriped of switchUseTrackerBitStriped

module SwitchUseTracker =

    let init 
            (switchOpMod: switchOpMode) 
            (switchCount: int<switchCount>) 
        =
        match switchOpMod with
        | switchOpMode.Standard -> switchCount |> SwitchUseCounts.init |> switchUseTracker.Standard
        | switchOpMode.BitStriped -> SwitchUseTrackerBitStriped.init switchCount |> switchUseTracker.BitStriped


    let getUseFlags (switchUses: switchUseTrackerBitStriped) = switchUses.useFlags

    let getSwitchUseCounts (switchUs: switchUseTracker) =
        match switchUs with
        | switchUseTracker.Standard switchUseTrackerStandard ->
            switchUseTrackerStandard
        | switchUseTracker.BitStriped switchUseTrackerBitStriped ->
            switchUseTrackerBitStriped |> SwitchUseTrackerBitStriped.getSwitchUseCounts


    let toSwitchOpMode (rollout: rollout) =
        match rollout with
        | B _uBRoll -> switchOpMode.Standard
        | U8 _uInt8Roll -> switchOpMode.Standard
        | U16 _uInt16Roll -> switchOpMode.Standard
        | I32 _intRoll -> switchOpMode.Standard
        | U64 _uInt64Roll -> switchOpMode.Standard
        | Bs64 _bs64Roll -> switchOpMode.BitStriped
