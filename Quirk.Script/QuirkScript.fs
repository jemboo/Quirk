namespace Quirk.Script
open FSharp.UMX
open Quirk.Project



type quirkScript = 
    private 
        {
            scriptName: string<scriptName>;
            projectFolder:string<projectName>
            scriptItems: scriptItem[]
        }

module QuirkScript =

    let create 
            (scriptName: string<scriptName>)
            (projectFolder:string<projectName>)
            (scriptItems: scriptItem[])
        =
        {
            quirkScript.scriptName = scriptName
            projectFolder = projectFolder
            scriptItems = scriptItems
        }

    let createRunScripts
            (quirkModelType: quirkModelType)
            (scriptName: string<scriptName>)
            (projectFolder:string<projectName>)
            (simParamSet: simParamSet)
            (modelParamSets: modelParamSet seq )
        =
        let scriptParamSet = simParamSet |> runParamSet.Sim
        let scriptItems = 
                modelParamSets
                |> Seq.map(ScriptItem.create quirkModelType scriptParamSet)
                |> Seq.toArray
        create
            scriptName projectFolder scriptItems



    let createReportScripts
            (quirkModelType: quirkModelType)
            (scriptName: string<scriptName>)
            (projectFolder:string<projectName>)
            (reportParamSet: reportParamSet)
            (modelParamSets: modelParamSet seq )
        =
        let scriptParamSet = reportParamSet |> runParamSet.Report
        let scriptItems = 
                modelParamSets
                |> Seq.map(ScriptItem.create quirkModelType scriptParamSet)
                |> Seq.toArray
        create
            scriptName projectFolder scriptItems




    let getScriptName 
            (quirkScript:quirkScript) =
        quirkScript.scriptName

    let getProjectFolder
            (quirkScript:quirkScript) =
        quirkScript.projectFolder

    let getScriptItems
            (quirkScript:quirkScript) =
        quirkScript.scriptItems
