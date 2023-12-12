namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Serialization
open Quirk.Cfg.Core
   

type cfgModelParamSetDto =
        { 
            id: Guid
            replicaNumber: int
            cfgModelParamValues: string[][]
        }
    
 module CfgModelParamSetDto =
    let toDto (cfgModelParamSet:cfgModelParamSet) : cfgModelParamSetDto =
        {
            cfgModelParamSetDto.id = cfgModelParamSet |> CfgModelParamSet.getId |> UMX.untag
            cfgModelParamSetDto.replicaNumber = cfgModelParamSet |> CfgModelParamSet.getReplicaNumber |> UMX.untag
            cfgModelParamSetDto.cfgModelParamValues =
               cfgModelParamSet
                    |> CfgModelParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> CfgModelParamValue.toArrayOfStrings)
        }
    let toJson (cfgModelParamSet:cfgModelParamSet) =
        cfgModelParamSet |> toDto |> Json.serialize

    
    let fromDto (cfgModelParamSetDto:cfgModelParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   cfgModelParamSetDto.cfgModelParamValues
                   |> Array.map(CfgModelParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence

            return CfgModelParamSet.create
                        (cfgModelParamSetDto.replicaNumber |> UMX.tag<replicaNumber>)
                        cfgPlexItemValueList
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgModelParamSetDto> cereal
            return! fromDto dto
        }
            


type cfgRunParamSetDto =
        { 
            id: Guid
            cfgRunParamValues: string[][]
        }
    
 module CfgRunParamSetDto =
    let toDto (cfgRunParamSet:cfgRunParamSet) : cfgRunParamSetDto =
        {
            cfgRunParamSetDto.id = cfgRunParamSet |> CfgRunParamSet.getId |> UMX.untag
            cfgRunParamSetDto.cfgRunParamValues =
               cfgRunParamSet
                    |> CfgRunParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> CfgRunParamValue.toArrayOfStrings)
        }
    let toJson (cfgRunParamSet:cfgRunParamSet) =
        cfgRunParamSet |> toDto |> Json.serialize

    
    let fromDto (cfgRunParamSetDto:cfgRunParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   cfgRunParamSetDto.cfgRunParamValues
                   |> Array.map(CfgRunParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence

            return CfgRunParamSet.create
                        cfgPlexItemValueList
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgRunParamSetDto> cereal
            return! fromDto dto
        }
            
        
        