namespace Quirk.Workspace

open System
open FSharp.UMX
open Quirk.Core

type workspace = private {

        wsComponents: Map<string<wsCompKey>, wsComponentData>
    }


module Workspace = 
    let make (compTup: (string<wsCompKey> * wsComponentData) seq)
        =
        {
            wsComponents = compTup |> Map.ofSeq
        }

    let getWsComponents (ws:workspace) = ws.wsComponents
