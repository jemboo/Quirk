namespace Quirk.Storage
open FSharp.UMX
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Core


type IProjectDataStore =
    abstract member GetProject : string -> string<projectName> -> Result<quirkProject, string>
    abstract member GetAllProjects : string -> Result<quirkProject[], string>
    abstract member SaveProject : string -> quirkProject -> Result<unit, string>
    abstract member GetNextScript : string -> string<projectName> -> Result<string<scriptName>*quirkScript, string>
    abstract member FinishScript : string -> string<projectName> -> string<scriptName> -> Result<unit, string>
    abstract member SaveScript : string -> quirkScript -> Result<unit, string>
    abstract member GetCfgPlex : string -> string<projectName> -> string<cfgPlexName> -> Result<cfgPlex, string>
    abstract member SaveCfgPlex : string -> cfgPlex -> Result<unit, string>

