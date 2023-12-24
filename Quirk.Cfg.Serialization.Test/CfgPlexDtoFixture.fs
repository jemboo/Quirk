namespace Quirk.Cfg.Serialization.Test

open System
open Xunit
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Sorting
open Quirk.Cfg.Shc
open Quirk.Cfg.Serialization

module CfgPlexDtoFixture =

    [<Fact>]
    let ``CfgPlexDto`` () =

        let cfgPlex = O_64.plex64
        let dtoCereal = cfgPlex |> CfgPlexDto.toJson

        let cfgPlexBack = dtoCereal 
                            |> CfgPlexDto.fromJson
                            |> Result.ExtractOrThrow

        Assert.True(CollectionProps.areEqual cfgPlex cfgPlexBack)
