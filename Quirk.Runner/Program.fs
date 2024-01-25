namespace Quirk.Runner
open System
open Argu
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage
open Quirk.Iter

module Program =

// CfgPlex
//--working-directory C:\Quirk --program-mode CfgPlex --project-name Shc_064 --cfgplex-name Shc_064_cfgPlex --log-level 1

// GenSimScript
//--working-directory C:\Quirk --program-mode GenSimScript --project-name Shc_064 --cfgplex-name Shc_064_cfgPlex --gen-start 0 --gen-end 100 --report-interval 10 --snapshot-interval 50 --first-script-index 0 --run-count 400 --maxrunsetsize 20 --log-level 1

// GenReportScript
//--working-directory C:\Quirk --program-mode GenReportScript --project-name Shc_064 --cfgplex-name Shc_064_cfgPlex --gen-start 0 --gen-end 100 --report-interval 10 --snapshot-interval 50 --first-script-index 0 --run-count 400 --maxrunsetsize 20 --report-type bins --log-level 1

// RunScript
//--working-directory C:\Quirk --program-mode RunScript --project-name Shc_064 --cfgplex-name Shc_064_cfgPlex --gen-start 0 --gen-end 100 --report-interval 10 --snapshot-interval 50 --use-parallel true --log-level 1



    let [<EntryPoint>] main argv =



        let parser = ArgumentParser.Create<CliArguments>(programName = "Quirk.Runner.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse argv
        
        let all = argResults.GetAllResults()
        let workingDirectoryArg = argResults.GetResults Working_Directory |> ArguUtils.firstOption |> Option.map(UMX.tag<workingDirectory>)
        let quirkProgramModeArg = argResults.GetResults Program_Mode |> ArguUtils.firstOption
        let projectNameArg = argResults.GetResults Project_Name |> ArguUtils.firstOption |> Option.map(UMX.tag<projectName>)
        let cfgPlexNameArg = argResults.GetResults CfgPlex_Name |> ArguUtils.firstOption |> Option.map(UMX.tag<cfgPlexName>)
        let firstScriptIndexArg = argResults.GetResults First_Script_Index |> ArguUtils.firstOption |> Option.map(int)
        let runCountArg = argResults.GetResults Run_Count |> ArguUtils.firstOption |> Option.map(int)
        let maxRunSetSizeArg = argResults.GetResults MaxRunSetSize |> ArguUtils.firstOption |> Option.map(int)
        let reportTypeArg = argResults.GetResults Report_Type |> ArguUtils.firstOption |> Option.map(UMX.tag<reportType>)
        let genStartTypeArg = argResults.GetResults Gen_Start |> ArguUtils.firstOption |> Option.map(UMX.tag<generation>)
        let genEndTypeArg = argResults.GetResults Gen_End |> ArguUtils.firstOption |> Option.map(UMX.tag<generation>)
        let reportIntervalTypeArg = argResults.GetResults Report_Interval |> ArguUtils.firstOption |> Option.map(UMX.tag<generation>)
        let snapshotIntervalTypeArg = argResults.GetResults Snapshot_Interval |> ArguUtils.firstOption |> Option.map(UMX.tag<generation>)
        let useParallelArg = argResults.GetResults Use_Parallel |> ArguUtils.firstOption |> Option.map(UMX.tag<useParallel>)
        let rootDir = workingDirectoryArg |> Option.get |> UMX.cast<workingDirectory,folderPath>
        let projectFileStore = new projectFileStore()
 
        
        Console.WriteLine($"*****************************")
        Console.WriteLine($"Running script")
        Console.WriteLine($"*****************************")


        let scriptResult = 
                quirkProgramModeArg 
                    |> ScriptDispatcher.fromQuirkProgramMode
                            rootDir
                            projectFileStore
                            projectNameArg
                            cfgPlexNameArg
                            firstScriptIndexArg
                            runCountArg
                            maxRunSetSizeArg
                            genStartTypeArg
                            genEndTypeArg
                            reportIntervalTypeArg
                            snapshotIntervalTypeArg
                            reportTypeArg
                            useParallelArg

        match scriptResult with
        | Result.Ok _ -> Console.WriteLine("Script ran successfully (1)")
        | Result.Error m -> Console.WriteLine($"Script ran with error: {m}")

        Console.ReadKey() |> ignore
        0
