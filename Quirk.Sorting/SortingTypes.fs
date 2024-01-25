namespace Quirk.Sorting

open Quirk.Core
open FSharp.UMX

[<Measure>] type sorterCount
[<Measure>] type sorterLength
[<Measure>] type sorterId
[<Measure>] type sorterParentId
[<Measure>] type sortableCount
[<Measure>] type sortableSetId
[<Measure>] type sortableSetCount
[<Measure>] type setOfSortableSetId
[<Measure>] type sorterSetConcatMapId
[<Measure>] type sorterSetParentMapId
[<Measure>] type sorterSetMutatorId
[<Measure>] type sorterSpeedBinSetId
[<Measure>] type sorterSetId
[<Measure>] type stageCount
[<Measure>] type stageWindowSize
[<Measure>] type switchCount
[<Measure>] type switchFrequency


type orderToSwitchCount = 
    | Record
    | For900
    | For999


module SortableCount =
    let makeSeq (sc:int<sortableCount>) = 
        seq { 0 .. ((sc |> UMX.untag ) - 1) }


module SwitchCount =

    let add (scA:int<switchCount>) (scB:int<switchCount>) =
        ((UMX.untag scA) + (UMX.untag scB)) |> UMX.tag<switchCount>

    let orderToRecordSwitchCount (ord: int<order>) =
        let d = ord |> UMX.untag
        let ct =
            match d with
            | 4 -> 5
            | 5 -> 9
            | 6 -> 12
            | 7 -> 16
            | 8 -> 19
            | 9 -> 25
            | 10 -> 29
            | 11 -> 35
            | 12 -> 39
            | 13 -> 45
            | 14 -> 51
            | 15 -> 56
            | 16 -> 60
            | 17 -> 71
            | 18 -> 77
            | 19 -> 85
            | 20 -> 91
            | 21 -> 100
            | 22 -> 107
            | 23 -> 115
            | 24 -> 120
            | 25 -> 132
            | 26 -> 139
            | 27 -> 150
            | 28 -> 155
            | 29 -> 165
            | 30 -> 172
            | 31 -> 180
            | 32 -> 65
            | 64 -> 100
            | _ -> 0
        ct |> UMX.tag<switchCount>

    let orderTo900SwitchCount (ord: int<order>) =
        let d = ord |> UMX.untag

        let ct =
            match d with
            | 6 -> 10
            | 7 -> 100
            | 8
            | 9 -> 160
            | 10
            | 11 -> 300
            | 12
            | 13 -> 400
            | 14
            | 15 -> 500
            | 16
            | 17 -> 800
            | 18
            | 19 -> 1000
            | 20
            | 21 -> 1300
            | 22
            | 23 -> 1600
            | 24
            | 25 -> 1900
            | _ -> 0
        ct |> UMX.tag<switchCount>

    let orderTo999SwitchCount (ord: int<order>) =
        let d = ord |> UMX.untag

        let ct =
            match d with
            | 6
            | 7 -> 600
            | 8
            | 9 -> 700
            | 10
            | 11 -> 800
            | 12
            | 13 -> 1000
            | 14
            | 15 -> 1200
            | 16
            | 17 -> 1600
            | 18
            | 19 -> 2000
            | 20
            | 21 -> 2200
            | 22
            | 23 -> 2600
            | 24
            | 25 -> 3000
            | 64 -> 25000
            | 128 -> 75000
            | _ -> 0
        ct  |> UMX.tag<switchCount>

    let fromOrder 
            (m:orderToSwitchCount) 
            (order:int<order>) =
        match m with
        | Record -> orderToRecordSwitchCount order
        | For900 -> orderTo900SwitchCount order
        | For999 -> orderTo999SwitchCount order

module SwitchFrequency =
    let max = 1.0 |> UMX.tag<switchFrequency>

module StageCount =

    let add (scA:int<stageCount>) (scB:int<stageCount>) =
        ((UMX.untag scA) + (UMX.untag scB)) |> UMX.tag<stageCount>


    let toSwitchCount 
            (ord: int<order>) 
            (tCt: int<stageCount>) 
         =
        (((ord |> UMX.untag) * (tCt |> UMX.untag)) / 2)
        |> UMX.tag<switchCount>


    let orderToRecordStageCount (ord: int<order>) =
        let d = ord |> UMX.untag

        let ct =
            match d with
            | 4 -> 3
            | 5
            | 6 -> 5
            | 7
            | 8 -> 6
            | 9
            | 10 -> 7
            | 11
            | 12 -> 8
            | 13
            | 14
            | 15
            | 16 -> 9
            | 17 -> 10
            | 18
            | 19
            | 20 -> 11
            | 21
            | 22
            | 23
            | 24 -> 12
            | 25
            | 26 -> 13
            | 27
            | 28
            | 29
            | 30
            | 31 -> 14
            | 32 -> 5
            | 64 -> 10
            | _ -> 0

        ct  |> UMX.tag<stageCount>


    let orderTo999StageCount (ord: int<order>) 
        =
        let d = ord |> UMX.untag

        let ct =
            match d with
            | 8
            | 9 -> 140
            | 10
            | 11
            | 12
            | 13
            | 14
            | 15 -> 160
            | 16
            | 17
            | 18
            | 19
            | 20
            | 21 -> 220
            | 22
            | 23
            | 24
            | 25 -> 240
            | 32 -> 600
            | _ -> 0

        ct  |> UMX.tag<stageCount>


    let orderTo900StageCount (ord: int<order>) 
        =
        let d = ord |> UMX.untag
        let ct =
            match d with
            | 8
            | 9 -> 35
            | 10
            | 11 -> 50
            | 12
            | 13 -> 60
            | 14
            | 15 -> 65
            | 16
            | 17 -> 95
            | 18
            | 19 -> 110
            | 20
            | 21 -> 120
            | 22
            | 23 -> 130
            | 24
            | 25 -> 140
            | _ -> 0

        ct  |> UMX.tag<stageCount>


    let fromOrder 
            (m:orderToSwitchCount) 
            (order:int<order>) =
        match m with
        | Record -> orderToRecordStageCount order
        | For900 -> orderTo900StageCount order
        | For999 -> orderTo999StageCount order


module SorterCount =

    let add (scA:int<sorterCount>) (scB:int<sorterCount>) =
        ((UMX.untag scA) + (UMX.untag scB)) |> UMX.tag<sorterCount>




module StageWindowSize =

    let ToSwitchCount 
            (ord: int<order>) 
            (tWz: int<stageWindowSize>) 
        =
        (((ord |> UMX.untag) * (tWz |> UMX.untag)) / 2)
        |> UMX.tag<stageCount>


type switchGenMode =
    | switch = 0
    | stage = 1
    | stageSymmetric = 2

module SwitchGenMode =
    let toString (sgm:switchGenMode) = 
        sgm |> string
    let fromString (cereal:string) =
        match cereal with
        | "switch" -> switchGenMode.switch |> Ok
        | "stage" -> switchGenMode.stage |> Ok
        | "stageSymmetric" -> switchGenMode.stageSymmetric |> Ok
        | _ -> $"Invalid string: { cereal } for switchGenMode" |> Error
        