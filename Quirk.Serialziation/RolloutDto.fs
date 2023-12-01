namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core


type rolloutDto =
    { rolloutfmt: string
      arrayLength: int
      arrayCount: int
      bitsPerSymbol: int
      base64: string }

module RolloutDto =

    let fromDto 
            (dto: rolloutDto) 
        =
        result {
            let! rft = dto.rolloutfmt |> RolloutFormat.fromString
            let bps = dto.bitsPerSymbol |> UMX.tag<bitsPerSymbol>
            let arrayLen = dto.arrayLength |> UMX.tag<arrayLength>
            let! bites = ByteUtils.fromBase64 dto.base64
            let bitpk = bites |> BitPack.fromBytes bps
            return! bitpk |> Rollout.fromBitPack rft arrayLen
        }

    let fromJson 
            (jstr: string) 
        =
        result {
            let! dto = Json.deserialize<rolloutDto> jstr
            return! fromDto dto
        }

    let toDto
            (rollout: rollout)
         =
         let fmt = 
            rollout 
                |> Rollout.getRolloutFormat
                |> RolloutFormat.toString

         let arrayLen =
             rollout
                |> Rollout.getArrayLength
                |> UMX.untag

         let arrayCount =
             rollout
                |> Rollout.getArrayCount
                |> UMX.untag

         let bitsPerSymbol =
             rollout
                |> Rollout.getBitsPerSymbol
                |> UMX.untag

         result {

            let! byteSeq = rollout |> Rollout.getDataBytes
            let data64 = byteSeq |> ByteUtils.toBase64

            return  { 
                rolloutfmt = fmt
                arrayLength = arrayLen
                arrayCount = arrayCount
                bitsPerSymbol = bitsPerSymbol
                base64 = data64 
            }
          }

    let toJson
        (rollout: rollout)
        =
        rollout
        |> toDto |> Result.ExtractOrThrow
        |> Json.serialize

