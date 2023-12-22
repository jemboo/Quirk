namespace Quirk.Script
open FSharp.UMX
open Quirk.Core


type scriptItem =
    | Run of scriptItemRun
    | Report of scriptItemReport


module ScriptItem =

    let yab = ()
    //let procScriptItem
    //        (projectFolderPath:string)
    //        (up:bool<useParallel>)
    //        (workspaceFileStoreF: string -> IWorkspaceStore)
    //        (scriptItem:scriptItem)
    //    =
    //        match scriptItem with
    //        | Run runCfg -> 
    //             runCfg |> RunCfg.procRunCfg projectFolderPath up workspaceFileStoreF

    //        | Report reportCfg -> 
    //             reportCfg |> ReportCfg.procReportCfg projectFolderPath up workspaceFileStoreF

