namespace Quirk.SortingOps

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open SysExt


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
