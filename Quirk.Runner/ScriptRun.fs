namespace Quirk.Runner
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage


module ScriptRun =



    let runQuirkRun
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (quirkRun:quirkRun)
        =
        result {
            let! prj = cCfgPlexDataStore.GetProject projectName
            let! projUpdated = quirkRun |> QuirkProject.updateProject prj
            let! res = cCfgPlexDataStore.SaveProject projUpdated
            return ()
        }


    let runQuirkScript
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (quirkScript:quirkScript)
        =
        result {
            let quirkRuns = quirkScript |> QuirkScript.getQuirkRuns
            let! yab = quirkRuns 
                      |> Array.map(runQuirkRun cCfgPlexDataStore projectName)
                      |> Array.toList
                      |> Result.sequence
            return ()
        }

