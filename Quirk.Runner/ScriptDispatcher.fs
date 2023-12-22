namespace Quirk.Runner
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Cfg.Shc
open Quirk.Storage


module ScriptDispatcher =

    let dispatchCfgPlex
            (cCfgPlexDataStore:ICfgPlexDataStore)
            (projectName:string<projectName>)
        =
        let fileUtils = new fileUtils()
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let shcO64 = O_64.plex64
                return! cCfgPlexDataStore.SaveCfgPlex shcO64
            }

        | [| "GenScript" |] -> () |> Ok
        | [| "RunScript"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchCfgPlex"



    let dispatchGenScript
            (cCfgPlexDataStore:ICfgPlexDataStore)
            (projectName:string<projectName>)
            (firstScriptIndex:int)
            (scriptCount:int)
        =
        ()  |> Ok


    let dispatchRunScript
            (cCfgPlexDataStore:ICfgPlexDataStore)
            (projectName:string<projectName>)
        =
        ()  |> Ok


    let fromQuirkProgramMode
            (cCfgPlexDataStore:ICfgPlexDataStore)
            (projectName:string<projectName>)
            (firstScriptIndex:int)
            (scriptCount:int)
            (rm:quirkProgramMode) = 
        let res =
            match rm with
            | CfgPlex -> dispatchCfgPlex cCfgPlexDataStore projectName
            | GenScript -> dispatchGenScript cCfgPlexDataStore projectName firstScriptIndex scriptCount
            | RunScript  -> dispatchRunScript cCfgPlexDataStore projectName

        res



