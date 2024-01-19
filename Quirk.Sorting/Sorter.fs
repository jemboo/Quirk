namespace Quirk.Sorting

open System
open FSharp.UMX
open Quirk.Core

        
type sorter =
    private
        { sorterId: Guid<sorterId>
          order: int<order>
          switches: array<switch> }

module Sorter =

    let getSorterId (sorter: sorter) = sorter.sorterId

    let getOrder (sorter: sorter) = sorter.order

    let getSwitches (sorter: sorter) = sorter.switches

    let getSwitchCount (sorter: sorter) =
        sorter.switches.Length |> UMX.tag<sorterCount>

    let toByteArray (sorter: sorter) =
        sorter |> getSwitches 
               |> Switch.toBitPack (sorter |> getOrder) 
               |> BitPack.getData
               |> Seq.toArray


    let fromSwitches 
            (sorterD:Guid<sorterId>)
            (order: int<order>) 
            (switches: seq<switch>) =
        { sorter.sorterId = sorterD
          sorter.order = order
          sorter.switches = switches |> Seq.toArray }


    let fromSwitchesWithPrefix
            (sorterD:Guid<sorterId>)
            (order: int<order>)
            (switchCtTarget: int<switchCount>)
            (switchesPfx: seq<switch>)
            (switches: seq<switch>) =
        let combinedSwitches =
            switches
            |> Seq.append switchesPfx
            |> Seq.take (switchCtTarget |> UMX.untag)
        fromSwitches sorterD order combinedSwitches


    let fromStagesWithPrefix
            (sorterD:Guid<sorterId>)
            (order: int<order>)
            (switchCtTarget: int<switchCount>)
            (switchesPfx: seq<switch>)
            (stages: seq<stage>) =
        let switches = stages |> Seq.map (fun st -> st.switches) |> Seq.concat
        fromSwitchesWithPrefix sorterD order switchCtTarget switchesPfx switches


    // creates a longer sorter with the switches added to the end.
    let appendSwitches
            (sorterD:Guid<sorterId>)
            (switchesToAppend: seq<switch>) 
            (sorter: sorter) =
        let newSwitches = switchesToAppend |> Seq.append sorter.switches |> Seq.toArray
        fromSwitches sorterD (sorter |> getOrder) newSwitches


    // concats the switches from all of the sorters into one.
    let concatSwitches
            (sorterD:Guid<sorterId>)
            (order:int<order>)
            (sorters: sorter seq) =
        let newSwitches = sorters |> Seq.map(getSwitches) |> Seq.concat
        fromSwitches sorterD order newSwitches


    // creates a longer sorter with the switches added to the beginning.
    let prependSwitches 
            (sorterD:Guid<sorterId>)
            (newSwitches: seq<switch>) 
            (sorter: sorter) 
        =
        let newSwitches = sorter.switches |> Seq.append newSwitches |> Seq.toArray
        fromSwitches sorterD (sorter |> getOrder) newSwitches


    let removeSwitchesFromTheStart
            (sorterD:Guid<sorterId>)
            (newLength: int<switchCount>) 
            (sortr: sorter) 
        =
        let curSwitchCt = sortr |> getSwitchCount |> UMX.untag
        let numSwitchesToRemove = curSwitchCt - (newLength |> UMX.untag)

        if numSwitchesToRemove < 0 then
            "New length is longer than sorter" |> Error
        else
            let trimmedSwitches =
                sortr
                |> getSwitches
                |> Seq.skip (numSwitchesToRemove)
                |> Seq.take (newLength |> UMX.untag)

            fromSwitches sorterD (sortr |> getOrder) trimmedSwitches |> Ok


    let removeSwitchesFromTheEnd
            (sorterD:Guid<sorterId>)
            (newLength: int<switchCount>) 
            (sortr: sorter) 
        =
        let curSwitchCt = sortr |> getSwitchCount |> UMX.untag
        let numSwitchesToRemove = curSwitchCt - (newLength |> UMX.untag)

        if numSwitchesToRemove < 0 then
            "New length is longer than sorter" |> Error
        else
            let trimmedSwitches = (sortr |> getSwitches) |> Seq.take numSwitchesToRemove
            fromSwitches sorterD (sortr |> getOrder) trimmedSwitches |> Ok


    let getSwitchesFromFirstStages 
            (stageCount: int<stageCount>) 
            (sorter: sorter) 
        =
        sorter.switches
        |> Stage.fromSwitches sorter.order
        |> Seq.take (stageCount |> UMX.untag)
        |> Seq.map (fun t -> t.switches)
        |> Seq.concat


    let fromTwoCycles
            (sorterD:Guid<sorterId>)
            (order: int<order>) 
            (switchCtTarget: int<switchCount>) 
            (wPfx: switch seq) 
            (twoCycleSeq: twoCycle seq) 
        =
        let switches =
            twoCycleSeq |> Seq.map (fun tc -> Switch.fromTwoCycle tc) |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCtTarget wPfx switches


    let makeAltEvenOdd
            (sorterD:Guid<sorterId>)
            (order: int<order>) 
            (wPfx: switch seq) 
            (switchCount: int<switchCount>) 
        =
        let switches =
            TwoCycle.makeAltEvenOdd order (Permutation.identity order)
            |> Seq.map (fun tc -> Switch.fromTwoCycle tc)
            |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCount wPfx switches


    //***********  IRando dependent  *********************************

    //cross two sorters, and pad with random switches, if necessary
    let crossOver (pfxLength:int<switchCount>)
                  (finalLength:int<switchCount>)
                  (sortrPfx:sorter) 
                  (sortrSfx:sorter)
                  (finalId: Guid<sorterId>)
                  (rnd: IRando) =

            let switchesPfx = (Switch.rndNonDegenSwitchesOfOrder (sortrPfx |> getOrder) rnd) 
                              |> Seq.append sortrPfx.switches
                              |> Seq.take (pfxLength |> UMX.untag)

            let switchesSfx = (Switch.rndNonDegenSwitchesOfOrder (sortrSfx |> getOrder) rnd) 
                              |> Seq.append sortrSfx.switches
                              |> Seq.skip (pfxLength |> UMX.untag)

            let finalSwitches = switchesSfx 
                                |> Seq.append switchesPfx
                                |> Seq.take (finalLength |> UMX.untag)
            
            fromSwitches finalId (sortrPfx |> getOrder) finalSwitches


    let randomSwitches
            (order: int<order>)
            (wPfx: switch seq)
            (switchCount: int<switchCount>) 
            (rnGen: unit -> rngGen)
            (sorterD:Guid<sorterId>)
            =
        let randy = (rnGen()) |> Rando.fromRngGen
        let switches = Switch.rndNonDegenSwitchesOfOrder order randy
        fromSwitchesWithPrefix sorterD order switchCount wPfx switches


    let randomStages
        (order: int<order>)
        (switchFreq: float<switchFrequency>)
        (wPfx: switch seq)
        (switchCount: int<switchCount>) 
        (rnGen: unit -> rngGen)
        (sorterD:Guid<sorterId>)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let _switches =
            (Stage.rndSeq order switchFreq randy)
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat
        fromSwitchesWithPrefix sorterD order switchCount wPfx _switches


    let randomStages2
        (order: int<order>)
        (wPfx: switch seq)
        (switchCount: int<switchCount>)
        (rnGen: unit -> rngGen)
        (sorterD:Guid<sorterId>)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let coreTc = TwoCycle.evenMode order
        let _switches =
            (Stage.rndSeq2 coreTc randy)
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat
        fromSwitchesWithPrefix sorterD order switchCount wPfx _switches


    let randomStagesCoConj
        (order: int<order>)
        (wPfx: switch seq)
        (switchCount: int<switchCount>)
        (rnGen: unit -> rngGen) 
        (sorterD:Guid<sorterId>)
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
        (order: int<order>)
        (wPfx: switch seq)
        (switchCount: int<switchCount>)
        (rnGen: unit -> rngGen)
        (sorterD:Guid<sorterId>)
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
        (order: int<order>)
        (wPfx: switch seq)
        (switchCount: int<switchCount>)
        (rnGen: unit -> rngGen)
        (sorterD:Guid<sorterId>)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let _switches =
            (Stage.rndPermDraw coreTc perms randy)
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCount wPfx _switches


    let randomSymmetric
            (order: int<order>)
            (wPfx: switch seq)
            (switchCount: int<switchCount>)
            (rnGen: unit -> rngGen)
            (sorterD:Guid<sorterId>)
            =
        let randy = (rnGen()) |> Rando.fromRngGen
        let switches =
            (Stage.rndSymmetric order randy)
            |> Seq.map (fun st -> st.switches)
            |> Seq.concat

        fromSwitchesWithPrefix sorterD order switchCount wPfx switches


    let randomBuddies
        (stageWindowSz: int<stageWindowSize>)
        (order: int<order>)
        (wPfx: switch seq)
        (switchCount: int<switchCount>)
        (rnGen: unit -> rngGen)
        (sorterD:Guid<sorterId>)
        =
        let randy = (rnGen()) |> Rando.fromRngGen
        let switches =
            (Stage.rndBuddyStages stageWindowSz SwitchFrequency.max order randy List.empty)
            |> Seq.collect (fun st -> st.switches |> List.toSeq)

        fromSwitchesWithPrefix sorterD order switchCount wPfx switches

