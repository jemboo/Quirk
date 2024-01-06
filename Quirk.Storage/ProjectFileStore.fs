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


type projectFileStore (wsRootDir:string, fileUtils:IFileUtils) =
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

    member this.getProjectPathToFolder (projectName:string<projectName>) = 
        Path.Combine(this.wsRootDir, projectName |> UMX.untag)
        |> UMX.tag<folderPath>

    member this.getProjectFileName (projectName:string<projectName>) =
           $"{projectName |> UMX.untag}.{this.fileExt}"
           |> UMX.tag<fnWExt>

    member this.getProjectPath (projectName:string<projectName>) =
           Path.Combine(
                projectName |> this.getProjectPathToFolder |> UMX.untag,
                projectName |> this.getProjectFileName |> UMX.untag
                )
           |> UMX.tag<fullFilePath>

    member this.getCfgPlexPath (projectName:string<projectName>) =
           Path.Combine(
                projectName |> this.getProjectPathToFolder |> UMX.untag,
                projectName |> this.getProjectFileName |> UMX.untag
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
            return (fileName, quirkScript)
        }

    member this.getCfgPlex (projectName:string<projectName>) =
        result {
            let projPath = this.getProjectPath projectName
            let! cereal = TextIO.readAllText projPath
            return! cereal |> QuirkProjectDto.fromJson
        }

    member this.saveCfgPlex (cfgPlex:cfgPlex) =
        result {
            let projectName = (cfgPlex |> CfgPlex.getProjectName)
            let projectPath = projectName |> this.getProjectPathToFolder |> UMX.untag
          
            //if Directory.Exists(projectPath) then
            //    return! $"A project already exists at {projectPath}" |> Error
            let dto = cfgPlex |> CfgPlexDto.toDto
            fileUtils.Save projectPath (projectName |> this.getProjectFileName |> UMX.untag) dto
            return ()
        }

    member this.SaveScript (quirkScript:quirkScript) =
        result {
            let projectName = (quirkScript |> QuirkScript.getProjectName)
            let toDoPath = this.getScriptToDoPathToFolder projectName
          
            let dto = quirkScript |> QuirkScriptDto.toDto
            let scriptName = quirkScript  |> QuirkScript.getScriptName
            fileUtils.Save 
                (toDoPath |> UMX.untag)
                (scriptName |> UMX.untag)
                dto
            return ()
        }

    member this.getProject (projectName:string<projectName>) =
        result {
            let projPath = this.getProjectPath projectName
            let! cereal = TextIO.readAllText projPath
            return! cereal |> QuirkProjectDto.fromJson
        }

    member this.saveProject (quirkProject:quirkProject) =
        let projName = quirkProject |> QuirkProject.getProjectName
        let projPath = this.getProjectPath projName
        let cereal = quirkProject |> QuirkProjectDto.toJson
        TextIO.writeToFileOverwrite projPath cereal


    member this.finishScript (projectName:string<projectName>) (scriptName: string<scriptName>)  =
        let scriptRunningFolder = this.getScriptRunningPathToFolder projectName |> UMX.untag
        let scriptRunningPath = Path.Combine(scriptRunningFolder, scriptName |> UMX.untag)
        let scriptCompletedFolder = this.getScriptCompletedPathToFolder projectName |> UMX.untag
        let scriptCompletedPath = Path.Combine(scriptCompletedFolder, scriptName |> UMX.untag)
        try
            Directory.CreateDirectory(scriptCompletedFolder) |> ignore
            File.Move(scriptRunningPath, scriptCompletedPath)
            () |> Ok
        with ex ->
            $"error in finishScript: { ex.Message}" |> Result.Error
            

    interface IProjectDataStore with
        member this.GetProject (projectName:string<projectName>) 
                    = this.getProject projectName
        member this.SaveProject (quirkProject:quirkProject) = this.saveProject quirkProject
        member this.GetNextScript (projectName:string<projectName>) = this.getNextScript projectName
        member this.FinishScript (projectName:string<projectName>) (scriptName: string<scriptName>) 
                    = this.finishScript projectName scriptName
        member this.SaveScript quirkScript 
                    = this.SaveScript quirkScript
        member this.GetCfgPlex (projectName:string<projectName>) = () |> Ok
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

