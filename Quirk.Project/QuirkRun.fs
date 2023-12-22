namespace Quirk.Project

open FSharp.UMX
open Quirk.Core



type quirkProjectType =
    | Shc
    | Ga


module QuirkProjectType =

    let toString 
            (quirkProjectType:quirkProjectType)
        =
        match quirkProjectType with
        | Shc -> "Shc"
        | Ga -> "Ga"


    let fromString 
            (qrt:string)
        =
        match qrt with
        | "Shc" -> quirkProjectType.Shc |> Ok
        | "Ga" -> quirkProjectType.Ga |> Ok
        | _ -> $"{qrt} not handled in QuirkProjectType.fromString" |> Error



type quirkRun = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            quirkProjectType: quirkProjectType
            quirkModelParamSet: modelParamSet
        }


module QuirkRun =

    let makeQuirkRunId
            (quirkModelParamSet:modelParamSet)
            (quirkProjectType:quirkProjectType)
        =
            [
                quirkModelParamSet :> obj;
                quirkProjectType :> obj
            ] 
            |> GuidUtils.guidFromObjs
            |> UMX.tag<quirkRunId>


    let create 
            (quirkProjectType: quirkProjectType)
            (quirkModelParamSet: modelParamSet)
        =
        { 
            quirkRunId = makeQuirkRunId quirkModelParamSet quirkProjectType
            quirkProjectType = quirkProjectType
            quirkModelParamSet = quirkModelParamSet
        }



    let getQuirkRunId (quirkRun:quirkRun) = 
            quirkRun.quirkRunId

    let getRunType (quirkRun:quirkRun) = 
            quirkRun.quirkProjectType

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

