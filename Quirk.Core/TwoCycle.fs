﻿namespace Quirk.Core

open System

// a permutation of the set {0, 1,.. (order-1)}, that is it's own inverse
type twoCycle = private { values: int[] }

module TwoCycle =

    //*************************************************************
    //*******************  ctors  ******************************
    //*************************************************************

    let create (vals: int[]) =
        if CollectionProps.isTwoCycle vals then
            { twoCycle.values = vals } |> Ok
        else
            "not a two cycle" |> Error

    let createNr (vals: int[]) =
        { twoCycle.values = vals }

    let fromPerm (perm: permutation) = create perm.values

    let toPerm (tc: twoCycle) = { permutation.values = tc.values }

    let create8 (b: uint8[]) = create (b |> Array.map (int))

    let create16 (b: uint16[]) = create (b |> Array.map (int))

    let getArray (tc: twoCycle) = tc.values

    let getOrder (tc: twoCycle) = Order.createNr tc.values.Length

    let identity  (ord: order) =
        { twoCycle.values = [| 0 .. (Order.value ord) - 1 |] }

    let isSorted (tc: twoCycle) =
        CollectionProps.isSorted tc.values

    let makeMonoCycle (order: order) (aDex: int) (bDex: int) =
        { values = 
            Permutation.makeMonoCycle order aDex bDex
                   |> Permutation.getArray }

    let makeAllMonoCycles (ord: order) =
        Permutation.makeAllMonoCycles ord
        |> Seq.map(Permutation.getArray >> createNr)

    let makeReflection (tc: twoCycle) =
        let _ref pos = Order.reflect (getOrder tc) pos
        { values = Array.init (tc.values.Length) (fun dex -> tc.values.[_ref dex] |> _ref) }


    //*************************************************************
    //*******************  operators  *****************************
    //*************************************************************


    let conjugate (perm: permutation) (tc: twoCycle) =
        { twoCycle.values =
            CollectionOps.conjIntArrays
                (Permutation.getArray perm)
                (getArray tc)
                (Array.zeroCreate perm.values.Length) }


    let conjugateBitRep (tc: twoCycle) (perm: permutation) =
        let oId = Permutation.identity (getOrder tc)
        let conjLhs = Permutation.permuteBitRep perm oId
        conjugate conjLhs tc


    let reflect (tcp: twoCycle) =
        let _refV pos =
            Order.reflect (Order.createNr tcp.values.Length) pos

        let _refl =
            Array.init (tcp.values.Length) (fun dex -> tcp.values.[_refV dex] |> _refV)

        { twoCycle.values = _refl }




    //*************************************************************
    //*******************  properties  ****************************
    //*************************************************************


    let isATwoCycle (tcp: twoCycle) =
        tcp |> getArray |> CollectionProps.isTwoCycle


    let hasAfixedPoint (tcp: twoCycle) =
        tcp.values |> Seq.mapi (fun dex v -> dex = v) |> Seq.contains (true)


    let isRflSymmetric (tcp: twoCycle) = tcp = (makeReflection tcp)


    let totalSwitchBarLengths (tcp: twoCycle) =
        tcp.values |> Array.mapi(fun dex v -> Math.Abs(dex - v))
                   |> Array.sum
                   |> (/) <| 2

    //*************************************************************
    //*******************  mutators  ******************************
    //*************************************************************

    let mutateByPair (pair: int * int) (tcp: twoCycle) =
        let tcpA = tcp |> getArray |> Array.copy
        let a, b = pair
        let c = tcpA.[a]
        let d = tcpA.[b]

        if (a = c) && (b = d) then
            tcpA.[a] <- b
            tcpA.[b] <- a
        elif (a = c) then
            tcpA.[a] <- b
            tcpA.[b] <- a
            tcpA.[d] <- d
        elif (b = d) then
            tcpA.[a] <- b
            tcpA.[b] <- a
            tcpA.[c] <- c
        else
            tcpA.[a] <- b
            tcpA.[c] <- d
            tcpA.[b] <- a
            tcpA.[d] <- c

        { twoCycle.values = tcpA }


    let mutateByReflPair (pairs: seq<(int * int)>) (tcp: twoCycle) =
        let ord = tcp.values.Length |> Order.createNr
        //true if _mutato will always turn this into another twoCyclePerm
        let _isMutatoCompatable (mut: twoCycle) =
            (CollectionProps.isTwoCycle mut.values)
            && (isRflSymmetric mut)
            && not (hasAfixedPoint mut)

        let _mutato (pair: int * int) =
            let tca = tcp |> getArray |> Array.copy
            let pA, pB = pair
            let tpA, tpB = tca.[pA], tca.[pB]
            let rA, rB = (pA |> Order.reflect ord), (pB |> Order.reflect ord)
            let rtA, rtB = (tpA |> Order.reflect ord), (tpB |> Order.reflect ord)

            tca.[pA] <- tpB
            tca.[tpB] <- pA

            tca.[pB] <- tpA
            tca.[tpA] <- pB

            tca.[rB] <- rtA
            tca.[rtA] <- rB

            tca.[rA] <- rtB
            tca.[rtB] <- rA

            { twoCycle.values = tca }

        let muts =
            pairs
            |> Seq.map (fun pr -> _mutato pr)
            |> Seq.take 5
            |> Seq.filter (_isMutatoCompatable)
            |> Seq.toList

        if muts.Length > 0 then
            muts.[0]
        else
            { tcp with values = tcp |> getArray |> Array.copy}




    //*************************************************************
    //*******************  generators  ****************************
    //*************************************************************


    // does not error - ignores bad inputs
    let makeFromTupleSeq (ord: order) (tupes: seq<int * int>) =
        let curPa = [| 0 .. (Order.value ord) - 1 |]

        let _validTupe t =
            ((fst t) <> (snd t)) && (Order.within ord (fst t)) && (Order.within ord (snd t))

        let _usableTup t =
            (curPa.[fst (t)] = fst (t)) && (curPa.[snd (t)] = snd (t))

        let _opPa tup =
            if (_validTupe tup) && (_usableTup tup) then
                curPa.[fst (tup)] <- snd (tup)
                curPa.[snd (tup)] <- fst (tup)

        tupes |> Seq.iter (_opPa)
        { twoCycle.values = curPa }



    let evenMode (order: order) =
        let d = (Order.value order)
        let dm = if (d % 2 > 0) then d - 1 else d

        let yak p =
            if p = dm then p
            else if (p % 2 = 0) then p + 1
            else p - 1

        { twoCycle.values = Array.init d (yak) }


    let oddMode (order: order) =
        let d = (Order.value order)
        let dm = if (d % 2 = 0) then d - 1 else d

        let yak p =
            if p = dm then p
            else if p = 0 then 0
            else if (p % 2 = 0) then p - 1
            else p + 1

        { twoCycle.values = Array.init d (yak) }


    let oddModeFromEvenDegreeWithCap (order: order) =
        let d = (Order.value order)

        let yak p =
            if p = 0 then d - 1
            else if p = d - 1 then 0
            else if (p % 2 = 0) then p - 1
            else p + 1

        { twoCycle.values = Array.init d (yak) }


    let oddModeWithCap (order: order) =
        let d = (Order.value order)

        if (d % 2 = 0) then
            oddModeFromEvenDegreeWithCap order
        else
            oddMode order


    let makeAltEvenOdd (order: order) (conj: permutation) =
        seq {
            while true do
                yield conjugate conj (evenMode order)
                yield conjugate conj (oddModeWithCap order)
        }

    let coConjugate (perm: permutation) =
        let order = Order.createNr perm.values.Length 
        seq {
            yield conjugate perm (evenMode order) 
            yield conjugate perm (oddModeWithCap order)
        }

    let bitConjugateEvenMode 
            (order:order) 
            (perms: permutation seq) 
        =
        let bread = evenMode order
        let _conjer  (perm: permutation) = 
            conjugateBitRep bread perm
    
        perms |> Seq.map(_conjer)


    //*************************************************************
    //***************    IRando dependent   ***********************
    //*************************************************************

    let makeRndMonoCycle (order: order) (rnd: IRando) =
        let sc = order |> Order.value |> uint64 |> SymbolSetSize.createNr
        let tup = RandVars.drawTwoWithoutRep sc rnd
        makeMonoCycle order (fst tup) (snd tup)


    let rndPartialTwoCycle (order: order) (cycleCount: int) (rnd: IRando) =
        { twoCycle.values = RandVars.rndPartialTwoCycle rnd order cycleCount }


    let rndFullTwoCycle (order: order) (rnd: IRando) =
        { values = RandVars.rndFullTwoCycle rnd order }


    let rndConj (tc: twoCycle) (rnd:IRando) = 
        let bread = Permutation.createNr (RandVars.randomPermutation rnd (tc |> getOrder))
        conjugate bread tc


    let rndCoConj (order: order) (rnd:IRando) = 
        let bread = Permutation.createNr (RandVars.randomPermutation rnd order)
        coConjugate bread

    let rndSeqSeparated (order: order) (tc: twoCycle) 
                    (minSeparation: int) (maxSeparation: int) (rnd:IRando) = 
        Permutation.getRandomSeparated order minSeparation maxSeparation rnd
        |> Seq.map(fun perm -> conjugate perm tc)


    let rndSymmetric (ord: order) (rnd: IRando) =
        let deg = (Order.value ord)
        let aRet = Array.init deg (id)

        let chunkRi (rfls: switchRfl) =
            match rfls with
            | Single (i, j, d) ->
                aRet.[i] <- j
                aRet.[j] <- i

            | Unreflectable (i, j, d) ->
                aRet.[i] <- j
                aRet.[j] <- i

            | Pair ((h, i), (j, k), d) ->
                aRet.[i] <- h
                aRet.[h] <- i
                aRet.[j] <- k
                aRet.[k] <- j

            | LeftOver (i, j, d) ->
                aRet.[i] <- j
                aRet.[j] <- i

        SwitchRfl.rndReflectivePairs ord rnd |> Seq.iter (chunkRi)
        { twoCycle.values = aRet }

