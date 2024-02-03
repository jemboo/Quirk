namespace Quirk.Project

open FSharp.UMX
open Quirk.Cfg
open Quirk.Sorting
open Quirk.SortingResults
open Quirk.Core


[<Measure>] type quirkWorldLineId
[<Measure>] type projectName


[<Measure>] type replicaNumber
[<Measure>] type modelParamName

[<Measure>] type modelParamSetId
[<Measure>] type reportType
[<Measure>] type reportParamName
[<Measure>] type reportParamSetName



type wsComponentType =
    | WsDescription = 0
    | SortableSet = 10
    | SorterSet = 20
    | SorterSetAncestry = 21
    | SorterSetConcatMap = 22
    | SorterSetEval = 23
    | SorterSetMutator = 24
    | SorterSetParentMap = 25
    | SorterSpeedBinSet = 30
    | SorterSetPruner = 40
    | WsParams = 50


type quirkModelType =
    | Shc
    | Ga


module QuirkModelType =

    let toString 
            (quirkProjectType:quirkModelType)
        =
        match quirkProjectType with
        | Shc -> "Shc"
        | Ga -> "Ga"


    let fromString 
            (qrt:string)
        =
        match qrt with
        | "Shc" -> quirkModelType.Shc |> Ok
        | "Ga" -> quirkModelType.Ga |> Ok
        | _ -> $"{qrt} not handled in QuirkProjectType.fromString (*62)" |> Error


type modelAlpha =
   private 
     { 
        order: int<order>
        sortableSetCfgType: sortableSetCfgType
        switchCount: int<switchCount>
        sorterEvalMode: sorterEvalMode
     }


module ModelAlpha =

    let load 
            (order: int<order>)
            (sortableSetCfgType: sortableSetCfgType) 
            (switchCount: int<switchCount>) 
            (sorterEvalMode: sorterEvalMode) =
        {
            order = order
            sortableSetCfgType=sortableSetCfgType
            switchCount=switchCount
            sorterEvalMode=sorterEvalMode
        }

    let getOrder (modelAlpha:modelAlpha) =
        modelAlpha.order

    let getSortableSetCfgType (modelAlpha:modelAlpha) =
        modelAlpha.sortableSetCfgType

    let getSwitchCount (modelAlpha:modelAlpha) =
        modelAlpha.switchCount

    let getSorterEvalMode (modelAlpha:modelAlpha) =
        modelAlpha.sorterEvalMode
  

  

type quirkModelAlphaDto =
        {
            order:int
            sortableSetCfgType: string
            switchCount: int
            sorterEvalMode: string
        }
    

 module QuirkModelAlphaDto =

    let toDto 
            (modelAlpha:modelAlpha) : quirkModelAlphaDto 
        =
        {
            quirkModelAlphaDto.order = 
                modelAlpha
                |> ModelAlpha.getOrder 
                |> UMX.untag

            quirkModelAlphaDto.sortableSetCfgType =
                modelAlpha 
                |> ModelAlpha.getSortableSetCfgType 
                |> SortableSetCfgType.toString

            quirkModelAlphaDto.switchCount =
                modelAlpha 
                |> ModelAlpha.getSwitchCount 
                |> UMX.untag

            quirkModelAlphaDto.sorterEvalMode =
                modelAlpha 
                |> ModelAlpha.getSorterEvalMode 
                |> SorterEvalMode.toString
        }

    let toJson (modelAlpha:modelAlpha) =
        modelAlpha |> toDto |> Json.serialize

    
    let fromDto (quirkModelAlphaDto:quirkModelAlphaDto) = 

        result {

            let order =
                 quirkModelAlphaDto.order
                 |> UMX.tag<order>

            let! sortableSetCfgType =
                 quirkModelAlphaDto.sortableSetCfgType
                 |> SortableSetCfgType.fromString

            let switchCount =
                quirkModelAlphaDto.switchCount
                |> UMX.tag<switchCount>

            let! sorterEvalMode =
                quirkModelAlphaDto.sorterEvalMode
                |> SorterEvalMode.fromString


            return ModelAlpha.load
                        order
                        sortableSetCfgType
                        switchCount
                        sorterEvalMode
        }
       
    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<quirkModelAlphaDto> cereal
            return! fromDto dto
        }
            




