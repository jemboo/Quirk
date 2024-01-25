namespace Quirk.Workspace

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open Quirk.SortingResults



type workspaceComponent =
    | SortableSet of sortableSet
    | SorterSet of sorterSet
    | SorterSetAncestry of sorterSetAncestry
    | SorterSetConcatMap of sorterSetConcatMap
    | SorterSetEval of sorterSetEval
    | SorterSetMutator of sorterSetMutator
    | SorterSetParentMap of sorterSetParentMap
    | SorterSpeedBinSet of sorterSpeedBinSet
    | SorterSetPruner of sorterSetPruner
    | WorkspaceParams of workspaceParams


module WorkspaceComponent =

    let getId (comp:workspaceComponent) =
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
        | WorkspaceParams workspaceParams ->
            workspaceParams |> WorkspaceParams.getId |> UMX.untag

    let asSortableSet (comp:workspaceComponent) =
        match comp with
        | SortableSet sortableSet -> 
             sortableSet |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SortableSet" |> Error


    let asSorterSet (comp:workspaceComponent) =
        match comp with
        | SorterSet sorterSet -> 
             sorterSet |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSet" |> Error


    let asSorterSetAncestry (comp:workspaceComponent) =
        match comp with
        | SorterSetAncestry sorterSetAncestry -> 
             sorterSetAncestry |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetAncestry" |> Error


    let asSorterSetMutator (comp:workspaceComponent) =
        match comp with
        | SorterSetMutator sorterSetMutator -> 
             sorterSetMutator |> Ok
        | _ -> 
             $"Workspace component type is {comp}, not SorterSetMutator" |> Error


    let asSorterSetParentMap (comp:workspaceComponent) =
        match comp with
        | SorterSetParentMap sorterSetParentMap -> 
             sorterSetParentMap |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetParentMap" |> Error


    let asSorterSetConcatMap (comp:workspaceComponent) =
        match comp with
        | SorterSetConcatMap sorterSetConcatMap -> 
             sorterSetConcatMap |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetConcatMap" |> Error


    let asSorterSetEval (comp:workspaceComponent) =
        match comp with
        | SorterSetEval sorterSetEval -> 
             sorterSetEval |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetEval" |> Error


    let asSorterSpeedBinSet (comp:workspaceComponent) =
        match comp with
        | SorterSpeedBinSet sorterSpeedBinSet -> 
             sorterSpeedBinSet |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSpeedBinSet" |> Error


    let asSorterSetPruner (comp:workspaceComponent) =
        match comp with
        | SorterSetPruner sorterSetPruner -> 
             sorterSetPruner |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetPruner" |> Error


    let asWorkspaceParams (comp:workspaceComponent) =
        match comp with
        | WorkspaceParams workspaceParams -> 
             workspaceParams |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not WorkspaceParams" |> Error

