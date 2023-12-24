namespace Quirk.Project

open FSharp.UMX
open Quirk.Core


[<Measure>] type runParamName
[<Measure>] type runParamSetName

type runParamValue =
    | GenerationStart of string<runParamName> * int<generation>
    | GenerationEnd of string<runParamName> * int<generation>
    | ReportIntervalShort of string<runParamName> * int<generation>
    | ReportIntervalLong of string<runParamName> * int<generation>


module RunParamValue =

    let makeGenerationStart (genStart: int<generation>) =
        ("generationStart" |> UMX.tag<runParamName>, genStart) |> runParamValue.GenerationStart

    let makeGenerationEnd (genEnd: int<generation>) =
        ("generationEnd" |> UMX.tag<runParamName>, genEnd) |> runParamValue.GenerationEnd

    let makeReportIntervalShort (genShort: int<generation>) =
        ("reportIntervalShort" |> UMX.tag<runParamName>, genShort) |> runParamValue.ReportIntervalShort

    let makeReportIntervalLong (genLong: int<generation>) =
        ("reportIntervalLong" |> UMX.tag<runParamName>, genLong) |> runParamValue.ReportIntervalLong


    let getRunParamName (runParamValue: runParamValue) =
        match runParamValue with
        | GenerationStart (n, o) -> n
        | GenerationEnd (n, nf) -> n
        | ReportIntervalShort (n, o) -> n
        | ReportIntervalLong (n, pc) -> n


    let toArrayOfStrings (runParamValue: runParamValue) =
        match runParamValue with
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



    let fromArrayOfStrings (lst: string array) : Result<runParamValue, string> =
        match lst with

        | [|"GenerationStart"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, ov |> UMX.tag<generation>)
                        |> runParamValue.GenerationStart
            }

        | [|"GenerationEnd"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseInt nf
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, nfValue |> UMX.tag<generation>)
                        |> runParamValue.GenerationEnd
            }

        | [|"ReportIntervalShort"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, ov |> UMX.tag<generation>) |> runParamValue.ReportIntervalShort
            }

        | [|"ReportIntervalLong"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<runParamName>
                return (rpName, pcValue |> UMX.tag<generation> )
                        |> runParamValue.ReportIntervalLong
            }

        | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            

type runParamSet = 
    private 
        { 
            id: Guid<runParamSetName>
            runParamValueMap: Map<string<runParamName>, runParamValue>
        }


module RunParamSet =

    let create 
            (runParamValues: runParamValue seq)
        =
        let runParamValueMap = 
            runParamValues
            |> Seq.map(fun rpv -> (rpv |> RunParamValue.getRunParamName, rpv ))
            |> Map.ofSeq

        let id = 
            [runParamValueMap :> obj] |> GuidUtils.guidFromObjs
            |> UMX.tag<runParamSetName>
        {
            runParamSet.id = id;
            runParamSet.runParamValueMap = runParamValueMap;
        }
        
    let getId (runParamSet:runParamSet) =
        runParamSet.id
        
    let getValueMap (runParamSet:runParamSet) =
        runParamSet.runParamValueMap
  