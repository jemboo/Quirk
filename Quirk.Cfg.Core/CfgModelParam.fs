namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open Quirk.Run.Core


type cfgModelParamType =
    | MutationRate
    | NoiseFraction
    | Order
    | ParentCount
    | ReproductionRate
    | SorterSetPruneMethod
    | StageWeight
    | SwitchGenMode


type cfgModelParamValue =
    | MutationRate of string<cfgModelParamName> * float<mutationRate>
    | NoiseFraction of string<cfgModelParamName> * float<noiseFraction>
    | Order of string<cfgModelParamName> * int<order>
    | ParentCount of string<cfgModelParamName> * int<sorterCount>
    | ReproductionRate of string<cfgModelParamName> * float<reproductionRate>
    | SorterSetPruneMethod of string<cfgModelParamName> * sorterSetPruneMethod
    | StageWeight of string<cfgModelParamName> * float<stageWeight>
    | SwitchGenMode of string<cfgModelParamName> * switchGenMode




module CfgModelParamType =
    
    let toCfgModelParamValue 
            (cfgModelParamName: string<cfgModelParamName>) 
            (cfgModelParamType:cfgModelParamType)
            (strVal:string)
        =
        match cfgModelParamType with
        | cfgModelParamType.MutationRate -> 
            result {
               let! value = StringUtil.parseFloat strVal
               return  (cfgModelParamName, value |> UMX.tag<mutationRate>) |> cfgModelParamValue.MutationRate
            }

        | cfgModelParamType.NoiseFraction ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (cfgModelParamName, value |> UMX.tag<noiseFraction>) |> cfgModelParamValue.NoiseFraction
            }

        | cfgModelParamType.Order ->
            result {
               let! value = StringUtil.parseInt strVal
               return  (cfgModelParamName, value |> UMX.tag<order>) |> cfgModelParamValue.Order
            }

        | cfgModelParamType.ParentCount ->
            result {
               let! value = StringUtil.parseInt strVal
               return  (cfgModelParamName, value |> UMX.tag<sorterCount>) |> cfgModelParamValue.ParentCount
            }

        | cfgModelParamType.ReproductionRate ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (cfgModelParamName, value |> UMX.tag<reproductionRate>) |> cfgModelParamValue.ReproductionRate
            }

        | cfgModelParamType.SorterSetPruneMethod ->
            result {
               let! value = SorterSetPruneMethod.fromReport strVal
               return  (cfgModelParamName, value) |> cfgModelParamValue.SorterSetPruneMethod
            }

        | cfgModelParamType.StageWeight ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (cfgModelParamName, value |> UMX.tag<stageWeight>) |> cfgModelParamValue.StageWeight
            }

        | cfgModelParamType.SwitchGenMode ->
            result {
               let! value = strVal |> SwitchGenMode.fromString
               return  (cfgModelParamName, value) |> cfgModelParamValue.SwitchGenMode
            }




