﻿namespace Quirk.Core
open SysExt
open System


module CollectionOps =

    let cartesianProduct 
            (seq_a: seq<'a>) 
            (seq_b: seq<'b>) 
        =
        seq {  
            for ae in seq_a do
               for be in seq_b do
                  yield (ae, be)
        }

    let rec crossProduct lists =
        match lists with
        | [] -> [[]]  // Base case: an empty list yields a list with an empty list
        | hd::tl ->
            // Recursive case: compute the cross product of the tail, then combine with the head
            [for x in hd do
                for rest in crossProduct tl do
                    yield x :: rest]


    let getItemsAtIndexes (indexes: int array)  (sequence: seq<'a>) =
        seq {
            for index in indexes do
                if index >= 0 then
                    let enumerator = sequence.GetEnumerator()
                    let mutable currentIndex = 0

                    while currentIndex <= index && enumerator.MoveNext() do
                        if currentIndex = index then
                            yield enumerator.Current
                        currentIndex <- currentIndex + 1
                else
                    failwith "Index must be non-negative"
        }


    // returns a sequence of items that occur more than once
    let itemsOccuringMoreThanOnce items =
        seq {
            let d = System.Collections.Generic.Dictionary()
            for i in items do
                match d.TryGetValue(i) with
                | false, _ -> d.[i] <- false // first observance
                | true, false ->
                    d.[i] <- true
                    yield i // second observance
                | true, true -> () // already seen at least twice
        }


    let getItemsUpToMaxTimes<'k,'v when 'k:equality> 
            (lookup: 'v->'k)
            (max:int) 
            (items:'v seq)  =
        seq {
            let d = System.Collections.Generic.Dictionary()
            for i in items do
                let key = lookup i
                match d.TryGetValue(key) with
                | false, _ ->
                    d.[key] <- 1
                    yield i
                | true, ct ->
                    d.[key] <- ct + 1
                    if (ct < max) then
                        yield i
        }

    let bookMarkArrays<'a> 
            (arrs:'a[][]) 
            =
        let mutable acc = 0
        let mutable j = 0
        let bookMarks = Array.zeroCreate (arrs.GetLength 0)
        for h in arrs do
            acc <- acc + h.Length
            bookMarks.[j] <- acc
            j <- j + 1
        bookMarks, (arrs |> Array.concat)


    let deBookMarkArray<'a> 
            (bookMarks:int[]) 
            (arr:'a[]) 
            =
        seq {
            let mutable dex = 0
            let mutable lastEnd = 0
            while (dex < bookMarks.Length) do
                let curRay = Array.create (bookMarks.[dex] - lastEnd)  Unchecked.defaultof<'a>
                Array.Copy(arr, lastEnd, curRay, 0, curRay.Length)
                yield curRay
                lastEnd <- bookMarks.[dex]
                dex <- dex + 1
        }

    //takes up to maxCt items from the sequence, returns less if it runs out
    let takeUpto<'a> 
            (maxCt: int) 
            (source: seq<'a>) 
            =
        source
        |> Seq.mapi (fun dex v -> (dex, v))
        |> Seq.takeWhile (fun tup -> (fst tup) < maxCt)
        |> Seq.map (snd)

    //takes up to maxCt items from the array, returns less if it runs out
    let takeUptoArray<'a> 
            (maxCt: int) 
            (source: array<'a>) 
            =
        if source.Length < maxCt then
            source
        else
            source |> Array.take(maxCt)


    let infinteLoop 
            (robbin:seq<'a>) 
            = 
        seq {while true do yield! robbin}


    // product map composition: a(b()).
    let inline arrayProduct< ^a when ^a:(static member op_Explicit:^a->int)>
                    (lhs: array<^a>) 
                    (rhs: array<^a>) 
                    (prod: array<^a>) 
               =
        let dmax = lhs.Length
        let mutable curdex = 0
        while curdex < dmax do
            prod.[curdex] <- lhs.[rhs.[curdex] |> int]
            curdex <- curdex + 1
        prod

        
    let inline arrayProductR< ^a when ^a:(static member op_Explicit:^a->int)>
                    (lhs: array<^a>) 
                    (rhs: array<^a>) 
                    (prod: array<^a>) 
                =
        try
            arrayProduct lhs rhs prod |> Ok
        with ex ->
            ("error in arrayProductR: " + ex.Message) |> Result.Error


    let allPowers   
            (a_core: array<int>) 
        =
        seq {
            let mutable _continue = true
            let mutable a_cur = Array.copy a_core
            yield a_cur

            while _continue do
                let a_next = Array.zeroCreate a_cur.Length
                a_cur <- arrayProduct a_core a_cur a_next
                _continue <- not (CollectionProps.isIdentity a_next)
                yield a_cur
        }

    let allPowersCapped 
            (maxCount: int) 
            (a_core: array<int>) 
        =
        seq {
            let mutable _continue = true
            let mutable dex = 0
            let mutable a_cur = Array.copy a_core

            while (_continue && (dex < maxCount)) do
                yield a_cur
                let a_next = Array.zeroCreate a_cur.Length
                a_cur <- arrayProduct a_core a_cur a_next
                _continue <- not (CollectionProps.isIdentity a_next)
                dex <- dex + 1
        }

    let filterByPickList 
            (data: 'a[]) 
            (picks: bool[]) 
        =
        try
            let pickCount = picks |> Array.map (fun v -> if v then 1 else 0) |> Array.sum
            let filtAr = Array.zeroCreate pickCount
            let mutable newDex = 0

            for i = 0 to (data.Length - 1) do
                if picks.[i] then
                    filtAr.[newDex] <- data.[i]
                    newDex <- newDex + 1

            filtAr |> Ok
        with ex ->
            ("error in filterByPickList: " + ex.Message) |> Result.Error


    let inline invertArray< ^a when ^a:(static member Zero:^a) and 
                                    ^a:(static member One:^a) and  
                                    ^a:(static member (+):^a* ^a -> ^a) and 
                                    ^a:(static member op_Explicit:^a->int)>
                (ar: array<^a>) 
                (inv_out: array<^a>) 
        =
        let mutable iv = zero_of ar.[0]
        let incr = one_of ar.[0]          
        for i = 0 to ar.Length - 1 do
            inv_out.[ar.[i] |> int] <- iv
            iv <- iv + incr
        inv_out


    let invertArrayR 
            (a: array<int>) 
            (inv_out: array<int>) 
            =
        try
            invertArray a inv_out |> Ok
        with ex ->
            ("error in inverseMapArray: " + ex.Message) |> Result.Error


    let histogram<'d, 'r when 'r: comparison> 
            (keymaker: 'd -> 'r) 
            (qua: seq<'d>) 
            =
        qua
        |> Seq.fold
            (fun acc fv ->
                let kk = keymaker fv

                if Map.containsKey kk acc then
                    Map.add kk (acc.[kk] + 1) acc
                else
                    Map.add kk 1 acc)
            Map.empty



    let conjIntArrays 
            (a_bread: array<int>)
            (a_core: array<int>)
            (a_out: array<int>)
        =
        let breadInv = Array.zeroCreate a_bread.Length |> invertArray a_bread
        let step1 = Array.zeroCreate a_bread.Length |> arrayProduct a_core breadInv
        Array.zeroCreate a_bread.Length |> arrayProduct a_bread step1


    // a_conj * a_core * (a_conj ^ -1)
    let conjIntArraysR
            (a_conj: array<int>)
            (a_core: array<int>)
        =
        try
            let a_out = Array.zeroCreate a_conj.Length
            conjIntArrays a_conj a_core a_out |> Ok
        with ex ->
            ("error in conjIntArrays: " + ex.Message) |> Result.Error




    //*************************************************************
    //***********    Array Stacking    ****************************
    //*************************************************************

    let stack (lowTohi: 'a[] seq) = lowTohi |> Seq.concat |> Seq.toArray

    let comboStack
            (subSeqs: 'a[][] seq)
            =
        let rec _cart LL =
            match LL with
            | [] -> Seq.singleton []
            | L :: Ls ->
                seq {
                    for x in L do
                        for xs in _cart Ls -> x :: xs
                }

        _cart (subSeqs |> Seq.toList) |> Seq.map (stack)


    let stackSortedBlocksOfTwoSymbols 
            (blockSizes: int<order> seq)
            (hival: 'a)
            (lowval: 'a)
            =
        let _allSorted (deg: int<order>) =
            Order.allTwoSymbolOrderedArrays deg hival lowval

        blockSizes |> Seq.map (_allSorted >> Seq.toArray) |> comboStack


    let stackSortedBlocks 
            (blockSizes: int<order> seq) 
            =
        let _allSorted (deg: int<order>) =
            Order.allTwoSymbolOrderedArrays deg true false

        blockSizes |> Seq.map (_allSorted >> Seq.toArray) |> comboStack



    //*************************************************************
    //************    Split Sequence   ****************************
    //*************************************************************

    let chunkByDelimiter<'a> 
            (strm: seq<'a>)
            (delim: 'a -> bool)
            =
        let swEnumer = strm.GetEnumerator()
        let mutable rz = ResizeArray()

        seq {
            while swEnumer.MoveNext() do
                rz.Add swEnumer.Current

                if delim swEnumer.Current then
                    yield rz.ToArray()
                    rz <- ResizeArray()
        }


    // returns an array of length chunkSz, which is made by converting vals to a
    // 2d array with chunkSz columns, and then summing over each column.
    let wrapAndSumCols
            (chunkSz: int)
            (vals: seq<int>)
            =
        let addArrays (a: int[]) (b: int[]) =
            Array.init a.Length (fun dex -> a.[dex] + b.[dex])

        vals |> Seq.chunkBySize chunkSz |> Seq.toArray |> Array.reduce addArrays


