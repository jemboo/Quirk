﻿namespace Quirk.Runner

open Argu

type quirkProgramMode =
    | CfgPlex
    | GenSimScript
    | GenReportScript
    | RunScript


module quirkProgramMode =

    let toString 
            (quirkProgramMode:quirkProgramMode)
        =
        match quirkProgramMode with
        | CfgPlex -> "CfgPlex"
        | GenSimScript -> "GenSimScript"
        | GenReportScript -> "GenReportScript"
        | RunScript -> "RunScript"

    let fromString (qrm: string) =
        match qrm.Split() with
        | [| "CfgPlex" |] -> quirkProgramMode.CfgPlex |> Ok 
        | [| "GenSimScript" |] -> quirkProgramMode.GenSimScript |> Ok
        | [| "GenReportScript" |] -> quirkProgramMode.GenReportScript |> Ok
        | [| "RunScript" |] -> quirkProgramMode.RunScript |> Ok
        | _ -> Error $"{qrm} not handled in QuirkScriptMode.fromString"



    
type CliArguments =
    | First_Script_Index of string
    | Log_level of int
    | CfgPlex_Name of string
    | Project_Name of string
    | Report_File_Name of string
    | Program_Mode of string
    | Script_Count of string
    | Use_Parallel of bool
    | Working_Directory of path:string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | First_Script_Index _ -> "the first script index to generate from the cfgplex"
            | Log_level _ -> "set the log level (0, 1, or 2)"
            | CfgPlex_Name _  -> "CfgPlex file name"
            | Project_Name _ -> "a subfolder of the working directory"
            | Program_Mode _ -> "specify cfgPlex, genSimScript, genReportScript or runScript"
            | Report_File_Name _ -> "the report file name"
            | Script_Count _ -> "the number of scripts to generate after the first index"
            | Use_Parallel _ -> "run the sorterEval loop in parallel"
            | Working_Directory _ -> "specify the working directory."




//--first-script-index 0 
//--log-level 1
//--project-name o64\StagePhenoPrune 
//--report-file-name standard 
//--run-mode CfgPlex 
//--script-count 3 
//--use-parallel true
//--working-directory C:\Quirk 
