namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Serialization
open Quirk.Cfg.Core
   

type cfgPlexItemDto =
        { 
            name: string
            rank: int
            cfgPlexItemValues: string[][]
        }
    
 module CfgPlexItemDto =
    let toDto (cfgPlexItem:cfgPlexItem) : cfgPlexItemDto =
        {
            name = cfgPlexItem |> CfgPlexItem.getName |> UMX.untag
            rank = cfgPlexItem |> CfgPlexItem.getRank |> UMX.untag
            cfgPlexItemValues =
               cfgPlexItem
                    |> CfgPlexItem.getCfgPlexItemValues
                    |> Array.map(fun itm -> itm |> ModelParamValue.toArrayOfStrings)
        }
    let toJson (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem |> toDto |> Json.serialize

    
    let fromDto (cfgPlexItemDto:cfgPlexItemDto) = 
        result {
            let! cfgPlexItemValueList =
                   cfgPlexItemDto.cfgPlexItemValues
                   |> Array.map(ModelParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence
            
            return CfgPlexItem.create
                        (cfgPlexItemDto.name |> UMX.tag<cfgPlexItemName>)
                        (cfgPlexItemDto.rank |> UMX.tag<cfgPlexItemRank>)
                        (cfgPlexItemValueList |> List.toArray)
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgPlexItemDto> cereal
            return! fromDto dto
        }
            
        
        
 type cfgPlexDto =
        { 
            cfgPlexName: string
            projectName: string
            cfgPlexItemDtos: cfgPlexItemDto[]
        }
    
 module CfgPlexDto 
    =
    let toDto (cfgPlex:cfgPlex) : cfgPlexDto =
        {
            cfgPlexName = cfgPlex |> CfgPlex.getCfgPlexName |> UMX.untag
            projectName = cfgPlex |> CfgPlex.getProjectName |> UMX.untag
            cfgPlexItemDtos =
               cfgPlex
                    |> CfgPlex.getCfgPlexItems
                    |> Array.map(fun itm -> itm |> CfgPlexItemDto.toDto)
        }
        
    let toJson (cfgPlex:cfgPlex) =
        cfgPlex |> toDto |> Json.serialize
   
    let fromDto (cfgPlexDto:cfgPlexDto) = 
        result {
            let! cfgPlexItems =
                   cfgPlexDto.cfgPlexItemDtos
                   |> Array.map(CfgPlexItemDto.fromDto)
                   |> Array.toList
                   |> Result.sequence
            return CfgPlex.create
                        (cfgPlexDto.cfgPlexName |> UMX.tag<cfgPlexName>)
                        (cfgPlexDto.projectName |> UMX.tag<projectName>)
                        (cfgPlexItems |> List.toArray)
        }
   
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgPlexDto> cereal
            return! fromDto dto
        }
           