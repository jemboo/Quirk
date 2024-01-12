namespace Quirk.Storage
open System
open System.IO
open System.Threading.Tasks
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Cfg.Core
open Quirk.Cfg.Serialization
open Quirk.Script
open Quirk.Serialization


type projectFileStore () =
    member this.cfgPlexFolder = "cfgPlex"
    member this.scriptFolder = "scripts"
    member this.reportFolder = "reports"
    member this.scriptToDoFolder = "toDo"
    member this.scriptRunningFolder = "running"
    member this.scriptCompletedFolder = "completed"
    member this.fileExt = "txt"
    //member this.wsRootDir = wsRootDir

    member this.getComponentFolderName (wsCompType:workspaceComponentType) =
        wsCompType |> string

    member this.getProjectPathToFolder 
                (wsRootDir:string<folderPath>) 
                (projectName:string<projectName>) 
            = 
        Path.Combine(wsRootDir |> UMX.untag, projectName |> UMX.untag)
        |> UMX.tag<folderPath>

    member this.getProjectFileName 
                    (projectName:string<projectName>) 
            =
           $"{projectName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>

    member this.getCfgPlexFileName 
                    (cfgPlexName:string<cfgPlexName>) 
              =
           $"{cfgPlexName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>

    member this.getScriptFileName (scriptName:string<scriptName>) =
           $"{scriptName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>


    member this.getProjectPath 
                (wsRootDir:string<folderPath>) 
                (projectName:string<projectName>) 
           =
           Path.Combine(
                projectName |> (this.getProjectPathToFolder wsRootDir)  |> UMX.untag,
                projectName |> this.getProjectFileName  |> UMX.untag
                )
           |> UMX.tag<fullFilePath>

    member this.getCfgPlexPath 
                (wsRootDir:string<folderPath>)
                (projectName:string<projectName>) 
                (cfgPlexName:string<cfgPlexName>)
           =
           Path.Combine(
                projectName |> (this.getProjectPathToFolder wsRootDir) |> UMX.untag,
                this.cfgPlexFolder,
                cfgPlexName |> this.getCfgPlexFileName  |> UMX.untag
                )
           |> UMX.tag<fullFilePath>

    member this.getScriptPathToFolder 
                (wsRootDir:string<folderPath>) 
                (projectName:string<projectName>) 
           = 
        Path.Combine(projectName |> (this.getProjectPathToFolder wsRootDir) |> UMX.untag, this.scriptFolder)
        |> UMX.tag<folderPath>

    member this.getReportPathToFolder 
                (wsRootDir:string<folderPath>) 
                (projectName:string<projectName>) 
           = 
        Path.Combine(projectName |> (this.getProjectPathToFolder wsRootDir) |> UMX.untag, this.reportFolder)
        |> UMX.tag<folderPath>

    member this.getScriptToDoPathToFolder 
              (wsRootDir:string<folderPath>) 
              (projectName:string<projectName>) 
           = 
        Path.Combine(projectName |> (this.getScriptPathToFolder wsRootDir) |> UMX.untag, this.scriptToDoFolder)
        |> UMX.tag<folderPath>

    member this.getScriptRunningPathToFolder 
                (wsRootDir:string<folderPath>) 
                (projectName:string<projectName>) 
            = 
        Path.Combine(projectName |> (this.getScriptPathToFolder wsRootDir) |> UMX.untag, this.scriptRunningFolder)
        |> UMX.tag<folderPath>

    member this.getScriptCompletedPathToFolder 
                (wsRootDir:string<folderPath>) 
                (projectName:string<projectName>) 
            = 
        Path.Combine(projectName |> (this.getScriptPathToFolder wsRootDir) |> UMX.untag, this.scriptCompletedFolder)
        |> UMX.tag<folderPath>

    member this.getScriptFileName (quirkScript:quirkScript) =
           $"{quirkScript |> QuirkScript.getScriptName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>

    member this.getComponentPathToFolder
                (wsRootDir:string<folderPath>)
                (projectName:string<projectName>)
                (wsCompType:workspaceComponentType) 
           = 
        Path.Combine(projectName |> (this.getProjectPathToFolder wsRootDir) |> UMX.untag, this.scriptCompletedFolder)


    member this.getNextScript 
                (wsRootDir:string<folderPath>) 
                (projectName:string<projectName>) 
            = 
        let sourceFolder = this.getScriptToDoPathToFolder wsRootDir projectName
        let moveToFolder = this.getScriptRunningPathToFolder wsRootDir projectName
        result {
            let! (fileName, fileContents) = 
                    TextIO.getFileTextFromFolderAndMove sourceFolder moveToFolder
            let! quirkScript = fileContents |> QuirkScriptDto.fromJson
            let scriptName = fileName |> UMX.untag |> Path.GetFileNameWithoutExtension |> UMX.tag<scriptName>
            return (scriptName, quirkScript)
        }

    member this.getCfgPlex
                (wsRootDir:string<folderPath>)
                (projectName:string<projectName>)
                (cfgPlexName:string<cfgPlexName>) 
           =
        result {
            let cfgPlexFullPath =
                this.getCfgPlexPath
                    wsRootDir
                    projectName
                    cfgPlexName
            let! cereal = TextIO.readAllText cfgPlexFullPath
            return! cereal |> CfgPlexDto.fromJson
        }

    member this.saveCfgPlex 
                (wsRootDir:string<folderPath>)
                (cfgPlex:cfgPlex) 
            =
        result {
            let cfgPlexFullPath =
                this.getCfgPlexPath wsRootDir
                    (cfgPlex |> CfgPlex.getProjectName)
                    (cfgPlex |> CfgPlex.getCfgPlexName)
                                
            let cereal = cfgPlex |> CfgPlexDto.toJson
            TextIO.writeToFileOverwrite cfgPlexFullPath cereal |> ignore
            return ()
        }

    member this.SaveScript
              (wsRootDir:string<folderPath>)
              (quirkScript:quirkScript) 
           =
        result {
            let projectName = (quirkScript |> QuirkScript.getProjectName)
            let scriptName = (quirkScript |> this.getScriptFileName)
            let toDoFolderPath = this.getScriptToDoPathToFolder wsRootDir projectName
            let toDoPath = Path.Combine(toDoFolderPath |> UMX.untag, scriptName |> UMX.untag)
                            |> UMX.tag<fullFilePath>
            let cereal = quirkScript |> QuirkScriptDto.toJson
            let! res = TextIO.writeToFileOverwrite toDoPath cereal
            return ()
        }

    // returns an empty project if none is found
    member this.getProject
              (wsRootDir:string<folderPath>)
              (projectName:string<projectName>) 
           =
        result {
            let projPath = this.getProjectPath wsRootDir projectName
            if (projPath |> UMX.untag |> File.Exists |> not) then
                return QuirkProject.createEmpty projectName
            else
            let! cereal = TextIO.readAllText projPath
            return! cereal |> QuirkProjectDto.fromJson
        }

    member this.saveProject
                (wsRootDir:string<folderPath>)
                (quirkProject:quirkProject) 
            =
        let projName = quirkProject |> QuirkProject.getProjectName
        let projPath = this.getProjectPath wsRootDir projName
        let cereal = quirkProject |> QuirkProjectDto.toJson
        TextIO.writeToFileOverwrite projPath cereal


    member this.finishScript
                (wsRootDir:string<folderPath>)
                (projectName:string<projectName>) 
                (scriptName: string<scriptName>) 
            =
        let scriptFileName = scriptName |> this.getScriptFileName
        let scriptRunningFolder = this.getScriptRunningPathToFolder wsRootDir projectName |> UMX.untag
        let scriptRunningPath = Path.Combine(scriptRunningFolder, scriptFileName |> UMX.untag)
        let scriptCompletedFolder = this.getScriptCompletedPathToFolder wsRootDir projectName |> UMX.untag
        let scriptCompletedPath = Path.Combine(scriptCompletedFolder, scriptFileName |> UMX.untag)
        try
            Directory.CreateDirectory(scriptCompletedFolder) |> ignore
            File.Move(scriptRunningPath, scriptCompletedPath)
            () |> Ok
        with ex ->
            $"error in finishScript: { ex.Message}" |> Result.Error
            

    member this.getAllProjects 
                (wsRootDir:string<folderPath>)
            =
            result {
                let! projects = 
                    IO.Directory.EnumerateDirectories(wsRootDir |> UMX.untag)
                    |> Seq.map(fun p -> p.Split(Path.DirectorySeparatorChar) |> Array.last |> UMX.tag<projectName>)
                    |> Seq.map(this.getProject wsRootDir)
                    |> Seq.toList
                    |> Result.sequence
                return projects |> List.toArray
            }


    interface IProjectDataStore with
        member this.GetProject (wsRootDir:string<folderPath>) (projectName:string<projectName>) 
                    = this.getProject wsRootDir projectName
        member this.GetAllProjects (wsRootDir:string<folderPath>)
                    = this.getAllProjects wsRootDir
        member this.SaveProject (wsRootDir:string<folderPath>) (quirkProject:quirkProject) 
                    = this.saveProject wsRootDir quirkProject
        member this.GetNextScript (wsRootDir:string<folderPath>) (projectName:string<projectName>) 
                    = this.getNextScript wsRootDir projectName
        member this.FinishScript (wsRootDir:string<folderPath>) (projectName:string<projectName>) (scriptName: string<scriptName>) 
                    = this.finishScript wsRootDir projectName scriptName
        member this.SaveScript (wsRootDir:string<folderPath>) quirkScript 
                    = this.SaveScript wsRootDir quirkScript
        member this.GetCfgPlex (wsRootDir:string<folderPath>) (projectName:string<projectName>) (cfgPlexName:string<cfgPlexName>) 
                    = this.getCfgPlex wsRootDir projectName cfgPlexName
        member this.SaveCfgPlex (wsRootDir:string<folderPath>) cfgPlex 
                    = this.saveCfgPlex wsRootDir cfgPlex
        member this.GetProjectAsync (wsRootDir:string<folderPath>) (projectName:string<projectName>) 
                    = Task.Run(fun () -> this.getProject wsRootDir projectName)
        member this.GetAllProjectsAsync (wsRootDir:string<folderPath>)
                    = Task.Run(fun () -> this.getAllProjects wsRootDir)
        member this.SaveProjectAsync (wsRootDir:string<folderPath>) (quirkProject:quirkProject) 
                    = Task.Run(fun () -> this.saveProject wsRootDir quirkProject)
        member this.GetNextScriptAsync (wsRootDir:string<folderPath>) (projectName:string<projectName>) 
                    = Task.Run(fun () -> this.getNextScript wsRootDir projectName)
        member this.FinishScriptAsync (wsRootDir:string<folderPath>) (projectName:string<projectName>) (scriptName: string<scriptName>) 
                    = Task.Run(fun () -> this.finishScript wsRootDir projectName scriptName)
        member this.SaveScriptAsync (wsRootDir:string<folderPath>) quirkScript 
                    = Task.Run(fun () -> this.SaveScript wsRootDir quirkScript)
        member this.GetCfgPlexAsync (wsRootDir:string<folderPath>) (projectName:string<projectName>) (cfgPlexName:string<cfgPlexName>) 
                    = Task.Run(fun () -> this.getCfgPlex wsRootDir projectName cfgPlexName)
        member this.SaveCfgPlexAsync (wsRootDir:string<folderPath>) cfgPlex 
                    = Task.Run(fun () -> this.saveCfgPlex wsRootDir cfgPlex)