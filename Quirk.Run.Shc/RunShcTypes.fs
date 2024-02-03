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
    let generationStart = "generationStart" |> UMX.tag<wsParamsKey>
    let generationEnd = "generationEnd" |> UMX.tag<wsParamsKey>
    let mutationRate = "mutationRate" |> UMX.tag<wsParamsKey>
    let noiseFraction = "noiseFraction" |> UMX.tag<wsParamsKey>
    let order = "order" |> UMX.tag<wsParamsKey>
    let quirkWorldLineId = "quirkWorldLineId" |> UMX.tag<wsParamsKey>
    let reportInterval = "reportInterval" |> UMX.tag<wsParamsKey>
    let reportType = "reportType" |> UMX.tag<wsParamsKey>
    let reproductionRate = "reproductionRate" |> UMX.tag<wsParamsKey>
    let rngGenCreate = "rngGenCreate" |> UMX.tag<wsParamsKey>
    let rngGenMutate = "rngGenMutate" |> UMX.tag<wsParamsKey>
    let rngGenPrune = "rngGenPrune" |> UMX.tag<wsParamsKey>
    let runType = "runType" |> UMX.tag<wsParamsKey>
    let snapshotInterval = "snapshotInterval" |> UMX.tag<wsParamsKey>
    let sortableSetCfgType = "sortableSetCfgType" |> UMX.tag<wsParamsKey>
    let sorterCount = "sorterCount" |> UMX.tag<wsParamsKey>
    let sorterCountMutated = "sorterCountMutated" |> UMX.tag<wsParamsKey>
    let sorterEvalMode = "sorterEvalMode" |> UMX.tag<wsParamsKey>
    let sorterLength = "sorterLength" |> UMX.tag<wsParamsKey>
    let sortableSetId = "sortableSetId" |> UMX.tag<wsParamsKey>
    let sorterSetPruneMethod = "sorterSetPruneMethod" |> UMX.tag<wsParamsKey>
    let stagesSkipped = "stagesSkipped" |> UMX.tag<wsParamsKey>
    let stageWeight = "stageWeight" |> UMX.tag<wsParamsKey>
    let switchCount = "switchCount" |> UMX.tag<wsParamsKey>
    let switchGenMode = "switchGenMode" |> UMX.tag<wsParamsKey>
    let useParallel = "useParallel" |> UMX.tag<wsParamsKey>
