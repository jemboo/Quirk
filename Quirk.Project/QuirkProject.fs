namespace Quirk.Project

open FSharp.UMX
open Quirk.Core



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

    let getProjectName (quirkProject:quirkProject) = quirkProject.projectName
    let getQuirkWorldLineMap (quirkProject:quirkProject) = quirkProject.quirkWorldLineMap

    let updateProject 
            (quirkProject:quirkProject)
            (quirkRun:quirkRun)
        =
        result {
            let wlId = quirkRun |> QuirkRun.getQuirkWorldLineId
            if (quirkProject.quirkWorldLineMap.ContainsKey(wlId)) then
                return! $"worldLine: {wlId} not found in project: {quirkProject.projectName |> UMX.untag}" |> Error

            let wlNew = quirkProject.quirkWorldLineMap[wlId] |> QuirkWorldLine.addQuirkRun quirkRun
            let mapNew = quirkProject.quirkWorldLineMap.Add(wlId, wlNew)
            return 
                { quirkProject with 
                    quirkWorldLineMap = mapNew
                }
        }

