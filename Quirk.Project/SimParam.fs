namespace Quirk.Project

open FSharp.UMX
open Quirk.Core


[<Measure>] type simParamName
[<Measure>] type simParamSetName

type simParamValue =
    | GenerationStart of string<simParamName> * int<generation>
    | GenerationEnd of string<simParamName> * int<generation>
    | ReportIntervalShort of string<simParamName> * int<generation>
    | ReportIntervalLong of string<simParamName> * int<generation>


module SimParamValue =

    let makeGenerationStart (genStart: int<generation>) =
        ("generationStart" |> UMX.tag<simParamName>, genStart) |> simParamValue.GenerationStart

    let makeGenerationEnd (genEnd: int<generation>) =
        ("generationEnd" |> UMX.tag<simParamName>, genEnd) |> simParamValue.GenerationEnd

    let makeReportIntervalShort (genShort: int<generation>) =
        ("reportIntervalShort" |> UMX.tag<simParamName>, genShort) |> simParamValue.ReportIntervalShort

    let makeReportIntervalLong (genLong: int<generation>) =
        ("reportIntervalLong" |> UMX.tag<simParamName>, genLong) |> simParamValue.ReportIntervalLong


    let getSimParamName (simParamValue: simParamValue) =
        match simParamValue with
        | GenerationStart (n, o) -> n
        | GenerationEnd (n, nf) -> n
        | ReportIntervalShort (n, o) -> n
        | ReportIntervalLong (n, pc) -> n


    let toArrayOfStrings (simParamValue: simParamValue) =
        match simParamValue with
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



    let fromArrayOfStrings (lst: string array) : Result<simParamValue, string> =
        match lst with

        | [|"GenerationStart"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<simParamName>
                return (rpName, ov |> UMX.tag<generation>)
                        |> simParamValue.GenerationStart
            }

        | [|"GenerationEnd"; n; nf|] ->
            result {
                let! nfValue = StringUtil.parseInt nf
                let rpName = n |> UMX.tag<simParamName>
                return (rpName, nfValue |> UMX.tag<generation>)
                        |> simParamValue.GenerationEnd
            }

        | [|"ReportIntervalShort"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<simParamName>
                return (rpName, ov |> UMX.tag<generation>) |> simParamValue.ReportIntervalShort
            }

        | [|"ReportIntervalLong"; n; pc;|] ->
            result {
                let! pcValue = StringUtil.parseInt pc
                let rpName = n |> UMX.tag<simParamName>
                return (rpName, pcValue |> UMX.tag<generation> )
                        |> simParamValue.ReportIntervalLong
            }

        | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            

type simParamSet = 
    private 
        { 
            id: Guid<simParamSetName>
            valueMap: Map<string<simParamName>, simParamValue>
        }


module SimParamSet =

    let create 
            (simParamValues: simParamValue seq)
        =
        let valueMap = 
            simParamValues
            |> Seq.map(fun rpv -> (rpv |> SimParamValue.getSimParamName, rpv ))
            |> Map.ofSeq

        let id = 
            [valueMap :> obj] |> GuidUtils.guidFromObjs
            |> UMX.tag<simParamSetName>
        {
            simParamSet.id = id;
            simParamSet.valueMap = valueMap;
        }
        
    let getId (simParamSet:simParamSet) =
        simParamSet.id
        
    let getValueMap (simParamSet:simParamSet) =
        simParamSet.valueMap
  