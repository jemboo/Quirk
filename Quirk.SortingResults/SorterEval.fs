namespace Quirk.SortingResults

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open Quirk.SortingOps

type sorterPerf = 
    | IsSuccessful of bool 
    | SortedSetSize of int<sortableCount>

module SorterPerf =
    let isSuccessful (ordr:int<order>) (sorterPrf:sorterPerf) =
        match sorterPrf with
        | IsSuccessful bv -> bv
        | SortedSetSize ssz -> (ssz |> UMX.untag) < (ordr |> UMX.untag) + 1

    let report (sp: sorterPerf option)
        =
        match sp with
        | Some perf ->
            match perf with
            | IsSuccessful bv ->
                sprintf "%b\t-" bv
            | SortedSetSize sz ->
                sprintf "-\t%d" (sz |> UMX.untag)
        | None -> "-\t-"

    let getProps (sp : sorterPerf option) =
        match sp with
        | Some perf ->
            match perf with
            | IsSuccessful bv ->
                (bv |> string, "")
            | SortedSetSize sz ->
                ("", sz |> UMX.untag |> string)
        | None -> ("", "")



type sorterEvalProps =
     | ErrorMsg 
     | Phenotype
     | SortableSetId
     | SortedSetSize
     | SorterId
     | StageCount
     | Success
     | SwitchCount


type sorterEvalMode = 
    | DontCheckSuccess
    | CheckSuccess 
    | GetSortedSetCount

module SorterEvalMode =
    let fromString (sv: string) =
        match sv with
        | "DontCheckSuccess" -> Ok DontCheckSuccess
        | "CheckSuccess" -> Ok CheckSuccess
        | "GetSortedSetCount" -> Ok GetSortedSetCount
        | _ -> Error "Invalid sorterEvalMode string (*1)"

    let toString (mode: sorterEvalMode) =
        match mode with
        | DontCheckSuccess -> "DontCheckSuccess"
        | CheckSuccess -> "CheckSuccess"
        | GetSortedSetCount -> "GetSortedSetCount"


type sorterEval =
    private
        { errorMessage: string option
          switchUseCts: switchUseCounts option
          sorterSpeed: sorterSpeed option
          sorterPrf: sorterPerf option
          sortrPhenotypeId: Guid<sorterPhenotypeId> option
          sortableSetId: Guid<sortableSetId>
          sortrId: Guid<sorterId> }


