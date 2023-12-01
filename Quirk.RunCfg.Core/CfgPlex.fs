namespace Quirk.RunCfg.Core

open FSharp.UMX
open Quirk.Core


type cfgPlexParamValue =
    | Order of int<order>
    | ReproductionRate of float<reproductionRate>
    | MutationRate of float<mutationRate>
    | NoiseFraction of float<noiseFraction>
    | ParentCount of int<sorterCount>
    | SorterSetPruneMethod of sorterSetPruneMethod
    | StageWeight of float<stageWeight>
    | SwitchGenMode of switchGenMode



module CfgPlexParamValue =
    let toArrayOfStrings (cfgPlexItemValue: cfgPlexParamValue) =
        match cfgPlexItemValue with
        | Order o ->
                [|
                    "Order";
                    o |> UMX.untag |> string
                |]
        | ReproductionRate mr ->
                [|
                  "ReproductionRate";
                  mr |> UMX.untag |> string
                |]
        | MutationRate o ->
                [|
                    "MutationRate";
                    o |> UMX.untag |> string
                |]
        | NoiseFraction nf ->
                [|
                    "NoiseFraction";
                    nf |> UMX.untag |> string
                |]
        | ParentCount pc ->
                [|
                     "ParentCount";
                     pc |> UMX.untag |> string
                |]
        | SorterSetPruneMethod ssp ->
                [|
                    "SorterSetPruneMethod";
                    ssp |> SorterSetPruneMethod.toReport
                |]
        | StageWeight sw ->
                [|
                    "StageWeight";
                     sw |> UMX.untag |> string
                |]
        | SwitchGenMode sgm ->
                [|
                    "SwitchGenMode";
                    sgm |> string
                |]


    let fromArrayOfStrings (lst: string array) : Result<cfgPlexParamValue, string> =
        match lst with
        | [|"Order"; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                return ov |> UMX.tag<order> |> cfgPlexParamValue.Order
            }
        | [|"ReproductionRate"; mr|] -> 
            result {
                let! mrValue = StringUtil.parseFloat mr
                return
                    mrValue 
                      |> UMX.tag<reproductionRate> 
                      |> cfgPlexParamValue.ReproductionRate
            }
        | [|"MutationRate"; o|] ->
            result {
                let! ov = StringUtil.parseFloat o
                return ov |> UMX.tag<mutationRate> |> cfgPlexParamValue.MutationRate
            }
        | [|"NoiseFraction"; nf|] ->
            result {
                let! nfValue = StringUtil.parseFloat nf
                return nfValue |> UMX.tag<noiseFraction> |> cfgPlexParamValue.NoiseFraction
            }
        | [|"ParentCount"; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                return pcValue |> UMX.tag<sorterCount> |> cfgPlexParamValue.ParentCount
            }
        | [|"SorterSetPruneMethod"; ssp|] ->
            result {
                let! ov = StringUtil.parseFloat ssp
                let! spm = SorterSetPruneMethod.fromReport ssp
                return spm |> cfgPlexParamValue.SorterSetPruneMethod
            }
        | [|"StageWeight"; o|] ->
            result {
                let! swv = StringUtil.parseFloat o
                let sw = swv |> UMX.tag<stageWeight>
                return sw |> cfgPlexParamValue.StageWeight
            }
        | [|"SwitchGenMode"; sgm|] ->
            result {
                let! smv = sgm |> SwitchGenMode.fromString
                return smv |> cfgPlexParamValue.SwitchGenMode
            }
            | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            


type cfgPlexParam = 
    private 
        { 
            name: string<cfgPlexItemName>
            cfgPlexItemRank: int<cfgPlexItemRank>
            cfgPlexItemValues: cfgPlexParamValue[]
        }


module CfgPlexParam =

    let create 
            (cfgPlexItemName: string<cfgPlexItemName>) 
            (cfgPlexItemRank: int<cfgPlexItemRank>)
            (cfgPlexItemValues: cfgPlexParamValue[])
        =
        {
            cfgPlexParam.name = cfgPlexItemName;
            cfgPlexParam.cfgPlexItemRank = cfgPlexItemRank;
            cfgPlexParam.cfgPlexItemValues = cfgPlexItemValues;
        }
    let getName (cfgPlexItem:cfgPlexParam) =
        cfgPlexItem.name
        
    let getRank (cfgPlexItem:cfgPlexParam) =
        cfgPlexItem.cfgPlexItemRank
        
    let getCfgPlexItemValues (cfgPlexItem:cfgPlexParam) =
        cfgPlexItem.cfgPlexItemValues
    

    let enumerateItems (cfgPlexItems: cfgPlexParam[]) =
        let listList =
                cfgPlexItems
                |> Array.sortBy(fun it -> it.cfgPlexItemRank |> UMX.untag)
                |> Array.map(fun it -> it.cfgPlexItemValues |> Array.toList)
                |> Array.toList
        CollectionOps.crossProduct listList



type cfgPlex =
     private 
        { 
            name: string<cfgPlexName>
            rngGen: rngGen
            cfgPlexItems: cfgPlexParam[]
        }


module CfgPlex =
    let create
            (name:string<cfgPlexName>)
            (rngGen:rngGen)
            (cfgPlexItems:cfgPlexParam[])
         =
        { 
            name = name
            rngGen = rngGen
            cfgPlexItems = cfgPlexItems
        }
        
    let getName (cfgPlex:cfgPlex) =
        cfgPlex.name
        
    let getRngGen (cfgPlex:cfgPlex) =
        cfgPlex.rngGen
        
    let getCfgPlexItems (cfgPlex:cfgPlex) =
        cfgPlex.cfgPlexItems
