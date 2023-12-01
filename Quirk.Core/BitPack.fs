namespace Quirk.Core

open FSharp.UMX
open SysExt


type bitPack =
    private
        { bitsPerSymbol: int<bitsPerSymbol>
          symbolCount: int<symbolCount>
          offset:int
          data: byte[] }

module BitPack =

    let getBitsPerSymbol (bitPack: bitPack) = bitPack.bitsPerSymbol

    let getSymbolCount (bitPack: bitPack) = bitPack.symbolCount
    
    let getOffset (bitPack: bitPack) = bitPack.offset

    let getData (bitPack: bitPack) = 
        bitPack.data |> Seq.skip bitPack.offset

    let create (bitsPerSymbol: int<bitsPerSymbol>) 
               (symbolCount: int<symbolCount>)
               (offst:int)
               (data: byte[]) =
        { bitPack.bitsPerSymbol = bitsPerSymbol
          symbolCount = symbolCount;
          offset = offst;
          data = data }

    let changeOffset (bitPck:bitPack) (newOffset:int) =
        {bitPck with offset = newOffset }

    let fromBytes (bitsPerSymbol: int<bitsPerSymbol>) 
                  (data: byte[]) =
        let bps = (bitsPerSymbol |> UMX.untag)
        let bitCt = (data.Length * 8)
        let skud = bitCt % bps
        let symbolCt = ((bitCt - skud) / bps) |> UMX.tag<symbolCount>
        create bitsPerSymbol symbolCt 0 data


    let toInts (bitPack: bitPack) =
        bitPack
        |> getData
        |> ByteUtils.getAllBitsFromByteSeq
        |> ByteUtils.bitsToSpIntPositions 
                    (bitPack |> getBitsPerSymbol)
                    (bitPack |> getSymbolCount)
        

    let toIntArrays (arrayLength: int<arrayLength>) (bitPack: bitPack) =
        let arrayLen = (arrayLength |> UMX.untag)
        toInts bitPack 
        |> Seq.chunkBySize arrayLen
        |> Seq.filter (fun ba -> ba.Length = arrayLen)


    let fromInts (bitsPerSymbl: int<bitsPerSymbol>) (ints: seq<int>) =
        let byteSeq, bitCt =
            ints
            |> ByteUtils.bitsFromSpIntPositions bitsPerSymbl
            |> ByteUtils.storeBitSeqInBytes

        let data = byteSeq |> Seq.toArray
        let symbolCt = bitCt / (UMX.untag bitsPerSymbl) 
                       |> UMX.tag<symbolCount>
        create bitsPerSymbl symbolCt 0 data


    let fromIntArrays (bitsPerSymbl: int<bitsPerSymbol>) 
                      (intArrays: seq<int[]>) =
        fromInts bitsPerSymbl (intArrays |> Seq.concat)


    let toBoolArray (arrayLength: int<arrayLength>) (bitPack: bitPack) =
        let arrayLen = (arrayLength |> UMX.untag)
        bitPack
        |> getData
        |> ByteUtils.getAllBitsFromByteSeq
        |> Seq.chunkBySize arrayLen
        |> Seq.filter (fun ba -> ba.Length = arrayLen)


    let toBoolArrays (arrayLength: int<arrayLength>) (bitPack: bitPack) =
        let arrayLv = (arrayLength |> UMX.untag)
        bitPack
        |> getData
        |> ByteUtils.getAllBitsFromByteSeq
        |> Seq.chunkBySize arrayLv
        |> Seq.filter (fun ba -> ba.Length = arrayLv)


    let fromBools (boolArrays: seq<bool>) =
        let bitsPerSymbl = 1 |> UMX.tag<bitsPerSymbol>
        let byteSeq, bitCt = boolArrays |> ByteUtils.storeBitSeqInBytes
        let data = byteSeq |> Seq.toArray
        let symbolCt = bitCt |> UMX.tag<symbolCount>
        create bitsPerSymbl symbolCt 0 data


    let fromBoolArrays (boolArrays: seq<bool[]>) =
        fromBools (boolArrays |> Seq.concat)



