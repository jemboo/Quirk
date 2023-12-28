namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Script

type scriptItemDto =
        { 
            quirkWorldLineId: Guid
            quirkModelType: string
            modelParamSetDto: modelParamSetDto
            runParamSetDto: runParamSetDto
        }
    
 module ScriptItemDto =
    let toDto (scriptItem:scriptItem) : scriptItemDto =
        {
            scriptItemDto.quirkWorldLineId = scriptItem |> ScriptItem.getQuirkWorldlineId |> UMX.untag
            scriptItemDto.quirkModelType = scriptItem |> ScriptItem.getQuirkModelType |> QuirkModelType.toString
            scriptItemDto.modelParamSetDto = scriptItem |> ScriptItem.getModelParamSet |> ModelParamSetDto.toDto
            scriptItemDto.runParamSetDto = scriptItem |> ScriptItem.getScriptParamSet |> RunParamSetDto.toDto
        }
    let toJson (quirkscriptItem:scriptItem) =
        quirkscriptItem |> toDto |> Json.serialize

    
    let fromDto (scriptItemDto:scriptItemDto) = 
        result {
            let! scriptModelType = scriptItemDto.quirkModelType |> QuirkModelType.fromString
            let! scriptParamSet = scriptItemDto.runParamSetDto |> RunParamSetDto.fromDto
            let! modelParamSet = scriptItemDto.modelParamSetDto |> ModelParamSetDto.fromDto

            return ScriptItem.create
                        scriptModelType
                        scriptParamSet
                        modelParamSet
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<scriptItemDto> cereal
            return! fromDto dto
        }
         
         

type quirkScriptDto =
        { 
            scriptName: string
            projectFolder: string
            scriptItems: scriptItemDto[]
        }
    
 module QuirkScriptDto =
    let toDto (quirkScript:quirkScript) : quirkScriptDto =
        {
            quirkScriptDto.scriptName = quirkScript |> QuirkScript.getScriptName |> UMX.untag
            quirkScriptDto.projectFolder = quirkScript |> QuirkScript.getProjectFolder |> UMX.untag
            quirkScriptDto.scriptItems = 
                    quirkScript 
                            |> QuirkScript.getScriptItems 
                            |> Array.map(ScriptItemDto.toDto)
        }
    let toJson (quirkquirkScript:quirkScript) =
        quirkquirkScript |> toDto |> Json.serialize

    
    let fromDto (quirkScriptDto:quirkScriptDto) = 
        result {
            let scriptName = quirkScriptDto.scriptName |> UMX.tag<scriptName>
            let projectFolder = quirkScriptDto.projectFolder |> UMX.tag<projectName>
            let! scriptItems = 
                    quirkScriptDto.scriptItems
                    |> Array.toList
                    |> List.map(ScriptItemDto.fromDto)
                    |> Result.sequence

            return QuirkScript.create
                        scriptName
                        projectFolder
                        (scriptItems |> List.toArray)
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkScriptDto> cereal
            return! fromDto dto
        }
            