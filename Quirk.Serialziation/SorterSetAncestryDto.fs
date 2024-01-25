namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Iter
open Quirk.Project
open Quirk.SortingResults
open Quirk.Sorting

type genInfoDto = {
    generation:int;
    sorterId:Guid;
    sorterPhenotypeId:Guid;
    sorterFitness:float
    }

module GenInfoDto =

    let fromDto (dto:genInfoDto) =
        result {
            let generation = dto.generation |> UMX.tag<generation>
            let sorterId = dto.sorterId |> UMX.tag<sorterId>
            let sorterPhenotypeId = dto.sorterPhenotypeId |> UMX.tag<sorterPhenotypeId>
            let sorterFitness = dto.sorterFitness |> UMX.tag<sorterFitness>
            return GenInfo.create generation sorterId sorterPhenotypeId sorterFitness
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<genInfoDto> jstr
            return! fromDto dto
        }

    let toDto (genInfo:genInfo) =
        {
            genInfoDto.generation =
                        genInfo 
                        |> GenInfo.getGeneration
                        |> UMX.untag

            sorterId =  genInfo 
                        |> GenInfo.getSorterId 
                        |> UMX.untag

            sorterPhenotypeId =  
                        genInfo 
                        |> GenInfo.getSorterPhenotypeId 
                        |> UMX.untag

            sorterFitness = 
                        genInfo 
                        |> GenInfo.getSorterFitness 
                        |> UMX.untag

        }

    let toJson (genInfo: genInfo) =
        genInfo |> toDto |> Json.serialize



type sorterAncestryDto = {
    sorterId:Guid;
    ancestors:genInfoDto[];
    }

module SorterAncestryDto =

    let fromDto (dto:sorterAncestryDto) =
        result {
           let sorterId = dto.sorterId |> UMX.tag<sorterId>
           let! ancestors = 
                    dto.ancestors 
                    |> Array.map(GenInfoDto.fromDto)
                    |> Array.toList
                    |> Result.sequence

           return SorterAncestry.load sorterId (ancestors |> List.toArray)
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterAncestryDto> jstr
            return! fromDto dto
        }

    let toDto (sorterAncestry:sorterAncestry) =
        {
            sorterAncestryDto.sorterId = 
                sorterAncestry 
                |> SorterAncestry.getSorterId
                |> UMX.untag;
            ancestors =
                sorterAncestry 
                |> SorterAncestry.getAncestors
                |> List.map(GenInfoDto.toDto)
                |> List.toArray;
        }

    let toJson (sorterAncestry: sorterAncestry) =
        sorterAncestry |> toDto |> Json.serialize



type sorterSetAncestryDto = { 
        id: Guid;
        generation:int;
        ancestors:sorterAncestryDto[];
        tag: Guid
     }

module SorterSetAncestryDto =

    let fromDto (dto:sorterSetAncestryDto) =
        result {
           let id = dto.id |> UMX.tag<sorterSetAncestryId>
           let generation = dto.generation |> UMX.tag<generation>
           let! ancestors = 
                    dto.ancestors
                    |> Array.map(SorterAncestryDto.fromDto)
                    |> Array.toList
                    |> Result.sequence
 
           return SorterSetAncestry.load 
                    id 
                    generation
                    (ancestors |> List.toArray)
                    dto.tag
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetAncestryDto> jstr
            return! fromDto dto
        }

    let toDto (sorterSetAncestry:sorterSetAncestry) =
        {
            sorterSetAncestryDto.id = 
                sorterSetAncestry 
                |> SorterSetAncestry.getId
                |> UMX.untag ;

            generation = sorterSetAncestry 
                |> SorterSetAncestry.getGeneration
                |> UMX.untag

            ancestors =
                sorterSetAncestry 
                |> SorterSetAncestry.getAncestorMap
                |> Map.toArray
                |> Array.map(snd)
                |> Array.map(SorterAncestryDto.toDto)

            tag = sorterSetAncestry |> SorterSetAncestry.getTag
        }

    let toJson (sorterAncestry: sorterSetAncestry) =
        sorterAncestry |> toDto |> Json.serialize