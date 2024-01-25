﻿namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Sorting
open Quirk.SortingResults

type sorterSetPrunerWholeDto = { 
        id: Guid
        prunedCount: int;
        noiseFraction: float option; 
        stageWeight:float;
        }


module SorterSetPrunerWholeDto =

    let fromDto (dto:sorterSetPrunerWholeDto) =
        result {
            return SorterSetPruner.load
                        (dto.id |> UMX.tag<sorterSetPrunerId> )
                        (dto.prunedCount |> UMX.tag<sorterCount>)
                        (dto.noiseFraction |> Option.map UMX.tag<noiseFraction>)
                        (dto.stageWeight |> UMX.tag<stageWeight>)
        }


    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetPrunerWholeDto> jstr
            return! fromDto dto
        }


    let toDto (sorterSetPrunerWhole: sorterSetPruner) =
        {
            id = sorterSetPrunerWhole 
                 |> SorterSetPruner.getId
                 |> UMX.untag
            prunedCount = sorterSetPrunerWhole
                |> SorterSetPruner.getPrunedCount
                |> UMX.untag
            noiseFraction = sorterSetPrunerWhole
                |> SorterSetPruner.getNoiseFraction |> Option.map(UMX.untag)
            stageWeight = sorterSetPrunerWhole
                 |> SorterSetPruner.getStageWeight
                 |> UMX.untag
        }

    let toJson (sorterSetPrunerWhole: sorterSetPruner) 
        =
        sorterSetPrunerWhole |> toDto |> Json.serialize



//type sorterSetPrunerShcDto 
//        = 
//        { 
//            id: Guid
//            prunedCount: int;
//            noiseFraction: float option; 
//            stageWeight:float;
//        }


//module SorterSetPrunerShcDto =

//    let fromDto (dto:sorterSetPrunerShcDto) =
//        result {
//            return SorterSetPrunerShc.load
//                        (dto.id |> SorterSetPrunerId.create )
//                        (dto.prunedCount |> SorterCount.create)
//                        (dto.noiseFraction)
//                        (dto.stageWeight |> StageWeight.create)
//        }

//    let fromJson (jstr: string) =
//        result {
//            let! dto = Json.deserialize<sorterSetPrunerShcDto> jstr
//            return! fromDto dto
//        }

//    let toDto (sorterSetPrunerShc: sorterSetPrunerShc) =
//        {
//            id = sorterSetPrunerShc 
//                 |> SorterSetPrunerShc.getId 
//                 |> SorterSetPrunerId.value
//            prunedCount = sorterSetPrunerShc
//                |> SorterSetPrunerShc.getPrunedCount
//                |> SorterCount.value
//            noiseFraction = sorterSetPrunerShc
//                |> SorterSetPrunerShc.getNoiseFraction
//            stageWeight = sorterSetPrunerShc
//                 |> SorterSetPrunerShc.getStageWeight
//                 |> StageWeight.value

//        }

//    let toJson (sorterSetPrunerShc: sorterSetPrunerShc) =
//        sorterSetPrunerShc |> toDto |> Json.serialize



//type sorterSetPrunerDto = { 
//        duType: string
//        cereal: string; }

//module SorterSetPrunerDto =

//    let fromDto (dto:sorterSetPrunerDto) =
//        result {
//            match dto.duType with
//            | "Whole" ->
//                return! dto.cereal 
//                        |> SorterSetPrunerWholeDto.fromJson
//                        |> Result.map(sorterSetPruner.Whole)
                                
//            | _ -> 
//                return! "not handled (009)" |> Error
//        }

//    let fromJson (jstr: string) =
//        result {
//            let! dto = Json.deserialize<sorterSetPrunerDto> jstr
//            return! fromDto dto
//        }

//    let toDto (sorterSetPruner: sorterSetPruner) =
//        match sorterSetPruner with
//        | Whole w ->
//            {
//                duType = "Whole"
//                cereal = w |> SorterSetPrunerWholeDto.toJson
//            }
//        | Shc w ->
//            {
//                duType = "Lhc"
//                cereal = w |> SorterSetPrunerShcDto.toJson
//            }
//        | Batch b -> failwith "not implemented"

//    let toJson (sorterSetPruner: sorterSetPruner) =
//        sorterSetPruner |> toDto |> Json.serialize



type jsonDataMapDto = { 
        id: Guid
        data: Map<string,string>
     }

module JsonDataMapDto =

    let fromDto (dto:jsonDataMapDto) =
        JsonDataMap.load
            (dto.id |> UMX.tag<jsonDataMapId>)
            (dto.data)

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<jsonDataMapDto> jstr
            return fromDto dto
        }

    let toDto (jsonDataMap: jsonDataMap) =
        {
            jsonDataMapDto.id = jsonDataMap |> JsonDataMap.getId |> UMX.untag
            data = jsonDataMap |> JsonDataMap.getData
        }

    let toJson (jsonDataMap: jsonDataMap) =
        jsonDataMap |> toDto |> Json.serialize