namespace Quirk.Runner
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Cfg.Shc
open Quirk.Storage


module ScriptDispatcher =
    

    let dispatchCfgPlex
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let shcO64 = (O_64.plex64 projectName cfgPlexName)
                return! (cCfgPlexDataStore.SaveCfgPlex rootDir shcO64).Result
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchCfgPlex"



    let dispatchGenSimScript
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
            (firstScriptIndex:int)
            (runCount:int)
            (maxRunSetSize:int)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let cfgPlex = cCfgPlexDataStore.GetCfgPlex rootDir projectName cfgPlexName

                let lsO64 = (O_64.quirkSimScripts projectName cfgPlexName firstScriptIndex runCount maxRunSetSize)
                             |> Array.toList
                let! saveRes = lsO64 |> List.map(fun qs -> (cCfgPlexDataStore.SaveScript rootDir qs).Result)
                              |> Result.sequence
                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchGenSimScript"


    let dispatchGenReportScript
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
            (firstScriptIndex:int)
            (runCount:int)
            (maxRunSetSize:int)
            (reportTypeArg:string<reportType>)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let lsO64 = (O_64.quirkReportScripts projectName cfgPlexName firstScriptIndex runCount maxRunSetSize)
                             |> Array.toList
                let! saveRes = lsO64 |> List.map(fun qs -> (cCfgPlexDataStore.SaveScript rootDir qs).Result)
                              |> Result.sequence
                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchGenReportScript"


    let dispatchRunScript
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (useParallel:bool)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let! (scriptName, script) = (cCfgPlexDataStore.GetNextScript rootDir projectName).Result
                let ul = script |> ScriptRun.runQuirkScript rootDir cCfgPlexDataStore projectName
                let qua = cCfgPlexDataStore.FinishScript rootDir projectName scriptName

                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchRunScript"


    let fromQuirkProgramMode
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectNameOpt:string<projectName> option)
            (cfgPlexNameOpt:string<cfgPlexName> option)
            (firstScriptIndexOpt:int option)
            (runCountOpt:int option)
            (maxRunSetSizeOpt:int option)
            (reportTypeArgOpt:string<reportType> option)
            (useParallelOpt:bool option)
            (quirkProgramModeOpt:quirkProgramMode option) = 

        let projectName = projectNameOpt |> Option.get
        let cfgPlexName = cfgPlexNameOpt |> Option.get
        let quirkProgramMode = quirkProgramModeOpt |> Option.get

        let res =
            match quirkProgramMode with
            | CfgPlex -> 
                    dispatchCfgPlex rootDir cCfgPlexDataStore projectName cfgPlexName
            | GenSimScript -> 
                    let firstScriptIndex = firstScriptIndexOpt |> Option.get
                    let runCount = runCountOpt |> Option.get
                    let maxRunSetSize = maxRunSetSizeOpt |> Option.get
                    dispatchGenSimScript rootDir cCfgPlexDataStore projectName cfgPlexName firstScriptIndex runCount maxRunSetSize
            | GenReportScript -> 
                    let firstScriptIndex = firstScriptIndexOpt |> Option.get
                    let runCount = runCountOpt |> Option.get
                    let maxRunSetSize = maxRunSetSizeOpt |> Option.get
                    let reportTypeArg = reportTypeArgOpt |> Option.get
                    dispatchGenReportScript rootDir cCfgPlexDataStore projectName cfgPlexName firstScriptIndex runCount maxRunSetSize reportTypeArg
            | RunScript  -> 
                    let useParallel = useParallelOpt |> Option.get
                    dispatchRunScript rootDir cCfgPlexDataStore projectName useParallel

        res



