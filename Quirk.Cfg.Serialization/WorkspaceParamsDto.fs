namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Serialization
open Quirk.Cfg.Core
open Quirk.Workspace
        
type wsParamsDto = { 
        id: Guid
        data: Map<string,string>
     }

module WsParamsDto =

    let fromDto (dto:wsParamsDto) =
        let rkm = dto.data 
                    |> Map.toSeq 
                    |> Seq.map(fun (k,v) -> (k |> UMX.tag<wsParamsKey>,v))
                    |> Map.ofSeq

        WsParams.load
            (dto.id |> UMX.tag<wsParamsId>)
            rkm

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<wsParamsDto> jstr
            return fromDto dto
        }

    let toDto (wsParams: wsParams) =
        {
            wsParamsDto.id = wsParams |> WsParams.getId |> UMX.untag
            data = wsParams 
                    |> WsParams.getMap
                    |> Map.toSeq
                    |> Seq.map(fun (k,v) -> (k |> UMX.untag,v))
                    |> Map.ofSeq
        }

    let toJson (wsParams: wsParams) =
        wsParams |> toDto |> Json.serialize