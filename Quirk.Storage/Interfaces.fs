namespace Quirk.Storage
open FSharp.UMX
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Core


type IProjectDataStore =
    abstract member GetProject : string<folderPath> -> string<projectName> -> Result<quirkProject, string>
    abstract member GetAllProjects : string<folderPath> -> Result<quirkProject[], string>
    abstract member SaveProject : string<folderPath> -> quirkProject -> Result<unit, string>
    abstract member GetNextScript : string<folderPath> -> string<projectName> -> Result<string<scriptName>*quirkScript, string>
    abstract member FinishScript : string<folderPath> -> string<projectName> -> string<scriptName> -> Result<unit, string>
    abstract member SaveScript : string<folderPath> -> quirkScript -> Result<unit, string>
    abstract member GetCfgPlex : string<folderPath> -> string<projectName> -> string<cfgPlexName> -> Result<cfgPlex, string>
    abstract member SaveCfgPlex : string<folderPath> -> cfgPlex -> Result<unit, string>

