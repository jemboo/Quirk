namespace Quirk.Storage
open FSharp.UMX
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script


type IProjectDataStore =
    abstract member GetProject : string<projectName> -> Result<bool, string>
    abstract member SaveScript : quirkScript -> Result<unit, string>
    abstract member SaveCfgPlex : cfgPlex -> Result<unit, string>

