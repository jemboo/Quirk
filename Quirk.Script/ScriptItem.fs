namespace Quirk.Script

open FSharp.UMX
open Quirk.Project


type scriptItem = 
    private 
        { 
            quirkRunId: Guid<quirkRunId>
            quirkModelType: quirkModelType
            modelParamSet: modelParamSet
            scriptParamSet: runParamSet
        }


module ScriptItem =

    let create
            (quirkModelType: quirkModelType)
            (scriptParamSet: runParamSet)
            (modelParamSet: modelParamSet)
        =
        let quirkRunId = modelParamSet 
                            |> ModelParamSet.getId
                            |> UMX.cast<modelParamSetId, quirkRunId>
        {
            scriptItem.quirkRunId = quirkRunId
            quirkModelType = quirkModelType
            modelParamSet = modelParamSet
            scriptParamSet = scriptParamSet
        }

    let getQuirkRunId (scriptItem:scriptItem) =
        scriptItem.quirkRunId

    let getQuirkModelType (scriptItem:scriptItem) =
        scriptItem.quirkModelType
        
    let getModelParamSet (scriptItem:scriptItem) =
        scriptItem.modelParamSet

        
    let getScriptParamSet (scriptItem:scriptItem) =
        scriptItem.scriptParamSet


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

