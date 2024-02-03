namespace Quirk.Core

open SysExt
open Microsoft.FSharp.Core
open System
open FSharp.UMX

// a permutation of the set {0, 1,.. (order-1)}
type permutation = private { values: int[] }

module Permutation =

    let create (vals: int[]) =
        if CollectionProps.isPermutation vals then
            { permutation.values = vals } |> Ok
        else
            "not a permutation" |> Error

    let createNr (vals: int[]) =
        { permutation.values = vals }

    let create8 (b: uint8[]) = create (b |> Array.map (int))

    let create16 (b: uint16[]) = create (b |> Array.map (int))

    let getArray (perm: permutation) = perm.values

    let getOrder (perm: permutation) = perm.values.Length |> UMX.tag<order>
    
    let identity (order: int<order>) =
        { values = CollectionProps.identity (order |> UMX.untag) }

    let makeMonoCycle 
            (order: int<order>) 
            (aDex: int) 
            (bDex: int) 
        =
        { values =
            Array.init (order |> UMX.untag) (fun i ->
                if (i = aDex) then bDex
                elif (i = bDex) then aDex
                else i) }

    let makeAllMonoCycles (ord: int<order>) =
        seq {
            for i = 0 to ((ord |> UMX.untag) - 1) do
                for j = 0 to i - 1 do
                    yield makeMonoCycle ord i j
        }

    let switchVals aVal bVal (perm:permutation)  
        =
        let ov = perm |> getOrder |> UMX.untag
        let oA = perm |> getArray
        { values =
            Array.init 
                ov
                (fun i ->
                    if (oA.[i] = aVal) then bVal
                    elif (oA.[i] = bVal) then aVal
                    else oA.[i] )
        }

    let rotate (order: int<order>) (dir: int) =
        let d = (order |> UMX.untag)
        { values = Array.init d (fun i -> (i + dir) % d) }

    let powers (maxCount: int option) (perm: permutation) =
        let permSeq =
            match maxCount with
            | Some mc -> perm.values |> CollectionOps.allPowersCapped mc
            | None -> perm.values |> CollectionOps.allPowers

        permSeq |> Seq.map (fun vs -> { permutation.values = vs })


    let fullRotationGroup (order: int<order>) =
        let r1 = rotate order 1
        powers None r1 |> Seq.toArray

    // {stackedSource(0) .. stackedSource(order/2)} creates a complete
    // test set for the merge sort (merge two sorted sets of order/2)
    let stackedSource (order: int<order>) (index:int) =
        let ov = (order |> UMX.untag)
        let hov = ov / 2
        let retVal = CollectionProps.identity (hov * 2)
        let mutable dex = 0
        while dex < hov do 
            if (index = hov) then
                retVal.[dex] <- hov + dex
                retVal.[hov + dex] <- dex
             // the result is identity for index=0
            else if (index > 0) then
                if (dex < hov - index) then
                    retVal.[dex] <- index + dex
                else
                    retVal.[dex] <- hov + dex

                if (dex < index) then
                    retVal.[hov + dex] <- dex
                else
                    retVal.[hov + dex] <- hov + dex - index
            dex <- dex + 1
        retVal

    let conjugate (conj: permutation) (pA: permutation) =
        let a_out = Array.zeroCreate (conj|> getOrder |> UMX.untag)
        CollectionOps.conjIntArrays (pA |> getArray) (conj |> getArray) a_out


    let toIntSet (perm: permutation) = { intSet.values = perm.values }

    let toIntSet8 (perm: permutation) =
        { intSet8.values = perm.values |> Array.map (uint8) }

    let inRange (order: int<order>) (value: int) =
        ((value > -1) && (value < (order |> UMX.untag)))

    let inverse (perm: permutation) =
        let ia = Array.zeroCreate perm.values.Length
        { values = CollectionOps.invertArray perm.values ia }

    let isSorted (perm: permutation) =
        CollectionProps.isSorted perm.values

    let isTwoCycle (perm: permutation) = CollectionProps.isTwoCycle perm.values

    // will work without error if the permutations are the same size
    let productNr (lhs: permutation) (rhs: permutation) =
        { permutation.values =
            CollectionOps.arrayProduct (lhs.values) (rhs.values) (Array.zeroCreate lhs.values.Length) }

    let product (lhs: permutation) (rhs: permutation) =
        if (lhs.values.Length <> rhs.values.Length) then
            "permuation orders dont match (*23)" |> Error
        else
            { permutation.values =
                CollectionOps.arrayProduct (lhs.values) (rhs.values) (Array.zeroCreate lhs.values.Length) }
            |> Ok

    let getWeight (perm:permutation) =
        perm |> getArray |> Array.mapi(fun i v -> Math.Abs(i - v))
                         |> Array.sum

    let getDistance (lhs:permutation) (rhs:permutation) = 
        let delta = (inverse lhs ) |> productNr rhs
        delta |> getWeight


    let applyToBoolArray (perm:permutation) (target:bool[]) =
        let arrayLen = perm |> getOrder |> UMX.untag
        let permA = perm |> getArray
        let aRet = Array.create (target.Length) false
        for dex = 0 to (arrayLen - 1) do
            aRet.[permA.[dex]] <- target.[dex]
        aRet


    let applyToInt32 (perm:permutation) (target:int) =
        let arrayLen = perm |> getOrder |> UMX.untag
        let aa = applyToBoolArray perm (target.toBoolArray arrayLen)
        0l.applyBoolArray aa


    let permuteBitRep (perm:permutation) (target:permutation) =
        target |> getArray |> Array.map(applyToInt32 perm) |> createNr


    //*************************************************************
    //***************  byte conversions  **************************
    //*************************************************************


    //let makeFromBytes (ord:order) (data:byte[]) =
    //    ByteArray.makeFromBytes ord create8 create16 data


    //let makeArrayFromBytes (ord:order) (data:byte[]) =
    //    ByteArray.makeArrayFromBytes ord create8 create16 data


    //let toBytes (perm:permutation) =
    //    ByteArray.toBytes (perm.values)


    //let arrayToBytes (perms:permutation[]) =
    //    ByteArray.arrayToBytes (perms |> Array.map(fun p -> p.values))



    //*************************************************************
    //***************    IRando dependent   ***********************
    //*************************************************************

    let createRandom (order: int<order>) (rnd: IRando) =
        let idArray = (identity order) |> getArray
        { values = (RandVars.fisherYatesShuffle rnd idArray |> Seq.toArray) }

    let createRandoms (order: int<order>) (rnd: IRando) =
        Seq.initInfinite (fun _ -> createRandom order rnd)

    let getRandomSeparated (order: int<order>) 
            (minSeparation:int) 
            (maxSeparation: int)
            (rnd: IRando)
        =
        let mutable curPerm = createRandom order rnd 
        seq {
            yield curPerm
            while true do
               let nextPerm = createRandom order rnd
               let dist = getDistance curPerm nextPerm
               if (dist > minSeparation) && (dist < maxSeparation) then
                  yield nextPerm
                  curPerm <- nextPerm
        }

    let mutate (rnd: IRando) (perm:permutation) 
        =
        let ov = perm |> getOrder |> UMX.untag
        let sc =  ov |> uint64 |> UMX.tag<symbolSetSize>
        let aDex, bDex = RandVars.drawTwoWithoutRep sc rnd
        switchVals aDex bDex perm