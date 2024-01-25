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
    let quirkWorldLineId = "quirkWorldLineId" |> UMX.tag<workspaceParamsKey>
    let runType = "runType" |> UMX.tag<workspaceParamsKey>
    let reportType = "reportType" |> UMX.tag<workspaceParamsKey>
    let generationStart = "generationStart" |> UMX.tag<workspaceParamsKey>
    let generationEnd = "generationEnd" |> UMX.tag<workspaceParamsKey>
    let generation_filter_short = "generation_filter_short" |> UMX.tag<workspaceParamsKey>
    let generation_filter_long = "generation_filter_long" |> UMX.tag<workspaceParamsKey>
    let reportInterval = "reportInterval" |> UMX.tag<workspaceParamsKey>
    let rngGenCreate = "rngGenCreate" |> UMX.tag<workspaceParamsKey>
    let rngGenMutate = "rngGenMutate" |> UMX.tag<workspaceParamsKey>
    let rngGenPrune = "rngGenPrune" |> UMX.tag<workspaceParamsKey>
    let mutationRate = "mutationRate" |> UMX.tag<workspaceParamsKey>
    let noiseFraction = "noiseFraction" |> UMX.tag<workspaceParamsKey>
    let order = "order" |> UMX.tag<workspaceParamsKey>
    let reproductionRate = "reproductionRate" |> UMX.tag<workspaceParamsKey>
    let sortableSetCfgType = "sortableSetCfgType" |> UMX.tag<workspaceParamsKey>
    let sorterCount = "sorterCount" |> UMX.tag<workspaceParamsKey>
    let sorterCountMutated = "sorterCountMutated" |> UMX.tag<workspaceParamsKey>
    let sorterEvalMode = "sorterEvalMode" |> UMX.tag<workspaceParamsKey>
    let stagesSkipped = "stagesSkipped" |> UMX.tag<workspaceParamsKey>
    let sorterLength = "sorterLength" |> UMX.tag<workspaceParamsKey>
    let stageWeight = "stageWeight" |> UMX.tag<workspaceParamsKey>
    let switchGenMode = "switchGenMode" |> UMX.tag<workspaceParamsKey>
    let sortableSetId = "sortableSetId" |> UMX.tag<workspaceParamsKey>
    let sorterSetPruneMethod = "sorterSetPruneMethod" |> UMX.tag<workspaceParamsKey>
    let useParallel = "useParallel" |> UMX.tag<workspaceParamsKey>


module RunShc =

    let getWorkspaceParamsFromSimParamSet
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

            let workspaceParams =
                WorkspaceParams.make Map.empty
                |> WorkspaceParamsAttrs.setRunType ShcWsParamKeys.runType runType.Sim
                |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generationStart generationStart
                |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generationEnd generationEnd
                |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_filter_short reportInterval
                |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_filter_long snapshotInterval

            return workspaceParams
        }


    let getWorkspaceParamsFromReportParamSet
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

            let workspaceParams =
                WorkspaceParams.make Map.empty
                |> WorkspaceParamsAttrs.setRunType ShcWsParamKeys.runType runType.Report
                |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generationStart generationStart
                |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generationEnd generationEnd
                |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.reportInterval reportInterval
                |> WorkspaceParamsAttrs.setReportType ShcWsParamKeys.reportType reportType

            return workspaceParams
        }

    

    let toWorkspaceParams 
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
                WorkspaceParams.make Map.empty
                |> WorkspaceParamsAttrs.setUseParallel ShcWsParamKeys.useParallel useParallel
                |> WorkspaceParamsAttrs.setQuirkWorldLineId ShcWsParamKeys.quirkWorldLineId quirkWorldLineId
                |> WorkspaceParamsAttrs.setMutationRate ShcWsParamKeys.mutationRate mutationRate
                |> WorkspaceParamsAttrs.setNoiseFraction ShcWsParamKeys.noiseFraction noiseFraction
                |> WorkspaceParamsAttrs.setOrder ShcWsParamKeys.order order
                |> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCount parentCount
                |> WorkspaceParamsAttrs.setReproductionRate ShcWsParamKeys.reproductionRate reproductionRate
                |> WorkspaceParamsAttrs.setSorterSetPruneMethod ShcWsParamKeys.sorterSetPruneMethod sorterSetPruneMethod
                |> WorkspaceParamsAttrs.setStageWeight ShcWsParamKeys.stageWeight stageWeight
                |> WorkspaceParamsAttrs.setSwitchGenMode ShcWsParamKeys.switchGenMode switchGenMode


            let runParamSet = quirkRun |> QuirkRun.getRunParamSet

            let! runWorkspaceParams =
                match runParamSet with
                | runParamSet.Sim simParamSet ->
                    getWorkspaceParamsFromSimParamSet simParamSet
                | runParamSet.Report reportParamSet -> 
                    getWorkspaceParamsFromReportParamSet reportParamSet


            let wasParams = wasParamsBase |> WorkspaceParams.merge runWorkspaceParams

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
                |> toWorkspaceParams
                    useParallel


            return ()
        }
