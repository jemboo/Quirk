namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Run.Core



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
            projectName: string<projectName>
            cfgPlexItems: cfgPlexItem[]
        }


module CfgPlex =
    let create
            (name:string<projectName>)
            (cfgPlexItems:cfgPlexItem[])
         =
        { 
            projectName = name
            cfgPlexItems = cfgPlexItems
        }
        
    let getProjectName (cfgPlex:cfgPlex) =
        cfgPlex.projectName
        
    let getCfgPlexItems (cfgPlex:cfgPlex) =
        cfgPlex.cfgPlexItems

    
    let makeModelParamSets 
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
        |> List.map(fun li -> li |> ModelParamSet.create replicaNumber )


    let createScriptItem
            (quirkProjectType:quirkProjectType)
            (cfgPlex:cfgPlex)
            (replicaNumber: int<replicaNumber>) 
        =
        makeModelParamSets cfgPlex replicaNumber
        |> List.map(QuirkRun.create quirkProjectType)


    let createQuirkRunSet
            (quirkProjectType:quirkProjectType)
            (cfgPlex:cfgPlex)
            (replicaNumber: int<replicaNumber>) 
        =
            let quirkRuns =
                makeModelParamSets cfgPlex replicaNumber
                |> List.map(QuirkRun.create quirkProjectType)
                |> List.toArray

            QuirkRunSet.create quirkRuns

