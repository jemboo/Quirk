namespace Quirk.Project

open FSharp.UMX
open Quirk.Core



type quirkModelType =
    | Shc
    | Ga


module QuirkModelType =

    let toString 
            (quirkProjectType:quirkModelType)
        =
        match quirkProjectType with
        | Shc -> "Shc"
        | Ga -> "Ga"


    let fromString 
            (qrt:string)
        =
        match qrt with
        | "Shc" -> quirkModelType.Shc |> Ok
        | "Ga" -> quirkModelType.Ga |> Ok
        | _ -> $"{qrt} not handled in QuirkProjectType.fromString" |> Error



type quirkRun = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            quirkModelType: quirkModelType
            quirkModelParamSet: modelParamSet
        }


module QuirkRun =

    let makeQuirkRunId
            (quirkModelParamSet:modelParamSet)
            (quirkProjectType:quirkModelType)
        =
            [
                quirkModelParamSet :> obj;
                quirkProjectType :> obj
            ] 
            |> GuidUtils.guidFromObjs
            |> UMX.tag<quirkRunId>


    let create 
            (quirkModelType: quirkModelType)
            (quirkModelParamSet: modelParamSet)
        =
        { 
            quirkRunId = makeQuirkRunId quirkModelParamSet quirkModelType
            quirkModelType = quirkModelType
            quirkModelParamSet = quirkModelParamSet
        }



    let getQuirkRunId (quirkRun:quirkRun) = 
            quirkRun.quirkRunId

    let getQuirkModelType (quirkRun:quirkRun) = 
            quirkRun.quirkModelType

    let getModelParamSet (quirkRun:quirkRun) = 
            quirkRun.quirkModelParamSet



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

