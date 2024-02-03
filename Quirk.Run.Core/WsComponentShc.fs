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
            (wsComponentName:string<wsComponentName>)
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
            "SortableSet" |> UMX.tag<wsComponentName>
            "SorterSet" |> UMX.tag<wsComponentName>
            "SorterSetAncestry" |> UMX.tag<wsComponentName>
            "SorterSetConcatMap" |> UMX.tag<wsComponentName>
            "SorterSetEval" |> UMX.tag<wsComponentName>
            "SorterSetMutator" |> UMX.tag<wsComponentName>
            "SorterSetParentMap" |> UMX.tag<wsComponentName>
            "SorterSpeedBinSet" |> UMX.tag<wsComponentName>
            "SorterSetPruner" |> UMX.tag<wsComponentName>
            "WsParams" |> UMX.tag<wsComponentName>
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

