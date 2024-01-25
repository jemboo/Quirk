namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Sorting

type sorterSetParentMapDto = 
    {
        id:Guid;
        sorterSetIdMutant: Guid; 
        sorterSetIdParent: Guid; 
        parentMap:Map<Guid, Guid>;
    }

module SorterSetParentMapDto =

    let fromDto (dto:sorterSetParentMapDto) =
        result {
            let id = dto.id |> UMX.tag<sorterSetParentMapId>          
            let sorterSetIdMutant = dto.sorterSetIdMutant |> UMX.tag<sorterSetId>
            let  sorterSetIdParent = dto.sorterSetIdParent |> UMX.tag<sorterSetId>
            let parentMap = 
                    dto.parentMap
                    |> Map.toSeq
                    |> Seq.map(fun (p,m) -> 
                         (p |> UMX.tag<sorterId>, m |> UMX.tag<sorterParentId>))
                    |> Map.ofSeq

            return SorterSetParentMap.load
                        id
                        sorterSetIdMutant
                        sorterSetIdParent
                        parentMap
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetParentMapDto> jstr
            return! fromDto dto
        }

    let toDto (sorterParentMap: sorterSetParentMap) =
        {
            id = sorterParentMap
                 |> SorterSetParentMap.getId
                 |> UMX.untag

            parentMap =
                sorterParentMap 
                |> SorterSetParentMap.getParentMap
                |> Map.toSeq
                |> Seq.map(fun (p,m) -> 
                        (p |> UMX.untag, m |> UMX.untag))
                |> Map.ofSeq

            sorterSetIdMutant =
                sorterParentMap 
                |> SorterSetParentMap.getChildSorterSetId
                |> UMX.untag

            sorterSetIdParent =
                sorterParentMap 
                |> SorterSetParentMap.getParentSorterSetId
                |> UMX.untag
        }

    let toJson (sorterParentMap: sorterSetParentMap) =
        sorterParentMap |> toDto |> Json.serialize
