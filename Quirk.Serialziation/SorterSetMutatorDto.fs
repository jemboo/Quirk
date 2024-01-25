namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Sorting

type sorterSetMutatorDto = { 
        id: Guid
        sorterCountFinal: int; 
        sorterMutatorDto:sorterMutatorDto;
      }


module SorterSetMutatorDto =

    let fromDto (dto:sorterSetMutatorDto) =
        result {
            let! sorterMutator = dto.sorterMutatorDto |> SorterMutatorDto.fromDto
            let sorterCountFinal =
                match dto.sorterCountFinal with
                | -1 -> None
                | v -> v |> UMX.tag<sorterCount> |> Some

            return SorterSetMutator.load
                        (dto.id |> UMX.tag<sorterSetMutatorId> )
                        sorterMutator
                        sorterCountFinal
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetMutatorDto> jstr
            return! fromDto dto
        }

    let toDto (sorterSetMutator: sorterSetMutator) =
        {
            id = sorterSetMutator 
                 |> SorterSetMutator.getId 
                 |> UMX.untag

            sorterCountFinal =
                match (sorterSetMutator |> SorterSetMutator.getSorterCountFinal) with
                | None -> -1
                | Some v -> v |> UMX.untag

            sorterMutatorDto =
                sorterSetMutator 
                |> SorterSetMutator.getSorterMutator
                |> SorterMutatorDto.toDto
        }

    let toJson (sorterSetMutator: sorterSetMutator) =
        sorterSetMutator |> toDto |> Json.serialize

