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


module ShcWsParamKeys =
    let quirkWorldLineId = "quirkWorldLineId" |> UMX.tag<wsParamsKey>
    let runType = "runType" |> UMX.tag<wsParamsKey>
    let reportType = "reportType" |> UMX.tag<wsParamsKey>
    let generationStart = "generationStart" |> UMX.tag<wsParamsKey>
    let generationEnd = "generationEnd" |> UMX.tag<wsParamsKey>
    let generation_filter_short = "generation_filter_short" |> UMX.tag<wsParamsKey>
    let generation_filter_long = "generation_filter_long" |> UMX.tag<wsParamsKey>
    let reportInterval = "reportInterval" |> UMX.tag<wsParamsKey>
    let rngGenCreate = "rngGenCreate" |> UMX.tag<wsParamsKey>
    let rngGenMutate = "rngGenMutate" |> UMX.tag<wsParamsKey>
    let rngGenPrune = "rngGenPrune" |> UMX.tag<wsParamsKey>
    let mutationRate = "mutationRate" |> UMX.tag<wsParamsKey>
    let noiseFraction = "noiseFraction" |> UMX.tag<wsParamsKey>
    let order = "order" |> UMX.tag<wsParamsKey>
    let reproductionRate = "reproductionRate" |> UMX.tag<wsParamsKey>
    let sortableSetCfgType = "sortableSetCfgType" |> UMX.tag<wsParamsKey>
    let sorterCount = "sorterCount" |> UMX.tag<wsParamsKey>
    let sorterCountMutated = "sorterCountMutated" |> UMX.tag<wsParamsKey>
    let sorterEvalMode = "sorterEvalMode" |> UMX.tag<wsParamsKey>
    let stagesSkipped = "stagesSkipped" |> UMX.tag<wsParamsKey>
    let sorterLength = "sorterLength" |> UMX.tag<wsParamsKey>
    let stageWeight = "stageWeight" |> UMX.tag<wsParamsKey>
    let switchGenMode = "switchGenMode" |> UMX.tag<wsParamsKey>
    let sortableSetId = "sortableSetId" |> UMX.tag<wsParamsKey>
    let sorterSetPruneMethod = "sorterSetPruneMethod" |> UMX.tag<wsParamsKey>
    let useParallel = "useParallel" |> UMX.tag<wsParamsKey>


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
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.generation_filter_short reportInterval
                |> WsParamsAttrs.setGeneration ShcWsParamKeys.generation_filter_long snapshotInterval

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

            let! (_, order) = 
                 modelParamSet 
                 |> ModelParamSet.getOrder ("order" |> UMX.tag<modelParamName>)

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
                |> WsParamsAttrs.setOrder ShcWsParamKeys.order order
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



    let doRun
            (rootDir:string<folderPath>)
            (projectDataStore:IProjectDataStore)
            (useParallel:bool<useParallel>)
            (quirkRun:quirkRun)
        =
        result {
            let wsParams = 
                quirkRun 
                |> toWsParams
                    useParallel


            return ()
        }
