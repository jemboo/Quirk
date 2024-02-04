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
        =
        result {
            let! order = wsParams |> WsParamsAttrs.getOrder ShcWsParamKeys.order
            let! sortableSetCfgType = wsParams |> WsParamsAttrs.getSortableSetCfgType ShcWsParamKeys.sortableSetCfgType
            let sortableSetCfg =
                    SortableSetCfg.make sortableSetCfgType order None
            let! sortableSet = sortableSetCfg |> SortableSetCfg.makeSortableSet

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
            let! res = initWorkspace wsParams
            let! genStart = wsParams |> WsParamsAttrs.getGeneration ShcWsParamKeys.generationStart
            let! quirkWlId = wsParams |> WsParamsAttrs.getQuirkWorldLineId ShcWsParamKeys.quirkWorldLineId
            let wsCompName = "WsParams" |> UMX.tag<wsCompKey>
            let wsComponentArgs = WsComponentArgs.create rootDir projectName quirkWlId wsCompName genStart
            let wsComponentData = wsParams |> wsComponentData.WsParams
            let wsComponentId = WsComponentTypeShc.getWsComponentID quirkWlId genStart wsCompName
            let wsComponent = WsComponent.load wsComponentId wsCompName wsComponentType.WsParams wsComponentData
            let! res = projectDataStore.SaveWsComponentShc wsComponentArgs wsComponent
            return ()
        }