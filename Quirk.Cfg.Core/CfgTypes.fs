namespace Quirk.Cfg.Core
open FSharp.UMX

[<Measure>] type cfgPlexItemName
[<Measure>] type cfgPlexItemRank
[<Measure>] type workingDirectory
[<Measure>] type projectName




type quirkProgramMode =
    | CfgPlex
    | GenScript
    | RunScript


module quirkProgramMode =

    let toString 
            (quirkProgramMode:quirkProgramMode)
        =
        match quirkProgramMode with
        | CfgPlex -> "CfgPlex"
        | GenScript -> "GenScript"
        | RunScript -> "RunScript"

    let fromString (qrm: string) =
        match qrm.Split() with
        | [| "CfgPlex" |] -> quirkProgramMode.CfgPlex |> Ok 
        | [| "GenScript" |] -> quirkProgramMode.GenScript |> Ok
        | [| "RunScript"; rn |] -> quirkProgramMode.RunScript |> Ok
        | _ -> Error $"{qrm} not handled in QuirkScriptMode.fromString"



    
