namespace Quirk.Run.Shc

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Run.Core

[<Measure>] type shcComponentId

module WorkspaceComponentTypeShc =

    let getShcComponentID 
            (quirkWorldLineId:Guid<quirkWorldLineId>)
            (generation:int<generation>)
            (workspaceComponentTypeShc:workspaceComponentType)
        =
        [
            quirkWorldLineId :> obj
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
            (quirkWorldLineId: Guid<quirkWorldLineId>)
            (wsc:workspaceComponentType)
            (generation:int<generation>) 
        =
        [
            quirkWorldLineId :> obj;
            wsc :> obj;
            generation :> obj
        ] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<workspaceComponentId>
