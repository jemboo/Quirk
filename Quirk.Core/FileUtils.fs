namespace Quirk.Core

open System.IO
open System

module TextIO =

    let readAllLines 
            (ext:string)
            (folder:string) 
            (fileName:string)
        =
        try
            let fne = sprintf "%s.%s" fileName ext
            let fp = Path.Combine(folder, fne)
            if File.Exists(fp) then
                File.ReadAllLines fp |> Ok
            else
                sprintf "not found (401): %s" fp |> Error
        with ex ->
            ("error in TextIO.readAllLines: " + ex.Message) |> Result.Error


    let fileExists
            (ext:string) 
            (folder:string) 
            (fileName:string)
        =
        try
            let fne = sprintf "%s.%s" fileName ext
            let fp = Path.Combine(folder, fne)
            File.Exists(fp) |> Ok
        with ex ->
            ("error in TextIO.fileExists: " + ex.Message) |> Result.Error


    let readAllText
            (ext:string)
            (folder:string) 
            (fileName:string)
        =
        try
            let fne = sprintf "%s.%s" fileName ext
            let fp = Path.Combine(folder, fne)
            if File.Exists(fp) then
                File.ReadAllText fp |> Ok
            else
                sprintf "not found (402): %s" fp |> Error
        with ex ->
            ("error in TextIO.readAllText: " + ex.Message) |> Result.Error


    let appendLines
            (ext:string)
            (folder:string) 
            (file:string)
            (data: seq<string>)
        =
        try
            let fne = sprintf "%s.%s" file ext
            let fp = Path.Combine(folder, fne)
            Directory.CreateDirectory(folder) |> ignore
            File.AppendAllLines(fp, data)
            true |> Ok
        with ex ->
            ("error in TextIO.appendLines: " + ex.Message) |> Result.Error


    let writeLinesEnsureHeader
            (ext:string)
            (folder:string) 
            (file:string)
            (hdr: seq<string>)
            (data: seq<string>)
        =
        try
            let fne = sprintf "%s.%s" file ext
            let fp = Path.Combine(folder, fne)
            Directory.CreateDirectory(folder) |> ignore
            if File.Exists(fp) then
                File.AppendAllLines(fp, data)
                true |> Ok
            else
                File.AppendAllLines(fp, hdr)
                File.AppendAllLines(fp, data)
                true |> Ok
        with ex ->
            ("error in TextIO.writeLinesIfNew: " + ex.Message) |> Result.Error


    let writeToFileIfMissing
            (ext:string)
            (folder:string) 
            (file:string)
            (data:string)
        =
        try
            let fne = sprintf "%s.%s" file ext
            let fp = Path.Combine(folder, fne)
            Directory.CreateDirectory(folder) |> ignore
            if File.Exists(fp) then
                true |> Ok
            else
                File.WriteAllText(fp, data)
                true |> Ok
        with ex ->
            ("error in TextIO.writeToFile: " + ex.Message) |> Result.Error


    let writeToFileOverwrite
            (ext:string)
            (folder:string) 
            (file:string)
            (data:string)
        =
        try
            let fne = sprintf "%s.%s" file ext
            let fp = Path.Combine(folder, fne)
            Directory.CreateDirectory(folder) |> ignore
            File.WriteAllText(fp, data)
            () |> Ok
        with ex ->
            ("error in TextIO.writeToFile: " + ex.Message) |> Result.Error




type IFileUtils =
    abstract member Read<'a> : string -> string -> Result<'a, string>
    abstract member Read2<'a> : string -> string -> string -> Result<'a, string>
    abstract member Read3<'a> : string -> string -> string -> string -> Result<'a, string>
    abstract member Save<'a> : string -> string -> 'a -> unit
    abstract member Save2<'a> : string -> string -> string -> 'a -> unit
    abstract member Save3<'a> : string -> string -> string -> string -> 'a -> unit
    abstract member GetFolders: string -> Result<seq<string>, string>
    abstract member GetFolders2: string -> string -> Result<seq<string>, string>
    abstract member GetFolders3: string -> string ->  string -> Result<seq<string>, string>
    abstract member GetFiles:  string -> Result<seq<string>, string>
    abstract member GetFiles2:  string -> string -> Result<seq<string>, string>
    abstract member GetFiles3:  string -> string -> string -> Result<seq<string>, string>


