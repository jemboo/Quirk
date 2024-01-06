namespace Quirk.Core.Test

open System
open Xunit
open FSharp.UMX
open Quirk.Core



module FileUtilsFixture =



    [<Fact>]
    let ``testFileEnumer``() =
        
        let fp = @"C:\Quirk\Shc_064\scripts\toDo"
        let fp2 = @"C:\Quirk\Shc_064\scripts\running"

        let yab = TextIO.getFileTextFromFolderAndMove
                    (fp |> UMX.tag<folderPath> )
                    (fp2 |> UMX.tag<folderPath> )


        
        let fp = @"C:\Quirk\Shc_064\scripts\toDo\yab.txt" |> UMX.tag<fullFilePath>
        let flder = fp |> TextIO.getFolder
        //let fileList = IO.Directory.EnumerateFiles fp
        //let found = fileList |> Seq.toArray

        //let ex = IO.Directory.Exists fp2

        //let fileList2 = IO.Directory.EnumerateFiles fp2
        //let found2 = fileList2 |> Seq.isEmpty


        Assert.Equal(84, 84)
