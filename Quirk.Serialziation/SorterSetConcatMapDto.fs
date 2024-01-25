namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Sorting

type sorterSetConcatMapDto  = {
        id:Guid;
        sorterSetBaseId: Guid; 
        sorterSetConcatId: Guid; 
        parentMap:Map<Guid, Guid[]>;
    }

module SorterSetConcatMapDto =

    let fromDto (dto:sorterSetConcatMapDto) =
        result {
            let id = dto.id |> UMX.tag<sorterSetConcatMapId>            
            let sorterSetBaseId = dto.sorterSetBaseId |> UMX.tag<sorterSetId>
            let sorterSetConcatId = dto.sorterSetConcatId |> UMX.tag<sorterSetId>
            let parentMap = 
                    dto.parentMap
                    |> Map.toSeq
                    |> Seq.map(fun (p,m) -> 
                         (p |> UMX.tag<sorterId>, 
                          m |> Array.map(UMX.tag<sorterId>)))
                    |> Map.ofSeq

            return SorterSetConcatMap.load
                        id
                        sorterSetBaseId
                        sorterSetConcatId
                        parentMap
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetConcatMapDto> jstr
            return! fromDto dto
        }

    let toDto (sorterSetConcatMap: sorterSetConcatMap) =
        {
            id = sorterSetConcatMap
                 |> SorterSetConcatMap.getId
                 |> UMX.untag

            parentMap =
                sorterSetConcatMap 
                |> SorterSetConcatMap.getConcatMap
                |> Map.toSeq
                |> Seq.map(fun (p,m) -> 
                        (p |> UMX.untag, 
                         m |> Array.map(UMX.untag)))
                |> Map.ofSeq

            sorterSetConcatId =
                sorterSetConcatMap 
                |> SorterSetConcatMap.getSorterSetConcatId
                |> UMX.untag

            sorterSetBaseId =
                sorterSetConcatMap 
                |> SorterSetConcatMap.getSorterSetBaseId
                |> UMX.untag
        }

    let toJson (sorterSetConcatMap: sorterSetConcatMap) =
        sorterSetConcatMap |> toDto |> Json.serialize

