namespace Quirk.SortingOps

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open SysExt

type sorterOpTracker =
    | SwitchUses of sortableBySwitchTracker
    | SwitchTrack of switchUseTracker


module SorterOpTracker =

    let getSwitchUseCounts (sorterOpTracker: sorterOpTracker) =
        match sorterOpTracker with
        | sorterOpTracker.SwitchUses sortableBySwitchTracker ->
            sortableBySwitchTracker |> SortableBySwitchTracker.getSwitchUseCounts
        | sorterOpTracker.SwitchTrack switchUseTracker ->
            switchUseTracker 
               |> SwitchUseTracker.getSwitchUseCounts
