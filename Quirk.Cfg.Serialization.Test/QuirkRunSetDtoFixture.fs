namespace Quirk.Cfg.Serialization.Test

open System
open Xunit
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Sorting
open Quirk.Cfg.Shc
open Quirk.Cfg.Serialization

module QuirkRunSetDtoFixture =

    [<Fact>]
    let ``QuirkRunSet`` () =

        let quirkRunSet = 
            QuirkRunSet.createFromCfgPlex
                quirkRunType.Shc
                quirkRunMode.Sim
                O_64.runParamSet1
                O_64.plex64
                (1 |> UMX.tag<replicaNumber>)

        let dtoCereal = quirkRunSet |> QuirkRunSetDto.toJson

        let quirkRunSetBack = 
                    dtoCereal 
                    |> QuirkRunSetDto.fromJson
                    |> Result.ExtractOrThrow

        Assert.True(CollectionProps.areEqual quirkRunSet quirkRunSetBack)
