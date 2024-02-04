namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting

type sorterDto = { id:Guid;order:int; switches:byte[] }

module SorterDto =

    let fromDto (dto:sorterDto) =
        result {
            let order = dto.order |> UMX.tag<order>
            let bps = order |> Switch.bitsPerSymbolRequired
            let bitPck = BitPack.fromBytes bps dto.switches
            let switches = Switch.fromBitPack bitPck
            let sorterId = dto.id |> UMX.tag<sorterId>
            return Sorter.fromSwitches sorterId order switches
        }


    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterDto> jstr
            return! fromDto dto
        }


    let toDto (sortr: sorter) =
        { 
          sorterDto.id = sortr |> Sorter.getSorterId |> UMX.untag;
          order =  sortr |> Sorter.getOrder |> UMX.untag;
          switches = sortr |> Sorter.toByteArray
        }


    let toJson (sortr: sorter) =
        sortr |> toDto |> Json.serialize


type sorterSetDto = { 
        order:int; 
        sorterIds:Guid[]; 
        offsets:int[]; 
        symbolCounts:int[]; 
        switches:byte[] }

module SorterSetDto =

    let fromDto (dto:sorterSetDto) =
        result {
            let order = dto.order |> UMX.tag<order>
            let bps = order |> Switch.bitsPerSymbolRequired
            let switchArrayPacks = 
                    dto.switches 
                            |> CollectionOps.deBookMarkArray dto.offsets
                            |> Seq.map(BitPack.fromBytes bps)
                            |> Seq.toArray
            let sorterA = switchArrayPacks
                            |> Array.mapi(fun i pack ->    
                   Sorter.fromSwitches (dto.sorterIds.[i] |> UMX.tag<sorterId>)
                                       order    
                                       (Switch.fromBitPack pack))
            return SorterSet.load order sorterA
        }


    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetDto> jstr
            return! fromDto dto
        }


    let toDto (sorterSt: sorterSet) =
        let sOrder = sorterSt |> SorterSet.getOrder
        let triple = sorterSt 
                     |> SorterSet.getSorters
                     |> Seq.map(fun s -> 
                        (s |> Sorter.getSorterId |> UMX.untag), 
                         s |> Sorter.toByteArray, 
                         s |> Sorter.getSwitches |> Array.length)
                     |> Seq.toArray
        let bookMarks, data = triple
                              |> Array.map(fun (_, sw, _) -> sw)
                              |> CollectionOps.bookMarkArrays
        {
            order =  sOrder |> UMX.untag;
            sorterIds = triple |> Array.map(fun (gu, _, _) -> gu);
            offsets = bookMarks;
            symbolCounts = triple |> Array.map(fun (_, _, sc) -> sc);
            switches = data;
        }


    let toJson (sorterSt: sorterSet) =
        sorterSt |> toDto |> Json.serialize

