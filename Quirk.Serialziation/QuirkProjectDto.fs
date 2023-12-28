namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Serialization
   

type quirkWorldLineDto =
        { 
            id: Guid
            quirkModelType: string
            modelParamSetDto: modelParamSetDto
            simParamSetDtos: simParamSetDto[]
            reportParamSetDtos: reportParamSetDto[]
        }
    

 module QuirkWorldLineDto =

    let toDto 
            (quirkWorldLine:quirkWorldLine) : quirkWorldLineDto 
        =
        {
            quirkWorldLineDto.id =
                quirkWorldLine |> QuirkWorldLine.getId |> UMX.untag

            quirkWorldLineDto.quirkModelType =
                quirkWorldLine 
                |> QuirkWorldLine.getQuirkModelType 
                |> QuirkModelType.toString

            quirkWorldLineDto.modelParamSetDto =
                quirkWorldLine 
                |> QuirkWorldLine.getModelParamSet 
                |> ModelParamSetDto.toDto

            quirkWorldLineDto.simParamSetDtos =
                quirkWorldLine 
                |> QuirkWorldLine.getSimParamSets
                |> Array.map(SimParamSetDto.toDto)

            quirkWorldLineDto.reportParamSetDtos =
                quirkWorldLine 
                |> QuirkWorldLine.getReportParamSets
                |> Array.map(ReportParamSetDto.toDto)

        }

    let toJson (quirkWorldLine:quirkWorldLine) =
        quirkWorldLine |> toDto |> Json.serialize

    
    let fromDto (quirkWorldLineDto:quirkWorldLineDto) = 

        result {

            let quirkWorldLineId =
                quirkWorldLineDto.id
                |> UMX.tag<quirkWorldLineId>

            let! quirkModelType =
                quirkWorldLineDto.quirkModelType
                |> QuirkModelType.fromString

            let! quirkModelParamSet =
                quirkWorldLineDto.modelParamSetDto
                |> ModelParamSetDto.fromDto

            let! simParamSets =
                quirkWorldLineDto.simParamSetDtos
                |> Array.toList
                |> List.map(SimParamSetDto.fromDto)
                |> Result.sequence

            let! reportParamSets =
                quirkWorldLineDto.reportParamSetDtos 
                |> Array.toList
                |> List.map(ReportParamSetDto.fromDto)
                |> Result.sequence

            return QuirkWorldLine.create
                        quirkWorldLineId
                        quirkModelType
                        quirkModelParamSet
                        (simParamSets |> List.toArray)
                        (reportParamSets |> List.toArray)
        }
       
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkWorldLineDto> cereal
            return! fromDto dto
        }
            


type quirkProjectDto =
        { 
            projectName:string
            quirkWorldLineDtos: quirkWorldLineDto[]
        }
    

 module QuirkProjectDto =

    let toDto 
            (quirkProject:quirkProject) : quirkProjectDto 
        =
        {
            quirkProjectDto.projectName =
                quirkProject
                |> QuirkProject.getProjectName 
                |> UMX.untag

            quirkWorldLineDtos =
                quirkProject
                |> QuirkProject.getQuirkWorldLineMap 
                |> Map.toArray
                |> Array.map(snd >> QuirkWorldLineDto.toDto)
        }


    let toJson (quirkProject:quirkProject) =
        quirkProject |> toDto |> Json.serialize

    
    let fromDto (quirkProjectDto:quirkProjectDto) = 

        result {

            let! quirkWorldLines =
                quirkProjectDto.quirkWorldLineDtos
                |> Array.map(QuirkWorldLineDto.fromDto)
                |> Array.toList
                |> Result.sequence

            let projectName = 
                quirkProjectDto.projectName |> UMX.tag<projectName>
            let quirkWorldLineMap = 
                quirkWorldLines 
                |> List.map(fun wL -> (wL |> QuirkWorldLine.getId, wL))
                |> Map.ofList

            return QuirkProject.create  
                        projectName
                        quirkWorldLineMap
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkProjectDto> cereal
            return! fromDto dto
        }
            