namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project

type modelParamSetDto =
        { 
            id: Guid
            replicaNumber: int
            modelParamValues: string[][]
        }
    
 module modelParamSetDto =
    let toDto (modelParamSet:modelParamSet) : modelParamSetDto =
        {
            modelParamSetDto.id = modelParamSet |> ModelParamSet.getId |> UMX.untag
            modelParamSetDto.replicaNumber = modelParamSet |> ModelParamSet.getReplicaNumber |> UMX.untag
            modelParamSetDto.modelParamValues =
               modelParamSet
                    |> ModelParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> CfgModelParamValue.toArrayOfStrings)
        }
    let toJson (quirkModelParamSet:modelParamSet) =
        quirkModelParamSet |> toDto |> Json.serialize

    
    let fromDto (modelParamSetDto:modelParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   modelParamSetDto.modelParamValues
                   |> Array.map(CfgModelParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence

            return ModelParamSet.create
                        (modelParamSetDto.replicaNumber |> UMX.tag<replicaNumber>)
                        cfgPlexItemValueList
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<modelParamSetDto> cereal
            return! fromDto dto
        }
            


type runParamSetDto =
        { 
            id: Guid
            runParamValues: string[][]
        }
    
 module runParamSetDto =
    let toDto (runParamSet:runParamSet) : runParamSetDto =
        {
            runParamSetDto.id = runParamSet |> RunParamSet.getId |> UMX.untag
            runParamSetDto.runParamValues =
               runParamSet
                    |> RunParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> RunParamValue.toArrayOfStrings)
        }
    let toJson (runParamSet:runParamSet) =
        runParamSet |> toDto |> Json.serialize

    
    let fromDto (runParamSetDto:runParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   runParamSetDto.runParamValues
                   |> Array.map(RunParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence

            return RunParamSet.create
                        cfgPlexItemValueList
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<runParamSetDto> cereal
            return! fromDto dto
        }
            
        
        