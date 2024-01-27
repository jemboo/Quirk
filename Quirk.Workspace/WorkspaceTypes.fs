namespace Quirk.Workspace
open System
open FSharp.UMX
open Quirk.Core

[<Measure>] type wsId

[<Measure>] type wsComponentName

[<Measure>] type wsComponentId

[<Measure>] type wsParamsId

[<Measure>] type wsParamsKey


type wsComponentType =
    | SortableSet = 10
    | SorterSet = 20
    | SorterSetAncestry = 21
    | SorterSetConcatMap = 22
    | SorterSetEval = 23
    | SorterSetMutator = 24
    | SorterSetParentMap = 25
    | SorterSpeedBinSet = 30
    | SorterSetPruner = 40
    | WsParams = 50


module WsComponentType =

    let toString (wsComponentType:wsComponentType)
        =
        wsComponentType |> string

    let fromString (cereal:string)
        =
       match cereal with
       | "SortableSet" -> wsComponentType.SortableSet |> Ok
       | "SorterSet" -> wsComponentType.SorterSet |> Ok
       | "SorterSetAncestry" -> wsComponentType.SorterSetAncestry |> Ok
       | "SorterSetConcatMap" -> wsComponentType.SorterSetConcatMap |> Ok
       | "SorterSetEval" -> wsComponentType.SorterSetEval |> Ok
       | "SorterSetMutator" -> wsComponentType.SorterSetMutator |> Ok
       | "SorterSetParentMap" -> wsComponentType.SorterSetParentMap |> Ok
       | "SorterSpeedBinSet" -> wsComponentType.SorterSpeedBinSet |> Ok
       | "SorterSetPruner" -> wsComponentType.SorterSetPruner |> Ok
       | "WsParams" -> wsComponentType.WsParams |> Ok
       | a -> $"{a} is not handled in WsComponentType.fromString" |> Error