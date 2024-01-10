module Tests

open FSharp.UMX
open System
open Xunit
open Quirk.Storage
open Quirk.Core

[<Fact>]
let ``getAllProjects`` () =
    let wsRoot = @"C:\Quirk" |> UMX.tag<folderPath>
    let projectFileStore = new projectFileStore()
    let yab = wsRoot |> projectFileStore.getAllProjects |> Result.ExtractOrThrow
    Assert.True(true)
