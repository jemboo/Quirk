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


//--working-directory C:\Quirk --project-name Shc_064b --cfgplex-name Shc_064_cfgPlex --program-mode CfgPlex --first-script-index 0 --script-count 400 --report-file-name bins --use-parallel true --log-level 1


//--working-directory C:\Quirk --project-name Shc_064b --cfgplex-name Shc_064_cfgPlex --program-mode GenSimScript --first-script-index 0 --script-count 400 --report-file-name bins --use-parallel true --log-level 1

    let [<EntryPoint>] main argv =



        let parser = ArgumentParser.Create<CliArguments>(programName = "Quirk.Runner.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse argv
        
        let all = argResults.GetAllResults()


        let firstScriptIndexArg = argResults.GetResults First_Script_Index |> List.head |> int
        let logLevelArg = argResults.GetResults Log_level |> List.head
        let cfgPlexNameArg = argResults.GetResults CfgPlex_Name |> List.head |> UMX.tag<cfgPlexName>
        let projectNameArg = argResults.GetResults Project_Name |> List.head |> UMX.tag<projectName>
        let reportFileNameArg = argResults.GetResults Report_File_Name |> List.head |> UMX.tag<reportName>
        let programModeArg = argResults.GetResults Program_Mode |> List.head
        let scriptCountArg = argResults.GetResults Script_Count |> List.head |> int
        let useParallelArg = argResults.GetResults Use_Parallel |> List.head
        let workingDirectoryArg = argResults.GetResults Working_Directory |> List.head |> UMX.tag<workingDirectory>
        
        let projectFileStore = new projectFileStore(workingDirectoryArg |> UMX.untag)

        let quirkProgramMode = programModeArg |> quirkProgramMode.fromString |> Result.ExtractOrThrow
           
        let scriptResult = 
                quirkProgramMode 
                    |> ScriptDispatcher.fromQuirkProgramMode
                            projectFileStore
                            projectNameArg
                            cfgPlexNameArg
                            firstScriptIndexArg
                            scriptCountArg


        match scriptResult with
        | Result.Ok _ -> Console.WriteLine("Script ran successfully")
        | Result.Error m -> Console.WriteLine($"Script ran with error: {m}")



        //let projectFolderArg = argResults.GetResults Project_Folder |> List.head

        //let projectFolder  = projectFolderArg |> ProjectFolder.create
        //let projectFolderPath = IO.Path.Combine(workingDirectoryArg, projectFolder |> ProjectFolder.value)

        //let (scriptFileName, klinkScript) = 
        //        ScriptFileRun.getNextKlinkScript projectFolderPath
        //        |> Result.ExtractOrThrow

        //let _makeFileStore (path:string) = 
        //    new WorkspaceFileStore(path) :> IWorkspaceStore

        //let useParallel = true |> UseParallel.create

        //Console.WriteLine($"Running script: {scriptFileName}")
        0
