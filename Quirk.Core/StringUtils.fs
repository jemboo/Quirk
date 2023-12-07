namespace Quirk.Core

open Microsoft.FSharp.Core
open System
open System.Security.Cryptography
open System.Text

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



    let joinSequence<'a> 
            (delim:string) 
            (strFormat:'a->string) 
            (lineData:'a seq)
        =
        let stringB = new StringBuilder()
        stringB.AppendJoin(delim, lineData |> Seq.map(strFormat)) |> ignore
        stringB.ToString()


    let joinGuids
            (delim:string) 
            (lineData:Guid seq)
         =
         joinSequence
            delim
            (fun gu -> gu.ToString())
            lineData


    let joinInts
            (delim:string) 
            (lineData:int seq)
         =
         joinSequence
            delim
            (fun gu -> gu.ToString())
            lineData


    let joinFloats
            (delim:string) 
            (lineData:float seq)
         =
         joinSequence
            delim
            (fun gu -> gu.ToString())
            lineData


    let splitToArray<'a>
            (delim:string)
            (parser:string -> Result<'a,string>)
            (gstr: string) 
            =
        result {
            let guiLst = gstr.Split(delim) |> Array.toList
            let! lstR = guiLst |> List.map(parser) |> Result.sequence
            return lstR |> List.toArray
        }


    let guidArrayFromString 
                (delim:string)
                (gstr: string) 
        =
        splitToArray 
            delim
            GuidUtils.guidFromStringR
            gstr
        //result {
        //    let guiLst = gstr.Split(",") |> Array.toList
        //    let! lstR = 
        //            guiLst 
        //            |> List.map(GuidUtils.guidFromStringR) 
        //            |> Result.sequence
        //    return lstR |> List.toArray
        //}