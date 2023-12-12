namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core
open Quirk.Run.Core


type quirkRun = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            quirkRunType: quirkRunType
            quirkRunMode: quirkRunMode
            cfgModelParamSet: cfgModelParamSet
            cfgRunParamSet: cfgRunParamSet
        }


module QuirkRun =

    let makeQuirkRunId
            (cfgModelParamSet:cfgModelParamSet)
            (quirkRunType:quirkRunType)
        =
            [
                cfgModelParamSet :> obj;
                quirkRunType :> obj
            ] 
            |> GuidUtils.guidFromObjs
            |> UMX.tag<quirkRunId>


    let create 
            (quirkRunType: quirkRunType)
            (quirkRunMode: quirkRunMode)
            (cfgRunParamSet: cfgRunParamSet)
            (cfgModelParamSet: cfgModelParamSet)
        =
        { 
            quirkRunId = makeQuirkRunId cfgModelParamSet quirkRunType
            quirkRunType = quirkRunType
            quirkRunMode = quirkRunMode
            cfgModelParamSet = cfgModelParamSet
            cfgRunParamSet = cfgRunParamSet
        }

    let createFromCfgPlex
            (quirkRunType:quirkRunType)
            (quirkRunMode:quirkRunMode)
            (cfgRunParamSet:cfgRunParamSet)
            (cfgPlex:cfgPlex)
            (replicaNumber: int<replicaNumber>) 
        =
        CfgPlex.makeModelParamSets cfgPlex replicaNumber
        |> List.map(create quirkRunType quirkRunMode cfgRunParamSet)


    let getQuirkRunId (quirkRun:quirkRun) = 
            quirkRun.quirkRunId

    let getRunMode (quirkRun:quirkRun) = 
            quirkRun.quirkRunMode

    let getRunType (quirkRun:quirkRun) = 
            quirkRun.quirkRunType

    let getRunParamSet (quirkRun:quirkRun) = 
            quirkRun.cfgRunParamSet

    let getModelParamSet (quirkRun:quirkRun) = 
            quirkRun.cfgModelParamSet



type quirkRunSet = 
    private 
        { 
            quirkRuns:quirkRun[]
        }


module QuirkRunSet = 
    
    let create 
            (quirkRuns: quirkRun seq)
        =
        { quirkRunSet.quirkRuns = quirkRuns |> Seq.toArray }


    let getQuirkRuns 
            (quirkRunSet:quirkRunSet) =
        quirkRunSet.quirkRuns


    let createFromCfgPlex
            (quirkRunType:quirkRunType)
            (quirkRunMode:quirkRunMode)
            (cfgRunParamSet:cfgRunParamSet)
            (cfgPlex:cfgPlex)
            (replicaNumber: int<replicaNumber>) 
        =
        {
            quirkRunSet.quirkRuns =
                CfgPlex.makeModelParamSets cfgPlex replicaNumber
                |> List.map(QuirkRun.create quirkRunType quirkRunMode cfgRunParamSet)
                |> List.toArray
        }
