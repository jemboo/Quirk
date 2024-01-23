namespace Quirk.Workspace
open System
open FSharp.UMX
open Quirk.Core

[<Measure>] type workspaceParamsId

[<Measure>] type workspaceParamsKey

type runType =
    | Sim
    | Report


module RunType =

    let toString 
            (runType:runType)
        =
        match runType with
        | Sim -> "Sim"
        | Report -> "Report"

    let fromString 
            (qrm: string) 
        =
        match qrm.Split() with
        | [| "Sim" |] -> runType.Sim |> Ok 
        | [| "Report" |] -> runType.Report |> Ok
        | _ -> Error $"{qrm} not handled in RunType.fromString"


type workspaceParams =
    private 
        { id: Guid<workspaceParamsId>; 
          data: Map<string<workspaceParamsKey>,string> }

module WorkspaceParams =

    let load 
            (id:Guid<workspaceParamsId>) 
            (data: Map<string<workspaceParamsKey>,string>)
        =
        {
            workspaceParams.id = id;
            data = data;
        }

    let make (data: Map<string<workspaceParamsKey>,string>) =
        let nextId = 
            data |> Map.toArray |> Array.map(fun tup -> tup :> obj)
            |> GuidUtils.guidFromObjs 
            |> UMX.tag<workspaceParamsId>
        load nextId data

    let getHeaders (workspaceParams:workspaceParams) =
        workspaceParams.data.Keys |> Seq.fold (fun cur nv -> $"{cur}\t{nv |> UMX.untag}") ""

    let getValues (workspaceParams:workspaceParams) =
        workspaceParams.data.Values |> Seq.fold (fun cur nv -> $"{cur}\t{nv |> UMX.untag}") ""

    let getId (workspaceParams:workspaceParams) =
        workspaceParams.id

    let getMap (workspaceParams:workspaceParams) =
        workspaceParams.data

    let addItem 
            (key:string<workspaceParamsKey>) 
            (cereal:string) 
            (workspaceParams:workspaceParams) 
        =
        let newMap = workspaceParams.data |> Map.add key cereal
        make newMap

    let addItems
            (kvps:(string<workspaceParamsKey>*string) seq) 
            (workspaceParams:workspaceParams) =
        Seq.fold (fun wp tup -> addItem (fst tup) (snd tup) wp) workspaceParams kvps


    let merge 
            (workspaceParamsA:workspaceParams)
            (workspaceParamsB:workspaceParams)
        =
        workspaceParamsA
        |> addItems  (workspaceParamsB |> getMap |> Map.toSeq)


    let getItem 
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        if workspaceParams.data.ContainsKey(key) then
           workspaceParams.data.[key] |> Ok
        else
            $"the key: {key} was not found (405)" |> Error