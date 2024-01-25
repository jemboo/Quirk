namespace Quirk.Run.Core

open System
open Quirk.Core


type causeId = private CauseId of Guid
module CauseId =
    let value (CauseId v) = v
    let create (v: Guid) = CauseId v
    let empty = 
        [("causeId" :> obj)]
            |> GuidUtils.guidFromObjs 
            |> create


type runId = private RunId of Guid
module RunId =
    let value (RunId v) = v
    let create (v: Guid) = RunId v


type reportName = private ReportName of string
module ReportName =

    let value (ReportName v) = v

    let create (value: string) =
        value |> ReportName




