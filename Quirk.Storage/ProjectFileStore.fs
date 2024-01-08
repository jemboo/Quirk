namespace Quirk.Storage
open System
open System.IO
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Cfg.Core
open Quirk.Cfg.Serialization
open Quirk.Script
open Quirk.Serialization
open System.Threading


type projectFileStore (wsRootDir:string) =
    member this.scriptFolder = "scripts"
    member this.reportFolder = "reports"
    member this.scriptToDoFolder = "toDo"
    member this.scriptRunningFolder = "running"
    member this.scriptCompletedFolder = "completed"
    member this.fileExt = "txt"
    member this.wsRootDir = wsRootDir

    member this.getComponentFolderName (wsCompType:workspaceComponentType) =
        wsCompType |> string

    member this.getProjectPathToFolder (projectName:string<projectName>) = 
        Path.Combine(this.wsRootDir, projectName |> UMX.untag)
        |> UMX.tag<folderPath>

    member this.getProjectFileName (projectName:string<projectName>) =
           $"{projectName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>

    member this.getCfgPlexFileName (cfgPlexName:string<cfgPlexName>) =
           $"{cfgPlexName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>

    member this.getScriptFileName (scriptName:string<scriptName>) =
           $"{scriptName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>


    member this.getProjectPath (projectName:string<projectName>) =
           Path.Combine(
                projectName |> this.getProjectPathToFolder |> UMX.untag,
                projectName |> this.getProjectFileName |> UMX.untag
                )
           |> UMX.tag<fullFilePath>

    member this.getCfgPlexPath 
                (projectName:string<projectName>) 
                (cfgPlexName:string<cfgPlexName>)
           =
           Path.Combine(
                projectName |> this.getProjectPathToFolder |> UMX.untag,
                cfgPlexName |> this.getCfgPlexFileName  |> UMX.untag
                )
           |> UMX.tag<fullFilePath>

    member this.getScriptPathToFolder (projectName:string<projectName>) = 
        Path.Combine(projectName |> this.getProjectPathToFolder |> UMX.untag, this.scriptFolder)
        |> UMX.tag<folderPath>

    member this.getReportPathToFolder (projectName:string<projectName>) = 
        Path.Combine(projectName |> this.getProjectPathToFolder |> UMX.untag, this.reportFolder)
        |> UMX.tag<folderPath>

    member this.getScriptToDoPathToFolder (projectName:string<projectName>) = 
        Path.Combine(projectName |> this.getScriptPathToFolder |> UMX.untag, this.scriptToDoFolder)
        |> UMX.tag<folderPath>

    member this.getScriptRunningPathToFolder (projectName:string<projectName>) = 
        Path.Combine(projectName |> this.getScriptPathToFolder |> UMX.untag, this.scriptRunningFolder)
        |> UMX.tag<folderPath>

    member this.getScriptCompletedPathToFolder (projectName:string<projectName>) = 
        Path.Combine(projectName |> this.getScriptPathToFolder |> UMX.untag, this.scriptCompletedFolder)
        |> UMX.tag<folderPath>

    member this.getScriptFileName (quirkScript:quirkScript) =
           $"{quirkScript |> QuirkScript.getScriptName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>

    member this.getComponentPathToFolder 
                    (projectName:string<projectName>)
                    (wsCompType:workspaceComponentType) = 
        Path.Combine(projectName |> this.getProjectPathToFolder |> UMX.untag, this.scriptCompletedFolder)


    member this.getNextScript (projectName:string<projectName>) = 
        let sourceFolder = this.getScriptToDoPathToFolder projectName
        let moveToFolder = this.getScriptRunningPathToFolder projectName
        result {
            let! (fileName, fileContents) = 
                    TextIO.getFileTextFromFolderAndMove sourceFolder moveToFolder
            let! quirkScript = fileContents |> QuirkScriptDto.fromJson
            let scriptName = fileName |> UMX.untag |> Path.GetFileNameWithoutExtension |> UMX.tag<scriptName>
            return (scriptName, quirkScript)
        }

    member this.getCfgPlex 
                (projectName:string<projectName>)
                (cfgPlexName:string<cfgPlexName>) 
           =
        result {
            let cfgPlexFullPath =
                this.getCfgPlexPath
                    projectName
                    cfgPlexName
            let! cereal = TextIO.readAllText cfgPlexFullPath
            return! cereal |> CfgPlexDto.fromJson
        }

    member this.saveCfgPlex (cfgPlex:cfgPlex) =
        result {
            let cfgPlexFullPath =
                this.getCfgPlexPath
                    (cfgPlex |> CfgPlex.getProjectName)
                    (cfgPlex |> CfgPlex.getCfgPlexName)
                                
            let cereal = cfgPlex |> CfgPlexDto.toJson
            TextIO.writeToFileOverwrite cfgPlexFullPath cereal |> ignore
            return ()
        }

    member this.SaveScript (quirkScript:quirkScript) =
        result {
            let projectName = (quirkScript |> QuirkScript.getProjectName)
            let scriptName = (quirkScript |> this.getScriptFileName)
            let toDoFolderPath = this.getScriptToDoPathToFolder projectName
            let toDoPath = Path.Combine(toDoFolderPath |> UMX.untag, scriptName |> UMX.untag)
                            |> UMX.tag<fullFilePath>
            let cereal = quirkScript |> QuirkScriptDto.toJson
            let! res = TextIO.writeToFileOverwrite toDoPath cereal
            return ()
        }

    // returns an empty project if none is found
    member this.getProject (projectName:string<projectName>) =
        result {
            let projPath = this.getProjectPath projectName
            if (projPath |> UMX.untag |> File.Exists |> not) then
                return QuirkProject.createEmpty projectName
            else
            let! cereal = TextIO.readAllText projPath
            return! cereal |> QuirkProjectDto.fromJson
        }

    member this.saveProject (quirkProject:quirkProject) =
        let projName = quirkProject |> QuirkProject.getProjectName
        let projPath = this.getProjectPath projName
        let cereal = quirkProject |> QuirkProjectDto.toJson
        TextIO.writeToFileOverwrite projPath cereal


    member this.finishScript 
                (projectName:string<projectName>) 
                (scriptName: string<scriptName>) 
            =
        let scriptFileName = scriptName |> this.getScriptFileName
        let scriptRunningFolder = this.getScriptRunningPathToFolder projectName |> UMX.untag
        let scriptRunningPath = Path.Combine(scriptRunningFolder, scriptFileName |> UMX.untag)
        let scriptCompletedFolder = this.getScriptCompletedPathToFolder projectName |> UMX.untag
        let scriptCompletedPath = Path.Combine(scriptCompletedFolder, scriptFileName |> UMX.untag)
        try
            Directory.CreateDirectory(scriptCompletedFolder) |> ignore
            File.Move(scriptRunningPath, scriptCompletedPath)
            () |> Ok
        with ex ->
            $"error in finishScript: { ex.Message}" |> Result.Error
            

    interface IProjectDataStore with
        member this.GetProject (projectName:string<projectName>) 
                    = this.getProject projectName
        member this.SaveProject (quirkProject:quirkProject) 
                    = this.saveProject quirkProject
        member this.GetNextScript (projectName:string<projectName>) 
                    = this.getNextScript projectName
        member this.FinishScript (projectName:string<projectName>) (scriptName: string<scriptName>) 
                    = this.finishScript projectName scriptName
        member this.SaveScript quirkScript 
                    = this.SaveScript quirkScript
        member this.GetCfgPlex (projectName:string<projectName>) (cfgPlexName:string<cfgPlexName>) 
                    = this.getCfgPlex projectName cfgPlexName
        member this.SaveCfgPlex cfgPlex = this.saveCfgPlex cfgPlex



    //member this.writeToFileIfMissing (wsCompType:workspaceComponentType option) (fileName:string) (data: string) =
    //    let folderPath = Path.Combine(this.wsRootDir, this.getComponentFolderName wsCompType)
    //    TextIO.writeToFileIfMissing this.fileExt folderPath fileName data

    //member this.writeToFileOverwrite (wsCompType:workspaceComponentType option) (fileName:string) (data: string) =
    //    let folderPath = Path.Combine(this.wsRootDir, this.getComponentFolderName wsCompType)
    //    TextIO.writeToFileOverwrite this.fileExt folderPath fileName data

    //member this.writeLinesEnsureHeader (wsCompType:workspaceComponentType option) (fileName:string) (hdr: seq<string>) (data: string seq) =
    //    let folderPath = Path.Combine(this.wsRootDir, this.getComponentFolderName wsCompType)
    //    TextIO.writeLinesEnsureHeader this.fileExt folderPath fileName hdr data

    //member this.appendLines (wsCompType:workspaceComponentType option) (fileName:string) (data: string seq) =
    //    let folderPath = Path.Combine(this.wsRootDir, this.getComponentFolderName wsCompType)
    //    TextIO.appendLines this.fileExt folderPath fileName data

    //member this.fileExists (wsCompType:workspaceComponentType option) (fileName:string) =
    //    let folderPath = Path.Combine(this.wsRootDir, this.getComponentFolderName wsCompType)
    //    TextIO.fileExists this.fileExt folderPath fileName

    //member this.readAllText (wsCompType:workspaceComponentType option) (fileName:string) =
    //    let folderPath = Path.Combine(this.wsRootDir, this.getComponentFolderName wsCompType)
    //    TextIO.readAllText this.fileExt folderPath fileName

    //member this.readAllLines (wsCompType:workspaceComponentType option) (fileName:string) =
    //    let folderPath = Path.Combine(this.wsRootDir, this.getComponentFolderName wsCompType)
    //    TextIO.readAllLines this.fileExt folderPath fileName

    //member this.getAllFiles (wsCompType:workspaceComponentType option) =
    //       let filePath = Path.Combine(this.wsRootDir, this.getComponentFolderName wsCompType)
    //       Directory.GetFiles(filePath)

