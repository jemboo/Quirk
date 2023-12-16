namespace Quirk.Project

open FSharp.UMX
open Quirk.Core
open Quirk.Sorting


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
    | MutationRate of string<quirkModelParamName> * float<mutationRate>
    | NoiseFraction of string<quirkModelParamName> * float<noiseFraction>
    | Order of string<quirkModelParamName> * int<order>
    | ParentCount of string<quirkModelParamName> * int<sorterCount>
    | ReproductionRate of string<quirkModelParamName> * float<reproductionRate>
    | SorterSetPruneMethod of string<quirkModelParamName> * sorterSetPruneMethod
    | StageWeight of string<quirkModelParamName> * float<stageWeight>
    | SwitchGenMode of string<quirkModelParamName> * switchGenMode




module CfgModelParamType =
    
    let toCfgModelParamValue 
            (quirkModelParamName: string<quirkModelParamName>) 
            (cfgModelParamType:cfgModelParamType)
            (strVal:string)
        =
        match cfgModelParamType with
        | cfgModelParamType.MutationRate -> 
            result {
               let! value = StringUtil.parseFloat strVal
               return  (quirkModelParamName, value |> UMX.tag<mutationRate>) |> cfgModelParamValue.MutationRate
            }

        | cfgModelParamType.NoiseFraction ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (quirkModelParamName, value |> UMX.tag<noiseFraction>) |> cfgModelParamValue.NoiseFraction
            }

        | cfgModelParamType.Order ->
            result {
               let! value = StringUtil.parseInt strVal
               return  (quirkModelParamName, value |> UMX.tag<order>) |> cfgModelParamValue.Order
            }

        | cfgModelParamType.ParentCount ->
            result {
               let! value = StringUtil.parseInt strVal
               return  (quirkModelParamName, value |> UMX.tag<sorterCount>) |> cfgModelParamValue.ParentCount
            }

        | cfgModelParamType.ReproductionRate ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (quirkModelParamName, value |> UMX.tag<reproductionRate>) |> cfgModelParamValue.ReproductionRate
            }

        | cfgModelParamType.SorterSetPruneMethod ->
            result {
               let! value = SorterSetPruneMethod.fromReport strVal
               return  (quirkModelParamName, value) |> cfgModelParamValue.SorterSetPruneMethod
            }

        | cfgModelParamType.StageWeight ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (quirkModelParamName, value |> UMX.tag<stageWeight>) |> cfgModelParamValue.StageWeight
            }

        | cfgModelParamType.SwitchGenMode ->
            result {
               let! value = strVal |> SwitchGenMode.fromString
               return  (quirkModelParamName, value) |> cfgModelParamValue.SwitchGenMode
            }




