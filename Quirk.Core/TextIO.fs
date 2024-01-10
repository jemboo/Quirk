namespace Quirk.Core

open System.IO
open System
open System.Threading
open FSharp.UMX



[<Measure>] type fileExt
[<Measure>] type fnWoExt
[<Measure>] type fnWExt
[<Measure>] type folderPath
[<Measure>] type fullFilePath

module TextIO =

    let makeFileNameWExt
            (fnWoExt:string<fnWoExt>)
            (fext:string<fileExt>)
        : string<fnWExt>
        =
        $"{fnWoExt |> UMX.untag}.{fext |> UMX.untag}" |> UMX.tag<fnWExt>

        
    let makeFullFilePath
            (folderPath:string<folderPath>)
            (fnWExt:string<fnWExt>)
        : string<fullFilePath>
        =
        Path.Combine(folderPath |> UMX.untag, fnWExt |> UMX.untag)  |> UMX.tag<fullFilePath>


    let getFolder
            (fullFilePath:string<fullFilePath>)
        : string<folderPath>
        =
        fullFilePath |> UMX.untag |> Path.GetDirectoryName |> UMX.tag<folderPath>


    let getFolders (folderPath:string) =
        result {
            if (Directory.Exists folderPath |> not) then
               return! $"directory: {folderPath} does not exist" |> Error
            else
               return Directory.EnumerateDirectories(folderPath)
        }

    let getFileNameWithoutExt
            (fnWExt:string<fnWExt>)
        : string<fnWoExt>
        =
        Path.GetFileNameWithoutExtension(fnWExt |> UMX.untag) |> UMX.tag<fnWoExt>


    let getFileNameWithExt
            (fullFilePath:string<fullFilePath>)
        : string<fnWExt>
        =
        Path.GetFileName(fullFilePath |> UMX.untag) |> UMX.tag<fnWExt>


    let readAllLines 
            (fullFilePath:string<fullFilePath>)
        =
        try
            let ffp = fullFilePath |> UMX.untag
            if File.Exists(ffp) then
                File.ReadAllLines ffp |> Ok
            else
                sprintf "not found (401): %s" ffp |> Error
        with ex ->
            ("error in TextIO.readAllLines: " + ex.Message) |> Result.Error


    let readAllText
            (fullFilePath:string<fullFilePath>)
        =
        try
            let ffp = fullFilePath |> UMX.untag
            if File.Exists(ffp) then
                File.ReadAllText ffp |> Ok
            else
                sprintf "not found (402): %s" ffp |> Error
        with ex ->
            ("error in TextIO.readAllText: " + ex.Message) |> Result.Error


    let appendLines
            (fullFilePath:string<fullFilePath>)
            (data: seq<string>)
        =
        try
            let ffp = fullFilePath |> UMX.untag
            let folder = fullFilePath |> getFolder
            Directory.CreateDirectory(folder |> UMX.untag) |> ignore
            File.AppendAllLines(ffp, data)
            true |> Ok
        with ex ->
            ("error in TextIO.appendLines: " + ex.Message) |> Result.Error


    let writeLinesEnsureHeader
            (fullFilePath:string<fullFilePath>)
            (hdr: seq<string>)
            (data: seq<string>)
        =
        try
            let ffp = fullFilePath |> UMX.untag
            let folder = fullFilePath |> getFolder
            Directory.CreateDirectory(folder |> UMX.untag) |> ignore
            if File.Exists(ffp) then
                File.AppendAllLines(ffp, data)
                true |> Ok
            else
                File.AppendAllLines(ffp, hdr)
                File.AppendAllLines(ffp, data)
                true |> Ok
        with ex ->
            ("error in TextIO.writeLinesIfNew: " + ex.Message) |> Result.Error


    let writeToFileIfMissing
            (fullFilePath:string<fullFilePath>)
            (data:string)
        =
        try
            let ffp = fullFilePath |> UMX.untag
            let folder = fullFilePath |> getFolder
            Directory.CreateDirectory(folder |> UMX.untag) |> ignore
            if File.Exists(ffp) then
                true |> Ok
            else
                File.WriteAllText(ffp, data)
                true |> Ok
        with ex ->
            ("error in TextIO.writeToFile: " + ex.Message) |> Result.Error


    let writeToFileOverwrite
            (fullFilePath:string<fullFilePath>)
            (data:string)
        =
        try
            let ffp = fullFilePath |> UMX.untag
            let folder = fullFilePath |> getFolder
            Directory.CreateDirectory(folder |> UMX.untag) |> ignore
            File.WriteAllText(ffp, data)
            () |> Ok
        with ex ->
            ("error in TextIO.writeToFile: " + ex.Message) |> Result.Error


    // gets the next file from sourceFolderPath, and returns a tuple a*b
    // a is the file name + extension without the path.
    // b is the file contents as a string
    // moves the found file to the moveToFolderPath
    // returns error message if sourceFolderPath does not exist
    // returns error message if there is no file in sourceFolderPath
    let getFileTextFromFolderAndMove 
                (sourceFolder:string<folderPath>) 
                (moveToFolder:string<folderPath>)
            : Result< string<fnWExt>*string, string>
            =
            use mutex = new Mutex(false, "FileMoveMutex")
            let mutable fileNameAndContents = ("" |> UMX.tag<fnWExt>, "") |> Ok
            if (sourceFolder |> UMX.untag |> Directory.Exists |> not) then
                $"{sourceFolder |> UMX.untag } not found" |> Error
            elif (sourceFolder |> UMX.untag |> Directory.EnumerateFiles |> Seq.isEmpty) then
                $" no files in {sourceFolder} " |> Error
            else
            if mutex.WaitOne() then
                    try
                        Directory.CreateDirectory(moveToFolder |> UMX.untag) |> ignore
                        let foundFileNameAndPath = 
                            IO.Directory.EnumerateFiles (sourceFolder |> UMX.untag)
                                |> Seq.head
                        let foundFileName = Path.GetFileName(foundFileNameAndPath)
                        let destinationFileNameAndPath = Path.Combine(moveToFolder |> UMX.untag, foundFileName)
                        File.Move(foundFileNameAndPath, destinationFileNameAndPath)
                        fileNameAndContents <- (foundFileName |> UMX.tag<fnWExt>, (File.ReadAllText destinationFileNameAndPath)) |> Ok
                        mutex.ReleaseMutex()
                    with ex ->
                            fileNameAndContents <- $"error in getNextScript: { ex.Message }" |> Result.Error
            fileNameAndContents
