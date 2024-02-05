namespace Quirk.Sorting

open System
open FSharp.UMX
open Quirk.Core

type sorterSet =
    private
        {
          id: Guid<sorterSetId>
          order: int<order>
          sorterMap: Map<Guid<sorterId>, sorter> 
        }

module SorterSet =

    let getId (sorterSet: sorterSet) = sorterSet.id

    let getOrder (sorterSet: sorterSet) = sorterSet.order

    let getSorterCount (sorterSet: sorterSet) =
            sorterSet.sorterMap.Count
            |> UMX.tag<sorterCount>

    let getSorters (sorterSet: sorterSet) = 
            sorterSet.sorterMap 
            |> Map.toArray
            |> Array.map(snd)
            |> Array.sortBy(Sorter.getSorterId)

    let getSortersById 
            (maxCt:int<sorterCount>) 
            (ids: Guid<sorterId> seq) 
            (sorterSet: sorterSet)
        =
        ids |> Seq.map(fun d -> sorterSet.sorterMap.TryFind d)
            |> Seq.filter(fun ov -> ov |> Option.isSome)
            |> Seq.map(fun ov -> ov |> Option.get)
            |> CollectionOps.takeUpto (maxCt |> UMX.untag)
    
    let tryGetSorterById
            (id: Guid<sorterId>) 
            (sorterSet: sorterSet)
        =
        sorterSet.sorterMap.TryFind id


    let getSorterByIdR
            (id: Guid<sorterId>) 
            (sorterSet: sorterSet)
        =
        if sorterSet.sorterMap.ContainsKey id then
            sorterSet.sorterMap.[id] |> Ok
        else
            sprintf "%s not found (404)" (id |> UMX.untag |> string)
            |> Error


    let generateSorterIds 
        (sorterStId:Guid<sorterSetId>) 
        =
        RandVars.rndGuidsLcg (sorterStId |> UMX.untag)
        |> Seq.map(UMX.tag<sorterId>)


    let load 
            (id:Guid<sorterSetId>) 
            (order: int<order>) 
            (sorters: seq<sorter>) 
        =
        let sorterMap =
            sorters 
            |> Seq.map (fun s -> (s |> Sorter.getSorterId, s)) 
            |> Map.ofSeq

        {
          sorterSet.id = id
          order = order
          sorterMap = sorterMap 
        }


    let createEmpty = 
        load (Guid.Empty |> UMX.tag<sorterSetId>) (0 |> UMX.tag<order>) (Seq.empty)


    let create
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (order: int<order>)
            (sorterGen: Guid<sorterId> -> sorter)
        =
        generateSorterIds sorterStId
        |> Seq.map (fun sId -> sorterGen sId)
        |> Seq.take(sorterCt |> UMX.untag)
        |> load sorterStId  order 


    let createMergedSorterSet
            (mergedId:Guid<sorterSetId>)
            (lhs:sorterSet)
            (rhs:sorterSet)
        =
        let mergedSorters = 
            (getSorters lhs)
            |> Array.append (getSorters rhs)
        load
            mergedId
            (lhs.order)
            mergedSorters


    let createRandom
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (order: int<order>)
            (rnGen: unit -> rngGen) 
            (sorterRndGen: (unit -> rngGen) -> Guid<sorterId> -> sorter)
        =
        let sorterGen = sorterRndGen rnGen
        create sorterStId sorterCt order sorterGen


    let createRandomSwitches
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (order: int<order>)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen)  
        =
        createRandom 
            sorterStId
            sorterCt
            order
            rnGen
            (Sorter.randomSwitches order wPfx switchCount)


    let createRandomStages
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (switchFreq: float<switchFrequency>)
            (order: int<order>)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen)   
        =
        createRandom 
            sorterStId
            sorterCt
            order
            rnGen
            (Sorter.randomStages order switchFreq wPfx switchCount)


    let createRandomStages2
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (order: int<order>)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen)  
        =
        createRandom 
            sorterStId 
            sorterCt 
            order 
            rnGen  
            (Sorter.randomStages2 order wPfx switchCount)


    let createRandomStagesCoConj
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (order: int<order>)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen) 
        =
        createRandom 
            sorterStId 
            sorterCt 
            order
            rnGen
            (Sorter.randomStagesCoConj order wPfx switchCount)


    let createRandomStagesSeparated
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (order: int<order>)
            (minSeparation: int)
            (maxSeparation: int)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen)   
        =
        createRandom 
            sorterStId
            sorterCt
            order
            rnGen 
            (Sorter.randomStagesSeparated minSeparation maxSeparation order wPfx switchCount)
            
    
    let createRandomOrbitDraws
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (coreTc:twoCycle) 
            (permSeed:permutation)
            (maxOrbit:int option)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen)   
        =
        let perms = permSeed 
                    |> Permutation.powers maxOrbit
                    |> Seq.toArray
        let order = (coreTc |> TwoCycle.getOrder)
        createRandom 
            sorterStId
            sorterCt
            order
            rnGen
            (Sorter.randomPermutaionChoice coreTc perms order wPfx switchCount)


    let createRandomSymmetric
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (order: int<order>)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen) 
        =
        createRandom 
            sorterStId 
            sorterCt 
            order
            rnGen
            (Sorter.randomSymmetric order wPfx switchCount)


    let createRandomBuddies
            (sorterStId:Guid<sorterSetId>)
            (sorterCt: int<sorterCount>)
            (stageWindowSz: int<stageWindowSize>)
            (order: int<order>)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen)   
        =
        createRandom 
            sorterStId
            sorterCt
            order
            rnGen
            (Sorter.randomBuddies stageWindowSz order wPfx switchCount) 


    let createMutationSet 
            (sorterBase: sorter[]) 
            (sorterCt: int<sorterCount>)
            (order: int<order>) 
            (sorterMutatr: sorterMutator) 
            (sorterStId:Guid<sorterSetId>)
            (randy: IRando) 
        =
        let _mutato dex id =
            let sortr = sorterBase.[dex % sorterBase.Length]
            let muty = 
                (sorterMutatr |> SorterMutator.getMutatorFunc) sortr id randy
            muty

        generateSorterIds sorterStId
        |> Seq.mapi(_mutato)
        |> Seq.filter(Result.isOk)
        |> Seq.map(Result.ExtractOrThrow)
        |> Seq.take(sorterCt |> UMX.untag)
        |> load sorterStId order


    //// creates a Map<mergeId*(pfxId*sfxId)>
    //let createAppendSetMap
    //        (sorterStIdPfx:sorterSetId)
    //        (sorterCtPfx:sorterCount)
    //        (sorterStIdSfx:sorterSetId)
    //        (sorterCtSfx:sorterCount)
    //        (sorterStIdAppendSet:sorterSetId)
    //    =
    //    let sorterIdsPfx = 
    //            generateSorterIds sorterStIdPfx 
    //                |> Seq.take (sorterCtPfx |> SorterCount.value) 
    //                |> Seq.toArray

    //    let sorterIdsSfx = 
    //            generateSorterIds sorterStIdSfx 
    //                |> Seq.take (sorterCtSfx |> SorterCount.value) 
    //                |> Seq.toArray

    //    let sorterIdInputs = 
    //        CollectionOps.cartesianProduct sorterIdsPfx sorterIdsSfx
    //        |> Seq.toArray

    //    let sorterIdMerged = generateSorterIds sorterStIdAppendSet

    //    sorterIdInputs
    //    |> Seq.zip sorterIdMerged
    //    |> Seq.toArray
    //    |> Map.ofSeq


    //let createAppendSet 
    //        (sorterSetPfx: sorterSet)
    //        (sorterSetSfx: sorterSet)
    //        (sorterSetIdAppendSet:sorterSetId)
    //    =
    //    let appendSetMap = 
    //        createAppendSetMap
    //            (sorterSetPfx |> getId)
    //            (sorterSetPfx |> getSorterCount)
    //            (sorterSetSfx |> getId)
    //            (sorterSetSfx |> getSorterCount)
    //            sorterSetIdAppendSet

    //    let _mergeEm mergeId pfxId sfxId =
    //        let sPfx = sorterSetPfx |> tryGetSorterById pfxId
    //        let sSfx = sorterSetPfx |> tryGetSorterById sfxId
    //        match sPfx, sSfx with
    //        | Some pfx, Some sfx -> 
    //            pfx |> Sorter.appendSwitches mergeId (sfx |> Sorter.getSwitches) |> Ok
    //        | _ -> "sorter not found" |> Error


    //    let mergedSorters =
    //            appendSetMap 
    //            |> Map.toSeq
    //            |> Seq.map(fun (mergeId, (pfxId, sfxId)) -> _mergeEm mergeId pfxId sfxId)
    //            |> Seq.toList
    //            |> Result.sequence
    //    mergedSorters