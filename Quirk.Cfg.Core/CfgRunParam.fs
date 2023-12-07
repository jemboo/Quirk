namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core
open Quirk.Run.Core


type cfgRunParamValue =
    | GenerationStart of string<cfgRunParamName> * int<generation>
    | GenerationEnd of string<cfgRunParamName> * int<generation>
    | ReportIntervalShort of string<cfgRunParamName> * int<generation>
    | ReportIntervalLong of string<cfgRunParamName> * int<generation>
    | QuirkRunMode of string<cfgRunParamName> * quirkRunMode
    | RunIDsToReport of string<cfgRunParamName> * (Guid<quirkRunId> [])


module CfgRunParamValue =

    let getModelCfgParamName (cfgRunParamValue: cfgRunParamValue) =
        match cfgRunParamValue with
        | GenerationStart (n, o) -> n
        | GenerationEnd (n, nf) -> n
        | ReportIntervalShort (n, o) -> n
        | ReportIntervalLong (n, pc) -> n
        | QuirkRunMode (n, pc) -> n
        | RunIDsToReport (n, ssp) -> n


    let toArrayOfStrings (cfgRunParamValue: cfgRunParamValue) =
        match cfgRunParamValue with
        | GenerationStart (n, o) ->
                [|
                    "GenerationStart";
                    n |> UMX.untag
                    o |> UMX.untag |> string
                |]

        | GenerationEnd (n, nf) ->
                [|
                    "GenerationEnd";
                    nf |> UMX.untag |> string
                |]

        | ReportIntervalShort (n, o) ->
                [|
                    "ReportIntervalShort";
                    n |> UMX.untag
                    o |> UMX.untag |> string
                |]

        | ReportIntervalLong (n, pc) ->
                [|
                     "ReportIntervalLong";
                     n |> UMX.untag
                     pc |> UMX.untag |> string
                |]

        | QuirkRunMode (n, pc) ->
                [|
                     "QuirkRunMode";
                     n |> UMX.untag
                     pc |> QuirkRunMode.toString
                |]

        | RunIDsToReport (n, pc) ->
                [|
                     "RunIDsToReport";
                     n |> UMX.untag;
                     pc |> Array.map(UMX.untag) 
                        |> StringUtil.joinGuids "\t"
                |]



    let fromArrayOfStrings (lst: string array) : Result<cfgRunParamValue, string> =
        match lst with

        | [|"GenerationStart"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<cfgRunParamName>
                return (rpName, ov |> UMX.tag<generation>)
                        |> cfgRunParamValue.GenerationStart
            }

        | [|"GenerationEnd"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseInt nf
                let rpName = n |> UMX.tag<cfgRunParamName>
                return (rpName, nfValue |> UMX.tag<generation>)
                        |> cfgRunParamValue.GenerationEnd
            }

        | [|"ReportIntervalShort"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<cfgRunParamName>
                return (rpName, ov |> UMX.tag<generation>) |> cfgRunParamValue.ReportIntervalShort
            }

        | [|"ReportIntervalLong"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<cfgRunParamName>
                return (rpName, pcValue |> UMX.tag<generation> )
                        |> cfgRunParamValue.ReportIntervalLong
            }

        | [|"QuirkRunMode"; n; qrm;|] ->
            result {
                let! pcValue = QuirkRunMode.fromString qrm
                let rpName = n |> UMX.tag<cfgRunParamName>
                return (rpName, pcValue)
                        |> cfgRunParamValue.QuirkRunMode
            }

        | [|"RunIDsToReport"; n; mr|] -> 
            result {
                let! gaValues = StringUtil.guidArrayFromString "\t" mr
                let rpName = n |> UMX.tag<cfgRunParamName>
                return
                    (rpName, gaValues |> Array.map(UMX.tag<quirkRunId>))
                      |> cfgRunParamValue.RunIDsToReport
            }

        | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            

type cfgRunParamSet = 
    private 
        { 
            quirkRunId: Guid<cfgRunParamSetId>
            runParamValueMap: Map<string<cfgRunParamName>, cfgRunParamValue>
        }


module CfgRunParamSet =

    let create 
            (runParamValues: cfgRunParamValue seq)
        =
        let runParamValueMap = 
            runParamValues
            |> Seq.map(fun rpv -> (rpv |> CfgRunParamValue.getModelCfgParamName, rpv ))
            |> Map.ofSeq

        let quirkRunId = 
            [runParamValueMap :> obj] |> GuidUtils.guidFromObjs
            |> UMX.tag<cfgRunParamSetId>

        {
            cfgRunParamSet.quirkRunId = quirkRunId;
            cfgRunParamSet.runParamValueMap = runParamValueMap;
        }
        
    let getQuirkRunId (runParamSet:cfgRunParamSet) =
        runParamSet.quirkRunId
        
    let getRunParamValueMap (runParamSet:cfgRunParamSet) =
        runParamSet.runParamValueMap
  