module SorterEval =
    let make (errorMsg:string option)
             (switchUseCts: switchUseCounts option) 
             (sorterSpeed: sorterSpeed option) 
             (sorterPrf: sorterPerf option) 
             (sortrPhenotypeId: Guid<sorterPhenotypeId> option)
             (sortableStId: Guid<sortableSetId>)
             (sortrId: Guid<sorterId>) 
        =
        { errorMessage = errorMsg
          switchUseCts = switchUseCts
          sorterSpeed = sorterSpeed
          sorterPrf = sorterPrf
          sortrPhenotypeId = sortrPhenotypeId
          sortableSetId = sortableStId
          sortrId = sortrId }

    let getErrorMessage (sorterEvl:sorterEval) =
        sorterEvl.errorMessage
    let getSwitchUseCounts (sorterEvl:sorterEval) =
        sorterEvl.switchUseCts
    let getSorterSpeed (sorterEvl:sorterEval) =
        sorterEvl.sorterSpeed
    let getSorterPerf (sorterEvl:sorterEval) =
        sorterEvl.sorterPrf
    let getSortrPhenotypeId (sorterEvl:sorterEval) =
        sorterEvl.sortrPhenotypeId
    let getSortableSetId (sorterEvl:sorterEval) =
        sorterEvl.sortableSetId
    let getSorterId (sorterEvl:sorterEval) =
        sorterEvl.sortrId

    let shouldRetest 
        (sorterEvalMod:sorterEvalMode) 
        (sorterEvl:sorterEval)
        : bool =
        if (sorterEvl.errorMessage |> Option.isSome)
            then false else
        match sorterEvalMod with
        | CheckSuccess -> (sorterEvl.sorterPrf |> Option.isNone)
        | DontCheckSuccess -> (sorterEvl.sorterSpeed |> Option.isNone)
        | GetSortedSetCount -> 
                match sorterEvl.sorterPrf with  
                | None -> true
                | Some perf -> 
                    match perf with
                    | IsSuccessful _ -> true
                    | sorterPerf.SortedSetSize _ -> false


    let failedForSure
        (sorterEvl:sorterEval)
        : bool =
        if (sorterEvl.errorMessage |> Option.isSome)
            then true
        else
            match sorterEvl.sorterPrf with  
            | None -> false
            | Some perf -> 
                match perf with
                | IsSuccessful sOrf -> not sOrf
                | sorterPerf.SortedSetSize _ -> false


    let evalSorterWithSortableSet 
            (sorterPerfEvalMod: sorterEvalMode) 
            (sortableSt: sortableSet) 
            (sortr: sorter) 
        : sorterEval =

        let _addSorterToErrorResultCase sortr resA =
            match resA with
            | Ok rv -> rv |> Ok
            | Error es -> (sortr, es) |> Error

        
        let sortrId = (sortr |> Sorter.getSorterId)
        let sortableStId = (sortableSt |> SortableSet.getSortableSetId)
        let sorterOpOutput =
            SortingRollout.makeSorterOpOutput sorterOpTrackMode.SwitchUses sortableSt sortr

        match sorterOpOutput with
        | Ok output ->
            let res = output |> SorterSpeed.fromSorterOpOutput
            match res with
            | Ok (sorterSpeed, sorterPhenotypId, switchUseCts) ->
                match sorterPerfEvalMod with
                | DontCheckSuccess -> 
                    make None (Some switchUseCts) (Some sorterSpeed) None (Some sorterPhenotypId) sortableStId sortrId
                | CheckSuccess -> 
                    let isSuccessfl = 
                        output 
                        |> SorterOpOutput.isSorted
                        |> sorterPerf.IsSuccessful
                        |> Option.Some
                    make None (Some switchUseCts) (Some sorterSpeed) isSuccessfl (Some sorterPhenotypId) sortableStId sortrId
                | GetSortedSetCount ->
                    let sortedSetCt = output |> SorterOpOutput.getRefinedSortableCount
                    match sortedSetCt with
                    | Ok ct ->
                        let sct = ct |> sorterPerf.SortedSetSize |> Some
                        make None (Some switchUseCts) (Some sorterSpeed) sct (Some sorterPhenotypId) sortableStId sortrId
                    | Error msg ->
                        make (Some msg) (Some switchUseCts) (Some sorterSpeed) None (Some sorterPhenotypId) sortableStId sortrId

            | Error msg -> make (Some msg) None None None None sortableStId sortrId

        | Error msg -> make  (Some msg) None None None None sortableStId sortrId


    let modifyForPrefix
            (ordr:int<order>)
            (tc:int<stageCount>)
            (sev:sorterEval) 
        =
        {sev with 
           sorterSpeed = 
           sev.sorterSpeed
           |> Option.map(SorterSpeed.modifyForPrefix ordr tc)}


    let getSorterEvalProps 
            (sev:sorterEval) 
        =
        let stageCt, switchCt = (sev.sorterSpeed |> SorterSpeed.getProps)
        let successful, sortableCt = (sev.sorterPrf |> SorterPerf.getProps)
        [|
            (sorterEvalProps.ErrorMsg, sev.errorMessage |> Option.defaultValue "");
            (sorterEvalProps.Phenotype, sev.sortrPhenotypeId |> Option.map(UMX.untag >> string) |> Option.defaultValue "");
            (sorterEvalProps.SortableSetId, sev.sortableSetId |> UMX.untag |> string);
            (sorterEvalProps.SorterId, sev.sortrId |> UMX.untag |> string);
            (sorterEvalProps.StageCount, stageCt);
            (sorterEvalProps.Success, successful);
            (sorterEvalProps.SwitchCount, switchCt);
            (sorterEvalProps.SortedSetSize, sortableCt);
        |] |> Map.ofArray
