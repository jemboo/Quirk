namespace Quirk.Runner
namespace Quirk.Runner

open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage
open System.Threading
open Quirk.Run.Shc
open Quirk.Iter


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
            (projectDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (useParallel:bool<useParallel>)
            (genStart:int<generation>)
            (genEnd:int<generation>)
            (quirkRun:quirkRun)
        =
        result {
            let! runRes = 
                match (quirkRun |> QuirkRun.getQuirkModelType) with
                    | quirkModelType.Shc ->
                        RunShc.doRun 
                            rootDir 
                            projectDataStore 
                            useParallel
                            quirkRun

                    | quirkModelType.Ga ->
                        RunGa.doRun 
                            rootDir 
                            projectDataStore 
                            useParallel
                            genStart
                            genEnd
                            quirkRun
            let! updateRes = updateProject rootDir projectDataStore projectName quirkRun
            return ()
        }


    let runQuirkScript
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (useParallel:bool<useParallel>)
            (genStart:int<generation>)
            (genEnd:int<generation>)
            (quirkScript:quirkScript)
        =
        result {
            let quirkRuns = quirkScript |> QuirkScript.getQuirkRuns
            let! yab = quirkRuns 
                      |> Array.map(runQuirkRun rootDir cCfgPlexDataStore projectName useParallel genStart genEnd)
                      |> Array.toList
                      |> Result.sequence
            return ()
        }

