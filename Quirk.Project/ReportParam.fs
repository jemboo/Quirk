namespace Quirk.Project

open FSharp.UMX
open Quirk.Core
open Quirk.Iter



type reportParamValue =
    | Generation of string<reportParamName> * int<generation>
    | ReportType of string<reportParamName> * string<reportType>


module ReportParamValue =

    let makeGenerationStart (genStart: int<generation>) =
        ("generationStart" |> UMX.tag<reportParamName>, genStart) |> reportParamValue.Generation

    let makeGenerationEnd (genEnd: int<generation>) =
        ("generationEnd" |> UMX.tag<reportParamName>, genEnd) |> reportParamValue.Generation

    let makeReportInterval (genShort: int<generation>) =
        ("reportInterval" |> UMX.tag<reportParamName>, genShort) |> reportParamValue.Generation

    let makeReportType (genLong: string<reportType>) =
        ("reportType" |> UMX.tag<reportParamName>, genLong) |> reportParamValue.ReportType


    let reportParamName (reportParamValue: reportParamValue) =
        match reportParamValue with
        | Generation (n, o) -> n
        | ReportType (n, rt) -> n


    let toArrayOfStrings (reportParamValue: reportParamValue) =
        match reportParamValue with
        | Generation (n, o) ->
                [|
                    "Generation";
                    n |> UMX.untag
                    o |> UMX.untag |> string
                |]
        | ReportType (n, o) ->
                [|
                    "ReportType";
                    n |> UMX.untag
                    o |> UMX.untag |> string
                |]


    let fromArrayOfStrings (lst: string array) : Result<reportParamValue, string> =
        match lst with

        | [|"Generation"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<reportParamName>
                return (rpName, ov |> UMX.tag<generation>)
                        |> reportParamValue.Generation
            }

        | [|"ReportType"; n; pc;|] ->
            result {
                let rpName = n |> UMX.tag<reportParamName>
                return (rpName, pc |> UMX.tag<reportType> )
                        |> reportParamValue.ReportType
            }

        | uhv -> $"not handled in reportParamValue.fromArrayOfStrings %A{uhv}" |> Error
            


type reportParamSet = 
    private 
        { 
            id: Guid<reportParamSetName>
            valueMap: Map<string<reportParamName>, reportParamValue>
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
            reportParamSet.valueMap = reportParamValueMap;
        }
        
    let getId (reportParamSet:reportParamSet) =
        reportParamSet.id
        
    let getValueMap (reportParamSet:reportParamSet) =
        reportParamSet.valueMap


    let getParamValue 
            (reportParamName:string<reportParamName>) 
            (reportParamSet:reportParamSet) 
        =
        reportParamSet.valueMap.TryFind reportParamName


    let getGeneration
            (reportParamName:string<reportParamName>)  
            (reportParamSet:reportParamSet) 
            =
        let paramValue = reportParamSet |> getParamValue reportParamName
        match paramValue with
        | Some v -> 
            match v with
            | reportParamValue.Generation (a,b) -> (a,b) |> Ok
            | _ -> "not a Generation" |> Error
        | _ -> $"reportParamName {reportParamName |> UMX.untag} not found" |> Error  


    let getReportType
            (reportParamName:string<reportParamName>)  
            (reportParamSet:reportParamSet) 
            =
        let paramValue = reportParamSet |> getParamValue reportParamName
        match paramValue with
        | Some v -> 
            match v with
            | reportParamValue.ReportType (a,b) -> (a,b) |> Ok
            | _ -> "not a ReportType" |> Error
        | _ -> $"reportParamName {reportParamName |> UMX.untag} not found" |> Error  