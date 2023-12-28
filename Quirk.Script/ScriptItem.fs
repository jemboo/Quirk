namespace Quirk.Script

open FSharp.UMX
open Quirk.Project


type scriptItem = 
    private 
        { 
            quirkWorldLineId: Guid<quirkWorldLineId>
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
        let quirkWorldLineId = modelParamSet 
                            |> ModelParamSet.getId
                            |> UMX.cast<modelParamSetId, quirkWorldLineId>
        {
            scriptItem.quirkWorldLineId = quirkWorldLineId
            quirkModelType = quirkModelType
            modelParamSet = modelParamSet
            scriptParamSet = scriptParamSet
        }

    let getQuirkWorldlineId (scriptItem:scriptItem) =
        scriptItem.quirkWorldLineId

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

