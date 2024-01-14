namespace Quirk.Project

open FSharp.UMX
open Quirk.Core
open Quirk.Sorting


type modelParamType =
    | MutationRate
    | NoiseFraction
    | Order
    | ParentCount
    | ReproductionRate
    | SorterSetPruneMethod
    | StageWeight
    | SwitchGenMode


type modelParamValue =
    | MutationRate of string<modelParamName> * float<mutationRate>
    | NoiseFraction of string<modelParamName> * float<noiseFraction>
    | Order of string<modelParamName> * int<order>
    | ParentCount of string<modelParamName> * int<sorterCount>
    | ReproductionRate of string<modelParamName> * float<reproductionRate>
    | SorterSetPruneMethod of string<modelParamName> * sorterSetPruneMethod
    | StageWeight of string<modelParamName> * float<stageWeight>
    | SwitchGenMode of string<modelParamName> * switchGenMode



module ModelParamType =
    
    let toModelParamValue 
            (quirkModelParamName: string<modelParamName>) 
            (cfgModelParamType:modelParamType)
            (strVal:string)
        =
        match cfgModelParamType with
        | modelParamType.MutationRate -> 
            result {
               let! value = StringUtil.parseFloat strVal
               return  (quirkModelParamName, value |> UMX.tag<mutationRate>) |> modelParamValue.MutationRate
            }

        | modelParamType.NoiseFraction ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (quirkModelParamName, value |> UMX.tag<noiseFraction>) |> modelParamValue.NoiseFraction
            }

        | modelParamType.Order ->
            result {
               let! value = StringUtil.parseInt strVal
               return  (quirkModelParamName, value |> UMX.tag<order>) |> modelParamValue.Order
            }

        | modelParamType.ParentCount ->
            result {
               let! value = StringUtil.parseInt strVal
               return  (quirkModelParamName, value |> UMX.tag<sorterCount>) |> modelParamValue.ParentCount
            }

        | modelParamType.ReproductionRate ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (quirkModelParamName, value |> UMX.tag<reproductionRate>) |> modelParamValue.ReproductionRate
            }

        | modelParamType.SorterSetPruneMethod ->
            result {
               let! value = SorterSetPruneMethod.fromReport strVal
               return  (quirkModelParamName, value) |> modelParamValue.SorterSetPruneMethod
            }

        | modelParamType.StageWeight ->
            result {
               let! value = StringUtil.parseFloat strVal
               return  (quirkModelParamName, value |> UMX.tag<stageWeight>) |> modelParamValue.StageWeight
            }

        | modelParamType.SwitchGenMode ->
            result {
               let! value = strVal |> SwitchGenMode.fromString
               return  (quirkModelParamName, value) |> modelParamValue.SwitchGenMode
            }


