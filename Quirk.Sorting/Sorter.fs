﻿namespace global

open System

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
        
type sorter =
    private
        { sorterId: sorterId
          order: order
          switches: array<switch> }

module Sorter =

    let getSorterId (sorter: sorter) = sorter.sorterId

    let getOrder (sorter: sorter) = sorter.order

    let getSwitches (sorter: sorter) = sorter.switches

    let getSwitchCount (sorter: sorter) =
        sorter.switches.Length |> SwitchCount.create

    let toByteArray (sorter: sorter) =
        sorter |> getSwitches 
               |> Switch.toBitPack (sorter |> getOrder) 
               |> BitPack.getData
               |> Seq.toArray


    let fromSwitches 
            (sorterD:sorterId)
            (order: order) 
            (switches: seq<switch>) =
        { sorter.sorterId = sorterD
          sorter.order = order
          sorter.switches = switches |> Seq.toArray }


    let fromSwitchesWithPrefix
            (sorterD:sorterId)
            (order: order)
            (switchCtTarget: switchCount)
            (switchesPfx: seq<switch>)
            (switches: seq<switch>) =
        let combinedSwitches =
            switches
            |> Seq.append switchesPfx
            |> Seq.take (switchCtTarget |> SwitchCount.value)
        fromSwitches sorterD order combinedSwitches


    let fromStagesWithPrefix
            (sorterD:sorterId)
            (order: order)
            (switchCtTarget: switchCount)
            (switchesPfx: seq<switch>)
            (stages: seq<stage>) =
        let switches = stages |> Seq.map (fun st -> st.switches) |> Seq.concat
        fromSwitchesWithPrefix sorterD order switchCtTarget switchesPfx switches


    // creates a longer sorter with the switches added to the end.
    let appendSwitches
            (sorterD:sorterId)
            (switchesToAppend: seq<switch>) 
            (sorter: sorter) =
        let newSwitches = switchesToAppend |> Seq.append sorter.switches |> Seq.toArray
        fromSwitches sorterD (sorter |> getOrder) newSwitches


    // concats the switches from all of the sorters into one.
    let concatSwitches
            (sorterD:sorterId)
            (order:order)
            (sorters: sorter seq) =
        let newSwitches = sorters |> Seq.map(getSwitches) |> Seq.concat
        fromSwitches sorterD order newSwitches


    // creates a longer sorter with the switches added to the beginning.
    let prependSwitches 
            (sorterD:sorterId)
            (newSwitches: seq<switch>) 
            (sorter: sorter) 
        =
        let newSwitches = sorter.switches |> Seq.append newSwitches |> Seq.toArray
        fromSwitches sorterD (sorter |> getOrder) newSwitches


    let removeSwitchesFromTheStart
            (sorterD:sorterId)
            (newLength: switchCount) 
            (sortr: sorter) 
        =
        let curSwitchCt = sortr |> getSwitchCount |> SwitchCount.value
        let numSwitchesToRemove = curSwitchCt - (SwitchCount.value newLength)

        if numSwitchesToRemove < 0 then
            "New length is longer than sorter" |> Error
        else
            let trimmedSwitches =
                sortr
                |> getSwitches
                |> Seq.skip (numSwitchesToRemove)
                |> Seq.take (newLength |> SwitchCount.value)

            fromSwitches sorterD (sortr |> getOrder) trimmedSwitches |> Ok


    let removeSwitchesFromTheEnd
            (sorterD:sorterId)
            (newLength: switchCount) 
            (sortr: sorter) 
        =
        let curSwitchCt = sortr |> getSwitchCount |> SwitchCount.value
        let numSwitchesToRemove = curSwitchCt - (SwitchCount.value newLength)

        if numSwitchesToRemove < 0 then
            "New length is longer than sorter" |> Error
        else
            let trimmedSwitches = (sortr |> getSwitches) |> Seq.take numSwitchesToRemove
            fromSwitches sorterD (sortr |> getOrder) trimmedSwitches |> Ok


    let getSwitchesFromFirstStages 
            (stageCount: stageCount) 
            (sorter: sorter) 
        =
        sorter.switches
        |> Stage.fromSwitches sorter.order
        |> Seq.take (StageCount.value stageCount)
        |> Seq.map (fun t -> t.switches)
        |> Seq.concat


    let fromTwoCycles
            (sorterD:sorterId)
            (order: order) 
            (switchCtTarget: switchCount) 
            (wPfx: switch seq) 
            (twoCycleSeq: twoCycle seq) 
        =
        let switches =
            twoCycleSeq |> Seq.map (fun tc -> Switch.fromTwoCycle tc) |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCtTarget wPfx switches


    let makeAltEvenOdd
            (sorterD:sorterId)
            (order: order) 
            (wPfx: switch seq) 
            (switchCount: switchCount) 
        =
        let switches =
            TwoCycle.makeAltEvenOdd order (Permutation.identity order)
            |> Seq.map (fun tc -> Switch.fromTwoCycle tc)
            |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCount wPfx switches


    //***********  IRando dependent  *********************************

    //cross two sorters, and pad with random switches, if necessary
    let crossOver (pfxLength:switchCount)
                  (finalLength:switchCount)
                  (sortrPfx:sorter) 
                  (sortrSfx:sorter)
                  (finalId: sorterId)
                  (rnd: IRando) =

            let switchesPfx = (Switch.rndNonDegenSwitchesOfOrder (sortrPfx |> getOrder) rnd) 
                              |> Seq.append sortrPfx.switches
                              |> Seq.take (pfxLength |> SwitchCount.value)

            let switchesSfx = (Switch.rndNonDegenSwitchesOfOrder (sortrSfx |> getOrder) rnd) 
                              |> Seq.append sortrSfx.switches
                              |> Seq.skip (pfxLength |> SwitchCount.value)

            let finalSwitches = switchesSfx 
                                |> Seq.append switchesPfx
                                |> Seq.take (finalLength |> SwitchCount.value)
            
            fromSwitches finalId (sortrPfx |> getOrder) finalSwitches


    let randomSwitches
            (order: order)
            (wPfx: switch seq)
            (switchCount: switchCount) 
            (rnGen: unit -> rngGen)
            (sorterD:sorterId)
            =
        let randy = (rnGen()) |> Rando.fromRngGen
        let switches = Switch.rndNonDegenSwitchesOfOrder order randy
        fromSwitchesWithPrefix sorterD order switchCount wPfx switches


    let randomStages
        (order: order)
        (switchFreq: switchFrequency)
        (wPfx: switch seq)
        (switchCount: switchCount) 
        (rnGen: unit -> rngGen)
        (sorterD:sorterId)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let _switches =
            (Stage.rndSeq order switchFreq randy)
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat
        fromSwitchesWithPrefix sorterD order switchCount wPfx _switches


    let randomStages2
        (order: order)
        (wPfx: switch seq)
        (switchCount: switchCount)
        (rnGen: unit -> rngGen)
        (sorterD:sorterId)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let coreTc = TwoCycle.evenMode order
        let _switches =
            (Stage.rndSeq2 coreTc randy)
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat
        fromSwitchesWithPrefix sorterD order switchCount wPfx _switches


    let randomStagesCoConj
        (order: order)
        (wPfx: switch seq)
        (switchCount: switchCount)
        (rnGen: unit -> rngGen) 
        (sorterD:sorterId)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let _switches =
            (Stage.rndSeqCoConj order randy)
            |> Seq.concat
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCount wPfx _switches


    let randomStagesSeparated
        (minSeparation: int)
        (maxSeparation: int)
        (order: order)
        (wPfx: switch seq)
        (switchCount: switchCount)
        (rnGen: unit -> rngGen)
        (sorterD:sorterId)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let _switches =
            (Stage.rndSeqSeparated order minSeparation maxSeparation randy)
            |> Seq.concat
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCount wPfx _switches


    let randomPermutaionChoice
        (coreTc:twoCycle)
        (perms: permutation[])
        (order: order)
        (wPfx: switch seq)
        (switchCount: switchCount)
        (rnGen: unit -> rngGen)
        (sorterD:sorterId)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let _switches =
            (Stage.rndPermDraw coreTc perms randy)
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCount wPfx _switches


    let randomSymmetric
            (order: order)
            (wPfx: switch seq)
            (switchCount: switchCount)
            (rnGen: unit -> rngGen)
            (sorterD:sorterId)
            =
        let randy = (rnGen()) |> Rando.fromRngGen
        let switches =
            (Stage.rndSymmetric order randy)
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCount wPfx switches


    let randomBuddies
        (stageWindowSz: stageWindowSize)
        (order: order)
        (wPfx: switch seq)
        (switchCount: switchCount)
        (rnGen: unit -> rngGen)
        (sorterD:sorterId)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let switches =
            (Stage.rndBuddyStages stageWindowSz SwitchFrequency.max order randy List.empty)
            |> Seq.collect (fun st -> st.switches |> List.toSeq)

        fromSwitchesWithPrefix sorterD order switchCount wPfx switches

