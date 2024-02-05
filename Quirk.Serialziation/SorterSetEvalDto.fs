namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Sorting
open Quirk.SortingResults
open Quirk.SortingOps

type sorterSpeedDto = {
    switchCt:int;
    stageCt:int
    }

module SorterSpeedDto =

    let fromDto (dto:sorterSpeedDto) =
        result {
            let switchCount = dto.switchCt |> UMX.tag<switchCount>
            let stageCount = dto.stageCt |> UMX.tag<stageCount>
            return SorterSpeed.create switchCount stageCount
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSpeedDto> jstr
            return! fromDto dto
        }

    let fromNullabelJson (jstr: string) =
        if jstr = null then None 
        else jstr |> fromJson |> Some


    let toDto (sorterSpeed:sorterSpeed) =
        {
            sorterSpeedDto.switchCt = sorterSpeed 
                                      |> SorterSpeed.getSwitchCount
                                      |> UMX.untag
            stageCt =  sorterSpeed 
                                      |> SorterSpeed.getStageCount
                                      |> UMX.untag
        }

    let toJson (sorterSpeed: sorterSpeed) =
        sorterSpeed |> toDto |> Json.serialize

    let ofOption (sorterSpeed: sorterSpeed option) =
        match sorterSpeed with
        | Some ss -> ss |> toJson
        | None -> null


type sorterPerfDto = {
    useSuccess:bool;
    isSuccessful:Nullable<bool>;
    sortedSetSize:Nullable<int>
    }

module SorterPerfDto =

    let fromDto (dto:sorterPerfDto) =
        result {
            if dto.useSuccess then
                return dto.isSuccessful.Value
                    |> sorterPerf.IsSuccessful
            else
                return dto.sortedSetSize.Value
                    |> UMX.tag<sortableCount>
                    |> sorterPerf.SortedSetSize
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterPerfDto> jstr
            return! fromDto dto
        }

    let fromNullabelJson (jstr: string) =
        if jstr = null then None 
        else jstr |> fromJson |> Some

    let toDto (sorterPerf:sorterPerf) =
        match sorterPerf with
        | IsSuccessful bv -> 
            {
                sorterPerfDto.useSuccess = true;
                isSuccessful = bv |> Nullable;
                sortedSetSize = Nullable();
            }
        | sorterPerf.SortedSetSize sc ->
            {
                sorterPerfDto.useSuccess = false;
                isSuccessful = Nullable();
                sortedSetSize = sc |> UMX.untag |> Nullable;
            }

    let toJson (sorterPerf: sorterPerf) =
        sorterPerf |> toDto |> Json.serialize

    let ofOption (sorterPerf: sorterPerf option) =
        match sorterPerf with
        | Some ss -> ss |> toJson
        | None -> null


type sorterEvalDto = { 
        errorMessage: string;
        switchUseCts:string; 
        sorterSpeed:string; 
        sorterPrf:string; 
        sortrPhenotypeId:Nullable<Guid>; 
        sortableSetId:Guid;
        sortrId:Guid
     }

module SorterEvalDto =

    let fromDto (dto:sorterEvalDto) =
        result {
            let errorMessage = 
                match dto.errorMessage with
                | null -> None
                | msg -> msg |> Some
                
            let! switchUseCts =
                result {
                    if dto.switchUseCts.Length = 0 then
                        return None 
                    else
                        let! sparseA = 
                                dto.switchUseCts 
                                |> SparseIntArrayDto.fromJson

                        return sparseA
                                |> SparseArray.toArray
                                |> SwitchUseCounts.make
                                |> Some
                }

            let! sorterSpeed =
                if dto.sorterSpeed = null then
                    None |> Ok
                 else
                    dto.sorterSpeed 
                    |> SorterSpeedDto.fromJson
                    |> Result.map(Some)

            let! sorterPrf =
                if dto.sorterPrf = null then
                    None |> Ok
                 else
                    dto.sorterPrf
                    |> SorterPerfDto.fromJson
                    |> Result.map(Some)

            let sortrPhenotypeId =
                if dto.sortrPhenotypeId.HasValue then
                    dto.sortrPhenotypeId.Value
                        |> UMX.tag<sorterPhenotypeId>
                        |> Some
                else
                    None

            return SorterEval.make
                        errorMessage
                        switchUseCts
                        sorterSpeed
                        sorterPrf
                        sortrPhenotypeId
                        (dto.sortableSetId |> UMX.tag<sortableSetId>)
                        (dto.sortrId |> UMX.tag<sorterId>)
        }

        
    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterEvalDto> jstr
            return! fromDto dto
        }


    let toDto(sorterEvl:sorterEval) =
        let errorMsg = sorterEvl |> SorterEval.getErrorMessage 
                                 |> StringUtil.nullOption
        let switchUseCts = sorterEvl |> SorterEval.getSwitchUseCounts
                                     |> SwitchUseCounts.ofOption
        {
            errorMessage = errorMsg;
            switchUseCts = switchUseCts |> SparseArray.fromArray 0 |> SparseIntArrayDto.toJson
            sorterSpeed = sorterEvl |> SorterEval.getSorterSpeed |> SorterSpeedDto.ofOption
            sorterPrf = sorterEvl |> SorterEval.getSorterPerf |> SorterPerfDto.ofOption
            sortrPhenotypeId = sorterEvl |> SorterEval.getSortrPhenotypeId
                                         |> Option.map(UMX.untag)
                                         |> Option.toNullable
            sortableSetId = sorterEvl |> SorterEval.getSortableSetId
                                      |> UMX.untag
            sortrId = sorterEvl |> SorterEval.getSorterId |> UMX.untag
        }

    let toJson (sorterEvl:sorterEval) =
        sorterEvl |> toDto |> Json.serialize



type sorterSetEvalDto = {
        sorterSetId:Guid; 
        sortableSetId:Guid; 
        sorterEvals:string[]; 
     }


module SorterSetEvalDto =

    let fromDto (dto:sorterSetEvalDto) =
        result {

            let! sorterEvals =
                    dto.sorterEvals
                    |> Array.map(SorterEvalDto.fromJson)
                    |> Array.toList
                    |> Result.sequence

            let sorterSetId =
                    dto.sorterSetId
                    |> UMX.tag<sorterSetId>

            let sortableSetId =
                    dto.sortableSetId
                    |> UMX.tag<sortableSetId>


            return SorterSetEval.load
                        sorterSetId
                        sortableSetId
                        (sorterEvals |> List.toArray)
        }

        
    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetEvalDto> jstr
            return! fromDto dto
        }


    let toDto (ssEvl:sorterSetEval) =
        {
            sorterSetId = ssEvl |> SorterSetEval.getSorterSetlId |> UMX.untag
            sortableSetId = ssEvl |> SorterSetEval.getSortableSetId |> UMX.untag
            sorterEvals = ssEvl |> SorterSetEval.getSorterEvalsArray |> Array.map(SorterEvalDto.toJson)
        }

    let toJson (sorterSetEvl:sorterSetEval) =
        sorterSetEvl |> toDto |> Json.serialize

