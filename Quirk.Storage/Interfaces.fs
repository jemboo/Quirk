namespace Quirk.Storage
open FSharp.UMX
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Core


type IProjectDataStore =
    abstract member GetProject : string<projectName> -> Result<quirkProject, string>
    abstract member SaveProject : quirkProject -> Result<unit, string>
    abstract member GetNextScript : string<projectName> -> Result<string<fnWExt>*quirkScript, string>
    abstract member FinishScript : string<projectName> -> string<scriptName> -> Result<unit, string>
    abstract member SaveScript : quirkScript -> Result<unit, string>
    abstract member GetCfgPlex : string<projectName> -> Result<unit, string>
    abstract member SaveCfgPlex : cfgPlex -> Result<unit, string>

