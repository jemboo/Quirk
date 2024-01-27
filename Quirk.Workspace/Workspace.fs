namespace Quirk.Workspace

open System
open FSharp.UMX
open Quirk.Core

type workspace = private {

        wsComponents: Map<string<wsComponentName>, wsComponentData>
    }


module Workspace = 

    let getWsComponents (ws:workspace) = ws.wsComponents
