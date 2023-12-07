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
            

   

type cfgRunParamSetDto =
        { 
            id: Guid
            runParamValues: string[][]
        }
    

 module CfgRunParamSetDto =

    let toDto 
            (cfgRunParamSet:cfgRunParamSet) : cfgRunParamSetDto 
        =
        {
            cfgRunParamSetDto.id = 
                cfgRunParamSet 
                |> CfgRunParamSet.getId
                |> UMX.untag

            cfgRunParamSetDto.runParamValues =
                cfgRunParamSet 
                |> CfgRunParamSet.getValueMap
                |> Map.toArray
                |> Array.map(snd >> CfgRunParamValue.toArrayOfStrings)
        }


    let toJson (runParamSet:cfgRunParamSet) =
        runParamSet |> toDto |> Json.serialize

    
    let fromDto (runParamSetDto:cfgRunParamSetDto) = 

        result {

            let! runParamValues =
                runParamSetDto.runParamValues
                |> Array.toList
                |> List.map(CfgRunParamValue.fromArrayOfStrings)
                |> Result.sequence

            return CfgRunParamSet.create  runParamValues
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgRunParamSetDto> cereal
            return! fromDto dto
        }
            
   

type quirkRunDto =
        { 
            quirkRunId: Guid
            quirkRunType: string
            quirkRunMode: string
            cfgModelParamSetDto: cfgModelParamSetDto
            cfgRunParamSetDto: cfgRunParamSetDto
        }
    

 module QuirkRunDto =

    let toDto 
            (quirkRun:quirkRun) : quirkRunDto 
        =
        {
            quirkRunDto.quirkRunId =
                quirkRun |> QuirkRun.getQuirkRunId |> UMX.untag

            quirkRunDto.quirkRunType =
                quirkRun 
                |> QuirkRun.getRunType 
                |> QuirkRunType.toString

            quirkRunDto.quirkRunMode =
                quirkRun 
                |> QuirkRun.getRunMode
                |> QuirkRunMode.toString

            quirkRunDto.cfgModelParamSetDto =
                quirkRun 
                |> QuirkRun.getModelParamSet 
                |> CfgModelParamSetDto.toDto
            
            quirkRunDto.cfgRunParamSetDto =
                quirkRun 
                |> QuirkRun.getRunParamSet 
                |> CfgRunParamSetDto.toDto
        }


    let toJson (quirkRun:quirkRun) =
        quirkRun |> toDto |> Json.serialize

    
    let fromDto (quirkRunDto:quirkRunDto) = 

        result {

            let! quirkRunType =
                quirkRunDto.quirkRunType
                |> QuirkRunType.fromString

            let! quirkRunMode =
                quirkRunDto.quirkRunMode
                |> QuirkRunMode.fromString

            let! cfgRunParamSet =
                quirkRunDto.cfgRunParamSetDto 
                |> CfgRunParamSetDto.fromDto

            let! cfgModelParamSet =
                quirkRunDto.cfgModelParamSetDto 
                |> CfgModelParamSetDto.fromDto

            return QuirkRun.create  
                        quirkRunType
                        quirkRunMode
                        cfgRunParamSet
                        cfgModelParamSet
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkRunDto> cereal
            return! fromDto dto
        }
            