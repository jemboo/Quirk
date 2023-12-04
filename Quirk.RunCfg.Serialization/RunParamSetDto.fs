namespace Quirk.RunCfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Serialization
open Quirk.RunCfg.Core
   

type runParamSetDto =
        { 
            runParamValues: string[][]
        }
    
 module RunParamSetDto =

    let toDto (runParamSet:runParamSet) : runParamSetDto 
        =
        {
            runParamValues =
              runParamSet 
                    |> RunParamSet.getRunParamValueMap
                    |> Map.toArray
                    |> Array.map(snd >> RunParamValue.toArrayOfStrings)
        }


    let toJson (runParamSet:runParamSet) =
        runParamSet |> toDto |> Json.serialize

    
    let fromDto (runParamSetDto:runParamSetDto) = 
        let _remap (n:string, a) =
            result {
                let! rpv = a |> RunParamValue.fromArrayOfStrings
                return (n |> UMX.tag<runParamName>, rpv)
            }


        result {

            let! runParamValues =
                runParamSetDto.runParamValues
                |> Array.toList
                |> List.map(RunParamValue.fromArrayOfStrings)
                |> Result.sequence

            return RunParamSet.create runParamValues
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<runParamSetDto> cereal
            return! fromDto dto
        }
            
      