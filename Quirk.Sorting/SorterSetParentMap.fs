namespace Quirk.Sorting

open System
open FSharp.UMX
open Quirk.Core


type sorterSetParentMap = 
        private {
        id: Guid<sorterSetParentMapId>;
        childSetId:Guid<sorterSetId>;
        parentSetId:Guid<sorterSetId>;
        parentMap:Map<Guid<sorterId>, Guid<sorterParentId>> }

module SorterSetParentMap =

    let getId
            (sorterParentMap:sorterSetParentMap) 
         =
         sorterParentMap.id


    let getParentMap 
             (sorterParentMap:sorterSetParentMap) 
         =
         sorterParentMap.parentMap


    let getChildSorterSetId
                (sorterParentMap:sorterSetParentMap) 
         =
         sorterParentMap.childSetId


    let getParentSorterSetId
                (sorterParentMap:sorterSetParentMap) 
         =
         sorterParentMap.parentSetId


    let load
            (id:Guid<sorterSetParentMapId>)
            (childSetId:Guid<sorterSetId>)
            (parentSetId:Guid<sorterSetId>)
            (parentMap:Map<Guid<sorterId>, Guid<sorterParentId>>)
        =
        {   
            id=id
            parentMap=parentMap
            childSetId=childSetId
            parentSetId=parentSetId
        }

    let makeId
            (parentSetId:Guid<sorterSetId>)
            (childSetId:Guid<sorterSetId>)
        =
        [|
            "sorterSetParentMap" :> obj
            parentSetId |> UMX.untag :> obj; 
            childSetId |> UMX.untag :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<sorterSetParentMapId>


    let create
            (childSetId:Guid<sorterSetId>)
            (parentSetId:Guid<sorterSetId>)
            (childSetCount:int<sorterCount>)
            (parentSorterIds:Guid<sorterId>[])
        =
        let parentMap =
            parentSorterIds
            |> Seq.map(UMX.cast<sorterId,sorterParentId>)
            |> CollectionOps.infinteLoop
            |> Seq.zip (childSetId |> SorterSet.generateSorterIds)
            |> Seq.take (childSetCount |> UMX.untag)
            |> Map.ofSeq

        let sorterParentMapId = 
                makeId
                   parentSetId
                   childSetId
        load
            sorterParentMapId
            childSetId
            parentSetId
            parentMap


    // adds self-mapping of the parent sorterId's
    let extendToParents
            (sspm:sorterSetParentMap)
        =
        sspm 
        |> getParentMap
        |> Map.values
        |> Seq.distinct
        |> Seq.map(fun v -> (v |> UMX.cast<sorterParentId,sorterId> , v))
        |> Seq.append
            (sspm |> getParentMap |> Map.toSeq)
        |> Map.ofSeq