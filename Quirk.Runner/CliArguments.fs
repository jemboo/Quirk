namespace Quirk.Runner

open Argu


type CliArguments =
    | First_Script_Index of string
    | Log_level of int
    | Project_Name of string
    | Report_File_Name of string
    | Run_Mode of string
    | Script_Count of string
    | Use_Parallel of bool
    | Working_Directory of path:string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | First_Script_Index _ -> "the first script index to generate from the cfgplex"
            | Log_level _ -> "set the log level (0, 1, or 2)"
            | Project_Name _ -> "a subfolder of the working directory"
            | Run_Mode _ -> "specify cfgPlex or genScript or runScript"
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
