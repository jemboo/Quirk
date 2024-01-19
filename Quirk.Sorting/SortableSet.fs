namespace Quirk.Sorting
open FSharp.UMX

open System
open Quirk.Core

type sortableSet =
    private
        { sortableSetId: Guid<sortableSetId>
          rollout: rollout
          symbolSetSize: uint64<symbolSetSize> }

module SortableSet =

    let getSymbolSetSize 
            (sortableSet: sortableSet) 
        = sortableSet.symbolSetSize

    let getSortableCount (sortableSet: sortableSet) =
        sortableSet.rollout
        |> Rollout.getArrayCount
        |> UMX.untag
        |> UMX.tag<sortableCount>

    let getSortableSetId (sortableSet: sortableSet) = sortableSet.sortableSetId

    let getRollout (sortableSet: sortableSet) = sortableSet.rollout

    let getOrder (sortableSet: sortableSet) =
        sortableSet.rollout
        |> Rollout.getArrayLength
        |> UMX.untag
        |> UMX.tag<order>

    let getSummaryReport (sortableSet:sortableSet)
        =
        sprintf "%s\t%d"
            (sortableSet |> getSortableSetId |> UMX.untag |> string)
            (sortableSet |> getRollout |> Rollout.getArrayCount |> UMX.untag)

    let makeTag (sortableSet:sortableSet)
        =
        sprintf "%s(%s)"
            ((sortableSet |> getRollout |> Rollout.getArrayCount |> UMX.untag ).ToString("D4"))
            ((sortableSet |> getSortableSetId |> UMX.untag |> string).Substring(0,8))

    // returns the first sortable, which would be the permutaion root of an orbit type sortable set
    let getIdAndPermRoot 
        (sortableSet:sortableSet)
        =
        ( sortableSet |> getSortableSetId 
          ,
          sortableSet.rollout
          |> Rollout.toIntArrays
          |> Seq.head
          |> Permutation.createNr
        )


    let make
        (sortableSetId: Guid<sortableSetId>) 
        (symbolSetSize: uint64<symbolSetSize>) 
        (rollout: rollout) 
        =
        { sortableSetId = sortableSetId
          rollout = rollout
          symbolSetSize = symbolSetSize }


    let createEmpty = 
        make 
            (Guid.Empty |> UMX.tag<sortableSetId>) 
            (0UL |> UMX.tag<symbolSetSize>) 
            Rollout.createEmpty


    let fromSortableBoolArrays
        (sortableSetId: Guid<sortableSetId>)
        (rolloutFormat: rolloutFormat)
        (order: int<order>)
        (sortableBoolSeq: seq<sortableBoolArray>)
        =
        result {
            let symbolSetSize = 2uL |> UMX.tag<symbolSetSize>
            let arrayLength = order |> UMX.cast<order, arrayLength>
            let boolArraySeq = sortableBoolSeq |> Seq.map (SortableBoolArray.getValues)
            let! rollout = boolArraySeq |> Rollout.fromBoolArrays rolloutFormat arrayLength
            return make sortableSetId symbolSetSize rollout
        }


    let toSortableBoolArrays 
            (sortableSet: sortableSet)
            =
        let order = sortableSet |> getOrder
        sortableSet
        |> getRollout
        |> Rollout.toBoolArrays
        |> Seq.map (SortableBoolArray.make order)


    let toSortableIntsArrays 
            (sortableSet: sortableSet) 
            =
        let order = sortableSet |> getOrder
        let symbolSetSize = getSymbolSetSize sortableSet
        sortableSet
        |> getRollout
        |> Rollout.toIntArrays
        |> Seq.map (SortableIntArray.make order symbolSetSize)


    let fromSortableIntArrays
            (sortableSetId: Guid<sortableSetId>)
            (order: int<order>)
            (symbolSetSize: uint64<symbolSetSize>)
            (sortableInts: seq<sortableIntArray>)
            =
        result {
            let arrayLength = order |> UMX.cast<order, arrayLength>
            let bitsPerSymbol = symbolSetSize |> BitsPerSymbol.fromSymbolSetSize
            let! rolly =
                sortableInts
                |> Seq.map (fun sints -> sints.values)
                |> IntRoll.fromArrays arrayLength bitsPerSymbol

            return make sortableSetId symbolSetSize (rolly |> rollout.I32)
        }


    let fromBitPack
        (sortableSetId: Guid<sortableSetId>)
        (rolloutFormat: rolloutFormat)
        (order: int<order>)
        (symbolSetSize: uint64<symbolSetSize>)
        (bitPk: bitPack)
        =
        result {
            let arrayLength = order |> UMX.cast<order, arrayLength>
            let! rollout = bitPk |> Rollout.fromBitPack rolloutFormat arrayLength
            return make sortableSetId symbolSetSize rollout
        }


    let toBitPack 
        (sortableSet: sortableSet) 
        =
        let symbolSetSize = sortableSet |> getSymbolSetSize
        let bitsPerSymbl = symbolSetSize |> BitsPerSymbol.fromSymbolSetSize
        sortableSet.rollout |> Rollout.toBitPack bitsPerSymbl


    let makeAllBits 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormat: rolloutFormat) 
            (order: int<order>) 
            =
        let sortableBits = SortableBoolArray.makeAllBits order
        fromSortableBoolArrays sortableSetId rolloutFormat order sortableBits


    let makeOrbits
        (sortableSetId: Guid<sortableSetId>)
        (maxCount: int<sortableCount> option)
        (perm: permutation)
        =
        let order = perm |> Permutation.getOrder
        let symbolSetSize = order |> UMX.untag |> uint64 |> UMX.tag<symbolSetSize>
        let sortableInts = SortableIntArray.makeOrbits maxCount perm
        fromSortableIntArrays sortableSetId order symbolSetSize sortableInts


    let makeMergeSortTestWithInts
            (sortableSetId: Guid<sortableSetId>)
            (order: int<order>) 
            =
        let symbolSetSize = order |> UMX.untag |> uint64 |> UMX.tag<symbolSetSize>
        let sortableInts = SortableIntArray.makeMergeSortTestWithInts order
        fromSortableIntArrays sortableSetId order symbolSetSize sortableInts


    let makeSortedStacks 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormat: rolloutFormat) 
            (orderStack: int<order>[]) 
            =
        let stackedOrder = Order.add orderStack
        let sortableBits = SortableBoolArray.makeSortedStacks orderStack
        fromSortableBoolArrays sortableSetId rolloutFormat stackedOrder sortableBits


    let makeRandomPermutation
        (order: int<order>)
        (sortableCount: int<sortableCount>)
        (rando: IRando)
        (sortableSetId: Guid<sortableSetId>)
        =
        let symbolSetSize = order |> UMX.untag |> uint64 |> UMX.tag<symbolSetSize>

        let sortableInts =
            SortableCount.makeSeq sortableCount
            |> Seq.map (fun _ -> SortableIntArray.makeRandomPermutation order rando)

        fromSortableIntArrays sortableSetId order symbolSetSize sortableInts


    let makeRandomBits
        (rolloutFormat: rolloutFormat)
        (order: int<order>)
        (pctTrue: float)
        (sortableCount: int<sortableCount>)
        (rando: IRando)
        (sortableSetId: Guid<sortableSetId>)
        =
        let sortableBools =
            SortableBoolArray.makeRandomBits order pctTrue rando
            |> Seq.take (sortableCount |> UMX.untag)

        fromSortableBoolArrays sortableSetId rolloutFormat order sortableBools


    let makeRandomSymbols
        (order: int<order>)
        (symbolSetSize: uint64<symbolSetSize>)
        (sortableCount: int<sortableCount>)
        (rando: IRando)
        (sortableSetId: Guid<sortableSetId>)
        =
        let sortableInts =
            SortableCount.makeSeq sortableCount
            |> Seq.map (fun _ -> SortableIntArray.makeRandomSymbol order symbolSetSize rando)

        fromSortableIntArrays sortableSetId order symbolSetSize sortableInts

    // Not finished
    let switchReduce
        (sortableSetId: Guid<sortableSetId>)
        (sortableSet: sortableSet)
        (sorter: sorter)
        =
        let sortableInts = Seq.empty<sortableIntArray>
        let order = sortableSet |> getOrder
        let symbolSetSize = sortableSet |> getSymbolSetSize
        fromSortableIntArrays sortableSetId order symbolSetSize sortableInts



