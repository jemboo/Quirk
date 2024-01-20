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
