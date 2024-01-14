namespace Quirk.Storage.Test

open FSharp.UMX
open System
open Xunit
open Quirk.Storage
open Quirk.Core
open Quirk.Project


module RolloutDtoFixture =

    [<Fact>]
    let ``getAllProjects`` () =
        let wsRoot = @"C:\Quirk" |> UMX.tag<folderPath>
        let projectFileStore = new projectFileStore()
        let yab = wsRoot |> projectFileStore.getAllProjects |> Result.ExtractOrThrow
        Assert.True(true)


    [<Fact>]
    let ``getFixedParameters`` () =
        let wsRoot = @"C:\Quirk" |> UMX.tag<folderPath>
        let projectName = @"Shc_064"  |> UMX.tag<projectName>
        let projectFileStore = new projectFileStore()
        let project = projectName |> projectFileStore.getProject wsRoot |> Result.ExtractOrThrow
        let constantParams = project |> QuirkProject.getSingletonParams
        Assert.True(true)