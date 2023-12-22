namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Serialization
open Quirk.Cfg.Core
   

type quirkScriptItemDto =
        { 
            quirkRunId: Guid
            quirkProjectType: string
            quirkScriptMode: string
            //modelParamSetDto: modelParamSetDto
            //quirkRunParamSetDto: quirkRunParamSetDto
        }
    

// module QuirkScriptItemDto =

//    let toDto 
//            (quirkRun:quirkRun) : quirkRunDto 
//        =
//        {
//            quirkRunDto.quirkRunId =
//                quirkRun |> QuirkRun.getQuirkRunId |> UMX.untag

//            quirkRunDto.quirkProjectType =
//                quirkRun 
//                |> QuirkRun.getRunType 
//                |> QuirkProjectType.toString

//            quirkRunDto.quirkScriptMode = ""
//                //quirkRun 
//                //|> QuirkRun.getScriptMode
//                //|> QuirkScriptMode.toString

//            quirkRunDto.modelParamSetDto =
//                quirkRun 
//                |> QuirkRun.getModelParamSet 
//                |> modelParamSetDto.toDto
            
//            quirkRunDto.quirkRunParamSetDto =
//                quirkRun 
//                |> QuirkRun.getRunParamSet 
//                |> quirkRunParamSetDto.toDto
//        }


//    let toJson (quirkRun:quirkRun) =
//        quirkRun |> toDto |> Json.serialize

    
//    let fromDto (quirkRunDto:quirkRunDto) = 

//        result {

//            let! quirkProjectType =
//                quirkRunDto.quirkProjectType
//                |> QuirkProjectType.fromString

//            let! quirkRunParamSet =
//                quirkRunDto.quirkRunParamSetDto 
//                |> quirkRunParamSetDto.fromDto

//            let! quirkModelParamSet =
//                quirkRunDto.modelParamSetDto 
//                |> modelParamSetDto.fromDto

//            return QuirkRun.create  
//                        quirkProjectType
//                        quirkRunParamSet
//                        quirkModelParamSet
//        }
       

//    let fromJson (cereal:string) =
//        result {
//            let! dto = Json.deserialize<quirkRunDto> cereal
//            return! fromDto dto
//        }
            


//type quirkScriptDto =
//        { 
//            quirkScriptItemDtos: quirkScriptItemDto[]
//        }
    

// module QuirkScriptDto =

//    let toDto 
//            (quirkRunSet:quirkRunSet) : quirkScriptDto 
//        =
//        {
//            quirkRunSetDto.quirkRunDtos =
//                quirkRunSet 
//                |> QuirkRunSet.getQuirkRuns 
//                |> Array.map(QuirkRunDto.toDto)

//        }


//    let toJson (quirkRunSet:quirkRunSet) =
//        quirkRunSet |> toDto |> Json.serialize

    
//    let fromDto (quirkRunSetDto:quirkRunSetDto) = 

//        result {

//            let! quirkRuns =
//                quirkRunSetDto.quirkRunDtos
//                |> Array.map(QuirkRunDto.fromDto)
//                |> Array.toList
//                |> Result.sequence


//            return QuirkRunSet.create  
//                        quirkRuns
//        }
       

//    let fromJson (cereal:string) =
//        result {
//            let! dto = Json.deserialize<quirkRunSetDto> cereal
//            return! fromDto dto
//        }
            