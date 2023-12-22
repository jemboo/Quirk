namespace Quirk.Script
open FSharp.UMX
open Quirk.Project



type quirkScript = 
    private 
        {
            scriptName: string<scriptName>;
            projectFolder:string<projectName>
            items: scriptItem[]
        }

module QuirkScript =

    let yab = ()

