namespace Quirk.Cfg

open System
open Quirk.Core
open Quirk.Sorting
open FSharp.UMX
open Quirk.SortingOps


type sortableSetCfgType =
    | All_Bits
    | All_Bits_Reduced
    | MergeWithBits
    | MergeWithInts
    | Orbit


module SortableSetCfgType =
    let fromString (cereal:string) =
        match cereal with
        | "All_Bits" -> sortableSetCfgType.All_Bits |> Ok
        | "All_Bits_Reduced" -> sortableSetCfgType.All_Bits_Reduced |> Ok
        | "MergeWithBits" -> sortableSetCfgType.MergeWithBits |> Ok
        | "MergeWithInts" -> sortableSetCfgType.MergeWithInts |> Ok
        | "Orbit" -> sortableSetCfgType.Orbit |> Ok
        | _ -> $"{cereal}: not matched in SortableSetCfgType.fromString"
                |> Error


type sortableSetCertainCfg =
    | All_Bits of int<order>
    | All_Bits_Reduced of int<order>*array<switch>
    | MergeWithBits of int<order>
    | MergeWithInts of int<order>
    | Orbit of permutation


module SortableSetCertainCfg =

    let getOrder 
            (sscc:sortableSetCertainCfg) 
        = 
        match sscc with
        | All_Bits o -> o
        | All_Bits_Reduced (o, _) -> o
        | MergeWithBits o -> o
        | MergeWithInts o -> o
        | Orbit p -> p |> Permutation.getOrder


    let getId (cfg:sortableSetCertainCfg) 
        = 
        [| "sortableSetCertainCfg" :> obj; cfg :> obj|] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<sortableSetId>
    

    let getConfigName 
            (sscc:sortableSetCertainCfg) 
        =
        match sscc with
            | All_Bits o -> 
                sprintf "%s_%d"
                    "All"
                    (sscc |> getOrder |> UMX.untag)

            | All_Bits_Reduced (o, a) ->
                sprintf "%s_%d_%d"
                    "Reduced"
                    (sscc |> getOrder |> UMX.untag)

                    (a.Length)
            | MergeWithBits o -> 
                sprintf "%s_%d"
                    "MergeWithBits"
                    (sscc |> getOrder |> UMX.untag)

            | MergeWithInts o -> 
                sprintf "%s_%d"
                    "MergeWithInts"
                    (sscc |> getOrder |> UMX.untag)

            | Orbit perm ->
                sprintf "%s_%d_%d"
                    "Orbit"
                    (sscc |> getOrder |> UMX.untag)
                    (perm |> Permutation.powers None |> Seq.length)



    let switchReduceBits
            (ordr:int<order>)
            (sortr:sorter)
        =
        result {
            let refinedSortableSetId = 
                (ordr, (sortr |> Sorter.getSwitches))
                        |> sortableSetCertainCfg.All_Bits_Reduced
                        |> getId
            let! baseSortableSet = 
                SortableSet.makeAllBits
                    (Guid.Empty |> UMX.tag<sortableSetId>)
                    rolloutFormat.RfBs64
                    ordr

            let! sorterOpOutput = 
                SortingRollout.makeSorterOpOutput
                    sorterOpTrackMode.SwitchUses
                    baseSortableSet
                    sortr

            let! refined = sorterOpOutput
                           |> SorterOpOutput.getRefinedSortableSet
                                refinedSortableSetId
            return refined
        }


    let makeSortableSet 
            (sscc:sortableSetCertainCfg) 
        = 
        match sscc with
        | All_Bits o ->
            SortableSet.makeAllBits
                (sscc |> getId)
                rolloutFormat.RfBs64
                o

        | All_Bits_Reduced (o, switchArray) -> 
            result {
                let sorter = Sorter.fromSwitches 
                                (Guid.Empty |> UMX.tag<sorterId>) 
                                o
                                switchArray
                let! refinedSortableSet =
                        switchReduceBits o sorter

                return refinedSortableSet
            }

        | MergeWithBits o ->
            let hOrder = (( o |> UMX.untag ) / 2 ) |> UMX.tag<order>
            SortableSet.makeSortedStacks
                (sscc |> getId)
                rolloutFormat.RfBs64
                [|hOrder; hOrder|]


        | MergeWithInts o ->
            SortableSet.makeMergeSortTestWithInts
                (sscc |> getId)
                o

        | Orbit perm -> 
                SortableSet.makeOrbits
                    (sscc |> getId)
                    None
                    perm


    let makeAllBitsReducedOneStage
            (order:int<order>) 
        =
        //if (stagesReduced |> StageCount.value) > 1 then
        //    failwith "StageReduction gt 1"
        let switchArray = 
            TwoCycle.evenMode order 
                    |> Switch.fromTwoCycle
                    |> Seq.toArray
        sortableSetCertainCfg.All_Bits_Reduced (order, switchArray)



type sortableSetCfg = 
     | Certain of sortableSetCertainCfg

module SortableSetCfg =

    let getId 
            (ssCfg: sortableSetCfg) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.getId


    let makeSortableSet
            (ssCfg: sortableSetCfg) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.makeSortableSet


    let getOrder
            (ssCfg: sortableSetCfg) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.getOrder


    let getCfgName
            (ssCfg: sortableSetCfg) 
        =
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.getConfigName

    let make 
            (sortableSetCfgType:sortableSetCfgType)
            (order:int<order>)
            (permutation: permutation option)
        =
        match sortableSetCfgType with
        | sortableSetCfgType.All_Bits ->
            sortableSetCertainCfg.All_Bits order
            |> sortableSetCfg.Certain
        | sortableSetCfgType.All_Bits_Reduced ->
            SortableSetCertainCfg.makeAllBitsReducedOneStage order
            |> sortableSetCfg.Certain
        | sortableSetCfgType.MergeWithBits ->
            sortableSetCertainCfg.MergeWithBits order
            |> sortableSetCfg.Certain
        | sortableSetCfgType.MergeWithInts ->
            sortableSetCertainCfg.MergeWithInts order
            |> sortableSetCfg.Certain
        | sortableSetCfgType.Orbit ->
            match permutation with
            | Some p ->
                sortableSetCertainCfg.Orbit p
                |> sortableSetCfg.Certain
            | None -> failwith $"permuation not specified in SortableSetCfg.make"
