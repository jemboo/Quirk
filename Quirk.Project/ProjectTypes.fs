﻿namespace Quirk.Project


[<Measure>] type quirkWorldLineId
[<Measure>] type projectName


[<Measure>] type replicaNumber
[<Measure>] type modelParamName

[<Measure>] type modelParamSetId
[<Measure>] type reportName
[<Measure>] type reportParamName
[<Measure>] type reportParamSetName


[<Measure>] type workspaceId
[<Measure>] type workspaceComponentId


type workspaceComponentType =
    | WorkspaceDescription = 0
    | SortableSet = 10
    | SorterSet = 20
    | SorterSetAncestry = 21
    | SorterSetConcatMap = 22
    | SorterSetEval = 23
    | SorterSetMutator = 24
    | SorterSetParentMap = 25
    | SorterSpeedBinSet = 30
    | SorterSetPruner = 40
    | WorkspaceParams = 50


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
        | _ -> $"{qrt} not handled in QuirkProjectType.fromString" |> Error


