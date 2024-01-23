namespace Quirk.Project

open FSharp.UMX
open Quirk.Core


[<Measure>] type simParamName
[<Measure>] type simParamSetName

type simParamValue =
    | Generation of string<simParamName> * int<generation>


module SimParamValue =

    let makeGenerationStart (genStart: int<generation>) =
        ("generationStart" |> UMX.tag<simParamName>, genStart) |> simParamValue.Generation

    let makeGenerationEnd (genEnd: int<generation>) =
        ("generationEnd" |> UMX.tag<simParamName>, genEnd) |> simParamValue.Generation

    let makeReportIntervalShort (genShort: int<generation>) =
        ("reportIntervalShort" |> UMX.tag<simParamName>, genShort) |> simParamValue.Generation

    let makeReportIntervalLong (genLong: int<generation>) =
        ("reportIntervalLong" |> UMX.tag<simParamName>, genLong) |> simParamValue.Generation


    let getSimParamName (simParamValue: simParamValue) =
        match simParamValue with
        | Generation (n, o) -> n


    let toArrayOfStrings (simParamValue: simParamValue) =
        match simParamValue with
        | Generation (n, o) ->
                [|
                    "Generation";
                    n |> UMX.untag
                    o |> UMX.untag |> string
                |]


    let fromArrayOfStrings (lst: string array) : Result<simParamValue, string> =
        match lst with
        | [|"Generation"; n; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                let rpName = n |> UMX.tag<simParamName>
                return (rpName, ov |> UMX.tag<generation>)
                        |> simParamValue.Generation
            }

        | uhv -> $"not handled in SimParamValue.fromArrayOfStrings %A{uhv}" |> Error
            

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


    let getSimParamValue 
            (simParamName:string<simParamName>) 
            (simParamSet:simParamSet) 
        =
        simParamSet.valueMap.TryFind simParamName

    let getGeneration
            (simParamName:string<simParamName>)  
            (simParamSet:simParamSet) 
            =
        let simParamValue = simParamSet |> getSimParamValue simParamName
        match simParamValue with
        | Some v -> 
            match v with
            | simParamValue.Generation (a,b) -> (a,b) |> Ok
            | _ -> "not a GenerationStart" |> Error
        | _ -> $"simParamName {simParamName |> UMX.untag} not found" |> Error  