﻿namespace Quirk.Workspace

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open Quirk.SortingResults


type wsComponentData =
    | SortableSet of sortableSet
    | SorterSet of sorterSet
    | SorterSetAncestry of sorterSetAncestry
    | SorterSetConcatMap of sorterSetConcatMap
    | SorterSetEval of sorterSetEval
    | SorterSetMutator of sorterSetMutator
    | SorterSetParentMap of sorterSetParentMap
    | SorterSpeedBinSet of sorterSpeedBinSet
    | SorterSetPruner of sorterSetPruner
    | WsParams of wsParams


module WsComponentData =

    let getId (comp:wsComponentData) =
        match comp with
        | SortableSet sortableSet -> 
            sortableSet |> SortableSet.getSortableSetId |> UMX.untag
        | SorterSet sorterSet -> 
            sorterSet |> SorterSet.getId |> UMX.untag
        | SorterSetAncestry sorterSetAncestry -> 
            sorterSetAncestry |> SorterSetAncestry.getId |> UMX.untag
        | SorterSetConcatMap sorterSetConcatMap -> 
            sorterSetConcatMap |> SorterSetConcatMap.getId |> UMX.untag
        | SorterSetEval sorterSetEval -> 
            sorterSetEval |> SorterSetEval.getSorterSetEvalId |> UMX.untag
        | SorterSetMutator sorterSetMutator -> 
            sorterSetMutator |> SorterSetMutator.getId |> UMX.untag
        | SorterSetParentMap sorterSetParentMap -> 
            sorterSetParentMap |> SorterSetParentMap.getId |> UMX.untag
        | SorterSpeedBinSet sorterSpeedBinSet ->
            sorterSpeedBinSet |> SorterSpeedBinSet.getId |> SorterSpeedBinSetId.value
        | SorterSetPruner sorterSetPruner ->
            sorterSetPruner |> SorterSetPruner.getId |> UMX.untag
        | WsParams wsParams ->
            wsParams |> WsParams.getId |> UMX.untag

    let asSortableSet (comp:wsComponentData) =
        match comp with
        | SortableSet sortableSet -> 
             sortableSet |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not SortableSet (*92)" |> Error


    let asSorterSet (comp:wsComponentData) =
        match comp with
        | SorterSet sorterSet -> 
             sorterSet |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not SorterSet (*93)" |> Error


    let asSorterSetAncestry (comp:wsComponentData) =
        match comp with
        | SorterSetAncestry sorterSetAncestry -> 
             sorterSetAncestry |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not SorterSetAncestry (*94)" |> Error


    let asSorterSetMutator (comp:wsComponentData) =
        match comp with
        | SorterSetMutator sorterSetMutator -> 
             sorterSetMutator |> Ok
        | _ -> 
             $"wsComponentData is {comp}, not SorterSetMutator (*95)" |> Error


    let asSorterSetParentMap (comp:wsComponentData) =
        match comp with
        | SorterSetParentMap sorterSetParentMap -> 
             sorterSetParentMap |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not SorterSetParentMap (*96)" |> Error


    let asSorterSetConcatMap (comp:wsComponentData) =
        match comp with
        | SorterSetConcatMap sorterSetConcatMap -> 
             sorterSetConcatMap |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not SorterSetConcatMap (*97)" |> Error


    let asSorterSetEval (comp:wsComponentData) =
        match comp with
        | SorterSetEval sorterSetEval -> 
             sorterSetEval |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not SorterSetEval (*98)" |> Error


    let asSorterSpeedBinSet (comp:wsComponentData) =
        match comp with
        | SorterSpeedBinSet sorterSpeedBinSet -> 
             sorterSpeedBinSet |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not SorterSpeedBinSet (*99)" |> Error


    let asSorterSetPruner (comp:wsComponentData) =
        match comp with
        | SorterSetPruner sorterSetPruner -> 
             sorterSetPruner |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not SorterSetPruner (*100)" |> Error


    let asWsParams (comp:wsComponentData) =
        match comp with
        | WsParams wsParams -> 
             wsParams |> Ok
        | _  -> 
             $"wsComponentData is {comp}, not WorkspaceParams (*101)" |> Error


type wsComponent =
   private 
     { 
        id: Guid<wsComponentId>;
        name: string<wsComponentName>
        wsComponentType: wsComponentType
        wsComponentData: wsComponentData
     }


module WsComponent =

    let load 
            (id: Guid<wsComponentId>) 
            (name: string<wsComponentName>) 
            (wsComponentType: wsComponentType) 
            (wsComponentData:wsComponentData) =
        {
            id=id
            name=name
            wsComponentType=wsComponentType
            wsComponentData=wsComponentData
        }


    let getId (wsComponent:wsComponent) =
        wsComponent.id

    let getName (wsComponent:wsComponent) =
        wsComponent.name

    let getWsComponentType (wsComponent:wsComponent) =
        wsComponent.wsComponentType

    let getWsComponentData (wsComponent:wsComponent) =
        wsComponent.wsComponentData





