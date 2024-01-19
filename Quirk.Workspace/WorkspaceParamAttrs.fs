namespace Quirk.Workspace
open System
open FSharp.UMX
open Quirk.Core
open Quirk.Serialization
open Quirk.Project
open Quirk.Sorting


module WorkspaceParamsAttrs =

    let getGeneration
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> UMX.tag<generation>
        }
    let setGeneration
            (key:string<workspaceParamsKey>) 
            (value:int<generation>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)

    let incrGeneration
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          let nextGen = Convert.ToInt32(cereal) + 1
          return         
            workspaceParams |> WorkspaceParams.addItem key (nextGen |> string)
        }

    //let generationIsGte
    //        (key:Guid<workspaceParamsKey>) 
    //        (genVal:int<generation>)
    //        (workspaceParams:workspaceParams) 
    //    =

    //    let _fg
    //            (key:Guid<workspaceParamsKey>) 
    //            (fF: generation -> bool)
    //            (workspaceParams:workspaceParams) 
    //        =
    //        workspaceParams |> getGeneration key |> Result.filterF fF

    //    workspaceParams 
    //        |> _fg
    //                key
    //                (fun gen -> 
    //                        (gen |> Generation.value) >= (genVal |> Generation.value))


    //let getGenerationFilter
    //        (key:Guid<workspaceParamsKey>) 
    //        (workspaceParams:workspaceParams) 
    //    =
    //    result {
    //      let! cereal = WorkspaceParams.getItem key workspaceParams
    //      return! cereal |> GenerationFilterDto.fromJson
    //    }
    //let setGenerationFilter
    //        (key:Guid<workspaceParamsKey>) 
    //        (gf:generationFilter)
    //        (workspaceParams:workspaceParams) 
    //    =
    //    workspaceParams |> WorkspaceParams.addItem key (gf |> GenerationFilterDto.toJson)



    let getMutationRate
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToDouble(cereal) |> UMX.tag<mutationRate>
        }
    let setMutationRate
            (key:string<workspaceParamsKey>) 
            (value:int<mutationRate>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)


    let getNoiseFraction
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          let cv = Convert.ToDouble (cereal)
          if cv <> 0 then return cv |> UMX.tag<noiseFraction>|> Some
          else return None
        }
    let setNoiseFraction
            (key:string<workspaceParamsKey>) 
            (value:int<noiseFraction> option) 
            (workspaceParams:workspaceParams) 
        =
        let cereal =
            match value with
            | Some nf -> nf |> UMX.untag |> string
            | None -> 0.0 |> string
        workspaceParams |> WorkspaceParams.addItem key cereal


    let getOrder
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> UMX.tag<order>
        }
    let setOrder
            (key:string<workspaceParamsKey>) 
            (value:int<order>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)


    let getRngGen
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! cereal |> RngGenDto.fromJson
        }
    let setRngGen
            (key:string<workspaceParamsKey>) 
            (value:rngGen)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> RngGenDto.toJson)

    let updateRngGen
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! curRngGen = workspaceParams |> getRngGen key
          let nextRngGen = curRngGen |> Rando.nextRngGen
          return setRngGen key nextRngGen workspaceParams
        }


    let getQuirkWorldLineId
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return cereal |> Guid |> UMX.tag<quirkWorldLineId>
        }
    let setQuirkWorldLineId
            (key:string<workspaceParamsKey>) 
            (value:Guid<quirkWorldLineId>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)


    //let getSortableSetCfgType
    //        (key:string<workspaceParamsKey>) 
    //        (workspaceParams:workspaceParams) 
    //    =
    //    result {
    //      let! cereal = WorkspaceParams.getItem key workspaceParams
    //      return! cereal |> SortableSetCfgType.fromString
    //    }
    //let setSortableSetCfgType
    //        (key:string<workspaceParamsKey>) 
    //        (sortableSetCfgType:sortableSetCfgType)
    //        (workspaceParams:workspaceParams) 
    //    =
    //    workspaceParams |> WorkspaceParams.addItem key (sortableSetCfgType |> string)


    let getSortableSetId
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return cereal |> Guid |> UMX.tag<sortableSetId>
        }
    let setSortableSetId
            (key:string<workspaceParamsKey>) 
            (value:Guid<sortableSetId>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)


    let getSorterCount
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> UMX.tag<sorterCount>
        }
    let setSorterCount
            (key:string<workspaceParamsKey>) 
            (value:int<sorterCount>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)


    let getSorterCountOption
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          if cereal = "None" then
             return None
          else return Convert.ToInt32(cereal) |> UMX.tag<sorterCount> |> Some
        }
    let setSorterCountOption
            (key:string<workspaceParamsKey>) 
            (value:int<sorterCount> option)
            (workspaceParams:workspaceParams) 
        =
        match value with
        | Some sc ->
            workspaceParams |> WorkspaceParams.addItem key (sc |> UMX.untag |> string)
        | None -> 
            workspaceParams |> WorkspaceParams.addItem key "None"


    let getSorterEvalMode
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! Json.deserialize<sorterEvalMode>(cereal)
        }
    let setSorterEvalMode
            (key:string<workspaceParamsKey>) 
            (value:sorterEvalMode)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (Json.serialize(value))


    let getSorterSetPruneMethod
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! cereal |> SorterSetPruneMethod.fromReport
        }
    let setSorterSetPruneMethod
            (key:string<workspaceParamsKey>) 
            (value:sorterSetPruneMethod)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> SorterSetPruneMethod.toReport )


    let getStageCount
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> UMX.tag<stageCount>
        }
    let setStageCount
            (key:string<workspaceParamsKey>) 
            (value:int<stageCount>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)


    let getStageWeight
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToDouble(cereal) |> UMX.tag<stageWeight>
        }
    let setStageWeight
            (key:string<workspaceParamsKey>) 
            (value:int<stageWeight>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)


    let getSwitchCount
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> UMX.tag<switchCount>
        }
    let setSwitchCount
            (key:string<workspaceParamsKey>) 
            (value:int<switchCount>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)


    let getSwitchGenMode
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! Json.deserialize<switchGenMode>(cereal)
        }
    let setSwitchGenMode
            (key:string<workspaceParamsKey>) 
            (value:switchGenMode)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (Json.serialize(value))


    let getUseParallel
            (key:string<workspaceParamsKey>) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToBoolean (cereal) |> UMX.tag<useParallel>
        }
    let setUseParallel
            (key:string<workspaceParamsKey>) 
            (value:bool<useParallel>)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UMX.untag |> string)






