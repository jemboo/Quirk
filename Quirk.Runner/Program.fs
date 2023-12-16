namespace Quirk.Runner
open System
open Argu
open FSharp.UMX
open Quirk.Cfg.Core
open Quirk.Script

module Program =




//--first-script-index 0 --log-level 1 --project-name o64\StagePhenoPrune --report-file-name standard --run-mode CfgPlex --script-count 3 --use-parallel true --working-directory C:\Quirk 


    let [<EntryPoint>] main argv =
        let parser = ArgumentParser.Create<CliArguments>(programName = "Quirk.Runner.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse argv
        
        let all = argResults.GetAllResults()


        let firstScriptIndexArg = argResults.GetResults First_Script_Index |> List.head |> int
        let logLevelArg = argResults.GetResults Log_level |> List.head
        let projectNameArg = argResults.GetResults Project_Name |> List.head |> UMX.tag<projectName>
        let reportFileNameArg = argResults.GetResults Report_File_Name |> List.head |> UMX.tag<reportName>
        let runModeArg = argResults.GetResults Run_Mode |> List.head
        let scriptCountArg = argResults.GetResults Script_Count |> List.head |> int
        let useParallelArg = argResults.GetResults Use_Parallel |> List.head
        let workingDirectoryArg = argResults.GetResults Working_Directory |> List.head |> UMX.tag<workingDirectory>


        let runMode = runModeArg |> quirkProgramMode.fromString |> Result.ExtractOrThrow
           
        let scriptResult = 
                runMode 
                    |> ScriptDispatcher.fromRunMode
                            workingDirectoryArg
                            projectNameArg
                            firstScriptIndexArg
                            scriptCountArg

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
