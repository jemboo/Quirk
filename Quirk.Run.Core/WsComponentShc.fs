namespace Quirk.Run.Core

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Iter
open Quirk.Project
open Quirk.Workspace

module WsComponentTypeShc =

    let getWsComponentID 
            (quirkWorldLineId:Guid<quirkWorldLineId>)
            (generation:int<generation>)
            (wsComponentName:string<wsCompKey>)
        =
        [
            quirkWorldLineId :> obj
            generation :> obj
            wsComponentName :> obj
        ]
        |> GuidUtils.guidFromObjs
        |> UMX.tag<wsComponentId>


    let shcWsComponentNames =
        [|
            "SortableSet" |> UMX.tag<wsCompKey>
            "SorterSet" |> UMX.tag<wsCompKey>
            "SorterSetAncestry" |> UMX.tag<wsCompKey>
            "SorterSetConcatMap" |> UMX.tag<wsCompKey>
            "SorterSetEval" |> UMX.tag<wsCompKey>
            "SorterSetMutator" |> UMX.tag<wsCompKey>
            "SorterSetParentMap" |> UMX.tag<wsCompKey>
            "SorterSpeedBinSet" |> UMX.tag<wsCompKey>
            "SorterSetPruner" |> UMX.tag<wsCompKey>
            "WsParams" |> UMX.tag<wsCompKey>
        |]




type rndGenTypeShc =
    | Create
    | Mutate
    | Prune


module RndGenProviderShc =

    let getRngGen 
            (quirkWorldLineId:Guid<quirkWorldLineId>)
            (generation:int<generation>)
            (rngType:rngType)
        =
        let gu = 
            [
                quirkWorldLineId :> obj;
                generation :> obj;
                rngType :> obj
            ] 
            |> GuidUtils.guidFromObjs

        RngGen.fromGuid rngType gu

