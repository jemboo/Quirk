namespace Quirk.Runner
open System.Threading.Tasks
open FSharp.UMX
open Quirk.Core
open Quirk.Cfg.Core
open Quirk.Project
open Quirk.Script
open Quirk.Storage
open System.Threading


module RunGa =

    let doRun
            (rootDir:string<folderPath>)
            (projectDataStore:IProjectDataStore)
            (useParallel:bool<useParallel>)
            (genStart:int<generation>)
            (genEnd:int<generation>)
            (quirkRun:quirkRun)
        =
        result {
            return ()
        }
