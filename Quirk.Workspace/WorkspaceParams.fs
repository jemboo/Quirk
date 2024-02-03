namespace Quirk.Workspace
open System
open FSharp.UMX
open Quirk.Core

type wsParams =
    private 
        { id: Guid<wsParamsId>; 
          data: Map<string<wsParamsKey>,string> }

module WsParams =

    let load 
            (id:Guid<wsParamsId>) 
            (data: Map<string<wsParamsKey>,string>)
        =
        {
            wsParams.id = id;
            data = data;
        }

    let make (data: Map<string<wsParamsKey>,string>) =
        let nextId = 
            data |> Map.toArray |> Array.map(fun tup -> tup :> obj)
            |> GuidUtils.guidFromObjs 
            |> UMX.tag<wsParamsId>
        load nextId data

    let getHeaders (wsParams:wsParams) =
        wsParams.data.Keys |> Seq.fold (fun cur nv -> $"{cur}\t{nv |> UMX.untag}") ""

    let getValues (wsParams:wsParams) =
        wsParams.data.Values |> Seq.fold (fun cur nv -> $"{cur}\t{nv |> UMX.untag}") ""

    let getId (wsParams:wsParams) =
        wsParams.id

    let getMap (wsParams:wsParams) =
        wsParams.data

    let addItem 
            (key:string<wsParamsKey>) 
            (cereal:string) 
            (wsParams:wsParams) 
        =
        let newMap = wsParams.data |> Map.add key cereal
        make newMap

    let addItems
            (kvps:(string<wsParamsKey>*string) seq) 
            (wsParams:wsParams) =
        Seq.fold (fun wp tup -> addItem (fst tup) (snd tup) wp) wsParams kvps


    let merge 
            (wsParamsA:wsParams)
            (wsParamsB:wsParams)
        =
        wsParamsA
        |> addItems  (wsParamsB |> getMap |> Map.toSeq)


    let getItem 
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        if wsParams.data.ContainsKey(key) then
           wsParams.data.[key] |> Ok
        else
            $"the key: {key} was not found (*102)" |> Error