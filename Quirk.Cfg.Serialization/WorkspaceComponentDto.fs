namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Serialization
open Quirk.Cfg.Core
open Quirk.Workspace


module WsComponentDataDto = 

    let toJson (comp:wsComponentData) =
        match comp with
        | SortableSet sortableSet -> 
             sortableSet |> SortableSetDto.toJson
        | SorterSet sorterSet -> 
             sorterSet |> SorterSetDto.toJson
        | SorterSetParentMap sorterSetParentMap -> 
             sorterSetParentMap |> SorterSetParentMapDto.toJson
        | SorterSetEval sorterSetEval -> 
             sorterSetEval |> SorterSetEvalDto.toJson
        | SorterSetAncestry sorterSetAncestry ->
             sorterSetAncestry |> SorterSetAncestryDto.toJson
        | SorterSpeedBinSet sorterSpeedBinSet ->
             sorterSpeedBinSet |> SorterSpeedBinSetDto.toJson
        | WsParams wsParams ->
            wsParams |> WsParamsDto.toJson


    let fromJson 
            (wct:wsComponentType) 
            (cereal:string) 
        =
        match wct with
        | wsComponentType.SortableSet ->
            cereal |> SortableSetDto.fromJson |> Result.map(wsComponentData.SortableSet)
        | wsComponentType.SorterSet ->
            cereal |> SorterSetDto.fromJson |> Result.map(wsComponentData.SorterSet)
        | wsComponentType.SorterSetAncestry ->
            cereal |> SorterSetAncestryDto.fromJson |> Result.map(wsComponentData.SorterSetAncestry)
        | wsComponentType.SorterSetParentMap ->
            cereal |> SorterSetParentMapDto.fromJson |> Result.map(wsComponentData.SorterSetParentMap)
        | wsComponentType.SorterSetEval ->
            cereal |> SorterSetEvalDto.fromJson |> Result.map(wsComponentData.SorterSetEval)
        | wsComponentType.SorterSpeedBinSet ->
            cereal |> SorterSpeedBinSetDto.fromJson |> Result.map(wsComponentData.SorterSpeedBinSet)
        | wsComponentType.WsParams ->
            cereal |> WsParamsDto.fromJson |> Result.map(wsComponentData.WsParams)
        | _ -> "unhandled wsComponentType" |> Error




type wsComponentDto =
     { 
        id: Guid; 
        name: string
        componentType:string
        componentData:string
     }

module WsComponentDto =

    let fromDto (dto:wsComponentDto) =
        result {
        
            let id = dto.id |> UMX.tag<wsComponentId>
            let name = dto.name |> UMX.tag<wsCompKey>
            let! componentType = dto.componentType |> WsComponentType.fromString
            let! componentData = 
                    dto.componentData |> WsComponentDataDto.fromJson componentType
            return 
                WsComponent.make
                    id
                    name
                    componentType
                    componentData
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<wsComponentDto> jstr
            return! fromDto dto
        }

    let toDto (wsComponent: wsComponent) =
        {
            wsComponentDto.id = 
                wsComponent 
                |> WsComponent.getId |> UMX.untag

            name = 
                wsComponent 
                |> WsComponent.getName
                |> UMX.untag

            componentType =
                wsComponent
                |> WsComponent.getWsComponentType
                |> WsComponentType.toString

            componentData =
                wsComponent
                |> WsComponent.getWsComponentData
                |> WsComponentDataDto.toJson

        }

    let toJson (wsComponent: wsComponent) =
        wsComponent |> toDto |> Json.serialize