module Tests

open System
open Xunit
open Quirk.Storage

[<Fact>]
let ``getAllProjects`` () =
    let wsRoot = @"C:\Quirk"
    let projectFileStore = new projectFileStore()
    let yab = wsRoot |> projectFileStore.getAllProjects |> Result.ExtractOrThrow
    Assert.True(true)
