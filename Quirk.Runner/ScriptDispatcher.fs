namespace Quirk.Runner
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Cfg.Shc
open Quirk.Storage
open Quirk.Iter


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
                return! (cCfgPlexDataStore.SaveCfgPlexAsync rootDir shcO64).Result
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchCfgPlex (*75)"



    let dispatchGenSimScript
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
            (firstScriptIndex:int)
            (runCount:int)
            (maxRunSetSize:int)
            (genStart:int<generation>)
            (genMax:int<generation>)
            (reportInterval:int<generation>)
            (snapShotInterval:int<generation>)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let! cfgPlex = cCfgPlexDataStore.GetCfgPlex rootDir projectName cfgPlexName
                let lsO64 = O_64.quirkSimScripts 
                                cfgPlex 
                                firstScriptIndex 
                                runCount 
                                maxRunSetSize 
                                genStart 
                                genMax 
                                reportInterval 
                                snapShotInterval
                            |> Array.toList
                let! saveRes = lsO64 |> List.map(fun qs -> (cCfgPlexDataStore.SaveScriptAsync rootDir qs).Result)
                              |> Result.sequence
                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchGenSimScript (*76)"


    let dispatchGenReportScript
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (cfgPlexName:string<cfgPlexName>)
            (firstScriptIndex:int)
            (runCount:int)
            (maxRunSetSize:int)
            (reportTypeArg:string<reportType>)
            (genStart:int<generation>)
            (genEnd:int<generation>)
            (reportInterval:int<generation>)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let! cfgPlex = cCfgPlexDataStore.GetCfgPlex rootDir projectName cfgPlexName

                let lsO64 = O_64.quirkReportScripts 
                                cfgPlex 
                                firstScriptIndex 
                                runCount 
                                maxRunSetSize
                                reportTypeArg
                                genStart
                                genEnd
                                reportInterval
                             |> Array.toList
                let! saveRes = lsO64 |> List.map(fun qs -> (cCfgPlexDataStore.SaveScriptAsync rootDir qs).Result)
                              |> Result.sequence
                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchGenReportScript (*77)"


    let dispatchRunScript
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectName:string<projectName>)
            (useParallel:bool<useParallel>)
            (genStart:int<generation>)
            (genEnd:int<generation>)
        =
        let nA  = (projectName |> UMX.untag).Split() 
        match nA with
        | [| "Shc_064" |] -> 
            result {
                let! (scriptName, script) = (cCfgPlexDataStore.GetNextScriptAsync rootDir projectName).Result
                let ul = 
                    script 
                    |> ScriptRun.runQuirkScript 
                            rootDir 
                            cCfgPlexDataStore 
                            projectName
                            useParallel
                            genStart
                            genEnd

                let qua = cCfgPlexDataStore.FinishScript rootDir projectName scriptName

                return ()
            }

        | [| "Shc_0128" |] -> () |> Ok
        | [| "Shc_016"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchRunScript (*78)"


    let fromQuirkProgramMode
            (rootDir:string<folderPath>)
            (cCfgPlexDataStore:IProjectDataStore)
            (projectNameOpt:string<projectName> option)
            (cfgPlexNameOpt:string<cfgPlexName> option)
            (firstScriptIndexOpt:int option)
            (runCountOpt:int option)
            (maxRunSetSizeOpt:int option)
            (genStartOpt:int<generation> option)
            (genEndOpt:int<generation> option)
            (reportIntervalOpt:int<generation> option)
            (snapshotIntervalOpt:int<generation> option)
            (reportTypeArgOpt:string<reportType> option)
            (useParallelOpt:bool<useParallel> option)
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
                    let genStart = genStartOpt |> Option.get
                    let genEnd = genEndOpt |> Option.get
                    let reportInterval = reportIntervalOpt |> Option.get
                    let snapshotInterval = snapshotIntervalOpt |> Option.get
                    dispatchGenSimScript 
                        rootDir 
                        cCfgPlexDataStore 
                        projectName 
                        cfgPlexName 
                        firstScriptIndex 
                        runCount 
                        maxRunSetSize 
                        genStart 
                        genEnd 
                        reportInterval 
                        snapshotInterval
            | GenReportScript -> 
                    let firstScriptIndex = firstScriptIndexOpt |> Option.get
                    let runCount = runCountOpt |> Option.get
                    let maxRunSetSize = maxRunSetSizeOpt |> Option.get
                    let reportTypeArg = reportTypeArgOpt |> Option.get
                    let genStart = genStartOpt |> Option.get
                    let genEnd = genEndOpt |> Option.get
                    let reportInterval = reportIntervalOpt |> Option.get
                    dispatchGenReportScript 
                        rootDir 
                        cCfgPlexDataStore 
                        projectName 
                        cfgPlexName 
                        firstScriptIndex 
                        runCount 
                        maxRunSetSize 
                        reportTypeArg 
                        genStart 
                        genEnd 
                        reportInterval
            | RunScript  -> 
                    let useParallel = useParallelOpt |> Option.get
                    let genStart = genStartOpt |> Option.get
                    let genEnd = genEndOpt |> Option.get
                    dispatchRunScript 
                        rootDir 
                        cCfgPlexDataStore 
                        projectName 
                        useParallel 
                        genStart 
                        genEnd

        res



