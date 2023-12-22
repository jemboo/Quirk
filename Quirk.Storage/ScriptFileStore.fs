namespace Quirk.Storage
open System
open System.IO
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Script
open Quirk.Cfg.Core
open Quirk.Cfg.Serialization


type scriptFileStore (wsRootDir:string, fileUtils:IFileUtils) =
    member this.scriptFolder = "scripts"
    member this.reportFolder = "reports"
    member this.scriptToDoFolder = "toDo"
    member this.scriptRunningFolder = "running"
    member this.scriptCompletedFolder = "completed"
    member this.fileExt = "txt"
    member this.wsRootDir = wsRootDir
    member this.fileUtils = fileUtils;

    member this.getComponentFolderName (wsCompType:workspaceComponentType) =
        wsCompType |> string

    member this.getProjectPath (projectName:string) = 
        Path.Combine(this.wsRootDir, projectName)

    member this.getProjectFileName (projectName:string) =
           $"{projectName}.{this.fileExt}"

    member this.getScriptPath (projectName:string) = 
        Path.Combine(this.getProjectPath projectName, this.scriptFolder)

    member this.getReportPath (projectName:string) = 
        Path.Combine(this.getProjectPath projectName, this.reportFolder)

    member this.getScriptToDoPath (projectName:string) = 
        Path.Combine(this.getScriptPath projectName, this.scriptToDoFolder)

    member this.getScriptRunningPath (projectName:string) = 
        Path.Combine(this.getScriptPath projectName, this.scriptRunningFolder)

    member this.getScriptCompletedPath (projectName:string) = 
        Path.Combine(this.getScriptPath projectName, this.scriptCompletedFolder)

    member this.getComponentFolderPath 
                    (projectName:string)
                    (wsCompType:workspaceComponentType) = 
        Path.Combine(this.getProjectPath projectName, this.scriptCompletedFolder)


    member this.getProjectFolders (projectName:string) = 
        this.fileUtils.GetFolders2 
                this.wsRootDir 
                projectName

    member this.getComponentFiles       
                (projectName:string) 
                (wsCompType:workspaceComponentType) = 
            this.fileUtils.GetFiles3
                    this.wsRootDir
                    projectName
                    (this.getComponentFolderName wsCompType)

    member this.scriptsToDo (projectName:string) =
        this.fileUtils.GetFiles 
                (this.getScriptToDoPath projectName)


    member this.SaveScript (quirkScript:quirkScript) =
        result {

            return ()
        }


    interface IScriptDataStore with
        member this.SaveScript quirkScript = this.SaveScript quirkScript



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

