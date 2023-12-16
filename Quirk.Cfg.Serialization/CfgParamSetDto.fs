namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Serialization
open Quirk.Cfg.Core
   

type quirkModelParamSetDto =
        { 
            id: Guid
            replicaNumber: int
            cfgModelParamValues: string[][]
        }
    
 module quirkModelParamSetDto =
    let toDto (quirkModelParamSet:quirkModelParamSet) : quirkModelParamSetDto =
        {
            quirkModelParamSetDto.id = quirkModelParamSet |> QuirkModelParamSet.getId |> UMX.untag
            quirkModelParamSetDto.replicaNumber = quirkModelParamSet |> QuirkModelParamSet.getReplicaNumber |> UMX.untag
            quirkModelParamSetDto.cfgModelParamValues =
               quirkModelParamSet
                    |> QuirkModelParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> CfgModelParamValue.toArrayOfStrings)
        }
    let toJson (quirkModelParamSet:quirkModelParamSet) =
        quirkModelParamSet |> toDto |> Json.serialize

    
    let fromDto (quirkModelParamSetDto:quirkModelParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   quirkModelParamSetDto.cfgModelParamValues
                   |> Array.map(CfgModelParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence

            return QuirkModelParamSet.create
                        (quirkModelParamSetDto.replicaNumber |> UMX.tag<replicaNumber>)
                        cfgPlexItemValueList
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkModelParamSetDto> cereal
            return! fromDto dto
        }
            


type quirkRunParamSetDto =
        { 
            id: Guid
            cfgRunParamValues: string[][]
        }
    
 module quirkRunParamSetDto =
    let toDto (quirkRunParamSet:quirkRunParamSet) : quirkRunParamSetDto =
        {
            quirkRunParamSetDto.id = quirkRunParamSet |> QuirkRunParamSet.getId |> UMX.untag
            quirkRunParamSetDto.cfgRunParamValues =
               quirkRunParamSet
                    |> QuirkRunParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> CfgRunParamValue.toArrayOfStrings)
        }
    let toJson (quirkRunParamSet:quirkRunParamSet) =
        quirkRunParamSet |> toDto |> Json.serialize

    
    let fromDto (quirkRunParamSetDto:quirkRunParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   quirkRunParamSetDto.cfgRunParamValues
                   |> Array.map(CfgRunParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence

            return QuirkRunParamSet.create
                        cfgPlexItemValueList
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkRunParamSetDto> cereal
            return! fromDto dto
        }
            
        
        