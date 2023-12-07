namespace Quirk.Run.Shc

open System
open Quirk.Core
open Quirk.Run.Core
open FSharp.UMX

[<Measure>] type shcComponentId

type workspaceComponentTypeShc =
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



module WorkspaceComponentTypeShc =

    let getShcComponentID 
            (quirkRunId:Guid<quirkRunId>)
            (generation:int<generation>)
            (workspaceComponentTypeShc:workspaceComponentTypeShc)
        =
        [
            quirkRunId :> obj
            generation :> obj
            workspaceComponentTypeShc :> obj
        ] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<shcComponentId>



type rndGenTypeShc =
    | Create
    | Mutate
    | Prune



module RndGenProvider =

    let getRngGen 
            (rngType:rngType)
            (rndGenTypeShc:rndGenTypeShc)
            (generation:int<generation>) 
        =
        let gu = 
            [
                rndGenTypeShc :> obj;
                generation :> obj
            ] 
            |> GuidUtils.guidFromObjs

        RngGen.fromGuid rngType gu


    let getShcWorkspaceComponentId
            (quirkRunId: Guid<quirkRunId>)
            (wsc:workspaceComponentTypeShc)
            (generation:int<generation>) 
        =
        [
            quirkRunId :> obj;
            wsc :> obj;
            generation :> obj
        ] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<workspaceComponentId>
