namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core
open Quirk.Run.Core


type cfgRunParamValue =
    | GenerationStart of string<cfgRunParamName> * int<generation>
    | GenerationEnd of string<cfgRunParamName> * int<generation>
    | ReportIntervalShort of string<cfgRunParamName> * int<generation>
    | ReportIntervalLong of string<cfgRunParamName> * int<generation>


module CfgRunParamValue =

    let makeGenerationStart (genStart: int<generation>) =
        ("generationStart" |> UMX.tag<cfgRunParamName>, genStart) |> cfgRunParamValue.GenerationStart

    let makeGenerationEnd (genEnd: int<generation>) =
        ("generationEnd" |> UMX.tag<cfgRunParamName>, genEnd) |> cfgRunParamValue.GenerationEnd

    let makeReportIntervalShort (genShort: int<generation>) =
        ("reportIntervalShort" |> UMX.tag<cfgRunParamName>, genShort) |> cfgRunParamValue.ReportIntervalShort

    let makeReportIntervalLong (genLong: int<generation>) =
        ("reportIntervalLong" |> UMX.tag<cfgRunParamName>, genLong) |> cfgRunParamValue.ReportIntervalLong



    let getModelCfgParamName (cfgRunParamValue: cfgRunParamValue) =
        match cfgRunParamValue with
        | GenerationStart (n, o) -> n
        | GenerationEnd (n, nf) -> n
        | ReportIntervalShort (n, o) -> n
        | ReportIntervalLong (n, pc) -> n


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

        | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            

type cfgRunParamSet = 
    private 
        { 
            id: Guid<cfgRunParamSetId>
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

        let id = 
            [runParamValueMap :> obj] |> GuidUtils.guidFromObjs
            |> UMX.tag<cfgRunParamSetId>
        {
            cfgRunParamSet.id = id;
            cfgRunParamSet.runParamValueMap = runParamValueMap;
        }
        
    let getId (runParamSet:cfgRunParamSet) =
        runParamSet.id
        
    let getValueMap (runParamSet:cfgRunParamSet) =
        runParamSet.runParamValueMap
  