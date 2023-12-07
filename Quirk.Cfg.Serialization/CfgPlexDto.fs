namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
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
                    |> Array.map(fun itm -> itm |> CfgModelParamValue.toArrayOfStrings)
        }
    let toJson (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem |> toDto |> Json.serialize

    
    let fromDto (cfgPlexItemDto:cfgPlexItemDto) = 
        result {
            let! cfgPlexItemValueList =
                   cfgPlexItemDto.cfgPlexItemValues
                   |> Array.map(CfgModelParamValue.fromArrayOfStrings)
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
            name: string
            cfgPlexItemDtos: cfgPlexItemDto[]
        }
    
 module CfgPlexDto 
    =
    let toDto (cfgPlex:cfgPlex) : cfgPlexDto =
        {
            name = cfgPlex |> CfgPlex.getName |> UMX.untag
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
                        (cfgPlexDto.name |> UMX.tag<cfgPlexName>)
                        (cfgPlexItems |> List.toArray)
        }
   
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgPlexDto> cereal
            return! fromDto dto
        }
           