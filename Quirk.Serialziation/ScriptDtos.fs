namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Script

type quirkRunDto =
        { 
            quirkWorldLineId: Guid
            quirkModelType: string
            modelParamSetDto: modelParamSetDto
            runParamSetDto: runParamSetDto
        }
    
 module quirkRunDto =
    let toDto (quirkRun:quirkRun) : quirkRunDto =
        {
            quirkRunDto.quirkWorldLineId = quirkRun |> QuirkRun.getQuirkWorldLineId |> UMX.untag
            quirkRunDto.quirkModelType = quirkRun |> QuirkRun.getQuirkModelType |> QuirkModelType.toString
            quirkRunDto.modelParamSetDto = quirkRun |> QuirkRun.getModelParamSet |> ModelParamSetDto.toDto
            quirkRunDto.runParamSetDto = quirkRun |> QuirkRun.getRunParamSet |> RunParamSetDto.toDto
        }
    let toJson (quirkquirkRun:quirkRun) =
        quirkquirkRun |> toDto |> Json.serialize

    
    let fromDto (quirkRunDto:quirkRunDto) = 
        result {
            let! scriptModelType = quirkRunDto.quirkModelType |> QuirkModelType.fromString
            let! scriptParamSet = quirkRunDto.runParamSetDto |> RunParamSetDto.fromDto
            let! modelParamSet = quirkRunDto.modelParamSetDto |> ModelParamSetDto.fromDto

            return QuirkRun.create
                        scriptModelType
                        scriptParamSet
                        modelParamSet
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkRunDto> cereal
            return! fromDto dto
        }
         
         

type quirkScriptDto =
        { 
            scriptName: string
            projectFolder: string
            quirkRuns: quirkRunDto[]
        }
    
 module QuirkScriptDto =
    let toDto (quirkScript:quirkScript) : quirkScriptDto =
        {
            quirkScriptDto.scriptName = quirkScript |> QuirkScript.getScriptName |> UMX.untag
            quirkScriptDto.projectFolder = quirkScript |> QuirkScript.getProjectFolder |> UMX.untag
            quirkScriptDto.quirkRuns = 
                    quirkScript 
                            |> QuirkScript.getScriptItems 
                            |> Array.map(quirkRunDto.toDto)
        }
    let toJson (quirkquirkScript:quirkScript) =
        quirkquirkScript |> toDto |> Json.serialize

    
    let fromDto (quirkScriptDto:quirkScriptDto) = 
        result {
            let scriptName = quirkScriptDto.scriptName |> UMX.tag<scriptName>
            let projectFolder = quirkScriptDto.projectFolder |> UMX.tag<projectName>
            let! quirkRuns = 
                    quirkScriptDto.quirkRuns
                    |> Array.toList
                    |> List.map(quirkRunDto.fromDto)
                    |> Result.sequence

            return QuirkScript.create
                        scriptName
                        projectFolder
                        (quirkRuns |> List.toArray)
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkScriptDto> cereal
            return! fromDto dto
        }
            