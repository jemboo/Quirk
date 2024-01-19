﻿namespace Quirk.Sorting

open System
open FSharp.UMX
open Quirk.Core

[<Struct>]
type switch = { low: int; hi: int }

module Switch =

    let toString (sw: switch) = 
            sprintf "(%d, %d)" sw.low sw.hi

    let fromIndex (dex:int) = 
        let indexFlt = (dex |> float) + 1.0
        let p = (sqrt (1.0 + 8.0 * indexFlt) - 1.0) / 2.0
        let pfloor = Math.Floor(p)
        if (p = pfloor) then
            {switch.low= (int pfloor) - 1; hi = (int pfloor) - 1 }
        else
            let lo = (float dex) - (pfloor * (pfloor + 1.0)) / 2.0 |> int
            let hi = (int pfloor)
            {switch.low = lo; hi = hi }

    let maxSwitchIndexForOrder (ord: int<order>) =
        uint32 ((ord |> UMX.untag) * ((ord |> UMX.untag) + 1) / 2)

    let fromSwitchIndexes (dexes: int seq) =
        dexes |> Seq.map (fromIndex)

    let removeDegenerateIndexes (dexes: int seq) =
        dexes |> fromSwitchIndexes |> Seq.filter (fun sw -> sw.low <> sw.hi)

    let toIndex (switch: switch) =
        (switch.hi * (switch.hi + 1)) / 2 + switch.low

    let toIndexes (switches: seq<switch>) =
        switches |> Seq.map(toIndex)

    let maxIndexForOrder (ordr:int<order>) =
        let ov = ordr |> UMX.untag
        (ov * (ov + 3)) / 2


    let bitsPerSymbolRequired (ordr:int<order>) =
        ordr |> maxIndexForOrder |> uint64
             |> UMX.tag<symbolSetSize>
             |> BitsPerSymbol.fromSymbolSetSize


    let toBitPack (ordr:int<order>)
                  (switchs: seq<switch>) =
        let bps = bitsPerSymbolRequired ordr
        switchs |> Seq.map(toIndex)
                |> BitPack.fromInts bps


    let fromBitPack (bitPck:bitPack) =
        bitPck |> BitPack.toInts
               |> fromSwitchIndexes


    // all switch indexes for order with lowVal
    let lowOverlapping (ord: int<order>) (lowVal: int) =
        seq {
            for hv = (lowVal + 1) to (ord |> UMX.untag) - 1 do
                yield (hv * (hv + 1)) / 2 + lowVal
        }

    // all switch indexes for order with hiVal
    let hiOverlapping (ord: int<order>) (hiVal: int) =
        seq {
            for lv = 0 to (hiVal - 1) do
                yield (hiVal * (hiVal + 1)) / 2 + lv
        }

    let zeroSwitches =
        seq {
            while true do
                yield { switch.low = 0; hi = 0 }
        }

    // produces switches the two cycles in the permutation
    let extractFromInts (pArray: int[]) =
        seq {
            for i = 0 to pArray.Length - 1 do
                let j = pArray.[i]

                if ((j > i) && (i = pArray.[j])) then
                    yield { switch.low = i; hi = j }
        }

    let fromPermutation (p: permutation) =
        extractFromInts (Permutation.getArray p)

    let fromTwoCycle (tc: twoCycle) = extractFromInts (TwoCycle.getArray tc)

    let makeAltEvenOdd 
            (order: int<order>) 
            (stageCt: int<stageCount>) 
        =
        result {
            let stages =
                TwoCycle.makeAltEvenOdd order (Permutation.identity order)
                |> Seq.take (stageCt |> UMX.untag)

            return stages |> Seq.map (fromTwoCycle) |> Seq.concat
        }

    // IRando dependent
    let rndNonDegenSwitchesOfOrder (order: int<order>) (rnd: IRando) =
        let maxDex = maxSwitchIndexForOrder order
        seq {
            while true do
                let p = (int ((rnd.NextUInt ()) % maxDex))
                let sw = fromIndex p
                if (sw.low <> sw.hi) then
                    yield sw
        }


    let rndSwitchesOfOrder 
            (order: int<order>) 
            (rnd: IRando) 
        =
        let maxDex = maxSwitchIndexForOrder order
        seq {
            while true do
                let p = (int ((rnd.NextUInt ()) % maxDex))
                yield fromIndex p
        }


    let rndSymmetric (order: int<order>) (rnd: IRando) =
        let aa (rnd: IRando) =
            (TwoCycle.rndSymmetric order rnd) |> fromTwoCycle
        seq {
            while true do
                yield! (aa rnd)
        }


    let mutateSwitches 
            (order: int<order>) 
            (mutationRate: float<mutationRate>) 
            (rnd: IRando) 
            (switches: seq<switch>) 
        =
        let mDex = uint32 ((order |> UMX.untag) * ((order |> UMX.untag ) + 1) / 2)

        let mutateSwitch (switch: switch) =
            match (rnd.NextFloat ()) with
            | k when k < (mutationRate |> UMX.untag)
                        -> fromIndex (int ((rnd.NextUInt ()) % mDex))
            | _ -> switch

        switches |> Seq.map (fun sw -> mutateSwitch sw)


    let reflect 
            (ord: int<order>) 
            (sw: switch) 
        =
        { switch.low = sw.hi |> Order.reflect ord
          switch.hi = sw.low |> Order.reflect ord }



    // filters the switchArray, removing switches that compare
    // indexes that are not in subset. It then relabels the indexes
    // according to the subset. Ex, if the subset was [2;5;8], then
    // index 2 -> 0; index 5-> 1; index 8 -> 2
    let rebufo 
            (order: int<order>) 
            (swa: switch array) 
            (subset: int list) 
        =
        let _mapSubset (order: int<order>) (subset: int list) =
            let aRet = Array.create (order |> UMX.untag) None
            subset |> List.iteri (fun dex dv -> aRet.[dv] <- Some dex)
            aRet

        let _reduce (redMap: int option[]) (sw: switch) =
            let rpL, rpH = (redMap.[sw.low], redMap.[sw.hi])

            match rpL, rpH with
            | Some l, Some h -> Some { switch.low = l; hi = h }
            | _, _ -> None

        let redMap = _mapSubset order subset

        swa
        |> Array.map (_reduce redMap)
        |> Array.filter (Option.isSome)
        |> Array.map (Option.get)


    // returns a sequence containing all the possible
    // order reductions of the switch array
    let allMasks 
        (orderSource: int<order>) 
        (orderDest: int<order>) 
        (swa: switch array) 
        =
        let sd, dd = (orderSource |> UMX.untag), (orderDest |> UMX.untag)
        if sd < dd then
            failwith "source order cannot be smaller than dest"
        Combinatorics.enumNchooseM sd dd |> Seq.map (rebufo orderSource swa)

    // returns a sequence containing random
    // order reductions of the switch array
    let rndMasks 
        (orderSource: int<order>) 
        (orderDest: int<order>) 
        (swa: switch array) 
        (rnd: IRando) 
        =
        let sd, dd = (orderSource |> UMX.untag), (orderDest |> UMX.untag)
        if sd < dd then
            failwith "source order cannot be smaller than dest"
        RandVars.rndNchooseM sd dd rnd |> Seq.map (rebufo orderSource swa)
