namespace Quirk.Cfg.Shc

open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Sorting
open Quirk.Script

module O_64 =

    let cpiMutationRates =
            CfgPlexItem.create
                ("mutationRates" |> UMX.tag<cfgPlexItemName>)
                (0 |> UMX.tag<cfgPlexItemRank>)
                (ModelParamValue.makeMutationRates [0.0075;])

    let cpiNoiseFractions =
            CfgPlexItem.create
                ("noiseFractions" |> UMX.tag<cfgPlexItemName>)
                (1 |> UMX.tag<cfgPlexItemRank>)
                (ModelParamValue.makeNoiseFractions [0.001;])

    let cpiOrders =
            CfgPlexItem.create
                ("orders" |> UMX.tag<cfgPlexItemName>)
                (2 |> UMX.tag<cfgPlexItemRank>)
                (ModelParamValue.makeOrders [64;])

    let cpiParentCounts =
            CfgPlexItem.create
                ("parentCounts" |> UMX.tag<cfgPlexItemName>)
                (3 |> UMX.tag<cfgPlexItemRank>)
                (ModelParamValue.makeParentCounts [16;])

    let cpiReproductionRates =
            CfgPlexItem.create
                ("reproductionRates" |> UMX.tag<cfgPlexItemName>)
                (4 |> UMX.tag<cfgPlexItemRank>)
                (ModelParamValue.makeReproductionRates [2.0;])


    let spc1 = 1 |> UMX.tag<sorterPhenotypeCount>  |> sorterSetPruneMethod.PhenotypeCap
    let spc2 = 2 |> UMX.tag<sorterPhenotypeCount>  |> sorterSetPruneMethod.PhenotypeCap
    let spc3 = 4 |> UMX.tag<sorterPhenotypeCount>  |> sorterSetPruneMethod.PhenotypeCap
    let spc4 = 8 |> UMX.tag<sorterPhenotypeCount>  |> sorterSetPruneMethod.PhenotypeCap
    let spc5 = 16 |> UMX.tag<sorterPhenotypeCount>  |> sorterSetPruneMethod.PhenotypeCap
    let spc6 = 32 |> UMX.tag<sorterPhenotypeCount>  |> sorterSetPruneMethod.PhenotypeCap
    let spc7 = 64 |> UMX.tag<sorterPhenotypeCount>  |> sorterSetPruneMethod.PhenotypeCap


    let cpiSorterSetPruneMethods =
            CfgPlexItem.create
                ("sorterSetPruneMethods" |> UMX.tag<cfgPlexItemName>)
                (5 |> UMX.tag<cfgPlexItemRank>)
                (ModelParamValue.makeSorterSetPruneMethods [spc2; spc4; spc6])

    let cpiStageWeights =
            CfgPlexItem.create
                ("stageWeights" |> UMX.tag<cfgPlexItemName>)
                (6 |> UMX.tag<cfgPlexItemRank>)
                (ModelParamValue.makeStageWeights [0.05; 0.1;])

    let cpiSwitchGenModes =
            CfgPlexItem.create
                ("switchGenModes" |> UMX.tag<cfgPlexItemName>)
                (7 |> UMX.tag<cfgPlexItemRank>)
                (ModelParamValue.makeSwitchGenModes [switchGenMode.stage; switchGenMode.stageSymmetric])


    let plex64 = 
            CfgPlex.create
                ("Shc_064" |> UMX.tag<projectName> )
                [| 
                   cpiMutationRates; 
                   cpiNoiseFractions; 
                   cpiOrders; 
                   cpiParentCounts; 
                   cpiReproductionRates; 
                   cpiSorterSetPruneMethods; 
                   cpiStageWeights; 
                   cpiSwitchGenModes; 
                |]


    let simParamSet1 = 
            SimParamSet.create
                [|
                    (0 |> UMX.tag<generation> |> SimParamValue.makeGenerationStart)
                    (500 |> UMX.tag<generation> |> SimParamValue.makeGenerationStart)
                    (5 |> UMX.tag<generation> |> SimParamValue.makeGenerationStart)
                    (50 |> UMX.tag<generation> |> SimParamValue.makeGenerationStart)
                |]


    let quirkRunSet = 
            CfgPlex.createQuirkRunSet
                quirkModelType.Shc
                plex64
                (1 |> UMX.tag<replicaNumber>)