namespace Quirk.Runner
open FSharp.UMX
open Quirk.Cfg.Core
open Argu


module ScriptDispatcher =

    let dispatchCfgPlex
            (workingDirectory:string<workingDirectory>)
            (projectName:string<projectName>)
        =
        match (projectName |> UMX.untag).Split() with
        | [| "CfgPlex" |] -> () |> Ok 
        | [| "GenScript" |] -> () |> Ok
        | [| "RunScript"; rn |] -> () |> Ok
        | _ -> Error $"{projectName |> UMX.untag} not handled in ScriptDispatcher.dispatchCfgPlex"


    let dispatchGenScript
            (workingDirectory:string<workingDirectory>)
            (projectName:string<projectName>)
            (firstScriptIndex:int)
            (scriptCount:int)
        =
        ()  |> Ok


    let dispatchRunScript
            (workingDirectory:string<workingDirectory>)
            (projectName:string<projectName>)
        =
        ()  |> Ok


    let fromRunMode
            (workingDirectory:string<workingDirectory>)
            (projectName:string<projectName>)
            (firstScriptIndex:int)
            (scriptCount:int)
            (rm:quirkProgramMode) = 
        match rm with
        | CfgPlex -> dispatchCfgPlex workingDirectory projectName
        | GenScript -> dispatchGenScript workingDirectory projectName firstScriptIndex scriptCount
        | RunScript  -> dispatchRunScript workingDirectory projectName



