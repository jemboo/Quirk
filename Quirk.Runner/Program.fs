namespace Quirk.Runner
open System
open Argu
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage

module Program =

// CfgPlex
//--working-directory C:\Quirk --program-mode CfgPlex --project-name Shc_064 --cfgplex-name Shc_064_cfgPlex --log-level 1

// GenSimScript
//--working-directory C:\Quirk --program-mode GenSimScript --project-name Shc_064 --cfgplex-name Shc_064_cfgPlex --first-script-index 0 --run-count 400 --maxrunsetsize 20 --log-level 1

// GenReportScript
//--working-directory C:\Quirk --program-mode GenReportScript --project-name Shc_064 --cfgplex-name Shc_064_cfgPlex --first-script-index 0 --run-count 400 --maxrunsetsize 20 --report-type bins --log-level 1

// RunScript
//--working-directory C:\Quirk --program-mode RunScript --project-name Shc_064 --cfgplex-name Shc_064_cfgPlex --use-parallel true --log-level 1



    let [<EntryPoint>] main argv =



        let parser = ArgumentParser.Create<CliArguments>(programName = "Quirk.Runner.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse argv
        
        let all = argResults.GetAllResults()
        let workingDirectoryArg = argResults.GetResults Working_Directory |> ArguUtils.wak |> Option.map(UMX.tag<workingDirectory>)

        let quirkProgramModeArg = argResults.GetResults Program_Mode |> ArguUtils.wak
        let projectNameArg = argResults.GetResults Project_Name |> ArguUtils.wak |> Option.map(UMX.tag<projectName>)
        let cfgPlexNameArg = argResults.GetResults CfgPlex_Name |> ArguUtils.wak |> Option.map(UMX.tag<cfgPlexName>)
        let firstScriptIndexArg = argResults.GetResults First_Script_Index |> ArguUtils.wak |> Option.map(int)
        let runCountArg = argResults.GetResults Run_Count |> ArguUtils.wak |> Option.map(int)
        let maxRunSetSizeArg = argResults.GetResults MaxRunSetSize |> ArguUtils.wak |> Option.map(int)
        let reportTypeArg = argResults.GetResults Report_Type |> ArguUtils.wak |> Option.map(UMX.tag<reportType> )
        let useParallelArg = argResults.GetResults Use_Parallel |> ArguUtils.wak
        let rootDir = workingDirectoryArg |> Option.get |> UMX.untag
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
                            reportTypeArg
                            useParallelArg

        match scriptResult with
        | Result.Ok _ -> Console.WriteLine("Script ran successfully (1)")
        | Result.Error m -> Console.WriteLine($"Script ran with error: {m}")

        Console.ReadKey() |> ignore

        0
