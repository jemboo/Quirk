namespace Quirk.Cfg.Serialization.Test

open System
open Xunit
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Sorting
open Quirk.Cfg.Shc
open Quirk.Cfg.Serialization

module CfgPlexDtoFixture =

    [<Fact>]
    let ``CfgPlexDto`` () =
        let projectName = "projectName" |> UMX.tag<projectName>
        let cfgPlexName = "cfgPlexName" |> UMX.tag<cfgPlexName>
        let cfgPlex = O_64.plex64 projectName cfgPlexName
        let dtoCereal = cfgPlex |> CfgPlexDto.toJson

        let cfgPlexBack = dtoCereal 
                            |> CfgPlexDto.fromJson
                            |> Result.ExtractOrThrow

        Assert.True(CollectionProps.areEqual cfgPlex cfgPlexBack)
