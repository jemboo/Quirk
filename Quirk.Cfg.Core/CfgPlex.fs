namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core



type cfgPlexItem = 
    private 
        { 
            cfgPlexItemName: string<cfgPlexItemName>
            cfgPlexItemRank: int<cfgPlexItemRank>
            cfgPlexItemValues: cfgModelParamValue[]
        }


module CfgPlexItem =

    let create
            (cfgPlexItemName: string<cfgPlexItemName>)
            (cfgPlexItemRank: int<cfgPlexItemRank>)
            (cfgPlexItemValues: cfgModelParamValue[])
        =
        {
            cfgPlexItem.cfgPlexItemName = cfgPlexItemName;
            cfgPlexItem.cfgPlexItemRank = cfgPlexItemRank;
            cfgPlexItem.cfgPlexItemValues = cfgPlexItemValues;
        }


    let getName (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.cfgPlexItemName

    let getRank (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.cfgPlexItemRank
        
    let getCfgPlexItemValues (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.cfgPlexItemValues
    


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


    let makeRunParamSets 
            (cfgPlex: cfgPlex)
            (replicaNumber: int<replicaNumber>) 
        =
        
        let _enumerateModelParamSetItems
                (cfgPlexItems: cfgPlexItem[])
            =
            let listList =
                    cfgPlexItems
                    |> Array.sortBy(fun it -> it.cfgPlexItemRank |> UMX.untag)
                    |> Array.map(fun it -> it.cfgPlexItemValues |> Array.toList)
                    |> Array.toList
            CollectionOps.crossProduct listList

        _enumerateModelParamSetItems cfgPlex.cfgPlexItems
        |> List.map(fun li -> li |> CfgModelParamSet.create replicaNumber )
