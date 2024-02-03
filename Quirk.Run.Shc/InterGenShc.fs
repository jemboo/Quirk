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



module InterGenShc =

    let getWsParamsFromSimParamSet
            (rootDir:string<folderPath>)
            (projectDataStore:IProjectDataStore)
            (wsParams:wsParams) =
        ()

    let procWl
            (rootDir:string<folderPath>)
            (projectName: string<projectName>) 
            (projectDataStore:IProjectDataStore)
            (wsParams:wsParams)
        =
        result {
            let! genStart = wsParams |> WsParamsAttrs.getGeneration ShcWsParamKeys.generationStart
            let! quirkWlId = wsParams |> WsParamsAttrs.getQuirkWorldLineId ShcWsParamKeys.quirkWorldLineId
            let wsCompName = "WsParams" |> UMX.tag<wsComponentName>
            let wsComponentArgs = WsComponentArgs.create rootDir projectName quirkWlId wsCompName genStart
            let wsComponentData = wsParams |> wsComponentData.WsParams
            let wsComponentId = WsComponentTypeShc.getWsComponentID quirkWlId genStart wsCompName
            let wsComponent = WsComponent.load wsComponentId wsCompName wsComponentType.WsParams wsComponentData
            let! res = projectDataStore.SaveWsComponentShc wsComponentArgs wsComponent
            return ()
        }