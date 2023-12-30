namespace Quirk.Project

open FSharp.UMX
open Quirk.Core



type reportParamValue =
    | GenerationStart of string<reportParamName> * int<generation>
    | GenerationEnd of string<reportParamName> * int<generation>
    | ReportInterval of string<reportParamName> * int<generation>
    | ReportName of string<reportParamName> * string<reportName>


module ReportParamValue =

    let makeGenerationStart (genStart: int<generation>) =
        ("generationStart" |> UMX.tag<reportParamName>, genStart) |> reportParamValue.GenerationStart

    let makeGenerationEnd (genEnd: int<generation>) =
        ("generationEnd" |> UMX.tag<reportParamName>, genEnd) |> reportParamValue.GenerationEnd

    let makeReportInterval (genShort: int<generation>) =
        ("reportInterval" |> UMX.tag<reportParamName>, genShort) |> reportParamValue.ReportInterval

    let makeReportName (genLong: string<reportName>) =
        ("reportName" |> UMX.tag<reportParamName>, genLong) |> reportParamValue.ReportName


    let reportParamName (reportParamValue: reportParamValue) =
        match reportParamValue with
        | GenerationStart (n, o) -> n
        | GenerationEnd (n, nf) -> n
        | ReportInterval (n, o) -> n
        | ReportName (n, rn) -> n


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

        | ReportInterval (n, o) ->
                [|
                    "ReportInterval";
                    n |> UMX.untag
                    o |> UMX.untag |> string
                |]

        | ReportName (n, pc) ->
                [|
                     "ReportName";
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

        | [|"ReportInterval"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<reportParamName>
                return (rpName, ov |> UMX.tag<generation>) |> reportParamValue.ReportInterval
            }

        | [|"ReportName"; n; pc;|] ->
            result {
                let rpName = n |> UMX.tag<reportParamName>
                return (rpName, pc |> UMX.tag<reportName> )
                        |> reportParamValue.ReportName
            }

        | uhv -> $"not handled in reportParamValue.fromArrayOfStrings %A{uhv}" |> Error
            


type reportParamSet = 
    private 
        { 
            id: Guid<reportParamSetName>
            reportParamValueMap: Map<string<reportParamName>, reportParamValue>
        }


module ReportParamSet =

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
  