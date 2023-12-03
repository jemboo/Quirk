namespace Quirk.RunCfg.Core

open FSharp.UMX
open Quirk.Core



type quirkRun = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            quirkRunType: quirkRunType
            replicaNumber: int<replicaNumber>
            cfgPlexItemValues: runParamValue[]
        }


module QuirkRun =

    let create 
            (cfgPlexItemName: string<cfgPlexItemName>) 
            (cfgPlexItemRank: int<cfgPlexItemRank>)
            (cfgPlexItemValues: runParamValue[])
        =
        ()

