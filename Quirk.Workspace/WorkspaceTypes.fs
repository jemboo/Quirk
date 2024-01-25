namespace Quirk.Workspace
open System
open FSharp.UMX
open Quirk.Core

[<Measure>] type workspaceId

[<Measure>] type workspaceParamsId

[<Measure>] type workspaceParamsKey

[<Measure>] type wsComponentName


type workspaceComponentType =
    | WorkspaceDescription = 0
    | SortableSet = 10
    | SorterSet = 20
    | SorterSetAncestry = 21
    | SorterSetConcatMap = 22
    | SorterSetEval = 23
    | SorterSetMutator = 24
    | SorterSetParentMap = 25
    | SorterSpeedBinSet = 30
    | SorterSetPruner = 40
    | WorkspaceParams = 50



module WsConstants =

    let  workSpaceComponentNameForParams = "workspaceParams" |> UMX.tag<wsComponentName>