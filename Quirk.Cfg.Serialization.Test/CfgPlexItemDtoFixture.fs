namespace Quirk.Cfg.Serialization.Test

open System
open Xunit
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Cfg.Core
open Quirk.Cfg.Serialization
open Quirk.SortingResults

module CfgPlexItemDtoFixture =

    //[<Fact>]
    //let ``quirkProgramMode`` () =

    //    let qrmRpt = "yowza" |> UMX.tag<reportName> |> quirkScriptMode.Report
    //    let cereal = qrmRpt |> QuirkScriptMode.toString
    //    let decerealR = cereal |> QuirkScriptMode.fromString
    //    let qrmRptBack = decerealR |> Result.ExtractOrThrow

    //    Assert.Equal (qrmRpt, qrmRptBack)



    [<Fact>]
    let ``cfgPlexItemDto`` () =

        let cfpiNameA = "cfpNameA" |> UMX.tag<cfgPlexItemName>
        let rpNameA = "rpNameA" |> UMX.tag<modelParamName>
        let cfpiRankA = 1 |> UMX.tag<cfgPlexItemRank>
        
        let cfpiNameB = "cfpNameB" |> UMX.tag<cfgPlexItemName>
        let rpNameB = "rpNameB" |> UMX.tag<modelParamName>
        let cfpiRankB = 2 |> UMX.tag<cfgPlexItemRank>
        
        let cfpiNameC = "cfpNameC" |> UMX.tag<cfgPlexItemName>
        let rpNameC = "rpNameC" |> UMX.tag<modelParamName>
        let cfpiRankC = 3 |> UMX.tag<cfgPlexItemRank>
        
        
        let cfpivMutationRate1 = (rpNameB, 0.1 |> UMX.tag<mutationRate> ) |> modelParamValue.MutationRate
        let cfpivMutationRate2 = (rpNameB, 0.2 |> UMX.tag<mutationRate> )  |> modelParamValue.MutationRate
        let cfpivNoiseFraction1 = (rpNameC, 0.25 |> UMX.tag<noiseFraction> )  |> modelParamValue.NoiseFraction
        let cfpivNoiseFraction2 = (rpNameC, 0.50 |> UMX.tag<noiseFraction> ) |> modelParamValue.NoiseFraction
        

        let cfgPlexItemValuesB = [| cfpivMutationRate1; cfpivMutationRate2 |]
        let cfgPlexItemValuesC = [| cfpivNoiseFraction1; cfpivNoiseFraction2 |]
        
        let cfpiB = CfgPlexItem.create cfpiNameB cfpiRankB cfgPlexItemValuesB
        let cfpiC = CfgPlexItem.create cfpiNameC cfpiRankC cfgPlexItemValuesC
        
        
        
        let cfgPlexName = "cfgPlexName" |> UMX.tag<cfgPlexName>
        let projectName = "projectName" |> UMX.tag<projectName>
        let cfgPlexItems = [|cfpiB; cfpiC|]
        
        let cfgPlex = CfgPlex.create cfgPlexName projectName cfgPlexItems
        let strVal = sprintf "%A" cfgPlex

        let cfgPlexCereal = cfgPlex |> CfgPlexDto.toJson 
        let cfgPlexBackR = cfgPlexCereal |> CfgPlexDto.fromJson 
        let cfgPlexBack = cfgPlexBackR |> Result.ExtractOrThrow 
        
        
        Assert.True(CollectionProps.areEqual cfgPlex cfgPlexBack )
