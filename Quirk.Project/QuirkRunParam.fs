namespace Quirk.Project

open FSharp.UMX
open Quirk.Core

type cfgRunParamValue =
    | GenerationStart of string<quirkRunParamName> * int<generation>
    | GenerationEnd of string<quirkRunParamName> * int<generation>
    | ReportIntervalShort of string<quirkRunParamName> * int<generation>
    | ReportIntervalLong of string<quirkRunParamName> * int<generation>


module CfgRunParamValue =

    let makeGenerationStart (genStart: int<generation>) =
        ("generationStart" |> UMX.tag<quirkRunParamName>, genStart) |> cfgRunParamValue.GenerationStart

    let makeGenerationEnd (genEnd: int<generation>) =
        ("generationEnd" |> UMX.tag<quirkRunParamName>, genEnd) |> cfgRunParamValue.GenerationEnd

    let makeReportIntervalShort (genShort: int<generation>) =
        ("reportIntervalShort" |> UMX.tag<quirkRunParamName>, genShort) |> cfgRunParamValue.ReportIntervalShort

    let makeReportIntervalLong (genLong: int<generation>) =
        ("reportIntervalLong" |> UMX.tag<quirkRunParamName>, genLong) |> cfgRunParamValue.ReportIntervalLong



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
                let rpName = n |> UMX.tag<quirkRunParamName>
                return (rpName, ov |> UMX.tag<generation>)
                        |> cfgRunParamValue.GenerationStart
            }

        | [|"GenerationEnd"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseInt nf
                let rpName = n |> UMX.tag<quirkRunParamName>
                return (rpName, nfValue |> UMX.tag<generation>)
                        |> cfgRunParamValue.GenerationEnd
            }

        | [|"ReportIntervalShort"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<quirkRunParamName>
                return (rpName, ov |> UMX.tag<generation>) |> cfgRunParamValue.ReportIntervalShort
            }

        | [|"ReportIntervalLong"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<quirkRunParamName>
                return (rpName, pcValue |> UMX.tag<generation> )
                        |> cfgRunParamValue.ReportIntervalLong
            }

        | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            

type quirkRunParamSet = 
    private 
        { 
            id: Guid<quirkRunParamSetId>
            runParamValueMap: Map<string<quirkRunParamName>, cfgRunParamValue>
        }


module QuirkRunParamSet =

    let create 
            (runParamValues: cfgRunParamValue seq)
        =
        let runParamValueMap = 
            runParamValues
            |> Seq.map(fun rpv -> (rpv |> CfgRunParamValue.getModelCfgParamName, rpv ))
            |> Map.ofSeq

        let id = 
            [runParamValueMap :> obj] |> GuidUtils.guidFromObjs
            |> UMX.tag<quirkRunParamSetId>
        {
            quirkRunParamSet.id = id;
            quirkRunParamSet.runParamValueMap = runParamValueMap;
        }
        
    let getId (runParamSet:quirkRunParamSet) =
        runParamSet.id
        
    let getValueMap (runParamSet:quirkRunParamSet) =
        runParamSet.runParamValueMap
  