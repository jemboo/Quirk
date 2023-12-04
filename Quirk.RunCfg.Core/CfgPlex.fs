namespace Quirk.RunCfg.Core

open FSharp.UMX
open Quirk.Core



type cfgPlexItem = 
    private 
        { 
            name: string<cfgPlexItemName>
            cfgPlexItemRank: int<cfgPlexItemRank>
            cfgPlexItemValues: runParamValue[]
        }


module CfgPlexItem =

    let create 
            (cfgPlexItemName: string<cfgPlexItemName>) 
            (cfgPlexItemRank: int<cfgPlexItemRank>)
            (cfgPlexItemValues: runParamValue[])
        =
        {
            cfgPlexItem.name = cfgPlexItemName;
            cfgPlexItem.cfgPlexItemRank = cfgPlexItemRank;
            cfgPlexItem.cfgPlexItemValues = cfgPlexItemValues;
        }

    let getName (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.name
        
    let getRank (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.cfgPlexItemRank
        
    let getCfgPlexItemValues (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.cfgPlexItemValues
    

    let enumerateItems (cfgPlexItems: cfgPlexItem[]) =
        let listList =
                cfgPlexItems
                |> Array.sortBy(fun it -> it.cfgPlexItemRank |> UMX.untag)
                |> Array.map(fun it -> it.cfgPlexItemValues |> Array.toList)
                |> Array.toList
        CollectionOps.crossProduct listList


    let makeRunParamSets 
            (cfgPlexItems: cfgPlexItem[]) 
            (quirkRunType:quirkRunType)
            (replicaNumber: int<replicaNumber>) 
        =
        enumerateItems cfgPlexItems
        |> List.map(fun li -> li |> RunParamSet.create2 replicaNumber quirkRunType )



type cfgPlex =
     private 
        { 
            name: string<cfgPlexName>
            cfgPlexItems: cfgPlexItem[]
        }


module CfgPlex =
    let create
            (name:string<cfgPlexName>)
            (cfgPlexItems:cfgPlexItem[])
         =
        { 
            name = name
            cfgPlexItems = cfgPlexItems
        }
        
    let getName (cfgPlex:cfgPlex) =
        cfgPlex.name
        
    let getCfgPlexItems (cfgPlex:cfgPlex) =
        cfgPlex.cfgPlexItems