type setOfSortableSet =
    private
        { setOfSortableSetId: Guid<setOfSortableSetId>
          sortableSetMap: Map<Guid<sortableSetId>, sortableSet> }


module SetOfSortableSet =

    let getSortableSetMap (setOfSortableSet: setOfSortableSet) =
        setOfSortableSet.sortableSetMap

    let getSetOfSortableSetId (setOfSortableSet: setOfSortableSet) = 
        setOfSortableSet.setOfSortableSetId

    let getSortableSets (setOfSortableSet: setOfSortableSet)
        =
        setOfSortableSet |> getSortableSetMap |> Map.values


    let getSummaryReports (setOfSortableSet: setOfSortableSet) 
        =
        setOfSortableSet |> getSortableSets |> Seq.map(SortableSet.getSummaryReport)


    let generateSortableSetIds 
        (setOfSortableSetId:Guid<setOfSortableSetId>) 
        =
        RandVars.rndGuidsLcg (setOfSortableSetId |> UMX.untag)
        |> Seq.map(UMX.tag<sortableSetId>)


    let load (setOfSortableSetId:Guid<setOfSortableSetId>) 
             (sortableSets: seq<sortableSet>) 
             =
        let sortableSetMap =
            sortableSets |> Seq.map (fun s -> (s |> SortableSet.getSortableSetId , s)) 
                         |> Map.ofSeq

        { setOfSortableSetId = setOfSortableSetId
          sortableSetMap = sortableSetMap }

    let create
        (setOfSortableSetId:Guid<setOfSortableSetId>)
        (sortableSetCount: int<sortableSetCount>)
        (sortableSetGen: Guid<sortableSetId> -> sortableSet option)
        =
        generateSortableSetIds setOfSortableSetId
        |> Seq.map (fun sId -> sortableSetGen sId )
        |> Seq.filter(Option.isSome)
        |> Seq.map(Option.get)
        |> Seq.take(sortableSetCount |> UMX.untag)
        |> load setOfSortableSetId


    let createRandom
        (setOfSortableSetId:Guid<setOfSortableSetId>)
        (sortableSetCount: int<sortableSetCount>)
        (rnGen: rngGen)
        (sortableSetRndGen: IRando -> Guid<sortableSetId> -> sortableSet option)
        =
        let randy = rnGen |> Rando.fromRngGen
        let sortableSetGen = sortableSetRndGen randy
        create setOfSortableSetId sortableSetCount sortableSetGen


    let makeOrbits
        (setOfSortableSetId:Guid<setOfSortableSetId>)
        (sortableSetCount: int<sortableSetCount>)
        (maxSortableCount: int<sortableCount> option)
        (perms: permutation array)
        =
        let mutable dex = -1
        let getPerm () =
            dex <- dex + 1
            perms.[dex]

        let sortableSetGen (id:Guid<sortableSetId>) = 
            SortableSet.makeOrbits id maxSortableCount (getPerm ()) 
            |> Result.toOption
        create setOfSortableSetId sortableSetCount sortableSetGen


    let makeRandomOrbits
        (setOfSortableSetId:Guid<setOfSortableSetId>)
        (sortableSetCount: int<sortableSetCount>)
        (order:int<order>)
        (maxSortableCount: int<sortableCount> option)
        (rnGen: rngGen)
        =
        let randy = rnGen |> Rando.fromRngGen
        let perms = Permutation.createRandoms order randy
                    |> Seq.take(sortableSetCount |> UMX.untag)
                    |> Seq.toArray
        makeOrbits
            setOfSortableSetId
            sortableSetCount
            maxSortableCount
            perms