module RngGenDtoFixture

open System
open Xunit
open FSharp.UMX
open Quirk.Core
open Quirk.Serialization


[<Fact>]
let ``rngGenDto``() =
    let rngGen = RngGen.create rngType.Lcg (123uL |> UMX.tag<randomSeed>)
    let dto = RngGenDto.toDto rngGen
    let rngGenBack = RngGenDto.fromDto dto |> Result.ExtractOrThrow
    Assert.Equal(rngGen, rngGenBack)

    let rngGen2 = RngGen.create rngType.Net (123uL |> UMX.tag<randomSeed>)

    let dto2 = RngGenDto.toDto rngGen2
    let rngGenBack2 = RngGenDto.fromDto dto2 |> Result.ExtractOrThrow
    Assert.Equal(rngGen2, rngGenBack2)




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