namespace Quirk.Storage
open FSharp.UMX
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Core
open System.Threading.Tasks


type IProjectDataStore =
    abstract member GetProject : string<folderPath> -> string<projectName> -> Result<quirkProject, string>
    abstract member GetAllProjects : string<folderPath> -> Result<quirkProject[], string>
    abstract member SaveProject : string<folderPath> -> quirkProject -> Result<unit, string>
    abstract member GetNextScript : string<folderPath> -> string<projectName> -> Result<string<scriptName>*quirkScript, string>
    abstract member FinishScript : string<folderPath> -> string<projectName> -> string<scriptName> -> Result<unit, string>
    abstract member SaveScript : string<folderPath> -> quirkScript -> Result<unit, string>
    abstract member GetCfgPlex : string<folderPath> -> string<projectName> -> string<cfgPlexName> -> Result<cfgPlex, string>
    abstract member SaveCfgPlex : string<folderPath> -> cfgPlex -> Result<unit, string>
    abstract member GetProjectAsync : string<folderPath> -> string<projectName> -> Task<Result<quirkProject, string>>
    abstract member GetAllProjectsAsync : string<folderPath> -> Task<Result<quirkProject[], string>>
    abstract member SaveProjectAsync : string<folderPath> -> quirkProject -> Task<Result<unit, string>>
    abstract member GetNextScriptAsync : string<folderPath> -> string<projectName> -> Task<Result<string<scriptName>*quirkScript, string>>
    abstract member FinishScriptAsync : string<folderPath> -> string<projectName> -> string<scriptName> -> Task<Result<unit, string>>
    abstract member SaveScriptAsync : string<folderPath> -> quirkScript -> Task<Result<unit, string>>
    abstract member GetCfgPlexAsync : string<folderPath> -> string<projectName> -> string<cfgPlexName> -> Task<Result<cfgPlex, string>>
    abstract member SaveCfgPlexAsync : string<folderPath> -> cfgPlex -> Task<Result<unit, string>>



type ITest =
    abstract member GetResult : float -> float -> float
    abstract member GetResultAsync : float -> float -> Task<float>


type TestImpl () =
    
   member this.getRes (a:float) (b:float)
        =
    a / b

   interface ITest with
        member this.GetResult (a:float) (b:float) = 
                this.getRes a b

        member this.GetResultAsync a b =
            Task.Run(fun () -> this.getRes a b)