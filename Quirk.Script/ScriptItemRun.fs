namespace Quirk.Script
open FSharp.UMX
open Quirk.Core
open Quirk.Project


type scriptItemRunShc = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            quirkModelParamSet: modelParamSet
            scriptParamSet: scriptParamSet
        }


module ScriptItemRunShc =

    let create 
            (runParamValues: runParamValue seq)
        =
        () //RunParamSet.create runParamValues

        
    let getId (scriptItemRunShc:scriptItemRunShc) =
        scriptItemRunShc.quirkRunId
        
    let getQuirkModelParamSet (scriptItemRunShc:scriptItemRunShc) =
        scriptItemRunShc.quirkModelParamSet
        
    let getScriptParamSet (scriptItemRunShc:scriptItemRunShc) =
        scriptItemRunShc.scriptParamSet



type scriptItemRunGa = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            quirkModelParamSet: modelParamSet
            scriptParamSet: scriptParamSet
        }


module ScriptItemRunGa =

    let create 
            (runParamValues: runParamValue seq)
        =
        ()
        
    let getId (scriptItemRunGa:scriptItemRunGa) =
        scriptItemRunGa.quirkRunId
        
    let getQuirkModelParamSet (scriptItemRunGa:scriptItemRunGa) =
        scriptItemRunGa.quirkModelParamSet
        
    let getScriptParamSet (scriptItemRunGa:scriptItemRunGa) =
        scriptItemRunGa.scriptParamSet



type scriptItemRun =
    | Ga of scriptItemRunGa
    | Shc of scriptItemRunShc


module ScriptItemRun =

    let getId (scriptItemRun:scriptItemRun) =
        match scriptItemRun with
        | scriptItemRun.Ga ga -> ga |> ScriptItemRunGa.getId
        | scriptItemRun.Shc shc -> shc |> ScriptItemRunShc.getId
        

    let getQuirkModelParamSet (scriptItemRun:scriptItemRun) =
        match scriptItemRun with
        | scriptItemRun.Ga ga -> ga |> ScriptItemRunGa.getQuirkModelParamSet
        | scriptItemRun.Shc shc -> shc |> ScriptItemRunShc.getQuirkModelParamSet
        

    let getScriptParamSet (scriptItemRun:scriptItemRun) =
        match scriptItemRun with
        | scriptItemRun.Ga ga -> ga |> ScriptItemRunGa.getScriptParamSet
        | scriptItemRun.Shc shc -> shc |> ScriptItemRunShc.getScriptParamSet



    //let procRunCfg 
    //        (projectFolderPath:string)
    //        (up:bool<useParallel>)
    //        (projectFileStore: IProjectDataStore)
    //        (scriptItemRun:scriptItemRun)
    //    =
    //        match scriptItemRun with
    //        | Shc shc -> ()
    //                //shc |> ShcRunCfg.procShcRunCfg projectFolderPath up workspaceFileStoreF

    //        | Ga ga -> ()
    //               // ga |> GaRunCfg.procGaRunCfg projectFolderPath up workspaceFileStoreF

