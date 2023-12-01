namespace Quirk.Core

open Microsoft.FSharp.Core
open System
open System.Security.Cryptography

module StringUtil =
    let parseInt (s: string) : Result<int, string> =
        let mutable result = 0
        if Int32.TryParse(s, &result) then
            Ok result
        else
            Error $"Failed to parse {s} as an int32"


    let parseUint64 (s: string) : Result<uint64, string> =
        let mutable result = 0uL
        if UInt64.TryParse(s, &result) then
            Ok result
        else
            Error $"Failed to parse {s} as a uint64"


    let parseFloat (s: string) : Result<float, string> =
        let mutable result = 0.0
        if Double.TryParse(s, &result) then
            Ok result
        else
            Error $"Failed to parse {s} as a float"
        
    let nullOption (sV:string option) =
        match sV with
        |Some sv -> sv
        |None -> null


    let toCsvLine<'a> 
            (strFormat:'a->string) 
            (lineData:'a seq)
        =
        lineData 
        |> Seq.fold 
                (fun st t ->
                    let cv = strFormat t 
                    if (st = "") then cv else $"{st}\t{cv}" 
                )
                ""