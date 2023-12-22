namespace Quirk.Storage
open Quirk.Cfg.Core
open Quirk.Script

type IScriptDataStore =
    abstract member SaveScript : quirkScript -> Result<unit, string>


type ICfgPlexDataStore =
    abstract member SaveCfgPlex : cfgPlex -> Result<unit, string>


type IProjectDataStore =
    abstract member NewProject : string -> Result<bool, string>

