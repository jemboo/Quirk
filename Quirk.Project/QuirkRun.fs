namespace Quirk.Project

open FSharp.UMX
open Quirk.Core



type quirkRun = 
    private 
        { 
            quirkWorldLineId: Guid<quirkWorldLineId>
            quirkModelType: quirkModelType
            modelParamSet: modelParamSet
            runParamSet: runParamSet
        }


module QuirkRun =

    let makeQuirkWorldLineId
            (quirkModelParamSet:modelParamSet)
            (quirkProjectType:quirkModelType)
        =
            [
                quirkModelParamSet :> obj;
                quirkProjectType :> obj
            ] 
            |> GuidUtils.guidFromObjs
            |> UMX.tag<quirkWorldLineId>


    let create 
            (quirkModelType: quirkModelType)
            (runParamSet: runParamSet)
            (quirkModelParamSet: modelParamSet)
        =
        { 
            quirkWorldLineId = makeQuirkWorldLineId quirkModelParamSet quirkModelType
            quirkModelType = quirkModelType
            modelParamSet = quirkModelParamSet
            runParamSet = runParamSet
        }

    let getQuirkWorldLineId (quirkRun:quirkRun) = 
            quirkRun.quirkWorldLineId

    let getQuirkModelType (quirkRun:quirkRun) = 
            quirkRun.quirkModelType

    let getModelParamSet (quirkRun:quirkRun) = 
            quirkRun.modelParamSet

    let getRunParamSet (quirkRun:quirkRun) = 
            quirkRun.runParamSet




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

