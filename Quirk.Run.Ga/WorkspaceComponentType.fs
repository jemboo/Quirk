namespace Quirk.Run.Shc

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Iter
open Quirk.Project
open Quirk.Run.Core
open Quirk.Workspace


[<Measure>] type gaComponentId

module WsComponentTypeGa =

    let getShcComponentID 
            (quirkWorldLineId:Guid<quirkWorldLineId>)
            (generation:int<generation>)
            (wsComponentType:wsComponentType)
        =
        [
            quirkWorldLineId :> obj
            generation :> obj
            wsComponentType :> obj
        ] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<gaComponentId>



type rndGenTypeGa =
    | Create
    | Mutate
    | Prune



module RndGenProvider =

    let getRngGen 
            (rngType:rngType)
            (rndGenTypeShc:rndGenTypeGa)
            (generation:int<generation>) 
        =
        let gu = 
            [
                rndGenTypeShc :> obj;
                generation :> obj
            ] 
            |> GuidUtils.guidFromObjs

        RngGen.fromGuid rngType gu


    let getShcWsComponentId
            (quirkWorldLineId: Guid<quirkWorldLineId>)
            (wsc:wsComponentType)
            (generation:int<generation>) 
        =
        [
            quirkWorldLineId :> obj;
            wsc :> obj;
            generation :> obj
        ] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<wsComponentId>
