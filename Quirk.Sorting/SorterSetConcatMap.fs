namespace Quirk.Sorting
open System
open FSharp.UMX
open Quirk.Core


type sorterSetConcatMap = 
        private {
        concatMap:Map<Guid<sorterId>, Guid<sorterId>[]> }


module SorterSetConcatMap 
        =
    let load (id:Guid<sorterSetConcatMapId>)
             (sorterSetBaseId:Guid<sorterSetId>)
             (sorterSetConcatId:Guid<sorterSetId>)
             (concatMap:Map<Guid<sorterId>, Guid<sorterId>[]>)
        =
        { 
          id = id
          sorterSetBaseId = sorterSetBaseId
          sorterSetConcatId = sorterSetConcatId
          concatMap = concatMap
        }


    let make (concats:Guid<sorterId>[] seq)
             (sorterSetBaseId:Guid<sorterSetId>)
             (sorterSetConcatId:Guid<sorterSetId>)
        =
        let concatMapId = 
            [| 
                (sorterSetBaseId |> UMX.untag);
                (sorterSetConcatId |> UMX.untag);
            |] 
            |> Array.map(fun tup -> tup:> obj) 
            |> GuidUtils.guidFromObjs 
            |> UMX.tag<sorterSetConcatMapId>

        let memberIds = sorterSetConcatId |> SorterSet.generateSorterIds
        let concatMap = 
                concats
                |> Seq.zip memberIds
                |> Seq.toArray
                |> Map.ofSeq
        { 
          id = concatMapId
          sorterSetBaseId = sorterSetBaseId
          sorterSetConcatId = sorterSetConcatId
          concatMap = concatMap
        }


    let getId (sscm: sorterSetConcatMap) = sscm.id
    let getSorterSetBaseId (sscm: sorterSetConcatMap) = sscm.sorterSetBaseId
    let getSorterSetConcatId (sscm: sorterSetConcatMap) = sscm.sorterSetConcatId
    let getConcatMap (sscm: sorterSetConcatMap) = sscm.concatMap

    let createSorterSet
            (sorterSetbase: sorterSet)
            (sorterSetConcatMap:sorterSetConcatMap)
        =
        let _mergeEm mergeId sorterIds =
            result {
                let order = sorterSetbase |> SorterSet.getOrder
                let! sorters = 
                    sorterIds 
                        |> Array.map(fun id -> sorterSetbase |> SorterSet.getSorterByIdR id)
                        |> Array.toList
                        |> Result.sequence

                return Sorter.concatSwitches mergeId order sorters
            }

        result {
            let! mergedSorters =
                    sorterSetConcatMap
                    |> getConcatMap
                    |> Map.toSeq
                    |> Seq.map(fun (mergeId, cats) -> _mergeEm mergeId cats)
                    |> Seq.toList
                    |> Result.sequence

            return SorterSet.load
                        sorterSetConcatMap.sorterSetConcatId
                        (sorterSetbase |> SorterSet.getOrder)
                        mergedSorters
        }


    let createForAppendSet
            (sorterSetbaseId: Guid<sorterSetId>)
            (sorterSetBaseCount: int<sorterCount>)
            (sorterSetIdAppendSet: Guid<sorterSetId>)
        =
        let baseSorterIds = 
            sorterSetbaseId 
                |> SorterSet.generateSorterIds
                |> Seq.take(sorterSetBaseCount |> UMX.untag)
                |> Seq.toArray

        let idCCts = 
            CollectionOps.cartesianProduct baseSorterIds baseSorterIds
            |> Seq.map(fun (fst, snd) -> [|fst;snd|])

        make 
            idCCts 
            sorterSetbaseId
            sorterSetIdAppendSet
        

    let createSorterSetAppend
            (sorterSetbase: sorterSet)
            (sorterSetIdAppendSet: Guid<sorterSetId>)
        =
        let sscm = 
            createForAppendSet 
                (sorterSetbase |> SorterSet.getId)
                (sorterSetbase |> SorterSet.getSorterCount)
                sorterSetIdAppendSet
        createSorterSet sorterSetbase sscm