module ModelParamValue =

    let makeMutationRates (rates: float seq) =
        rates |> Seq.map(fun r -> ("mutationRate" |> UMX.tag<modelParamName>, r |> UMX.tag<mutationRate>) |> modelParamValue.MutationRate)
        |> Seq.toArray

    let makeNoiseFractions (rates: float seq) =
        rates |> Seq.map(fun r -> ("noiseFraction" |> UMX.tag<modelParamName>, r |> UMX.tag<noiseFraction>) |> modelParamValue.NoiseFraction)
        |> Seq.toArray

    let makeOrders (rates: int seq) =
        rates |> Seq.map(fun r -> ("order" |> UMX.tag<modelParamName>, r |> UMX.tag<order>) |> modelParamValue.Order)
        |> Seq.toArray

    let makeParentCounts (rates: int seq) =
        rates |> Seq.map(fun r -> ("parentCount" |> UMX.tag<modelParamName>, r |> UMX.tag<sorterCount>) |> modelParamValue.ParentCount)
        |> Seq.toArray

    let makeReproductionRates (rates: float seq) =
        rates |> Seq.map(fun r -> ("reproductionRate" |> UMX.tag<modelParamName>, r |> UMX.tag<reproductionRate>) |> modelParamValue.ReproductionRate)
        |> Seq.toArray

    let makeSorterSetPruneMethods (rates: sorterSetPruneMethod seq) =
        rates |> Seq.map(fun r -> ("sorterSetPruneMethod" |> UMX.tag<modelParamName>, r ) |> modelParamValue.SorterSetPruneMethod)
        |> Seq.toArray

    let makeStageWeights (rates: float seq) =
        rates |> Seq.map(fun r -> ("stageWeight" |> UMX.tag<modelParamName>, r |> UMX.tag<stageWeight>) |> modelParamValue.StageWeight)
        |> Seq.toArray

    let makeSwitchGenModes (rates: switchGenMode seq) =
        rates |> Seq.map(fun r -> ("switchGenMode" |> UMX.tag<modelParamName>, r ) |> modelParamValue.SwitchGenMode)
        |> Seq.toArray


    let getModelParamName (modelParamValue: modelParamValue) =
        match modelParamValue with
        | MutationRate (n, o) -> n
        | NoiseFraction (n, nf) -> n
        | Order (n, o) -> n
        | ParentCount (n, pc) -> n
        | ReproductionRate (n, mr) -> n
        | SorterSetPruneMethod (n, ssp) -> n
        | StageWeight (n, sw) -> n
        | SwitchGenMode (n, sgm) -> n



    let toArrayOfStrings (modelParamValue: modelParamValue) =
        match modelParamValue with
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


    let toReportString  (modelParamValue: modelParamValue) =
        let a = modelParamValue |> toArrayOfStrings
        $"{a.[1]}: {a.[2]}"


    let fromArrayOfStrings (lst: string array) : Result<modelParamValue, string> =
        match lst with

        | [|"MutationRate"; n; o|] ->
            result {
                let! ov = StringUtil.parseFloat o
                let rpName = n |> UMX.tag<modelParamName>
                return (rpName, ov |> UMX.tag<mutationRate>)
                        |> modelParamValue.MutationRate
            }

        | [|"NoiseFraction"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseFloat nf
                let rpName = n |> UMX.tag<modelParamName>
                return (rpName, nfValue |> UMX.tag<noiseFraction>)
                        |> modelParamValue.NoiseFraction
            }

        | [|"Order"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<modelParamName>
                return (rpName, ov |> UMX.tag<order>) |> modelParamValue.Order
            }

        | [|"ParentCount"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<modelParamName>
                return (rpName, pcValue |> UMX.tag<sorterCount> )
                        |> modelParamValue.ParentCount
            }

        | [|"ReproductionRate"; n; mr|] -> 
            result {
                let! mrValue = StringUtil.parseFloat mr
                let rpName = n |> UMX.tag<modelParamName>
                return
                    (rpName, mrValue |> UMX.tag<reproductionRate> )
                      |> modelParamValue.ReproductionRate
            }

        | [|"SorterSetPruneMethod"; n; ssp|] ->
            result {
                let! spm = SorterSetPruneMethod.fromReport ssp
                let rpName = n |> UMX.tag<modelParamName>
                return (rpName, spm )
                        |> modelParamValue.SorterSetPruneMethod
            }

        | [|"StageWeight"; n; o|] ->
            result {
                let! swv = StringUtil.parseFloat o
                let sw = swv |> UMX.tag<stageWeight>
                let rpName = n |> UMX.tag<modelParamName>
                return (rpName, sw )
                        |> modelParamValue.StageWeight
            }

        | [|"SwitchGenMode"; n; sgm|] ->
            result {
                let! smv = sgm |> SwitchGenMode.fromString
                let rpName = n |> UMX.tag<modelParamName>
                return (rpName, smv) |> modelParamValue.SwitchGenMode
            }

        | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            


type modelParamSet = 
    private 
        { 
            id: Guid<modelParamSetId>
            replicaNumber : int<replicaNumber>
            valueMap: Map<string<modelParamName>, modelParamValue>
        }


module ModelParamSet =

    let create
            (replicaNumber: int<replicaNumber>)
            (modelParamValues: modelParamValue seq)
        =
        let modelParamValueMap = 
            modelParamValues
            |> Seq.map(fun rpv -> (rpv |> ModelParamValue.getModelParamName, rpv ))
            |> Map.ofSeq

        let quirkModelParamSetId = 
            [
                replicaNumber :> obj;
                modelParamValueMap :> obj
            ] 
            |> GuidUtils.guidFromObjs
            |> UMX.tag<modelParamSetId>

        {
            modelParamSet.id = quirkModelParamSetId;
            modelParamSet.replicaNumber = replicaNumber;
            modelParamSet.valueMap = modelParamValueMap;
        }

        
    let getId (quirkModelParamSet:modelParamSet) =
        quirkModelParamSet.id
            
    let getReplicaNumber (modelParamSet:modelParamSet) =
        modelParamSet.replicaNumber

    let getValueMap (modelParamSet:modelParamSet) =
        modelParamSet.valueMap
  
    let getAllModelParamValues (modelParamSet:modelParamSet) = 
        modelParamSet.valueMap 
        |> Map.toSeq
        |> Seq.map(snd)

    let getModelParamValue 
            (modelParamName:string<modelParamName>) 
            (modelParamSet:modelParamSet) 
        =
        modelParamSet.valueMap.TryFind modelParamName

    let getModelParamValues
            (modelParamNames:string<modelParamName> seq) 
            (modelParamSet:modelParamSet) 
        =
        modelParamNames
        |> Seq.map(modelParamSet.valueMap.TryFind)
        |> Seq.filter(Option.isSome)
        |> Seq.map(Option.get)
        |> Seq.toArray

