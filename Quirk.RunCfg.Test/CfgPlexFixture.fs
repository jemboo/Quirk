module CfgPlexFixture

open System
open Xunit
open FSharp.UMX
open Quirk.Core
open Quirk.RunCfg.Core
open Quirk.RunCfg.Serialization

[<Fact>]
let ``cfgPlexItemDto`` () =

    let cfpiNameA = "cfpNameA" |> UMX.tag<cfgPlexItemName>
    let rpNameA = "rpNameA" |> UMX.tag<runParamName>
    let cfpiRankA = 1 |> UMX.tag<cfgPlexItemRank>
        
    let cfpiNameB = "cfpNameB" |> UMX.tag<cfgPlexItemName>
    let rpNameB = "rpNameB" |> UMX.tag<runParamName>
    let cfpiRankB = 2 |> UMX.tag<cfgPlexItemRank>
        
    let cfpiNameC = "cfpNameC" |> UMX.tag<cfgPlexItemName>
    let rpNameC = "rpNameC" |> UMX.tag<runParamName>
    let cfpiRankC = 3 |> UMX.tag<cfgPlexItemRank>
        
        
    let cfpivOrder1 = (rpNameA, 16 |> UMX.tag<order>) |> runParamValue.Order
    let cfpivMutationRate1 = (rpNameB, 0.1 |> UMX.tag<mutationRate> ) |> runParamValue.MutationRate
    let cfpivMutationRate2 = (rpNameB, 0.2 |> UMX.tag<mutationRate> )  |> runParamValue.MutationRate
    let cfpivNoiseFraction1 = (rpNameC, 0.25 |> UMX.tag<noiseFraction> )  |> runParamValue.NoiseFraction
    let cfpivNoiseFraction2 = (rpNameC, 0.50 |> UMX.tag<noiseFraction> ) |> runParamValue.NoiseFraction
        
    let cfgPlexItemValuesA = [| cfpivOrder1 |]
    let cfgPlexItemValuesB = [| cfpivMutationRate1; cfpivMutationRate2 |]
    let cfgPlexItemValuesC = [| cfpivNoiseFraction1; cfpivNoiseFraction2 |]
        
    let cfpiA = CfgPlexItem.create cfpiNameA cfpiRankA cfgPlexItemValuesA
    let cfpiB = CfgPlexItem.create cfpiNameB cfpiRankB cfgPlexItemValuesB
    let cfpiC = CfgPlexItem.create cfpiNameC cfpiRankC cfgPlexItemValuesC
        
        
        
    let cfgPlexName = "cfgPlexName" |> UMX.tag<cfgPlexName>
    let rngGen = RngGen.createLcg (123uL |> UMX.tag<randomSeed>)
    let cfgPlexItems = [|cfpiA; cfpiB; cfpiC|]
        
    let cfgPlex = CfgPlex.create cfgPlexName rngGen cfgPlexItems
    let cfgPlexCereal = cfgPlex |> CfgPlexDto.toJson 
    let cfgPlexBackR = cfgPlexCereal |> CfgPlexDto.fromJson 
    let cfgPlexBack = cfgPlexBackR |> Result.ExtractOrThrow 
        
        
    Assert.True(CollectionProps.areEqual cfgPlex cfgPlexBack )
