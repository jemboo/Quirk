namespace Quirk.RunCfg.Core

open FSharp.UMX
open Quirk.Core
open System


type runParamValue =
    | MutationRate of string<runParamName> * float<mutationRate>
    | NoiseFraction of string<runParamName> * float<noiseFraction>
    | Order of string<runParamName> * int<order>
    | ParentCount of string<runParamName> * int<sorterCount>
    | QuirkRunType of string<runParamName> * quirkRunType
    | ReplicaNumber of string<runParamName> * int<replicaNumber>
    | ReproductionRate of string<runParamName> * float<reproductionRate>
    | SorterSetPruneMethod of string<runParamName> * sorterSetPruneMethod
    | StageWeight of string<runParamName> * float<stageWeight>
    | SwitchGenMode of string<runParamName> * switchGenMode



module RunParamValue =

    let getRunParamName (cfgPlexItemValue: runParamValue) =
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


    let toArrayOfStrings (cfgPlexItemValue: runParamValue) =
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
                     pc |> string
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


    let fromArrayOfStrings (lst: string array) : Result<runParamValue, string> =
        match lst with

        | [|"MutationRate"; n; o|] ->
            result {
                let! ov = StringUtil.parseFloat o
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, ov |> UMX.tag<mutationRate>)
                        |> runParamValue.MutationRate
            }

        | [|"NoiseFraction"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseFloat nf
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, nfValue |> UMX.tag<noiseFraction>)
                        |> runParamValue.NoiseFraction
            }

        | [|"Order"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, ov |> UMX.tag<order>) |> runParamValue.Order
            }

        | [|"ParentCount"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, pcValue |> UMX.tag<sorterCount> )
                        |> runParamValue.ParentCount
            }

        | [|"QuirkRunType"; n; pc;|] ->
            result {
                let! pcValue = QuirkRunType.fromString pc
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, pcValue)
                        |> runParamValue.QuirkRunType
            }

        | [|"ReproductionRate"; n; mr|] -> 
            result {
                let! mrValue = StringUtil.parseFloat mr
                let rpName = n |> UMX.tag<runParamName>
                return
                    (rpName, mrValue |> UMX.tag<reproductionRate> )
                      |> runParamValue.ReproductionRate
            }

        | [|"SorterSetPruneMethod"; n; ssp|] ->
            result {
                let! ov = StringUtil.parseFloat ssp
                let! spm = SorterSetPruneMethod.fromReport ssp
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, spm )
                        |> runParamValue.SorterSetPruneMethod
            }

        | [|"StageWeight"; n; o|] ->
            result {
                let! swv = StringUtil.parseFloat o
                let sw = swv |> UMX.tag<stageWeight>
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, sw )
                        |> runParamValue.StageWeight
            }

        | [|"SwitchGenMode"; n; sgm|] ->
            result {
                let! smv = sgm |> SwitchGenMode.fromString
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, smv) |> runParamValue.SwitchGenMode
            }
            | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            


//type runParam = 
//    private 
//        { 
//            runParamName: string<runParamName>
//            cfgPlexItemRank: int<cfgPlexItemRank>
//            runParamValue: runParamValue
//        }


//module RunParam =

//    let create 
//            (runParamName: string<runParamName>) 
//            (cfgPlexItemRank: int<cfgPlexItemRank>)
//            (runParamValue: runParamValue)
//        =
//        {
//            runParam.runParamName = runParamName;
//            runParam.cfgPlexItemRank = cfgPlexItemRank;
//            runParam.runParamValue = runParamValue;
//        }

//    let getName (runParam:runParam) =
//        runParam.runParamValue
        
//    let getRank (runParam:runParam) =
//        runParam.runParamValue
        
//    let getRunParamValue (runParam:runParam) =
//        runParam.runParamValue
  


type runParamSet = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            runParamValueMap: Map<string<runParamName>, runParamValue>
        }


module RunParamSet =

    let create 
            (replicaNumber: int<replicaNumber>)
            (quirkRunType:quirkRunType)
            (runParamValues: runParamValue seq)
        =
        let replicaRunParamValue =
                (("replicaNumber" |> UMX.tag<runParamName>),
                  replicaNumber) |> runParamValue.ReplicaNumber 

        let quirkRunTypeParamValue =
                (("quirkRunType" |> UMX.tag<runParamName>),
                  quirkRunType) |> runParamValue.QuirkRunType 


        let runParamValueMap = 
            runParamValues |> Seq.append
                (seq {replicaRunParamValue; quirkRunTypeParamValue})
            |> Seq.map(fun rpv -> (rpv |> RunParamValue.getRunParamName, rpv ))
            |> Map.ofSeq

        let quirkRunId = 
            [runParamValueMap :> obj] |> GuidUtils.guidFromObjs
            |> UMX.tag<quirkRunId>

        {
            runParamSet.quirkRunId = quirkRunId;
            runParamSet.runParamValueMap = runParamValueMap;
        }
        
    let getQuirkRunId (runParamSet:runParamSet) =
        runParamSet.quirkRunId
        
    let getRunParamValueMap (runParamSet:runParamSet) =
        runParamSet.runParamValueMap
  