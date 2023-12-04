namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core
open Quirk.Run.Core


type cfgModelParamValue =
    | MutationRate of string<cfgModelParamName> * float<mutationRate>
    | NoiseFraction of string<cfgModelParamName> * float<noiseFraction>
    | Order of string<cfgModelParamName> * int<order>
    | ParentCount of string<cfgModelParamName> * int<sorterCount>
    | QuirkRunType of string<cfgModelParamName> * quirkRunType
    | ReplicaNumber of string<cfgModelParamName> * int<replicaNumber>
    | ReproductionRate of string<cfgModelParamName> * float<reproductionRate>
    | SorterSetPruneMethod of string<cfgModelParamName> * sorterSetPruneMethod
    | StageWeight of string<cfgModelParamName> * float<stageWeight>
    | SwitchGenMode of string<cfgModelParamName> * switchGenMode


module CfgModelParamValue =

    let getModelCfgParamName (cfgPlexItemValue: cfgModelParamValue) =
        match cfgPlexItemValue with
        | MutationRate (n, o) -> n
        | NoiseFraction (n, nf) -> n
        | Order (n, o) -> n
        | ParentCount (n, pc) -> n
        | QuirkRunType (n, pc) -> n
        | ReplicaNumber (n, pc) -> n
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

        | QuirkRunType (n, pc) ->
                [|
                     "QuirkRunType";
                     n |> UMX.untag
                     pc |> QuirkRunType.toString
                |]

        | ReplicaNumber (n, pc) ->
                [|
                     "ReplicaNumber";
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

        | [|"QuirkRunType"; n; pc;|] ->
            result {
                let! pcValue = QuirkRunType.fromString pc
                let rpName = n |> UMX.tag<cfgModelParamName>
                return (rpName, pcValue)
                        |> cfgModelParamValue.QuirkRunType
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
                let! ov = StringUtil.parseFloat ssp
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
            quirkRunId: Guid<quirkRunId>
            runParamValueMap: Map<string<cfgModelParamName>, cfgModelParamValue>
        }


module CfgModelParamSet =

    let create 
            (runParamValues: cfgModelParamValue seq)
        =
        let runParamValueMap = 
            runParamValues
            |> Seq.map(fun rpv -> (rpv |> CfgModelParamValue.getModelCfgParamName, rpv ))
            |> Map.ofSeq

        let quirkRunId = 
            [runParamValueMap :> obj] |> GuidUtils.guidFromObjs
            |> UMX.tag<quirkRunId>

        {
            cfgModelParamSet.quirkRunId = quirkRunId;
            cfgModelParamSet.runParamValueMap = runParamValueMap;
        }


    let create2
            (replicaNumber: int<replicaNumber>)
            (quirkRunType:quirkRunType)
            (runParamValues: cfgModelParamValue seq)
        =
        let replicaRunParamValue =
                (("replicaNumber" |> UMX.tag<cfgModelParamName>),
                  replicaNumber) |> cfgModelParamValue.ReplicaNumber 

        let quirkRunTypeParamValue =
                (("quirkRunType" |> UMX.tag<cfgModelParamName>),
                  quirkRunType) |> cfgModelParamValue.QuirkRunType 

        let fullSeq = 
            runParamValues |> Seq.append
                (seq {replicaRunParamValue; quirkRunTypeParamValue})

        create fullSeq

        
    let getQuirkRunId (runParamSet:cfgModelParamSet) =
        runParamSet.quirkRunId
        
    let getRunParamValueMap (runParamSet:cfgModelParamSet) =
        runParamSet.runParamValueMap
  