module CfgModelParamValue =

    let makeMutationRates (rates: float seq) =
        rates |> Seq.map(fun r -> ("mutationRate" |> UMX.tag<cfgModelParamName>, r |> UMX.tag<mutationRate>) |> cfgModelParamValue.MutationRate)
        |> Seq.toArray

    let makeNoiseFractions (rates: float seq) =
        rates |> Seq.map(fun r -> ("noiseFraction" |> UMX.tag<cfgModelParamName>, r |> UMX.tag<noiseFraction>) |> cfgModelParamValue.NoiseFraction)
        |> Seq.toArray

    let makeOrders (rates: int seq) =
        rates |> Seq.map(fun r -> ("order" |> UMX.tag<cfgModelParamName>, r |> UMX.tag<order>) |> cfgModelParamValue.Order)
        |> Seq.toArray

    let makeParentCounts (rates: int seq) =
        rates |> Seq.map(fun r -> ("parentCount" |> UMX.tag<cfgModelParamName>, r |> UMX.tag<sorterCount>) |> cfgModelParamValue.ParentCount)
        |> Seq.toArray

    let makeReproductionRates (rates: float seq) =
        rates |> Seq.map(fun r -> ("reproductionRate" |> UMX.tag<cfgModelParamName>, r |> UMX.tag<reproductionRate>) |> cfgModelParamValue.ReproductionRate)
        |> Seq.toArray

    let makeSorterSetPruneMethods (rates: sorterSetPruneMethod seq) =
        rates |> Seq.map(fun r -> ("sorterSetPruneMethod" |> UMX.tag<cfgModelParamName>, r ) |> cfgModelParamValue.SorterSetPruneMethod)
        |> Seq.toArray

    let makeStageWeights (rates: float seq) =
        rates |> Seq.map(fun r -> ("stageWeight" |> UMX.tag<cfgModelParamName>, r |> UMX.tag<stageWeight>) |> cfgModelParamValue.StageWeight)
        |> Seq.toArray

    let makeSwitchGenModes (rates: switchGenMode seq) =
        rates |> Seq.map(fun r -> ("switchGenMode" |> UMX.tag<cfgModelParamName>, r ) |> cfgModelParamValue.SwitchGenMode)
        |> Seq.toArray




    let getModelCfgParamName (cfgPlexItemValue: cfgModelParamValue) =
        match cfgPlexItemValue with
        | MutationRate (n, o) -> n
        | NoiseFraction (n, nf) -> n
        | Order (n, o) -> n
        | ParentCount (n, pc) -> n
        | ReproductionRate (n, mr) -> n
        | SorterSetPruneMethod (n, ssp) -> n
        | StageWeight (n, sw) -> n
        | SwitchGenMode (n, sgm) -> n


    let toArrayOfStrings (cfgPlexItemValue: cfgModelParamValue) =
        match cfgPlexItemValue with
        | MutationRate (n, o) ->
                [|
                    "MutationRate";
                    n |> UMX.untag
                    o |> UMX.untag |> string
                |]

        | NoiseFraction (n, nf) ->
                [|
                    "NoiseFraction";
                    n |> UMX.untag
                    nf |> UMX.untag |> string
                |]

        | Order (n, o) ->
                [|
                    "Order";
                    n |> UMX.untag
                    o |> UMX.untag |> string
                |]

        | ParentCount (n, pc) ->
                [|
                     "ParentCount";
                     n |> UMX.untag
                     pc |> UMX.untag |> string
                |]

        | ReproductionRate (n, mr) ->
                [|
                  "ReproductionRate";
                  n |> UMX.untag
                  mr |> UMX.untag |> string
                |]

        | SorterSetPruneMethod (n, ssp) ->
                [|
                    "SorterSetPruneMethod";
                    n |> UMX.untag
                    ssp |> SorterSetPruneMethod.toReport
                |]

        | StageWeight (n, sw) ->
                [|
                    "StageWeight";
                    n |> UMX.untag
                    sw |> UMX.untag |> string
                |]

        | SwitchGenMode (n, sgm) ->
                [|
                    "SwitchGenMode";
                    n |> UMX.untag
                    sgm |> string
                |]


    let fromArrayOfStrings (lst: string array) : Result<cfgModelParamValue, string> =
        match lst with

        | [|"MutationRate"; n; o|] ->
            result {
                let! ov = StringUtil.parseFloat o
                let rpName = n |> UMX.tag<cfgModelParamName>
                return (rpName, ov |> UMX.tag<mutationRate>)
                        |> cfgModelParamValue.MutationRate
            }

        | [|"NoiseFraction"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseFloat nf
                let rpName = n |> UMX.tag<cfgModelParamName>
                return (rpName, nfValue |> UMX.tag<noiseFraction>)
                        |> cfgModelParamValue.NoiseFraction
            }

        | [|"Order"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<cfgModelParamName>
                return (rpName, ov |> UMX.tag<order>) |> cfgModelParamValue.Order
            }

        | [|"ParentCount"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<cfgModelParamName>
                return (rpName, pcValue |> UMX.tag<sorterCount> )
                        |> cfgModelParamValue.ParentCount
            }

        | [|"ReproductionRate"; n; mr|] -> 
            result {
                let! mrValue = StringUtil.parseFloat mr
                let rpName = n |> UMX.tag<cfgModelParamName>
                return
                    (rpName, mrValue |> UMX.tag<reproductionRate> )
                      |> cfgModelParamValue.ReproductionRate
            }

        | [|"SorterSetPruneMethod"; n; ssp|] ->
            result {
                let! spm = SorterSetPruneMethod.fromReport ssp
                let rpName = n |> UMX.tag<cfgModelParamName>
                return (rpName, spm )
                        |> cfgModelParamValue.SorterSetPruneMethod
            }

        | [|"StageWeight"; n; o|] ->
            result {
                let! swv = StringUtil.parseFloat o
                let sw = swv |> UMX.tag<stageWeight>
                let rpName = n |> UMX.tag<cfgModelParamName>
                return (rpName, sw )
                        |> cfgModelParamValue.StageWeight
            }

        | [|"SwitchGenMode"; n; sgm|] ->
            result {
                let! smv = sgm |> SwitchGenMode.fromString
                let rpName = n |> UMX.tag<cfgModelParamName>
                return (rpName, smv) |> cfgModelParamValue.SwitchGenMode
            }

        | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            


type cfgModelParamSet = 
    private 
        { 
            id: Guid<cfgModelParamSetId>
            replicaNumber : int<replicaNumber>
            valueMap: Map<string<cfgModelParamName>, cfgModelParamValue>
        }


module CfgModelParamSet =

    let create
            (replicaNumber: int<replicaNumber>)
            (runParamValues: cfgModelParamValue seq)
        =
        let runParamValueMap = 
            runParamValues
            |> Seq.map(fun rpv -> (rpv |> CfgModelParamValue.getModelCfgParamName, rpv ))
            |> Map.ofSeq

        let cfgModelParamSetId = 
            [
                replicaNumber :> obj;
                runParamValueMap :> obj
            ] 
            |> GuidUtils.guidFromObjs
            |> UMX.tag<cfgModelParamSetId>

        {
            cfgModelParamSet.id = cfgModelParamSetId;
            cfgModelParamSet.replicaNumber = replicaNumber;
            cfgModelParamSet.valueMap = runParamValueMap;
        }

        
    let getId (cfgModelParamSet:cfgModelParamSet) =
        cfgModelParamSet.id
        
            
    let getReplicaNumber (cfgModelParamSet:cfgModelParamSet) =
        cfgModelParamSet.replicaNumber

    let getValueMap (cfgModelParamSet:cfgModelParamSet) =
        cfgModelParamSet.valueMap
  