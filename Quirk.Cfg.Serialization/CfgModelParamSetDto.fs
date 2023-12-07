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
            runParamValues: string[][]
        }
    
 module CfgModelParamSetDto =

    let toDto 
            (cfgModelParamSet:cfgModelParamSet) : cfgModelParamSetDto 
        =
        {
            cfgModelParamSetDto.id = 
                cfgModelParamSet 
                |> CfgModelParamSet.getId
                |> UMX.untag

            cfgModelParamSetDto.replicaNumber = 
                cfgModelParamSet
                |> CfgModelParamSet.getReplicaNumber
                |> UMX.untag

            cfgModelParamSetDto.runParamValues =
                cfgModelParamSet 
                |> CfgModelParamSet.getValueMap
                |> Map.toArray
                |> Array.map(snd >> CfgModelParamValue.toArrayOfStrings)
        }


    let toJson (runParamSet:cfgModelParamSet) =
        runParamSet |> toDto |> Json.serialize

    
    let fromDto (runParamSetDto:cfgModelParamSetDto) = 
        let replicaNumber = 
            runParamSetDto.replicaNumber |> UMX.tag<replicaNumber>

        result {

            let! runParamValues =
                runParamSetDto.runParamValues
                |> Array.toList
                |> List.map(CfgModelParamValue.fromArrayOfStrings)
                |> Result.sequence

            return CfgModelParamSet.create replicaNumber runParamValues
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgModelParamSetDto> cereal
            return! fromDto dto
        }
            
      