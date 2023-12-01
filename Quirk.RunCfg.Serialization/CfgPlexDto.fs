namespace Quirk.RunCfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Serialization
open Quirk.RunCfg.Core
   

type cfgPlexParamDto =
        { 
            name: string
            rank: int
            cfgPlexItemValues: string[][]
        }
    
 module CfgPlexItemDto =
    let toDto (cfgPlexItem:cfgPlexParam) : cfgPlexParamDto =
        {
            name = cfgPlexItem |> CfgPlexParam.getName |> UMX.untag
            rank = cfgPlexItem |> CfgPlexParam.getRank |> UMX.untag
            cfgPlexItemValues =
               cfgPlexItem
                    |> CfgPlexParam.getCfgPlexItemValues
                    |> Array.map(fun itm -> itm |> CfgPlexParamValue.toArrayOfStrings)
        }
    let toJson (cfgPlexItem:cfgPlexParam) =
        cfgPlexItem |> toDto |> Json.serialize

    
    let fromDto (cfgPlexItemDto:cfgPlexParamDto) = 
        result {
            let! cfgPlexItemValueList =
                   cfgPlexItemDto.cfgPlexItemValues
                   |> Array.map(CfgPlexParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence
            
            return CfgPlexParam.create
                        (cfgPlexItemDto.name |> UMX.tag<cfgPlexItemName>)
                        (cfgPlexItemDto.rank |> UMX.tag<cfgPlexItemRank>)
                        (cfgPlexItemValueList |> List.toArray)
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgPlexParamDto> cereal
            return! fromDto dto
        }
            
        
            
    
 type cfgPlexDto =
        { 
            name: string
            rngGenDto: rngGenDto
            cfgPlexItemDtos: cfgPlexParamDto[]
        }
    
 module CfgPlexDto =
    let toDto (cfgPlex:cfgPlex) : cfgPlexDto =
        {
            name = cfgPlex |> CfgPlex.getName |> UMX.untag
            rngGenDto = cfgPlex |> CfgPlex.getRngGen |> RngGenDto.toDto
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
            let! rngGen = cfgPlexDto.rngGenDto |> RngGenDto.fromDto
            return CfgPlex.create
                        (cfgPlexDto.name |> UMX.tag<cfgPlexName>)
                        rngGen
                        (cfgPlexItems |> List.toArray)
        }
   

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgPlexDto> cereal
            return! fromDto dto
        }
           