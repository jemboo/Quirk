namespace Quirk.Core
open FSharp.UMX
open SysExt

[<Measure>] type order
[<Measure>] type useParallel
[<Measure>] type symbolSetSize
[<Measure>] type bitsPerSymbol
[<Measure>] type byteWidth
[<Measure>] type mutationRate
[<Measure>] type symbolCount
[<Measure>] type arrayCount
[<Measure>] type arrayLength
[<Measure>] type boundedFloat

module Order =

    let create8 (value: int) =
        if (value > 0) && (value < 9) then
            value |> UMX.tag<order> |> Ok
        else
            "order must be greater than 0 and less than 9" |> Error

    let within (b: int<order>) v = 
        (v >= 0) && (v < (b |> UMX.untag))

    let maxSwitchesPerStage (order: int<order>) = 
        (order  |> UMX.untag) / 2

    let switchCount (order: int<order>) = ((order  |> UMX.untag) * ((order  |> UMX.untag) - 1)) / 2

    let add (degs: int<order> seq) =
        degs |> Seq.map (UMX.untag) |> Seq.reduce (+)  |> UMX.tag<order>

    let reflect (ord: int<order>) (src: int) = (ord  |> UMX.untag) - src - 1

    let binExp (ord: int<order>) = (1 <<< (ord  |> UMX.untag))

    let bitMaskUint64 (ord: int<order>) = (1uL <<< (ord  |> UMX.untag)) - 1uL

    let bytesNeededFor (ord: int<order>) =
        match (ord  |> UMX.untag) with
        | x when (x < 256) -> 1
        | x when (x < 256 * 256) -> 2
        | _ -> 4

    let twoSymbolOrderedArray (order: int<order>) (hiCt: int) (hiVal: 'a) (loVal: 'a) =
        [| for i in 0 .. ((order  |> UMX.untag) - hiCt - 1) -> loVal
           for i in ((order  |> UMX.untag) - hiCt) .. ((order |> UMX.untag) - 1) -> hiVal |]

    // Returns a order + 1 length int array of
    // of all possible sorted 0-1 sequences of length order
    let allTwoSymbolOrderedArrays (order: int<order>) (hiVal: 'a) (loVal: 'a) =
        seq {
            for i = 0 to (order  |> UMX.untag) do
                yield (twoSymbolOrderedArray order i hiVal loVal)
        }


    let orderedBooleanArray (order: int<order>) (hiCt: int) =
        [| for i in 0 .. ((order  |> UMX.untag) - hiCt - 1) -> false
           for i in ((order  |> UMX.untag) - hiCt) .. ((order  |> UMX.untag) - 1) -> true |]


    // Returns a order + 1 length int array of
    // of all possible sorted boolean sequences of length order
    let allBoolArrays (order: int<order>) =
        seq {
            for i = 0 to (order  |> UMX.untag) do
                yield (orderedBooleanArray order i)
        }

    let allSortableAsInt (order: int<order>) =
        try
            let itemCt = order |> binExp
            Array.init<int> itemCt (id) |> Ok
        with ex ->
            ("error in allIntForDegree: " + ex.Message) |> Result.Error


    let allSortableAsUint64 (order: int<order>) =
        try
            let itemCt = order |> binExp
            Array.init<uint64> itemCt (uint64) |> Ok
        with ex ->
            ("error in allUint64ForDegree: " + ex.Message) |> Result.Error



module BitsPerSymbol =

    let fromSymbolSetSize (symbolSetSiz: uint64<symbolSetSize>) =
        let sc = symbolSetSiz |> UMX.untag
        (sc.leftmost_index + 1) |> UMX.tag<bitsPerSymbol>



module BoundedFloat =

    let create (value: float) =
        if (value < -1.0) then
            -1.0 |> UMX.tag<boundedFloat>
        elif (value > 1.0) then
            1.0 |> UMX.tag<boundedFloat>
        else
            value |> UMX.tag<boundedFloat>

    let addValue
            (zto:float<boundedFloat>) 
            (delta:float)
        =
        create ((zto |> UMX.untag) + delta)
