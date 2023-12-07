namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core
open Quirk.Run.Core


type quirkRun = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            quirkRunType: quirkRunType
            replicaNumber: int<replicaNumber>
            cfgPlexItemValues: cfgModelParamValue[]
        }


module QuirkRun =

    let create 
            (cfgPlexItemRank: int<cfgPlexItemRank>)
            (cfgPlexItemValues: cfgModelParamValue[])
        =
        ()

