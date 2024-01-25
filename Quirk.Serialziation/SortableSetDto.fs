namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Project
open Quirk.Sorting

type sortableSetAllBitsDto =
    { sortableSetRId: Guid
      order: int
      fmt: string }

module SortableSetAllBitsDto =

    let fromDto (dto: sortableSetAllBitsDto) =
        result {
            let order = dto.order |> UMX.tag<order>
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            return! SortableSet.makeAllBits (dto.sortableSetRId |> UMX.tag<sortableSetId>) ssFormat order
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sortableSetAllBitsDto> jstr
            return! fromDto dto
        }

    let toDto 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormat: rolloutFormat) 
            (ord: int<order>) 
        =
        { 
          sortableSetAllBitsDto.sortableSetRId = sortableSetId |> UMX.untag;
          order = ord |> UMX.untag;
          fmt = rolloutFormat |> RolloutFormat.toString 
        }

    let toJson 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormat: rolloutFormat) 
            (ord: int<order>)
        =
        ord |> toDto sortableSetId rolloutFormat |> Json.serialize



type sortableSetOrbitDto =
    { 
        sortableSetRId: Guid
        maxCount: int
        permutation: int[]
        fmt: string
    }

module SortableSetOrbitDto =

    let fromDto (dto: sortableSetOrbitDto) =
        result {
            let maxCt =
                match dto.maxCount with
                | v when v > 0 -> v |> UMX.tag<sortableCount> |> Some
                | _ -> None

            let! perm = dto.permutation |> Permutation.create
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            return! SortableSet.makeOrbits
                        (dto.sortableSetRId |> UMX.tag<sortableSetId>)
                        maxCt
                        perm
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sortableSetOrbitDto> jstr
            return! fromDto dto
        }

    let toDto 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormt: rolloutFormat) 
            (maxCount: int) 
            (perm: permutation) 
        =
        { 
          sortableSetOrbitDto.sortableSetRId = sortableSetId |> UMX.untag
          maxCount = maxCount
          sortableSetOrbitDto.permutation = perm |> Permutation.getArray
          fmt = rolloutFormt |> RolloutFormat.toString 
        }

    let toJson 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormt: rolloutFormat) 
            (maxCount: int) 
            (perm: permutation) 
        =
        perm |> toDto sortableSetId rolloutFormt maxCount |> Json.serialize



type sortableSetSortedStacksDto =
    { 
        sortableSetId: Guid
        orderStack: int[]
        fmt: string 
    }

module SortableSetSortedStacksDto =

    let fromDto (dto: sortableSetSortedStacksDto) =
        result {
            let orderStack = dto.orderStack |> Array.map (UMX.tag<order>)
            let! ssFormat = dto.fmt |> RolloutFormat.fromString

            return!
                SortableSet.makeSortedStacks
                    (dto.sortableSetId |> UMX.tag<sortableSetId>)
                    ssFormat
                    orderStack
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sortableSetSortedStacksDto> jstr
            return! fromDto dto
        }

    let toDto 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormt: rolloutFormat) 
            (orderStack: int<order>[]) 
        =
        { 
          sortableSetSortedStacksDto.sortableSetId = sortableSetId |> UMX.untag
          orderStack = orderStack |> Array.map UMX.untag
          fmt = rolloutFormt |> RolloutFormat.toString 
        }

    let toJson 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormt: rolloutFormat) 
            (orderStack: int<order>[])
        =
        orderStack |> toDto sortableSetId rolloutFormt |> Json.serialize



type sortableSetRandomPermutationDto =
    { 
      sortableSetId: Guid
      order: int
      sortableCount: int
      rngGenId: int
      fmt: string 
    }

module SortableSetRandomPermutationDto =

    let fromDto 
            (dto: sortableSetRandomPermutationDto) 
            (rnGenLookup: int -> Result<rngGen, string>) 
        =
        result {
            let! rngGen = rnGenLookup dto.rngGenId
            let randy = Rando.fromRngGen rngGen
            let order = dto.order |> UMX.tag<order>
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            let sortableCt = dto.sortableCount |> UMX.tag<sortableCount>

            return!
                SortableSet.makeRandomPermutation
                    order
                    sortableCt
                    randy
                    (dto.sortableSetId |> UMX.tag<sortableSetId>)
        }

    let fromJson (jstr: string) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! dto = Json.deserialize<sortableSetRandomPermutationDto> jstr
            return! fromDto dto rnGenLookup
        }

    let toDto
            (sortableSetId: Guid<sortableSetId>)
            (rolloutFormt: rolloutFormat)
            (ord: int<order>)
            (sortableCt: int<sortableCount>)
            (rngId: int)
        =
        { 
          sortableSetRandomPermutationDto.sortableSetId = sortableSetId |> UMX.untag
          sortableSetRandomPermutationDto.order = ord |> UMX.untag
          sortableSetRandomPermutationDto.rngGenId = rngId
          sortableSetRandomPermutationDto.sortableCount = (sortableCt |> UMX.untag)
          fmt = rolloutFormt |> RolloutFormat.toString 
        }

    let toJson
        (sortableSetId: Guid<sortableSetId>)
        (rolloutFormt: rolloutFormat)
        (ord: int<order>)
        (sortableCt: int<sortableCount>)
        (rngId: int)
        =
        rngId |> toDto sortableSetId rolloutFormt ord sortableCt |> Json.serialize


