namespace Quirk.Workspace
open System
open FSharp.UMX
open Quirk.Core
open Quirk.Serialization
open Quirk.Project
open Quirk.Sorting
open Quirk.SortingResults
open Quirk.Iter
open Quirk.Cfg


module WsParamsAttrs =

    let getGeneration
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToInt32(cereal) |> UMX.tag<generation>
        }
    let setGeneration
            (key:string<wsParamsKey>) 
            (value:int<generation>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let incrGeneration
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          let nextGen = Convert.ToInt32(cereal) + 1
          return         
            wsParams |> WsParams.addItem key (nextGen |> string)
        }

    let generationIsGte
            (key:string<wsParamsKey>) 
            (genVal:int<generation>)
            (wsParams:wsParams) 
        =

        let _fg
                (key:string<wsParamsKey>) 
                (fF: int<generation> -> bool)
                (wsParams:wsParams) 
            =
            wsParams |> getGeneration key |> Result.filterF fF

        wsParams 
            |> _fg
                    key
                    (fun gen -> 
                            (gen |> UMX.untag) >= (genVal |> UMX.untag))


    let getGenerationFilter
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return! cereal |> GenerationFilterDto.fromJson
        }
    let setGenerationFilter
            (key:string<wsParamsKey>) 
            (gf:generationFilter)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (gf |> GenerationFilterDto.toJson)



    let getMutationRate
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToDouble(cereal) |> UMX.tag<mutationRate>
        }
    let setMutationRate
            (key:string<wsParamsKey>) 
            (value:float<mutationRate>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getNoiseFraction
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToDouble(cereal) |> UMX.tag<noiseFraction>
        }
    let setNoiseFraction
            (key:string<wsParamsKey>) 
            (value:float<noiseFraction>) 
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getOrder
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToInt32(cereal) |> UMX.tag<order>
        }
    let setOrder
            (key:string<wsParamsKey>) 
            (value:int<order>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getReproductionRage
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToDouble (cereal) |> UMX.tag<reproductionRate>
        }
    let setReproductionRate
            (key:string<wsParamsKey>) 
            (value:float<reproductionRate>) 
            (wsParams:wsParams) 
        =
        let cereal = value |> UMX.untag |> string
        wsParams |> WsParams.addItem key cereal


    let getReportType
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return cereal |> UMX.tag<reportType>
        }
    let setReportType
            (key:string<wsParamsKey>) 
            (value:string<reportType>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getRngGen
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return! cereal |> RngGenDto.fromJson
        }
    let setRngGen
            (key:string<wsParamsKey>) 
            (value:rngGen)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> RngGenDto.toJson)

    let updateRngGen
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! curRngGen = wsParams |> getRngGen key
          let nextRngGen = curRngGen |> Rando.nextRngGen
          return setRngGen key nextRngGen wsParams
        }


    let getQuirkWorldLineId
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return cereal |> Guid |> UMX.tag<quirkWorldLineId>
        }
    let setQuirkWorldLineId
            (key:string<wsParamsKey>) 
            (value:Guid<quirkWorldLineId>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getSortableSetCfgType
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return! cereal |> SortableSetCfgType.fromString
        }
    let setSortableSetCfgType
            (key:string<wsParamsKey>) 
            (sortableSetCfgType:sortableSetCfgType)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (sortableSetCfgType |> string)


    let getSortableSetId
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return cereal |> Guid |> UMX.tag<sortableSetId>
        }
    let setSortableSetId
            (key:string<wsParamsKey>) 
            (value:Guid<sortableSetId>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getSorterCount
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToInt32(cereal) |> UMX.tag<sorterCount>
        }
    let setSorterCount
            (key:string<wsParamsKey>) 
            (value:int<sorterCount>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getSorterCountOption
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          if cereal = "None" then
             return None
          else return Convert.ToInt32(cereal) |> UMX.tag<sorterCount> |> Some
        }
    let setSorterCountOption
            (key:string<wsParamsKey>) 
            (value:int<sorterCount> option)
            (wsParams:wsParams) 
        =
        match value with
        | Some sc ->
            wsParams |> WsParams.addItem key (sc |> UMX.untag |> string)
        | None -> 
            wsParams |> WsParams.addItem key "None"


    let getSorterEvalMode
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return! Json.deserialize<sorterEvalMode>(cereal)
        }
    let setSorterEvalMode
            (key:string<wsParamsKey>) 
            (value:sorterEvalMode)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (Json.serialize(value))


    let getSorterSetPruneMethod
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return! cereal |> SorterSetPruneMethod.fromReport
        }
    let setSorterSetPruneMethod
            (key:string<wsParamsKey>) 
            (value:sorterSetPruneMethod)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> SorterSetPruneMethod.toReport )


    let getStageCount
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToInt32(cereal) |> UMX.tag<stageCount>
        }
    let setStageCount
            (key:string<wsParamsKey>) 
            (value:int<stageCount>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getStageWeight
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToDouble(cereal) |> UMX.tag<stageWeight>
        }
    let setStageWeight
            (key:string<wsParamsKey>) 
            (value:float<stageWeight>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getSwitchCount
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToInt32(cereal) |> UMX.tag<switchCount>
        }
    let setSwitchCount
            (key:string<wsParamsKey>) 
            (value:int<switchCount>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)


    let getSwitchGenMode
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return! Json.deserialize<switchGenMode>(cereal)
        }
    let setSwitchGenMode
            (key:string<wsParamsKey>) 
            (value:switchGenMode)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (Json.serialize(value))


    let getUseParallel
            (key:string<wsParamsKey>) 
            (wsParams:wsParams) 
        =
        result {
          let! cereal = WsParams.getItem key wsParams
          return Convert.ToBoolean (cereal) |> UMX.tag<useParallel>
        }
    let setUseParallel
            (key:string<wsParamsKey>) 
            (value:bool<useParallel>)
            (wsParams:wsParams) 
        =
        wsParams |> WsParams.addItem key (value |> UMX.untag |> string)






