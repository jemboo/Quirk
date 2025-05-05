namespace Quirk.Run.Shc
open System.Threading.Tasks
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage
open System.Threading
open Quirk.Workspace
open Quirk.Run.Core
open Quirk.Cfg



module InterGenShc =

    let getWsParamsFromSimParamSet
            (rootDir:string<folderPath>)
            (projectDataStore:IProjectDataStore)
            (wsParams:wsParams) =
        ()

    let initWorkspace
                (wsParams:wsParams)
                (projectDataStore:IProjectDataStore)
                (wsComponentStorageArgs:wsComponentStorageArgs)
        =
        result {
            let! genStart = wsParams |> WsParamsAttrs.getGeneration ShcWsParamKeys.generationStart
            let! order = wsParams |> WsParamsAttrs.getOrder ShcWsParamKeys.order
            let! sortableSetCfgType = wsParams |> WsParamsAttrs.getSortableSetCfgType ShcWsParamKeys.sortableSetCfgType
            let sortableSetCfg = SortableSetCfg.make sortableSetCfgType order None
            let! sortableSet = sortableSetCfg |> SortableSetCfg.makeSortableSet
            let wscdSortableSet = sortableSet |> wsComponentData.SortableSet
            let! worldLineId = wsParams |> WsParamsAttrs.getQuirkWorldLineId ShcWsParamKeys.quirkWorldLineId
            let wscIdSortableSet = WsComponentTypeShc.getWsComponentID worldLineId genStart ShcWsCompKeys.sortableSet
            let wscSortableSet = WsComponent.make wscIdSortableSet ShcWsCompKeys.sortableSet wsComponentType.SortableSet wscdSortableSet
            let! res = projectDataStore.SaveWsComponentShc wsComponentStorageArgs wscSortableSet


            //let sorterSetCfg = 
            //        new sorterSetRndCfg(
            //                    wnSorterSetParent,
            //                    order,
            //                    switchGenMode,
            //                    switchCount,
            //                    sorterCount)




            //let rngGenProvider = 
            //            RngGenProvider.make this.rngGen

            //let! wsCompSorterSet = 
            //          ssCfg |> SorterSetRndCfg.makeSorterSet rngGenProvider

            return ()
        }


    let procWl
            (rootDir:string<folderPath>)
            (projectName: string<projectName>) 
            (projectDataStore:IProjectDataStore)
            (wsParams:wsParams)
        =
        result {
            let! genStart = wsParams |> WsParamsAttrs.getGeneration ShcWsParamKeys.generationStart
            let! worldLineId = wsParams |> WsParamsAttrs.getQuirkWorldLineId ShcWsParamKeys.quirkWorldLineId
            let wsComponentStorageArgs = WsComponentStorageArgs.create rootDir projectName worldLineId genStart ShcWsCompKeys.wsParams
            let! res = initWorkspace wsParams projectDataStore wsComponentStorageArgs

            let wscdParams = wsParams |> wsComponentData.WsParams
            let wscIdParams = WsComponentTypeShc.getWsComponentID worldLineId genStart ShcWsCompKeys.wsParams
            let wscParams = WsComponent.make wscIdParams ShcWsCompKeys.wsParams wsComponentType.WsParams wscdParams
            let! res = projectDataStore.SaveWsComponentShc wsComponentStorageArgs wscParams
            return ()
        }