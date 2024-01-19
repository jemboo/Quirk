namespace Quirk.Runner
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
    let generation_current = "generation_current" |> UMX.tag<workspaceParamsKey>
    let generation_filter_short = "generation_filter_short" |> UMX.tag<workspaceParamsKey>
    let generation_filter_long = "generation_filter_long" |> UMX.tag<workspaceParamsKey>
    let generation_max = "generation_max" |> UMX.tag<workspaceParamsKey>
    let rngGenCreate = "rngGenCreate" |> UMX.tag<workspaceParamsKey>
    let rngGenMutate = "rngGenMutate" |> UMX.tag<workspaceParamsKey>
    let rngGenPrune = "rngGenPrune" |> UMX.tag<workspaceParamsKey>
    let mutationRate = "mutationRate" |> UMX.tag<workspaceParamsKey>
    let noiseFraction = "noiseFraction" |> UMX.tag<workspaceParamsKey>
    let order = "order" |> UMX.tag<workspaceParamsKey>
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


    let toWorkspaceParams 
            (quirkRun:quirkRun)
        =
        result {
            let modelParamSet = quirkRun |> QuirkRun.getModelParamSet
            let! simParamSet = quirkRun 
                              |> QuirkRun.getRunParamSet
                              |> RunParamSet.toSimParamSet


            let quirkWorldLineId = QuirkRun.makeQuirkWorldLineId modelParamSet quirkModelType.Shc

            return 
                WorkspaceParams.make Map.empty
                |> WorkspaceParamsAttrs.setQuirkWorldLineId ShcWsParamKeys.quirkWorldLineId quirkWorldLineId
                //|> WorkspaceParamsAttrs.setSortableSetCfgType ShcWsParamKeys.sortableSetCfgType (shcInitRunCfg.sortableSetCfgType)

        }


        //let _nextRngGen rng =
        //    rng
        //    |> Rando.fromRngGen
        //    |> Rando.toRngGen

        //let rngGenCreate = (_nextRngGen shcInitRunCfg.rngGen)
        //let rngGenMutate = (_nextRngGen rngGenCreate)
        //let rngGenPrune = (_nextRngGen rngGenMutate)

        //|> WorkspaceParamsAttrs.setRunId ShcWsParamKeys.runId (shcInitRunCfg |> getRunId)
        //|> WorkspaceParamsAttrs.setSortableSetCfgType ShcWsParamKeys.sortableSetCfgType (shcInitRunCfg.sortableSetCfgType)
        //|> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_current (0 |> Generation.create)
        //|> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_max (shcInitRunCfg.newGenerations)
        //|> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenCreate rngGenCreate
        //|> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenMutate rngGenMutate
        //|> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenPrune rngGenPrune
        //|> WorkspaceParamsAttrs.setMutationRate ShcWsParamKeys.mutationRate shcInitRunCfg.mutationRate
        //|> WorkspaceParamsAttrs.setNoiseFraction ShcWsParamKeys.noiseFraction (Some shcInitRunCfg.noiseFraction)
        //|> WorkspaceParamsAttrs.setOrder ShcWsParamKeys.order shcInitRunCfg.order
        //|> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCount shcInitRunCfg.sorterCount
        //|> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCountMutated shcInitRunCfg.sorterCountMutated
        //|> WorkspaceParamsAttrs.setSorterEvalMode ShcWsParamKeys.sorterEvalMode shcInitRunCfg.sorterEvalMode
        //|> WorkspaceParamsAttrs.setStageCount ShcWsParamKeys.stagesSkipped shcInitRunCfg.stagesSkipped
        //|> WorkspaceParamsAttrs.setStageWeight ShcWsParamKeys.stageWeight shcInitRunCfg.stageWeight
        //|> WorkspaceParamsAttrs.setSwitchCount ShcWsParamKeys.sorterLength shcInitRunCfg.switchCount
        //|> WorkspaceParamsAttrs.setSwitchGenMode ShcWsParamKeys.switchGenMode shcInitRunCfg.switchGenMode
        //|> WorkspaceParamsAttrs.setSorterSetPruneMethod ShcWsParamKeys.sorterSetPruneMethod shcInitRunCfg.sorterSetPruneMethod
        //|> WorkspaceParamsAttrs.setGenerationFilter ShcWsParamKeys.generation_filter_short shcInitRunCfg.reportFilter
        //|> WorkspaceParamsAttrs.setGenerationFilter ShcWsParamKeys.generation_filter_long shcInitRunCfg.fullReportFilter
        //|> WorkspaceParamsAttrs.setUseParallel ShcWsParamKeys.useParallel useParallel





    let doRun
            (rootDir:string<folderPath>)
            (projectDataStore:IProjectDataStore)
            (quirkRun:quirkRun)
        =
        result {
            
            return ()
        }