type sortableSetRandomBitsDto =
    { 
      sortableSetId: Guid
      order: int
      pctOnes: float
      sortableCount: int
      rngGenId: int
      fmt: string 
    }

module SortableSetRandomBitsDto =

    let fromDto 
            (dto: sortableSetRandomBitsDto) 
            (rnGenLookup: int -> Result<rngGen, string>) 
        =
        result {
            let! rngGen = rnGenLookup dto.rngGenId
            let randy = Rando.fromRngGen rngGen
            let order = dto.order |> UMX.tag<order>
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            let sortableCt = dto.sortableCount |> UMX.tag<sortableCount>

            return!
                SortableSet.makeRandomBits
                    ssFormat
                    order
                    dto.pctOnes
                    sortableCt
                    randy
                    (dto.sortableSetId |> UMX.tag<sortableSetId>)
        }

    let fromJson (jstr: string) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! dto = Json.deserialize<sortableSetRandomBitsDto> jstr
            return! fromDto dto rnGenLookup
        }

    let toDto
            (sortableSetId: Guid<sortableSetId>)
            (rolloutFormt: rolloutFormat)
            (ord: int<order>)
            (pctOnes: float)
            (sortableCt: int<sortableCount>)
            (rngGenId: int)
        =
        {
          sortableSetRandomBitsDto.sortableSetId = sortableSetId |> UMX.untag
          order = ord |> UMX.untag
          pctOnes = pctOnes
          sortableCount = sortableCt |> UMX.untag
          rngGenId = rngGenId
          fmt = rolloutFormt |> RolloutFormat.toString 
        }

    let toJson
            (sortableSetId: Guid<sortableSetId>)
            (rolloutFormt: rolloutFormat)
            (ord: int<order>)
            (pctOnes: float)
            (sortableCt: int<sortableCount>)
            (rngGenId: int)
        =
        rngGenId
        |> toDto sortableSetId rolloutFormt ord pctOnes sortableCt
        |> Json.serialize



type sortableSetRandomSymbolsDto =
    { sortableSetId: Guid
      order: int
      symbolSetSize: uint64
      sortableCount: int
      rngGenId: int
      fmt: string }

module SortableSetRandomSymbolsDto =

    let fromDto (dto: sortableSetRandomSymbolsDto) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! rngGen = rnGenLookup dto.rngGenId
            let randy = Rando.fromRngGen rngGen
            let order = dto.order |> UMX.tag<order>
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            let symbolSetSize = dto.symbolSetSize |> UMX.tag<symbolSetSize>
            let sortableCt = dto.sortableCount |> UMX.tag<sortableCount>

            return!
                SortableSet.makeRandomSymbols
                    order
                    symbolSetSize
                    sortableCt
                    randy
                    (dto.sortableSetId |> UMX.tag<sortableSetId>)
        }

    let fromJson (jstr: string) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! dto = Json.deserialize<sortableSetRandomSymbolsDto> jstr
            return! fromDto dto rnGenLookup
        }

    let toDto
            (sortableSetId: Guid<sortableSetId>)
            (rolloutFormt: rolloutFormat)
            (order: int<order>)
            (symbolSetSz: uint64<symbolSetSize>)
            (sortableCt: int<sortableCount>)
            (rngGenId: int)  
        =
        { 
          sortableSetRandomSymbolsDto.sortableSetId = sortableSetId |> UMX.untag
          order = order |> UMX.untag
          symbolSetSize = symbolSetSz |> UMX.untag
          sortableCount = sortableCt |> UMX.untag
          rngGenId = rngGenId
          fmt = rolloutFormt |> RolloutFormat.toString 
        }

    let toJson
            (sortableSetId: Guid<sortableSetId>)
            (rolloutFormt: rolloutFormat)
            (order: int<order>)
            (symbolSetSz: uint64<symbolSetSize>)
            (sortableCt: int<sortableCount>)
            (rngGenId: int)  
        =
        rngGenId
        |> toDto sortableSetId rolloutFormt order symbolSetSz sortableCt
        |> Json.serialize



type sortableSetExplicitTableDto =
    { 
      sortableSetId: Guid
      order: int
      symbolSetSize: uint64
      bitPackRId: int
      fmt: string 
    }

