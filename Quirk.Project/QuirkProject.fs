﻿namespace Quirk.Project

open FSharp.UMX


type quirkWorldLine = 
    private 
        { 
            id: Guid<quirkWorldLineId>
            quirkModelType: quirkModelType
            modelParamSet: modelParamSet
            simParamSets: simParamSet[]
            reportParamSets: reportParamSet[]
        }

module QuirkWorldLine =
    let create 
            (quirkWorldLineId:Guid<quirkWorldLineId>)
            (quirkModelType:quirkModelType)
            (modelParamSet:modelParamSet)
            (simParamSets:simParamSet[])
            (reportParamSets:reportParamSet[])
        =
            { 
                quirkWorldLine.id = quirkWorldLineId
                quirkModelType = quirkModelType
                modelParamSet = modelParamSet
                simParamSets = simParamSets
                reportParamSets = reportParamSets
            }

    let getId (quirkWorldLine:quirkWorldLine) = quirkWorldLine.id
    let getQuirkModelType (quirkWorldLine:quirkWorldLine) = quirkWorldLine.quirkModelType
    let getModelParamSet (quirkWorldLine:quirkWorldLine) = quirkWorldLine.modelParamSet
    let getSimParamSets (quirkWorldLine:quirkWorldLine) = quirkWorldLine.simParamSets
    let getReportParamSets (quirkWorldLine:quirkWorldLine) = quirkWorldLine.reportParamSets
    let addSimParamSet 
            (quirkWorldLine:quirkWorldLine) 
            (simParamSet:simParamSet)
        =
        {
            quirkWorldLine with
                simParamSets = quirkWorldLine.simParamSets |> Array.append [| simParamSet |]
        }
    let addReportParamSet 
            (quirkWorldLine:quirkWorldLine) 
            (reportParamSet:reportParamSet)
        =
        {
            quirkWorldLine with
                reportParamSets = quirkWorldLine.reportParamSets |> Array.append [| reportParamSet |]
        }

    let addQuirkRun
            (quirkRun:quirkRun)
            (quirkWorldLine:quirkWorldLine) 
        =
        match quirkRun.runParamSet with
        | runParamSet.Sim sps -> addSimParamSet quirkWorldLine sps
        | runParamSet.Report rps -> addReportParamSet quirkWorldLine rps


    let createFromQuirkRun
            (quirkRun:quirkRun)
        =
        let quirkWorldLine =
            { 
                quirkWorldLine.id = quirkRun |> QuirkRun.getQuirkWorldLineId
                quirkModelType =  quirkRun |> QuirkRun.getQuirkModelType
                modelParamSet =  quirkRun |> QuirkRun.getModelParamSet
                simParamSets =  [||]
                reportParamSets =  [||]
            }
        addQuirkRun quirkRun quirkWorldLine



type quirkProject =
    private 
        { 
            projectName: string<projectName>
            quirkWorldLineMap: Map<Guid<quirkWorldLineId>, quirkWorldLine>
        }


module QuirkProject =
    let create 
            (projectName: string<projectName>)
            (quirkWorldLineMap: Map<Guid<quirkWorldLineId>, quirkWorldLine>)
        =
        {
            quirkProject.projectName = projectName
            quirkWorldLineMap = quirkWorldLineMap
        }

    let createEmpty
            (projectName: string<projectName>)
        =
        {
            quirkProject.projectName = projectName
            quirkWorldLineMap = Map.empty
        }

    let getProjectName (quirkProject:quirkProject) = 
            quirkProject.projectName

    let getQuirkWorldLineMap (quirkProject:quirkProject) = 
            quirkProject.quirkWorldLineMap

    let getQuirkWorldLines (quirkProject:quirkProject) = 
            quirkProject.quirkWorldLineMap |> Map.toSeq |> Seq.map(snd)

    let getSingletonParams (quirkProject:quirkProject) = 
        let modelParamValues = 
                quirkProject 
                |> getQuirkWorldLines
                |> Seq.map(QuirkWorldLine.getModelParamSet)
                |> Seq.map(ModelParamSet.getAllModelParamValues)
                |> Seq.concat
                |> Seq.toArray
                |> Array.distinct
                |> Array.groupBy(ModelParamValue.getModelParamName)
                    
        let singetonModelParamValues = 
            modelParamValues 
            |> Array.filter(fun (names, mbrs) -> mbrs.Length = 1)
            |> Array.map(snd >> Array.head)

        singetonModelParamValues


    let getVariableParamNames (quirkProject:quirkProject) = 
        let modelParamValues = 
                quirkProject 
                |> getQuirkWorldLines
                |> Seq.map(QuirkWorldLine.getModelParamSet)
                |> Seq.map(ModelParamSet.getAllModelParamValues)
                |> Seq.concat
                |> Seq.toArray
                |> Array.distinct
                |> Array.groupBy(ModelParamValue.getModelParamName)
                    
        let variableParamNames = 
            modelParamValues 
            |> Array.filter(fun (names, mbrs) -> mbrs.Length > 1)
            |> Array.map(fst)

        variableParamNames



    let updateProject 
            (quirkProject:quirkProject)
            (quirkRun:quirkRun)
        =
        result {
            let wlId = quirkRun |> QuirkRun.getQuirkWorldLineId
            let wlNew =
                if (quirkProject.quirkWorldLineMap.ContainsKey(wlId)) then
                    quirkProject.quirkWorldLineMap[wlId] |> QuirkWorldLine.addQuirkRun quirkRun
                else
                    QuirkWorldLine.createFromQuirkRun quirkRun

            let mapNew = quirkProject.quirkWorldLineMap.Add(wlId, wlNew)
            return 
                { quirkProject with 
                    quirkWorldLineMap = mapNew
                }
        }

