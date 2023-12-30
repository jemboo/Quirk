namespace Quirk.Project

open FSharp.UMX
open Quirk.Core
open System


[<Measure>] type quirkRunSetName


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
            id:Guid<quirkRunSetName>
            projectName:string<projectName>
            quirkRuns:quirkRun[]
        }


module QuirkRunSet = 
    
    let create 
            (projectName:string<projectName>)
            (quirkRuns: quirkRun seq)
        =
        let quirkRunsA = quirkRuns |> Seq.toArray
        let id = quirkRuns |> Seq.map box |> GuidUtils.guidFromObjs
        { 
            quirkRunSet.quirkRuns = quirkRunsA
            projectName = projectName
            id = id |> UMX.tag<quirkRunSetName>
        }

    let getId (quirkRunSet:quirkRunSet) = quirkRunSet.id

    let getProjectName (quirkRunSet:quirkRunSet) = quirkRunSet.projectName

    let getQuirkRuns (quirkRunSet:quirkRunSet) = quirkRunSet.quirkRuns