module SortableSetExplicitTableDto =

    let fromDto (dto: sortableSetExplicitTableDto) (bitPackLookup: int -> Result<bitPack, string>) =
        result {
            let! bitPack = bitPackLookup dto.bitPackRId
            let order = dto.order |> UMX.tag<order>
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            let symbolSetSize = dto.symbolSetSize |> UMX.tag<symbolSetSize>
            let sortableSetId = dto.sortableSetId |> UMX.tag<sortableSetId>
            return! SortableSet.fromBitPack sortableSetId ssFormat order symbolSetSize bitPack
        }

    let fromJson (jstr: string) (bitPackLookup: int -> Result<bitPack, string>) =
        result {
            let! dto = Json.deserialize<sortableSetExplicitTableDto> jstr
            return! fromDto dto bitPackLookup
        }

    let toDto
        (sortableSetId: Guid<sortableSetId>)
        (rolloutFormt: rolloutFormat)
        (order: int<order>)
        (symbolSetSz: uint64<symbolSetSize>)
        (sortableCt: int<sortableCount>)
        (bitPackRId: int)  =
        { 
          sortableSetExplicitTableDto.sortableSetId = sortableSetId |> UMX.untag
          order = order |> UMX.untag
          symbolSetSize = symbolSetSz |> UMX.untag
          bitPackRId = bitPackRId
          fmt = rolloutFormt |> RolloutFormat.toString 
        }

    let toJson
        (sortableSetId: Guid<sortableSetId>)
        (rolloutFormt: rolloutFormat)
        (order: int<order>)
        (symbolSetSz: uint64<symbolSetSize>)
        (sortableCt: int<sortableCount>)
        (rngGenId: int) =
        rngGenId
        |> toDto sortableSetId rolloutFormt order symbolSetSz sortableCt
        |> Json.serialize



type sortableSetSwitchReducedDto =
    { 
      sortableSetRId: Guid
      sorterId: int
      sortableSetSourceId: int
      fmt: string 
    }

module SortableSetSwitchReducedDto =

    let fromDto
        (dto: sortableSetSwitchReducedDto)
        (sortableSetLookup: int -> Result<sortableSet, string>)
        (sorterLookup: Guid -> Result<sorter, string>) =
        result {
            let! sortableSet = sortableSetLookup dto.sortableSetSourceId
            let! sorter = sorterLookup  (Guid.NewGuid()) //dto.sorterId
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            return! SortableSet.switchReduce 
                        (dto.sortableSetRId |> UMX.tag<sortableSetId>) 
                        sortableSet 
                        sorter
        }

    let fromJson
        (jstr: string)
        (sortableSetLookup: int -> Result<sortableSet, string>)
        (sorterLookup: Guid -> Result<sorter, string>) =
        result {
            let! dto = Json.deserialize<sortableSetSwitchReducedDto> jstr
            return! fromDto dto sortableSetLookup sorterLookup
        }

    let toDto 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormt: rolloutFormat) 
            (sorterId: int) 
            (sortableSetSourceId: int) 
            =
        { 
          sortableSetSwitchReducedDto.sorterId = sorterId
          fmt = rolloutFormt |> RolloutFormat.toString
          sortableSetRId = sortableSetId |> UMX.untag
          sortableSetSourceId = sortableSetSourceId 
        }

    let toJson 
            (sortableSetId: Guid<sortableSetId>) 
            (rolloutFormt: rolloutFormat) 
            (sorterId: int) 
            (sortableSetSourceId: int) 
        =
        toDto sortableSetId rolloutFormt sorterId sortableSetSourceId |> Json.serialize



type sortableSetDto =
    { 
      sortableSetId: Guid
      rolloutDto: string
      symbolSetSize: uint64 
    }

module SortableSetDto =
    let fromDto 
            (dto: sortableSetDto)
        =
        result {
            let! rollout = dto.rolloutDto |> RolloutDto.fromJson
            let symbolSetSize = dto.symbolSetSize |> UMX.tag<symbolSetSize>
            let sortableSetId = dto.sortableSetId |> UMX.tag<sortableSetId>
            return SortableSet.make sortableSetId symbolSetSize rollout
        }

    let fromJson 
            (jstr: string) 
        =
        result {
            let! dto = Json.deserialize<sortableSetDto> jstr
            return! fromDto dto
        }

    let toDto
            (ss: sortableSet)
        =
        { 
          sortableSetDto.sortableSetId = ss |> SortableSet.getSortableSetId |> UMX.untag
          rolloutDto = ss |> SortableSet.getRollout |> RolloutDto.toJson
          symbolSetSize = ss |> SortableSet.getSymbolSetSize |> UMX.untag
        }

    let toJson
            (ss: sortableSet)
        =
        ss |> toDto |> Json.serialize

