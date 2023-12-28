namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Script

type modelParamSetDto =
        { 
            id: Guid
            replicaNumber: int
            modelParamValues: string[][]
        }
    
 module ModelParamSetDto =
    let toDto (modelParamSet:modelParamSet) : modelParamSetDto =
        {
            modelParamSetDto.id = modelParamSet |> ModelParamSet.getId |> UMX.untag
            modelParamSetDto.replicaNumber = modelParamSet |> ModelParamSet.getReplicaNumber |> UMX.untag
            modelParamSetDto.modelParamValues =
               modelParamSet
                    |> ModelParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> ModelParamValue.toArrayOfStrings)
        }
    let toJson (quirkModelParamSet:modelParamSet) =
        quirkModelParamSet |> toDto |> Json.serialize

    
    let fromDto (modelParamSetDto:modelParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   modelParamSetDto.modelParamValues
                   |> Array.map(ModelParamValue.fromArrayOfStrings)
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
            


type simParamSetDto =
        { 
            id: Guid
            simParamValues: string[][]
        }
    
 module SimParamSetDto =
    let toDto (simParamSet:simParamSet) : simParamSetDto =
        {
            simParamSetDto.id = simParamSet |> SimParamSet.getId |> UMX.untag
            simParamSetDto.simParamValues =
               simParamSet
                    |> SimParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> SimParamValue.toArrayOfStrings)
        }
    let toJson (simParamSet:simParamSet) =
        simParamSet |> toDto |> Json.serialize

    
    let fromDto (simParamSetDto:simParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   simParamSetDto.simParamValues
                   |> Array.map(SimParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence

            return SimParamSet.create
                        cfgPlexItemValueList
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<simParamSetDto> cereal
            return! fromDto dto
        }
            
        


type reportParamSetDto =
        { 
            id: Guid
            reportParamValues: string[][]
        }
    
 module ReportParamSetDto =
    let toDto (reportParamSet:reportParamSet) : reportParamSetDto =
        {
            reportParamSetDto.id = reportParamSet |> ReportParamSet.getId |> UMX.untag
            reportParamSetDto.reportParamValues =
               reportParamSet
                    |> ReportParamSet.getValueMap
                    |> Map.toArray
                    |> Array.map(fun (k,v) -> v |> ReportParamValue.toArrayOfStrings)
        }
    let toJson (reportParamSet:reportParamSet) =
        reportParamSet |> toDto |> Json.serialize

    
    let fromDto (reportParamSetDto:reportParamSetDto) = 
        result {
            let! cfgPlexItemValueList =
                   reportParamSetDto.reportParamValues
                   |> Array.map(ReportParamValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence

            return ReportParamSet.create
                        cfgPlexItemValueList
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<reportParamSetDto> cereal
            return! fromDto dto
        }
            
        
        


type runParamSetDto =
        { 
            runParamSetType : string
            simParamSetDto: simParamSetDto option
            reportParamSetDto: reportParamSetDto option
        }
    
 module RunParamSetDto =
    let toDto (runParamSet:runParamSet) : runParamSetDto =
        match runParamSet with
        | runParamSet.Sim runPs ->
            {
                runParamSetDto.runParamSetType = 
                    runParamSetType.Sim |> RunParamSetType.toString
                simParamSetDto = runPs |> SimParamSetDto.toDto |> Some
                reportParamSetDto = None
            }
        | runParamSet.Report rptPs ->
            {
                runParamSetDto.runParamSetType = 
                    runParamSetType.Report |> RunParamSetType.toString
                simParamSetDto = None
                reportParamSetDto = rptPs |> ReportParamSetDto.toDto |> Some
            }

    let toJson (runParamSet:runParamSet) =
        runParamSet |> toDto |> Json.serialize

    
    let fromDto (runParamSetDto:runParamSetDto) = 
        result {
            let! runParamSetType =
                   runParamSetDto.runParamSetType
                   |> RunParamSetType.fromString
            match runParamSetType with
            | runParamSetType.Sim ->
                let! simParamSetDto = 
                    runParamSetDto.simParamSetDto 
                    |> Result.ofOption "simParamSetDto is missing"
                let! simParamSet = simParamSetDto |> SimParamSetDto.fromDto
                return simParamSet |> runParamSet.Sim

            | Report ->
                let! reportParamSetDto = 
                    runParamSetDto.reportParamSetDto 
                    |> Result.ofOption "reportParamSetDto is missing"
                let! reportParamSet = reportParamSetDto |> ReportParamSetDto.fromDto
                return reportParamSet |> runParamSet.Report
        }
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<runParamSetDto> cereal
            return! fromDto dto
        }
            
        