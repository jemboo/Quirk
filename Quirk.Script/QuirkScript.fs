namespace Quirk.Script
open FSharp.UMX
open Quirk.Project



type quirkScript = 
    private 
        {
            scriptName: string<scriptName>;
            projectName:string<projectName>
            quirkRuns: quirkRun[]
        }

module QuirkScript =

    let create 
            (scriptName: string<scriptName>)
            (projectName:string<projectName>)
            (quirkRuns: quirkRun[])
        =
        {
            quirkScript.scriptName = scriptName
            projectName = projectName
            quirkRuns = quirkRuns
        }

    let createFromRunSet 
            (quirkRunSet:quirkRunSet)
        =
        {
            scriptName = quirkRunSet |> QuirkRunSet.getId |> UMX.untag |> string |> UMX.tag<scriptName>
            projectName = quirkRunSet |> QuirkRunSet.getProjectName  
            quirkRuns = quirkRunSet |> QuirkRunSet.getQuirkRuns 
        }

    let getScriptName (quirkScript:quirkScript) = quirkScript.scriptName
    let getProjectName (quirkScript:quirkScript) = quirkScript.projectName
    let getScriptItems (quirkScript:quirkScript) = quirkScript.quirkRuns


    //let createRunScripts
    //        (quirkModelType: quirkModelType)
    //        (scriptName: string<scriptName>)
    //        (projectFolder:string<projectName>)
    //        (simParamSet: simParamSet)
    //        (modelParamSets: modelParamSet seq)
    //    =
    //    let scriptParamSet = simParamSet |> runParamSet.Sim
    //    let scriptItems = 
    //            modelParamSets
    //            |> Seq.map(QuirkRun.create quirkModelType scriptParamSet)
    //            |> Seq.toArray
    //    create
    //        scriptName projectFolder scriptItems


    //let createReportScripts
    //        (quirkModelType: quirkModelType)
    //        (scriptName: string<scriptName>)
    //        (projectFolder:string<projectName>)
    //        (reportParamSet: reportParamSet)
    //        (modelParamSets: modelParamSet seq)
    //    =
    //    let scriptParamSet = reportParamSet |> runParamSet.Report
    //    let scriptItems = 
    //            modelParamSets
    //            |> Seq.map(QuirkRun.create quirkModelType scriptParamSet)
    //            |> Seq.toArray
    //    create
    //        scriptName projectFolder scriptItems

