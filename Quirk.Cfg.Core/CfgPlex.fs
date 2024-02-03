namespace Quirk.Cfg.Core

open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Run.Core



[<Measure>] type cfgPlexName

type cfgPlexItem = 
    private 
        { 
            cfgPlexItemName: string<cfgPlexItemName>
            cfgPlexItemRank: int<cfgPlexItemRank>
            cfgPlexItemValues: modelParamValue[]
        }


module CfgPlexItem =

    let create
            (cfgPlexItemName: string<cfgPlexItemName>)
            (cfgPlexItemRank: int<cfgPlexItemRank>)
            (cfgPlexItemValues: modelParamValue[])
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
            cfgPlexName: string<cfgPlexName>
            projectName: string<projectName>
            cfgPlexItems: cfgPlexItem[]
        }


module CfgPlex =

    let create
            (cfgPlexName:string<cfgPlexName>)
            (projectName:string<projectName>)
            (cfgPlexItems:cfgPlexItem[])
         =
        { 
            cfgPlexName = cfgPlexName
            projectName = projectName
            cfgPlexItems = cfgPlexItems
        }
        
        
    let getCfgPlexName (cfgPlex:cfgPlex) =
        cfgPlex.cfgPlexName

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
            (quirkModelType:quirkModelType)
            (runParamSet: runParamSet)
            (cfgPlex:cfgPlex)
            (replicaNumber: int<replicaNumber>) 
        =
        makeModelParamSets cfgPlex replicaNumber
        |> List.map(QuirkRun.create quirkModelType runParamSet)


    let createQuirkRuns
            (quirkModelType:quirkModelType)
            (runParamSet: runParamSet)
            (cfgPlex:cfgPlex)
            (maxIndex:int)
        =
        let replicaNums = 
            Seq.initInfinite UMX.tag<replicaNumber>
        let _quirkRuns rn =
            makeModelParamSets cfgPlex rn
            |> List.map(QuirkRun.create quirkModelType runParamSet)
            |> List.toSeq

        replicaNums 
        |> Seq.map _quirkRuns 
        |> Seq.concat
        |> Seq.take (maxIndex + 1)


    let createSelectedQuirkRunSets
            (quirkModelType:quirkModelType)
            (runParamSet: runParamSet)
            (selectedIndexes: int[])
            (maxRunsetSize:int)
            (cfgPlex:cfgPlex)
        =
        let maxIndex = selectedIndexes |> Array.max
        createQuirkRuns
            quirkModelType
            runParamSet
            cfgPlex
            maxIndex
        |> CollectionProps.filterByIndexes selectedIndexes
        |> Seq.chunkBySize maxRunsetSize
        |> Seq.map(QuirkRunSet.create cfgPlex.projectName)


    //let createQuirkRunSet
    //        (quirkModelType:quirkModelType)
    //        (runParamSet: runParamSet)
    //        (cfgPlex:cfgPlex)
    //        (replicaNumber: int<replicaNumber>) 
    //    =
    //        let quirkRuns =
    //            makeModelParamSets cfgPlex replicaNumber
    //            |> List.map(QuirkRun.create quirkModelType runParamSet)
    //            |> List.toArray

    //        QuirkRunSet.create quirkRuns

