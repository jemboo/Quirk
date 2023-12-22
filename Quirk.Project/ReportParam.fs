namespace Quirk.Project

open FSharp.UMX
open Quirk.Core



type reportParamValue =
    | GenerationStart of string<reportParamName> * int<generation>
    | GenerationEnd of string<reportParamName> * int<generation>
    | ReportIntervalShort of string<reportParamName> * int<generation>
    | ReportIntervalLong of string<reportParamName> * int<generation>


module ReportParamValue =

    let makeGenerationStart (genStart: int<generation>) =
        ("generationStart" |> UMX.tag<reportParamName>, genStart) |> reportParamValue.GenerationStart

    let makeGenerationEnd (genEnd: int<generation>) =
        ("generationEnd" |> UMX.tag<reportParamName>, genEnd) |> reportParamValue.GenerationEnd

    let makeReportIntervalShort (genShort: int<generation>) =
        ("reportIntervalShort" |> UMX.tag<reportParamName>, genShort) |> reportParamValue.ReportIntervalShort

    let makeReportIntervalLong (genLong: int<generation>) =
        ("reportIntervalLong" |> UMX.tag<reportParamName>, genLong) |> reportParamValue.ReportIntervalLong


    let reportParamName (reportParamValue: reportParamValue) =
        match reportParamValue with
        | GenerationStart (n, o) -> n
        | GenerationEnd (n, nf) -> n
        | ReportIntervalShort (n, o) -> n
        | ReportIntervalLong (n, pc) -> n


    let toArrayOfStrings (reportParamValue: reportParamValue) =
        match reportParamValue with
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


    let fromArrayOfStrings (lst: string array) : Result<reportParamValue, string> =
        match lst with

        | [|"GenerationStart"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<reportParamName>
                return (rpName, ov |> UMX.tag<generation>)
                        |> reportParamValue.GenerationStart
            }

        | [|"GenerationEnd"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseInt nf
                let rpName = n |> UMX.tag<reportParamName>
                return (rpName, nfValue |> UMX.tag<generation>)
                        |> reportParamValue.GenerationEnd
            }

        | [|"ReportIntervalShort"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<reportParamName>
                return (rpName, ov |> UMX.tag<generation>) |> reportParamValue.ReportIntervalShort
            }

        | [|"ReportIntervalLong"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<reportParamName>
                return (rpName, pcValue |> UMX.tag<generation> )
                        |> reportParamValue.ReportIntervalLong
            }

        | uhv -> $"not handled in reportParamValue.fromArrayOfStrings %A{uhv}" |> Error
            


type reportParamSet = 
    private 
        { 
            id: Guid<reportParamSetName>
            reportParamValueMap: Map<string<reportParamName>, reportParamValue>
        }


module reportParamSet =

    let create 
            (reportParamValues: reportParamValue seq)
        =
        let reportParamValueMap = 
            reportParamValues
            |> Seq.map(fun rpv -> (rpv |> ReportParamValue.reportParamName, rpv ))
            |> Map.ofSeq

        let id = 
            [reportParamValueMap :> obj] |> GuidUtils.guidFromObjs
            |> UMX.tag<reportParamSetName>
        {
            reportParamSet.id = id;
            reportParamSet.reportParamValueMap = reportParamValueMap;
        }
        
    let getId (reportParamSet:reportParamSet) =
        reportParamSet.id
        
    let getValueMap (reportParamSet:reportParamSet) =
        reportParamSet.reportParamValueMap
  