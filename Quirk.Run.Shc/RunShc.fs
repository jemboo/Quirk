namespace Quirk.Run.Shc
open System.Threading.Tasks
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage
open System.Threading
open Quirk.Workspace
open System


module RunShc =

    let getWsParamsFromSimParamSet
            (simParamSet:simParamSet) =
        result {

            let! (_, generationStart) = 
                    simParamSet 
                    |> SimParamSet.getGeneration ("generationStart" |> UMX.tag<simParamName>)

            let! (_, generationEnd) = 
                    simParamSet 
                    |> SimParamSet.getGeneration ("generationEnd" |> UMX.tag<simParamName>)

            let! (_, reportInterval) = 
                    simParamSet 
                    |> SimParamSet.getGeneration ("reportInterval" |> UMX.tag<simParamName>)

            let! (_, snapshotInterval) = 
                    simParamSet 
                    |> SimParamSet.getGeneration ("snapshotInterval" |> UMX.tag<simParamName>)

            let wsParams =
                WsParams.make Map.empty
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.generationStart generationStart
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.generationEnd generationEnd
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.reportInterval reportInterval
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.snapshotInterval snapshotInterval

            return wsParams
        }


    let getWsParamsFromReportParamSet
            (reportParamSet:reportParamSet) =
        result {
        
            let! (_, generationStart) = 
                    reportParamSet 
                    |> ReportParamSet.getGeneration ("generationStart" |> UMX.tag<reportParamName>)

            let! (_, generationEnd) = 
                    reportParamSet 
                    |> ReportParamSet.getGeneration ("generationEnd" |> UMX.tag<reportParamName>)

            let! (_, reportInterval) = 
                    reportParamSet 
                    |> ReportParamSet.getGeneration ("reportInterval" |> UMX.tag<reportParamName>)

            let! (_, reportType) = 
                    reportParamSet 
                    |> ReportParamSet.getReportType ("reportType" |> UMX.tag<reportParamName>)

            let wsParams =
                WsParams.make Map.empty
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.generationStart generationStart
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.generationEnd generationEnd
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.reportInterval reportInterval
                |> WsParamsAttrs.setReportType ShcWsParamKeys.reportType reportType

            return wsParams
        }

    

    let toWsParams 
            (useParallel:bool<useParallel>)
            (quirkRun:quirkRun)
        =
        result {

            let modelParamSet = quirkRun |> QuirkRun.getModelParamSet

            let quirkWorldLineId = QuirkRun.makeQuirkWorldLineId modelParamSet quirkModelType.Shc

            let! (_, mutationRate) = 
                 modelParamSet 
                 |> ModelParamSet.getMutationRate ("mutationRate" |> UMX.tag<modelParamName>)

            let! (_, noiseFraction) = 
                 modelParamSet 
                 |> ModelParamSet.getNoiseFraction ("noiseFraction" |> UMX.tag<modelParamName>)

            let! (_, modelAlpha) = 
                 modelParamSet 
                 |> ModelParamSet.getModelAlpha ("modelAlpha" |> UMX.tag<modelParamName>)

            let! (_, parentCount) = 
                 modelParamSet 
                 |> ModelParamSet.getParentCount ("parentCount" |> UMX.tag<modelParamName>)

            let! (_, reproductionRate) = 
                 modelParamSet 
                 |> ModelParamSet.getReproductionRate ("reproductionRate" |> UMX.tag<modelParamName>)

            let! (_, sorterSetPruneMethod) = 
                 modelParamSet 
                 |> ModelParamSet.getSorterSetPruneMethod ("sorterSetPruneMethod" |> UMX.tag<modelParamName>)

            let! (_, stageWeight) = 
                 modelParamSet 
                 |> ModelParamSet.getStageWeight ("stageWeight" |> UMX.tag<modelParamName>)

            let! (_, switchGenMode) = 
                 modelParamSet 
                 |> ModelParamSet.getSwitchGenMode ("switchGenMode" |> UMX.tag<modelParamName>)


            let wasParamsBase = 
                WsParams.make Map.empty
                |> WsParamsAttrs.setUseParallel ShcWsParamKeys.useParallel useParallel
                |> WsParamsAttrs.setQuirkWorldLineId ShcWsParamKeys.quirkWorldLineId quirkWorldLineId
                |> WsParamsAttrs.setMutationRate ShcWsParamKeys.mutationRate mutationRate
                |> WsParamsAttrs.setNoiseFraction ShcWsParamKeys.noiseFraction noiseFraction
                |> WsParamsAttrs.setOrder ShcWsParamKeys.order (modelAlpha |> ModelAlpha.getOrder)
                |> WsParamsAttrs.setSortableSetCfgType ShcWsParamKeys.sortableSetCfgType (modelAlpha |> ModelAlpha.getSortableSetCfgType)
                |> WsParamsAttrs.setSwitchCount ShcWsParamKeys.switchCount (modelAlpha |> ModelAlpha.getSwitchCount)
                |> WsParamsAttrs.setSorterEvalMode ShcWsParamKeys.sorterEvalMode (modelAlpha |> ModelAlpha.getSorterEvalMode)
                |> WsParamsAttrs.setSorterCount ShcWsParamKeys.sorterCount parentCount
                |> WsParamsAttrs.setReproductionRate ShcWsParamKeys.reproductionRate reproductionRate
                |> WsParamsAttrs.setSorterSetPruneMethod ShcWsParamKeys.sorterSetPruneMethod sorterSetPruneMethod
                |> WsParamsAttrs.setStageWeight ShcWsParamKeys.stageWeight stageWeight
                |> WsParamsAttrs.setSwitchGenMode ShcWsParamKeys.switchGenMode switchGenMode


            let runParamSet = quirkRun |> QuirkRun.getRunParamSet

            let! runWsParams =
                match runParamSet with
                | runParamSet.Sim simParamSet ->
                    getWsParamsFromSimParamSet simParamSet
                | runParamSet.Report reportParamSet -> 
                    getWsParamsFromReportParamSet reportParamSet


            let wasParams = wasParamsBase |> WsParams.merge runWsParams

            return wasParams
        }



    let doSimRun
            (rootDir:string<folderPath>)
            (projectName: string<projectName>) 
            (projectDataStore:IProjectDataStore)
            (wsParams:wsParams)

        =
        result {
            let! res = InterGenShc.procWl rootDir projectName projectDataStore wsParams

            return ()
        }


    let doReportRun
            (rootDir:string<folderPath>)
            (projectName: string<projectName>) 
            (projectDataStore:IProjectDataStore)
            (wsParams:wsParams)
        =
        result {
            let! res = InterGenShc.procWl rootDir projectName projectDataStore wsParams

            return ()
        }

    let doRun
            (rootDir:string<folderPath>)
            (projectName: string<projectName>) 
            (projectDataStore:IProjectDataStore)
            (useParallel:bool<useParallel>)
            (quirkRun:quirkRun)
        =
        let res = 
            result {
                let! wsParams = quirkRun |> toWsParams useParallel
                return 
                    match (quirkRun |> QuirkRun.getRunParamSet) with
                    | runParamSet.Sim sps -> doSimRun rootDir projectName projectDataStore wsParams
                    | runParamSet.Report rps -> doReportRun rootDir projectName projectDataStore wsParams
            }
        match res with
        | Ok v -> Console.WriteLine "Ok"
        | Error m -> Console.WriteLine $"error: {m}"
        ()



    //let doRun
    //        (rootDir:string<folderPath>)
    //        (projectName: string<projectName>) 
    //        (projectDataStore:IProjectDataStore)
    //        (useParallel:bool<useParallel>)
    //        (quirkRun:quirkRun)
    //    =
    //    let wsParamsR = quirkRun |> toWsParams useParallel
    //    let res = 
    //        match wsParamsR with
    //        | Error m -> Console.WriteLine $"error: {m}"
    //        | Ok wsParams ->
    //             match (quirkRun |> QuirkRun.getRunParamSet) with
    //             | runParamSet.Sim sps -> doSimRun rootDir projectName projectDataStore wsParams
    //             | runParamSet.Report rps -> doReportRun rootDir projectName projectDataStore wsParams

    //    match res with
    //    | Ok v -> Console.WriteLine "Ok"
    //    | Error m -> Console.WriteLine $"error: {m}"
    //    ()