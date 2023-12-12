namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Serialization
open Quirk.Cfg.Core
   

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
            


type quirkRunSetDto =
        { 
            quirkRunDtos: quirkRunDto[]
        }
    

 module QuirkRunSetDto =

    let toDto 
            (quirkRunSet:quirkRunSet) : quirkRunSetDto 
        =
        {
            quirkRunSetDto.quirkRunDtos =
                quirkRunSet 
                |> QuirkRunSet.getQuirkRuns 
                |> Array.map(QuirkRunDto.toDto)

        }


    let toJson (quirkRunSet:quirkRunSet) =
        quirkRunSet |> toDto |> Json.serialize

    
    let fromDto (quirkRunSetDto:quirkRunSetDto) = 

        result {

            let! quirkRuns =
                quirkRunSetDto.quirkRunDtos
                |> Array.map(QuirkRunDto.fromDto)
                |> Array.toList
                |> Result.sequence


            return QuirkRunSet.create  
                        quirkRuns
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkRunSetDto> cereal
            return! fromDto dto
        }
            