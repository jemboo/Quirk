namespace Quirk.Runner
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Cfg.Shc
open Quirk.Storage


module ScriptDispatcher =

    let dispatchCfgPlex
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064b" |] -> 
            result {
                let shcO64 = (O_64.plex64 projectName cfgPlexName)
                return! cCfgPlexDataStore.SaveCfgPlex shcO64
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchCfgPlex"



    let dispatchGenSimScript
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
            (firstScriptIndex:int)
            (scriptCount:int)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064b" |] -> 
            result {
                let cfgPlex = cCfgPlexDataStore.GetCfgPlex projectName cfgPlexName

                let lsO64 = (O_64.quirkSimScripts projectName cfgPlexName firstScriptIndex scriptCount)
                             |> Array.toList
                let! saveRes = lsO64 |> List.map(cCfgPlexDataStore.SaveScript)
                              |> Result.sequence
                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchGenSimScript"


    let dispatchGenReportScript
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
            (firstScriptIndex:int)
            (scriptCount:int)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064b" |] -> 
            result {
                let lsO64 = (O_64.quirkReportScripts projectName cfgPlexName firstScriptIndex scriptCount)
                             |> Array.toList
                let! saveRes = lsO64 |> List.map(cCfgPlexDataStore.SaveScript)
                              |> Result.sequence
                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchGenReportScript"


    let dispatchRunScript
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064b" |] -> 
            result {
                let! (scriptName, script) = cCfgPlexDataStore.GetNextScript projectName
                let ul = script |> ScriptRun.runQuirkScript cCfgPlexDataStore projectName
                let qua = cCfgPlexDataStore.FinishScript projectName scriptName

                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchRunScript"


    let fromQuirkProgramMode
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
            (firstScriptIndex:int)
            (scriptCount:int)
            (qpm:quirkProgramMode) = 
        let res =
            match qpm with
            | CfgPlex -> dispatchCfgPlex cCfgPlexDataStore projectName cfgPlexName
            | GenSimScript -> dispatchGenSimScript cCfgPlexDataStore projectName cfgPlexName firstScriptIndex scriptCount
            | GenReportScript -> dispatchGenReportScript cCfgPlexDataStore projectName cfgPlexName firstScriptIndex scriptCount
            | RunScript  -> dispatchRunScript cCfgPlexDataStore projectName

        res



