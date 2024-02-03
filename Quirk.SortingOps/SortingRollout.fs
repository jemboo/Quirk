﻿namespace Quirk.SortingOps

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting


module SortingRollout =

    //*********************************************************************
    //********************    Switch Track    *******************************
    //*********************************************************************

    // uses a bool[sorter.switchcount * sortableCount]
    // array to store each switch use
    let private sortAndMakeSwitchTrackForIntRoll 
            (sortr: sorter) 
            (intRoll: intRoll) 
            (sortableCount: int<sortableCount>) 
            =
        let rollArray = intRoll |> IntRoll.getData
        let orderV = sortr |> Sorter.getOrder |> UMX.untag
        let switchCountV = sortr |> Sorter.getSwitchCount |> UMX.untag
        let switches = sortr |> Sorter.getSwitches

        let switchingLg =
            SortableBySwitchTrackerStandard.create (sortr |> Sorter.getSwitchCount) sortableCount

        let rollingUseArray = switchingLg |> SortableBySwitchTrackerStandard.getData

        // loop over sortables, then over switches
        let mutable sortableIndex = 0
        while (sortableIndex < (UMX.untag sortableCount)) do
            let mutable looP = true
            let sortableSetRolloutOffset = sortableIndex * orderV
            let switchEventRolloutOffset = sortableIndex * switchCountV

            let mutable localSwitchOffset = 0

            while ((localSwitchOffset < switchCountV) && looP) do
                let switch = switches.[localSwitchOffset]
                let lv = rollArray.[switch.low + sortableSetRolloutOffset]
                let hv = rollArray.[switch.hi + sortableSetRolloutOffset]

                if (lv > hv) then
                    rollArray.[switch.hi + sortableSetRolloutOffset] <- lv
                    rollArray.[switch.low + sortableSetRolloutOffset] <- hv
                    rollingUseArray.[localSwitchOffset + switchEventRolloutOffset] <- true

                looP <-
                    ((localSwitchOffset % 20 > 0)
                     || (not (CollectionProps.isSortedOffset rollArray sortableSetRolloutOffset orderV)))

                localSwitchOffset <- localSwitchOffset + 1

            sortableIndex <- sortableIndex + 1

        switchingLg |> sortableBySwitchTracker.ArrayRoll


    // uses a bool[sorter.switchcount * sortableCount]
    // array to store each switch use
    let private sortAndMakeSwitchTrackForUInt8Roll
            (sortr: sorter)
            (uInt8Roll: uInt8Roll)
            (sortableCount: int<sortableCount>)
        =
        let rollArray = uInt8Roll |> Uint8Roll.getData
        let orderV = sortr |> Sorter.getOrder |> UMX.untag
        let switchCountV = sortr |> Sorter.getSwitchCount |> UMX.untag
        let switches = sortr |> Sorter.getSwitches

        let switchingLg =
            SortableBySwitchTrackerStandard.create (sortr |> Sorter.getSwitchCount) sortableCount

        let rollingUseArray = switchingLg |> SortableBySwitchTrackerStandard.getData

        // loop over sortables, then over switches
        let mutable sortableIndex = 0
        while (sortableIndex < (UMX.untag sortableCount)) do

            let mutable looP = true
            let mutable localSwitchOffset = 0
            let sortableSetRolloutOffset = sortableIndex * orderV
            let switchEventRolloutOffset = sortableIndex * switchCountV

            while ((localSwitchOffset < switchCountV) && looP) do
                let switch = switches.[localSwitchOffset]
                let lv = rollArray.[switch.low + sortableSetRolloutOffset]
                let hv = rollArray.[switch.hi + sortableSetRolloutOffset]

                if (lv > hv) then
                    rollArray.[switch.hi + sortableSetRolloutOffset] <- lv
                    rollArray.[switch.low + sortableSetRolloutOffset] <- hv
                    rollingUseArray.[localSwitchOffset + switchEventRolloutOffset] <- true

                looP <-
                    ((localSwitchOffset % 20 > 0)
                     || (not (CollectionProps.isSortedOffset rollArray sortableSetRolloutOffset orderV)))

                localSwitchOffset <- localSwitchOffset + 1

            sortableIndex <- sortableIndex + 1

        switchingLg |> sortableBySwitchTracker.ArrayRoll


    // uses a bool[sorter.switchcount * sortableCount]
    // array to store each switch use
    let private sortAndMakeSwitchTrackForUInt16Roll
        (sortr: sorter)
        (uInt8Roll: uInt16Roll)
        (sortableCount: int<sortableCount>)
        =
        let rollArray = uInt8Roll |> Uint16Roll.getData
        let orderV = sortr |> Sorter.getOrder |> UMX.untag
        let switchCountV = sortr |> Sorter.getSwitchCount |> UMX.untag
        let switches = sortr |> Sorter.getSwitches

        let switchingLg =
            SortableBySwitchTrackerStandard.create (sortr |> Sorter.getSwitchCount) sortableCount

        let rollingUseArray = switchingLg |> SortableBySwitchTrackerStandard.getData

        // loop over sortables, then over switches
        let mutable sortableIndex = 0
        while (sortableIndex < (UMX.untag sortableCount)) do

            let mutable looP = true
            let mutable localSwitchOffset = 0
            let sortableSetRolloutOffset = sortableIndex * orderV
            let switchEventRolloutOffset = sortableIndex * switchCountV

            while ((localSwitchOffset < switchCountV) && looP) do
                let switch = switches.[localSwitchOffset]
                let lv = rollArray.[switch.low + sortableSetRolloutOffset]
                let hv = rollArray.[switch.hi + sortableSetRolloutOffset]

                if (lv > hv) then
                    rollArray.[switch.hi + sortableSetRolloutOffset] <- lv
                    rollArray.[switch.low + sortableSetRolloutOffset] <- hv
                    rollingUseArray.[localSwitchOffset + switchEventRolloutOffset] <- true

                looP <-
                    ((localSwitchOffset % 20 > 0)
                     || (not (CollectionProps.isSortedOffset rollArray sortableSetRolloutOffset orderV)))

                localSwitchOffset <- localSwitchOffset + 1

            sortableIndex <- sortableIndex + 1

        switchingLg |> sortableBySwitchTracker.ArrayRoll


    // uses a bool[sorter.switchcount * sortableCount]
    // array to store each switch use
    let sortAndMakeSwitchTrackForBs64Roll 
            (sortr: sorter) 
            (bs64Roll: bs64Roll) 
            (sortableCount: int<sortableCount>) 
            =
        let rollArray = bs64Roll |> Bs64Roll.getData
        let orderV = sortr |> Sorter.getOrder |> UMX.untag
        let switchCountV = sortr |> Sorter.getSwitchCount |> UMX.untag
        let switches = sortr |> Sorter.getSwitches

        let switchingLg =
            SortableBySwitchTrackerBitStriped.create 
                (sortr |> Sorter.getSwitchCount) 
                sortableCount

        let switchUseArray = switchingLg |> SortableBySwitchTrackerBitStriped.getData

        // loop over sortables, then over switches
        let mutable sortableIndex = 0
        let mutable sortableSetOffset = 0

        while (sortableSetOffset < (bs64Roll |> Bs64Roll.getDataArrayLength)) do
            let switchEventOffset = sortableIndex * switchCountV
            let mutable localSwitchOffset = 0

            while (localSwitchOffset < switchCountV) do
                let switch = switches.[localSwitchOffset]
                let lv = rollArray.[switch.low + sortableSetOffset]
                let hv = rollArray.[switch.hi + sortableSetOffset]
                rollArray.[switch.hi + sortableSetOffset] <- (lv ||| hv)
                rollArray.[switch.low + sortableSetOffset] <- (lv &&& hv)

                switchUseArray.[switchEventOffset + localSwitchOffset] <- ((~~~hv) &&& lv)
                localSwitchOffset <- localSwitchOffset + 1

            sortableIndex <- sortableIndex + 1
            sortableSetOffset <- sortableSetOffset + orderV

        switchingLg |> sortableBySwitchTracker.BitStriped



    // Uses the sorter to sort a copy of the rollingSorableData.
    // returns the transformed copy of the rollingSorableData along with 
    // a bool[sorter.switchcount * sortableCount] to store each switch use.
    let applySorterAndMakeSwitchTrack 
            (sortr: sorter) 
            (sortableSt: sortableSet) 
            =
        let rolloutCopy = sortableSt |> SortableSet.getRollout |> Rollout.copy

        let sortableCount =
            rolloutCopy |> Rollout.getArrayCount |> UMX.cast<arrayCount,sortableCount>

        let switchingTrack =
            match rolloutCopy with
            | B _ -> failwith "not implemented"

            | U8 _uInt8Roll -> sortAndMakeSwitchTrackForUInt8Roll sortr _uInt8Roll sortableCount

            | U16 _uInt16Roll -> sortAndMakeSwitchTrackForUInt16Roll sortr _uInt16Roll sortableCount

            | I32 _intRoll -> sortAndMakeSwitchTrackForIntRoll sortr _intRoll sortableCount

            | U64 _uInt64Roll -> failwith "not implemented"

            | Bs64 _bs64Roll -> sortAndMakeSwitchTrackForBs64Roll sortr _bs64Roll sortableCount

        SorterOpOutput.make sortr sortableSt rolloutCopy (switchingTrack |> sorterOpTracker.SwitchUses)



    //*********************************************************************
    //********************    Switch Use Counts    ************************
    //*********************************************************************

    // uses an int[sorter.switchcount] array to accumulate
    // the counts of each switch use
    let private sortAndMakeSwitchUsesForIntRoll 
                    (sortr: sorter) 
                    (intRoll: intRoll) 
                    (sortableCount: int<sortableCount>) 
                    =
        let rollArray = intRoll |> IntRoll.getData
        let orderV = sortr |> Sorter.getOrder |> UMX.untag
        let switches = sortr |> Sorter.getSwitches
        let switchCountV = sortr |> Sorter.getSwitchCount |> UMX.untag
        let switchUses = SwitchUseCounts.init (sortr |> Sorter.getSwitchCount)
        let useCounts = (SwitchUseCounts.getUseCounters switchUses)

        let mutable sortableIndex = 0
        while (sortableIndex < (UMX.untag sortableCount)) do

            let mutable looP = true
            let mutable localSwitchOffset = 0
            let sortableSetRolloutOffset = sortableIndex * orderV

            while ((localSwitchOffset < switchCountV) && looP) do
                let switch = switches.[localSwitchOffset]
                let lv = rollArray.[switch.low + sortableSetRolloutOffset]
                let hv = rollArray.[switch.hi + sortableSetRolloutOffset]

                if (lv > hv) then
                    rollArray.[switch.hi + sortableSetRolloutOffset] <- lv
                    rollArray.[switch.low + sortableSetRolloutOffset] <- hv
                    useCounts.[localSwitchOffset] <- useCounts.[localSwitchOffset] + 1

                looP <-
                    ((localSwitchOffset % 20 > 0)
                     || (not (CollectionProps.isSortedOffset rollArray sortableSetRolloutOffset orderV)))

                localSwitchOffset <- localSwitchOffset + 1

            sortableIndex <- sortableIndex + 1

        switchUses |> switchUseTracker.Standard


    // uses an int[sorter.switchcount] array to accumulate
    // the counts of each switch use
    let private sortAndMakeSwitchUsesForUInt8Roll
        (sortr: sorter)
        (uInt8Roll: uInt8Roll)
        (sortableCount: int<sortableCount>)
        =
        let switches = sortr |> Sorter.getSwitches
        let rollArray = uInt8Roll |> Uint8Roll.getData
        let orderV = sortr |> Sorter.getOrder |> UMX.untag
        let switchUses = SwitchUseCounts.init (sortr |> Sorter.getSwitchCount)
        let useCounts = (SwitchUseCounts.getUseCounters switchUses)
        let switchCountV = sortr |> Sorter.getSwitchCount |> UMX.untag

        let mutable sortableIndex = 0
        while (sortableIndex < (UMX.untag sortableCount)) do

            let mutable looP = true
            let mutable localSwitchOffset = 0
            let sortableSetRolloutOffset = sortableIndex * orderV

            while ((localSwitchOffset < switchCountV) && looP) do
                let switch = switches.[localSwitchOffset]
                let lv = rollArray.[switch.low + sortableSetRolloutOffset]
                let hv = rollArray.[switch.hi + sortableSetRolloutOffset]

                if (lv > hv) then
                    rollArray.[switch.hi + sortableSetRolloutOffset] <- lv
                    rollArray.[switch.low + sortableSetRolloutOffset] <- hv
                    useCounts.[localSwitchOffset] <- useCounts.[localSwitchOffset] + 1

                looP <-
                    ((localSwitchOffset % 20 > 0)
                     || (not (CollectionProps.isSortedOffset rollArray sortableSetRolloutOffset orderV)))

                localSwitchOffset <- localSwitchOffset + 1

            sortableIndex <- sortableIndex + 1

        switchUses |> switchUseTracker.Standard


    // uses an int[sorter.switchcount] array to accumulate
    // the counts of each switch use
    let private sortAndMakeSwitchUsesForUInt16Roll
                    (sortr: sorter)
                    (uInt8Roll: uInt16Roll)
                    (sortableCount: int<sortableCount>) 
                    =
        let switches = sortr |> Sorter.getSwitches
        let rollArray = uInt8Roll |> Uint16Roll.getData
        let switchUses = SwitchUseCounts.init (sortr |> Sorter.getSwitchCount)
        let useCounts = (SwitchUseCounts.getUseCounters switchUses)
        let orderV = sortr |> Sorter.getOrder |> UMX.untag
        let switchCountV = sortr |> Sorter.getSwitchCount |> UMX.untag

        let mutable sortableIndex = 0
        while (sortableIndex < (UMX.untag sortableCount)) do

            let mutable looP = true
            let mutable localSwitchOffset = 0
            let sortableSetRolloutOffset = sortableIndex * orderV

            while ((localSwitchOffset < switchCountV) && looP) do
                let switch = switches.[localSwitchOffset]
                let lv = rollArray.[switch.low + sortableSetRolloutOffset]
                let hv = rollArray.[switch.hi + sortableSetRolloutOffset]

                if (lv > hv) then
                    rollArray.[switch.hi + sortableSetRolloutOffset] <- lv
                    rollArray.[switch.low + sortableSetRolloutOffset] <- hv
                    useCounts.[localSwitchOffset] <- useCounts.[localSwitchOffset] + 1

                looP <-
                    ((localSwitchOffset % 20 > 0)
                     || (not (CollectionProps.isSortedOffset rollArray sortableSetRolloutOffset orderV)))

                localSwitchOffset <- localSwitchOffset + 1

            sortableIndex <- sortableIndex + 1

        switchUses |> switchUseTracker.Standard


    let sortAndMakeSwitchUsesForBs64Roll 
                (sortr: sorter) 
                (bs64Roll: bs64Roll) 
                =
        let switches = sortr |> Sorter.getSwitches
        let switchUses = SwitchUseTrackerBitStriped.init (sortr |> Sorter.getSwitchCount)
        let rollArray = bs64Roll |> Bs64Roll.getData
        let useFlags = switchUses |> SwitchUseTrackerBitStriped.getUseFlags
        let orderV = sortr |> Sorter.getOrder |> UMX.untag
        let switchCountV = sortr |> Sorter.getSwitchCount |> UMX.untag

        let mutable sortableSetOffset = 0

        while (sortableSetOffset < (bs64Roll |> Bs64Roll.getDataArrayLength)) do
            let mutable localSwitchOffset = 0

            while (localSwitchOffset < switchCountV) do
                let switch = switches.[localSwitchOffset]
                let lv = rollArray.[switch.low + sortableSetOffset]
                let hv = rollArray.[switch.hi + sortableSetOffset]
                rollArray.[switch.hi + sortableSetOffset] <- (lv ||| hv)
                rollArray.[switch.low + sortableSetOffset] <- (lv &&& hv)

                let rv = useFlags.[localSwitchOffset]
                useFlags.[localSwitchOffset] <- (((~~~hv) &&& lv) ||| rv)
                localSwitchOffset <- localSwitchOffset + 1

            sortableSetOffset <- sortableSetOffset + orderV

        switchUses |> switchUseTracker.BitStriped


    // Uses the sorter to sort a copy of the rollingSorableData
    // returns the transformed copy of the rollingSorableData
    // along with an int[sorter.switchcount] to accumulate the
    // count of each switch use
    let applySorterAndMakeSwitchUses 
                (sortr: sorter) 
                (sortableSt: sortableSet) 
                =
        let rolloutCopy = sortableSt |> SortableSet.getRollout |> Rollout.copy
        let sortableCount =
            rolloutCopy |> Rollout.getArrayCount |> UMX.cast<arrayCount,sortableCount>

        let switchUseTracker =
            match rolloutCopy with
            | B _ -> failwith "not implemented"

            | U8 _uInt8Roll -> sortAndMakeSwitchUsesForUInt8Roll sortr _uInt8Roll sortableCount

            | U16 _uInt16Roll -> sortAndMakeSwitchUsesForUInt16Roll sortr _uInt16Roll sortableCount

            | I32 _intRoll -> sortAndMakeSwitchUsesForIntRoll sortr _intRoll sortableCount

            | U64 _uInt64Roll -> failwith "not implemented"

            | Bs64 _bs644Roll -> sortAndMakeSwitchUsesForBs64Roll sortr _bs644Roll

        SorterOpOutput.make sortr sortableSt rolloutCopy (switchUseTracker |> sorterOpTracker.SwitchTrack)


    let makeSorterOpOutput (sorterOpTrackMod: sorterOpTrackMode) 
                           (sortableSt: sortableSet) 
                           (sortr: sorter) 
                           =
        let _wrapo f sorter sortableSet =
            try
                f sorter sortableSet |> Ok
            with ex ->
                (sprintf "error in makeSorterOpOutput: %s (*88)" ex.Message) |> Result.Error

        match sorterOpTrackMod with
        | sorterOpTrackMode.SwitchTrack -> _wrapo applySorterAndMakeSwitchTrack sortr sortableSt
        | sorterOpTrackMode.SwitchUses -> _wrapo applySorterAndMakeSwitchUses sortr sortableSt
