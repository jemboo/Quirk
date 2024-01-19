namespace Quirk.Sorting
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting

type sortableIntArray =
    private
        { values: int[]
          order: int<order>
          symbolSetSize: uint64<symbolSetSize> }


type sortableBoolArray =
    private { values: bool[]; order: int<order> }


module SortableIntArray =

    let Identity (order: int<order>) (symbolCount: uint64<symbolSetSize>) =
        let ordV = (order |> UMX.untag)
        let sc = symbolCount |> UMX.untag |> int

        if (ordV <> sc) then
            sprintf "order %d and symbolcount %d don't match" ordV sc |> Error
        else
            { sortableIntArray.values = [| 0 .. ordV - 1 |]
              order = order
              symbolSetSize = symbolCount }
            |> Ok

    let apply f (p: sortableIntArray) = f p.values
    let value p = apply id p
    let getValues (sia: sortableIntArray) = sia.values
    let getOrder (sia: sortableIntArray) = sia.order
    let getSymbolSetSize (sia: sortableIntArray) = sia.symbolSetSize


    let make (order: int<order>) (symbolSetSize: uint64<symbolSetSize>) (vals: int[]) =
        { sortableIntArray.values = vals
          order = order
          symbolSetSize = symbolSetSize }


    let copy (toCopy: sortableIntArray) =
        { toCopy with values = toCopy.values |> Array.copy }


    let isSorted (sortableInts: sortableIntArray) =
        sortableInts |> getValues |> CollectionProps.isSorted_idiom


    let makeOrbits (maxCount: int<sortableCount> option) (perm: permutation) =
        let order = perm |> Permutation.getOrder
        let symbolSetSize = order |> UMX.untag |> uint64 |> UMX.tag<symbolSetSize>
        let intOpt = maxCount |> Option.map UMX.untag

        Permutation.powers intOpt perm
        |> Seq.map (Permutation.getArray)
        |> Seq.map (fun arr ->
            { sortableIntArray.values = arr
              order = order
              symbolSetSize = symbolSetSize })


    // test set for the merge sort (merge two sorted sets of order/2)
    let makeMergeSortTestWithInts (order: int<order>) =
        let hov = (order |> UMX.untag ) / 2
        let symbolSetSize = order |> UMX.untag |> uint64 |> UMX.tag<symbolSetSize>
        [|0 .. hov|]
        |> Array.map (
            fun dex ->
                { sortableIntArray.values = Permutation.stackedSource order dex 
                  order = order
                  symbolSetSize = symbolSetSize }
            )


    let makeRandomPermutation (order: int<order>) (randy: IRando) =
        let symbolSetSize = order |> UMX.untag |> uint64 |> UMX.tag<symbolSetSize>

        { sortableIntArray.values = RandVars.randomPermutation randy order
          order = order
          symbolSetSize = symbolSetSize }


    let makeRandomSymbol (order: int<order>) (symbolSetSize: uint64<symbolSetSize>) (randy: IRando) =
        let arrayLength = order |> UMX.untag
        let intArray = RandVars.randSymbols symbolSetSize randy arrayLength |> Seq.toArray

        { sortableIntArray.values = intArray
          order = order
          symbolSetSize = symbolSetSize }


    let makeRandomSymbols 
                (order: int<order>) 
                (symbolSetSize: uint64<symbolSetSize>) 
                (rnd: IRando) 
        =
        seq {
            while true do
                yield makeRandomSymbol order symbolSetSize rnd
        }



module SortableBoolArray =

    let apply f (p: sortableBoolArray) = f p.values
    let getValues p = apply id p
    let getOrder (sia: sortableBoolArray) = sia.order

    let make (order: int<order>) (vals: bool[]) =
        { sortableBoolArray.values = vals
          order = order }


    let copy (toCopy: sortableBoolArray) =
        { toCopy with values = toCopy.values |> Array.copy }


    let isSorted (sortableBools: sortableBoolArray) =
        sortableBools |> getValues |> CollectionProps.isSorted_idiom


    let makeAllBits (order: int<order>) =
        let symbolSetSize = 2uL |> UMX.tag<symbolSetSize>
        let bitShift = order |> UMX.untag

        { 0uL .. (1uL <<< bitShift) - 1uL }
        |> Seq.map (ByteUtils.uint64ToBoolArray order)
        |> Seq.map (fun arr ->
            { sortableBoolArray.values = arr
              order = order })


    let makeAllForOrder (order: int<order>) =
        let bitShift = order |> UMX.untag

        { 0uL .. (1uL <<< bitShift) - 1uL }
        |> Seq.map (ByteUtils.uint64ToBoolArray order)
        |> Seq.map (fun arr ->
            { sortableBoolArray.values = arr
              order = order })


    let makeSortedStacks (orderStack: int<order>[]) =
        let stackedOrder = Order.add orderStack

        CollectionOps.stackSortedBlocks orderStack
        |> Seq.map (fun arr ->
            { sortableBoolArray.values = arr
              order = stackedOrder })


    let makeRandomBits 
            (order: int<order>) 
            (pctTrue: float) 
            (randy: IRando) 
        =
        let arrayLength = order |> UMX.untag

        Seq.initInfinite (fun _ ->
            { sortableBoolArray.values = RandVars.randBits pctTrue randy arrayLength |> Seq.toArray
              order = order })


    let allBooleanVersions (sortableInts: sortableIntArray) =
        let order = sortableInts |> SortableIntArray.getOrder |> UMX.untag
        let values = sortableInts |> SortableIntArray.getValues
        let threshHolds = values |> Set.ofArray |> Set.toArray |> Array.sort |> Array.skip 1

        threshHolds
        |> Seq.map (fun thresh -> Array.init order (fun dex -> if (values.[dex] >= thresh) then true else false))



    let expandToSortableBits (sortableIntsSeq: seq<sortableIntArray>) =
        let order = sortableIntsSeq |> Seq.head |> SortableIntArray.getOrder

        sortableIntsSeq
        |> Seq.map (allBooleanVersions)
        |> Seq.concat
        |> Seq.distinct
        |> Seq.map (make order)
