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
    let getQuirkRuns (quirkScript:quirkScript) = quirkScript.quirkRuns

