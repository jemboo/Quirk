namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Serialization
open Quirk.Cfg.Core
open Quirk.Workspace
        
type workspaceParamsDto = { 
        id: Guid
        data: Map<string,string>
     }

module WorkspaceParamsDto =

    let fromDto (dto:workspaceParamsDto) =
        let rkm = dto.data 
                    |> Map.toSeq 
                    |> Seq.map(fun (k,v) -> (k |> UMX.tag<workspaceParamsKey>,v))
                    |> Map.ofSeq

        WorkspaceParams.load
            (dto.id |> UMX.tag<workspaceParamsId>)
            rkm

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<workspaceParamsDto> jstr
            return fromDto dto
        }

    let toDto (workspaceParams: workspaceParams) =
        {
            workspaceParamsDto.id = workspaceParams |> WorkspaceParams.getId |> UMX.untag
            data = workspaceParams 
                        |> WorkspaceParams.getMap
                        |> Map.toSeq
                        |> Seq.map(fun (k,v) -> (k |> UMX.untag,v))
                        |> Map.ofSeq
        }

    let toJson (workspaceParams: workspaceParams) =
        workspaceParams |> toDto |> Json.serialize