type fileUtils = 

    new () = {}

    member this.read<'a> (folderPath:string) (fileName:string) =
        result {
            let fp = Path.Combine(folderPath, fileName)
            if File.Exists(fp) then
                return! Json.deserialize<'a> fp
            else
                return! $"{fp} not found" |> Error
        }

    member this.save<'a> (folderPath:string) (fileName:string) (content:'a) =
        if (Directory.Exists folderPath |> not) then
            Directory.CreateDirectory(folderPath) |> ignore

        let fileContent = Json.serialize content
        let fp = Path.Combine(folderPath, fileName)
        File.WriteAllText(fp, fileContent)


    member this.getFolders (folderPath:string) =
        result {
            if (Directory.Exists folderPath |> not) then
               return! $"directory: {folderPath} does not exist" |> Error
            else
               return Directory.EnumerateDirectories(folderPath)
        }


    member this.getFiles (folderPath:string) =
        result {
            if (Directory.Exists folderPath |> not) then
               return! $"directory: {folderPath} does not exist" |> Error
            else
               return Directory.EnumerateFiles (folderPath)
        }


    member this.read2<'a> (folderPath:string) (folder:string) (fileName:string) : Result<'a, string> =
        let comby = Path.Combine(folderPath, folder)
        this.read comby fileName

    member this.read3<'a> (folderPath:string) (folder1:string) (folder2:string) (fileName:string) : Result<'a, string> =
        this.read (Path.Combine(folderPath, folder1, folder2)) fileName

    member this.save2<'a> (folderPath:string) (folder:string) (fileName:string) (content:'a) =
        this.save (Path.Combine(folderPath, folder)) fileName content

    member this.save3<'a> (folderPath:string) (folder1:string) (folder2:string) (fileName:string) (content:'a) =
        this.save (Path.Combine(folderPath, folder1, folder2)) fileName content

    member this.getFolders2<'a> (folderPath:string) (folder:string) =
        this.getFolders (Path.Combine(folderPath, folder))

    member this.getFolders3<'a> (folderPath:string) (folder1:string) (folder2:string) =
        this.getFolders (Path.Combine(folderPath, folder1, folder2))

    member this.getFiles2<'a> (folderPath:string) (folder:string) =
        this.getFiles (Path.Combine(folderPath, folder))

    member this.getFiles3<'a> (folderPath:string) (folder1:string) (folder2:string) =
        this.getFiles (Path.Combine(folderPath, folder1, folder2))


    interface IFileUtils with
        member this.Read<'a> (folderPath:string) (fileName:string) = 
            this.read<'a> folderPath fileName

        member this.Read2<'a> (folderPath:string) (folder:string) (fileName:string) = 
            this.read2<'a> folderPath folder fileName

        member this.Read3<'a> (folderPath:string) (folder1:string) (folder2:string) (fileName:string) = 
            this.read3<'a> folderPath folder1 folder2 fileName

        member this.Save<'a> (folderPath:string) (fileName:string) (content:'a)  = 
            this.save<'a> folderPath fileName content

        member this.Save2<'a> (folderPath:string) (folder:string) (fileName:string) (content:'a)  = 
            this.save2<'a> folderPath folder fileName content

        member this.Save3<'a> (folderPath:string) (folder1:string) (folder2:string) (fileName:string) (content:'a)  = 
            this.save3<'a> folderPath folder1 folder2 fileName content

        member this.GetFolders (folderPath:string) = 
            this.getFolders folderPath

        member this.GetFolders2 (folderPath:string) (folder:string) = 
            this.getFolders2 folderPath folder

        member this.GetFolders3 (folderPath:string) (folder1:string) (folder2:string) = 
            this.getFolders3 folderPath folder1 folder2

        member this.GetFiles (folderPath:string) = 
            this.getFiles folderPath

        member this.GetFiles2 (folderPath:string) (folder:string) = 
            this.getFiles2 folderPath folder

        member this.GetFiles3 (folderPath:string) (folder1:string) (folder2:string) = 
            this.getFiles3 folderPath folder1 folder2