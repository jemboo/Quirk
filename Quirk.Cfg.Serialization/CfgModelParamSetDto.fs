namespace Quirk.Cfg.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Serialization
open Quirk.Cfg.Core
   

type cfgModelParamSetDto =
        { 
            runParamValues: string[][]
        }
    
 module CfgModelParamSetDto =

    let toDto (runParamSet:cfgModelParamSet) : cfgModelParamSetDto 
        =
        {
            runParamValues =
              runParamSet 
                    |> CfgModelParamSet.getRunParamValueMap
                    |> Map.toArray
                    |> Array.map(snd >> CfgModelParamValue.toArrayOfStrings)
        }


    let toJson (runParamSet:cfgModelParamSet) =
        runParamSet |> toDto |> Json.serialize

    
    let fromDto (runParamSetDto:cfgModelParamSetDto) = 
        let _remap (n:string, a) =
            result {
                let! rpv = a |> CfgModelParamValue.fromArrayOfStrings
                return (n |> UMX.tag<cfgModelParamName>, rpv)
            }


        result {

            let! runParamValues =
                runParamSetDto.runParamValues
                |> Array.toList
                |> List.map(CfgModelParamValue.fromArrayOfStrings)
                |> Result.sequence

            return CfgModelParamSet.create runParamValues
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgModelParamSetDto> cereal
            return! fromDto dto
        }
            
      