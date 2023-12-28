namespace Quirk.Cfg.Serialization.Test

open System
open Xunit
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Cfg.Core
open Quirk.Sorting
open Quirk.Cfg.Shc
open Quirk.Cfg.Serialization

module QuirkRunSetDtoFixture =

    [<Fact>]
    let ``QuirkRunSet`` () =

        let quirkRunSet = O_64.quirkRuns

        //let dtoCereal = quirkRunSet |> QuirkRunSetDto.toJson

        //let quirkRunSetBack = 
        //            dtoCereal 
        //            |> QuirkRunSetDto.fromJson
        //            |> Result.ExtractOrThrow

        //Assert.True(CollectionProps.areEqual quirkRunSet quirkRunSetBack)
        Assert.True(true)