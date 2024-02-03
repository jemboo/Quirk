namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core


module RngTypeDto =

    let toString (rngt: rngType) =
        match rngt with
        | rngType.Lcg -> nameof rngType.Lcg
        | rngType.Net -> nameof rngType.Net
        | _ -> failwith (sprintf "no match for RngType: %A" rngt)

    let fromString str =
        match str with
        | nameof rngType.Lcg -> rngType.Lcg |> Ok
        | nameof rngType.Net -> rngType.Net |> Ok
        | _ -> Error(sprintf "no match for RngType: %s (*80)" str)




type rngGenDto = { rngType: string; seed: uint64 }

module RngGenDto =

    let fromDto (dto: rngGenDto) =
        result {
            let! typ = RngTypeDto.fromString dto.rngType
            let rs = dto.seed |> UMX.tag<randomSeed>
            return RngGen.create typ rs
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<rngGenDto> jstr
            return! fromDto dto
        }

    let toDto (rngGen: rngGen) =
        { rngType = rngGen |> RngGen.getType |> RngType.toString
          seed =  rngGen |> RngGen.getSeed |> UMX.untag }

    let toJson (rngGen: rngGen) = rngGen |> toDto |> Json.serialize





type rngGenProviderDto = { id: Guid; rngGenDto: string }

module RngGenProviderDto =

    let fromDto (dto: rngGenProviderDto) =
        result {
            let id = dto.id |> RngGenProviderId.create
            let! rngGen = dto.rngGenDto |> RngGenDto.fromJson
            return RngGenProvider.load id rngGen
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<rngGenProviderDto> jstr
            return! fromDto dto
        }

    let toDto (rngGen: rngGenProvider) =
        { 
            id = rngGen |> RngGenProvider.getId |> RngGenProviderId.value
            rngGenDto =  rngGen |> RngGenProvider.getFixedRngGen |> RngGenDto.toJson 
        }

    let toJson (rngGen: rngGenProvider) = rngGen |> toDto |> Json.serialize