module CfgModelParamValue =

    let makeMutationRates (rates: float seq) =
        rates |> Seq.map(fun r -> ("mutationRate" |> UMX.tag<quirkModelParamName>, r |> UMX.tag<mutationRate>) |> cfgModelParamValue.MutationRate)
        |> Seq.toArray

    let makeNoiseFractions (rates: float seq) =
        rates |> Seq.map(fun r -> ("noiseFraction" |> UMX.tag<quirkModelParamName>, r |> UMX.tag<noiseFraction>) |> cfgModelParamValue.NoiseFraction)
        |> Seq.toArray

    let makeOrders (rates: int seq) =
        rates |> Seq.map(fun r -> ("order" |> UMX.tag<quirkModelParamName>, r |> UMX.tag<order>) |> cfgModelParamValue.Order)
        |> Seq.toArray

    let makeParentCounts (rates: int seq) =
        rates |> Seq.map(fun r -> ("parentCount" |> UMX.tag<quirkModelParamName>, r |> UMX.tag<sorterCount>) |> cfgModelParamValue.ParentCount)
        |> Seq.toArray

    let makeReproductionRates (rates: float seq) =
        rates |> Seq.map(fun r -> ("reproductionRate" |> UMX.tag<quirkModelParamName>, r |> UMX.tag<reproductionRate>) |> cfgModelParamValue.ReproductionRate)
        |> Seq.toArray

    let makeSorterSetPruneMethods (rates: sorterSetPruneMethod seq) =
        rates |> Seq.map(fun r -> ("sorterSetPruneMethod" |> UMX.tag<quirkModelParamName>, r ) |> cfgModelParamValue.SorterSetPruneMethod)
        |> Seq.toArray

    let makeStageWeights (rates: float seq) =
        rates |> Seq.map(fun r -> ("stageWeight" |> UMX.tag<quirkModelParamName>, r |> UMX.tag<stageWeight>) |> cfgModelParamValue.StageWeight)
        |> Seq.toArray

    let makeSwitchGenModes (rates: switchGenMode seq) =
        rates |> Seq.map(fun r -> ("switchGenMode" |> UMX.tag<quirkModelParamName>, r ) |> cfgModelParamValue.SwitchGenMode)
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
                let rpName = n |> UMX.tag<quirkModelParamName>
                return (rpName, ov |> UMX.tag<mutationRate>)
                        |> cfgModelParamValue.MutationRate
            }

        | [|"NoiseFraction"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseFloat nf
                let rpName = n |> UMX.tag<quirkModelParamName>
                return (rpName, nfValue |> UMX.tag<noiseFraction>)
                        |> cfgModelParamValue.NoiseFraction
            }

        | [|"Order"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<quirkModelParamName>
                return (rpName, ov |> UMX.tag<order>) |> cfgModelParamValue.Order
            }

        | [|"ParentCount"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<quirkModelParamName>
                return (rpName, pcValue |> UMX.tag<sorterCount> )
                        |> cfgModelParamValue.ParentCount
            }

        | [|"ReproductionRate"; n; mr|] -> 
            result {
                let! mrValue = StringUtil.parseFloat mr
                let rpName = n |> UMX.tag<quirkModelParamName>
                return
                    (rpName, mrValue |> UMX.tag<reproductionRate> )
                      |> cfgModelParamValue.ReproductionRate
            }

        | [|"SorterSetPruneMethod"; n; ssp|] ->
            result {
                let! spm = SorterSetPruneMethod.fromReport ssp
                let rpName = n |> UMX.tag<quirkModelParamName>
                return (rpName, spm )
                        |> cfgModelParamValue.SorterSetPruneMethod
            }

        | [|"StageWeight"; n; o|] ->
            result {
                let! swv = StringUtil.parseFloat o
                let sw = swv |> UMX.tag<stageWeight>
                let rpName = n |> UMX.tag<quirkModelParamName>
                return (rpName, sw )
                        |> cfgModelParamValue.StageWeight
            }

        | [|"SwitchGenMode"; n; sgm|] ->
            result {
                let! smv = sgm |> SwitchGenMode.fromString
                let rpName = n |> UMX.tag<quirkModelParamName>
                return (rpName, smv) |> cfgModelParamValue.SwitchGenMode
            }

        | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            


type quirkModelParamSet = 
    private 
        { 
            id: Guid<quirkModelParamSetId>
            replicaNumber : int<replicaNumber>
            valueMap: Map<string<quirkModelParamName>, cfgModelParamValue>
        }


module QuirkModelParamSet =

    let create
            (replicaNumber: int<replicaNumber>)
            (runParamValues: cfgModelParamValue seq)
        =
        let runParamValueMap = 
            runParamValues
            |> Seq.map(fun rpv -> (rpv |> CfgModelParamValue.getModelCfgParamName, rpv ))
            |> Map.ofSeq

        let quirkModelParamSetId = 
            [
                replicaNumber :> obj;
                runParamValueMap :> obj
            ] 
            |> GuidUtils.guidFromObjs
            |> UMX.tag<quirkModelParamSetId>

        {
            quirkModelParamSet.id = quirkModelParamSetId;
            quirkModelParamSet.replicaNumber = replicaNumber;
            quirkModelParamSet.valueMap = runParamValueMap;
        }

        
    let getId (quirkModelParamSet:quirkModelParamSet) =
        quirkModelParamSet.id
        
            
    let getReplicaNumber (quirkModelParamSet:quirkModelParamSet) =
        quirkModelParamSet.replicaNumber

    let getValueMap (quirkModelParamSet:quirkModelParamSet) =
        quirkModelParamSet.valueMap
  