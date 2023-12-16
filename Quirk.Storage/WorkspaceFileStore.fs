namespace Quirk.Storage
open System
open System.IO
open FSharp.UMX
open Quirk.Core
open Quirk.Project


type WorkspaceFileStore (wsRootDir:string) =

    member this.wsRootDir = wsRootDir
    member this.fileExt = "txt"


    member this.getFolderName (wsCompType:workspaceComponentType option) =
        match wsCompType with
        | Some v -> v |> string
        | None -> ""

    member this.writeToFileIfMissing (wsCompType:workspaceComponentType option) (fileName:string) (data: string) =
        TextIO.writeToFileIfMissing this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName data

    member this.writeToFileOverwrite (wsCompType:workspaceComponentType option) (fileName:string) (data: string) =
        TextIO.writeToFileOverwrite this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName data

    member this.writeLinesEnsureHeader (wsCompType:workspaceComponentType option) (fileName:string) (hdr: seq<string>) (data: string seq) =
        TextIO.writeLinesEnsureHeader this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName hdr data

    member this.appendLines (wsCompType:workspaceComponentType option) (fileName:string) (data: string seq) =
        TextIO.appendLines this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName data

    member this.fileExists (wsCompType:workspaceComponentType option) (fileName:string) =
        TextIO.fileExists this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName

    member this.readAllText (wsCompType:workspaceComponentType option) (fileName:string) =
        TextIO.readAllText this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName

    member this.readAllLines (wsCompType:workspaceComponentType option) (fileName:string) =
        TextIO.readAllLines this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName

    member this.getAllFiles (wsCompType:workspaceComponentType option) =
           let filePath = Path.Combine(this.wsRootDir, this.getFolderName wsCompType)
           Directory.GetFiles(filePath)
