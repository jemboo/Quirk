namespace Quirk.Runner
open System.Threading.Tasks
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage
open System.Threading


module ScriptRun =

    let updateProject
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (quirkRun:quirkRun)
        =
        result {
            use mutex = new Mutex(false, "ProjectUpdateMutex")
            if mutex.WaitOne() then
                let! prj = (cCfgPlexDataStore.GetProjectAsync rootDir projectName).Result
                let! projUpdated = quirkRun |> QuirkProject.updateProject prj
                let! res = (cCfgPlexDataStore.SaveProjectAsync rootDir projUpdated).Result
                mutex.ReleaseMutex()
            return ()
        }


    let runQuirkRun
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (quirkRun:quirkRun)
        =
        result {
            let! res = updateProject rootDir cCfgPlexDataStore projectName quirkRun
            return ()
        }


    let runQuirkScript
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (quirkScript:quirkScript)
        =
        result {
            let quirkRuns = quirkScript |> QuirkScript.getQuirkRuns
            let! yab = quirkRuns 
                      |> Array.map(runQuirkRun rootDir cCfgPlexDataStore projectName)
                      |> Array.toList
                      |> Result.sequence
            return ()
        }

