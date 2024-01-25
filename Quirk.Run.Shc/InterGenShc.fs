namespace Quirk.Run.Shc
open System.Threading.Tasks
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage
open System.Threading
open Quirk.Workspace



module InterGenShc =

    let getWorkspaceParamsFromSimParamSet
            (simParamSet:simParamSet) =
        ()