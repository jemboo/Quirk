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
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let shcO64 = O_64.plex64
                return! cCfgPlexDataStore.SaveCfgPlex shcO64
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchCfgPlex"



    let dispatchGenSimScript
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (firstScriptIndex:int)
            (scriptCount:int)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let lsO64 = O_64.quirkSimScripts firstScriptIndex scriptCount
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
            (firstScriptIndex:int)
            (scriptCount:int)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let lsO64 = O_64.quirkReportScripts firstScriptIndex scriptCount
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
        | [| "Shc_064" |] -> 
            result {
                let shcO64 = O_64.plex64
                return! cCfgPlexDataStore.SaveCfgPlex shcO64
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchRunScript"


    let fromQuirkProgramMode
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (firstScriptIndex:int)
            (scriptCount:int)
            (qpm:quirkProgramMode) = 
        let res =
            match qpm with
            | CfgPlex -> dispatchCfgPlex cCfgPlexDataStore projectName
            | GenSimScript -> dispatchGenSimScript cCfgPlexDataStore projectName firstScriptIndex scriptCount
            | GenReportScript -> dispatchGenReportScript cCfgPlexDataStore projectName firstScriptIndex scriptCount
            | RunScript  -> dispatchRunScript cCfgPlexDataStore projectName

        res



