﻿namespace Quirk.Core
open System
open System.Collections
open System.Runtime.CompilerServices
open FSharp.UMX
open SysExt

type booleanRoll =
    private
        { arrayCount: int<arrayCount>
          arrayLength: int<arrayLength>
          data: bool[] }

module BooleanRoll =

    let create 
            (arrayCount: int<arrayCount>) 
            (arrayLength: int<arrayLength>)
        =
        let dataLength = (arrayCount |> UMX.untag) * (arrayLength |> UMX.untag)
        { 
          arrayCount = arrayCount
          arrayLength = arrayLength
          data = Array.zeroCreate<bool> dataLength 
        }

    let createEmpty =
        { arrayCount = (0 |> UMX.tag<arrayCount>)
          arrayLength = (0 |> UMX.tag<arrayLength>)
          data = Array.zeroCreate<bool> 0 }

    let getArrayCount (booleanRoll: booleanRoll) = booleanRoll.arrayCount

    let getArrayLength (booleanRoll: booleanRoll) = booleanRoll.arrayLength

    let getData (booleanRoll: booleanRoll) = booleanRoll.data

    let copy (booleanRoll: booleanRoll) =
        { booleanRoll with data = booleanRoll.data |> Array.copy }

    let fromBitPack 
            (arrayLength: int<arrayLength>) 
            (bitPack: bitPack) 
        =
        result {
            let arrayCount =
                ((bitPack.symbolCount |> UMX.untag) / (arrayLength |> UMX.untag))
                |> UMX.tag<arrayCount>

            let data =
                bitPack |> BitPack.getData 
                |> ByteUtils.getAllBitsFromByteSeq 
                |> Seq.toArray

            return
                { booleanRoll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  data = data }
        }


    let toBitPack (booleanRoll: booleanRoll) =
        result {
            let bitsPerSymbl = 1 |> UMX.tag<bitsPerSymbol>

            let symbolCt =
                ((booleanRoll.arrayCount |> UMX.untag)
                 * (booleanRoll.arrayLength |> UMX.untag))
                |> UMX.tag<symbolCount>

            let byteSeq, bitCt = 
                booleanRoll.data 
                |> ByteUtils.storeBitSeqInBytes

            let data = byteSeq |> Seq.toArray

            return BitPack.create bitsPerSymbl symbolCt 0 data
        }


    let fromBoolArrays 
            (arrayLength: int<arrayLength>) 
            (boolAs: seq<bool[]>) 
            =
        result {
            let bools = boolAs |> Seq.concat |> Seq.toArray

            let arrayCount =
                (bools.Length / (UMX.untag arrayLength)) |> UMX.tag<arrayCount>

            return
                { booleanRoll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  data = bools }
        }


    //let fromIntArrays 
    //        (arrayLength: int<arrayLength>) 
    //        (aas: seq<int[]>)
    //        =
    //    result {
    //        let boolA =
    //            aas
    //            |> Seq.concat
    //            |> Seq.map (fun bv -> if (bv > 0) then true else false)
    //            |> Seq.toArray

    //        let arrayCount =
    //            (boolA.Length / (UMX.untag arrayLength)) |> UMX.tag<arrayCount>

    //        return
    //            { booleanRoll.arrayCount = arrayCount
    //              arrayLength = arrayLength
    //              data = boolA }
    //    }


    let toIntArrays (booleanRoll: booleanRoll) =
        booleanRoll.data
        |> Seq.map (fun bv -> if bv then 1 else 0)
        |> Seq.chunkBySize (booleanRoll.arrayLength |> UMX.untag)


    let toBoolArrays (booleanRoll: booleanRoll) =
        booleanRoll.data
        |> Seq.chunkBySize (booleanRoll.arrayLength |> UMX.untag)


    let uniqueMembers (booleanRoll: booleanRoll) =
        booleanRoll
        |> toBoolArrays
        |> Seq.distinct
        |> fromBoolArrays (booleanRoll |> getArrayLength)


    let uniqueUnsortedMembers (booleanRoll: booleanRoll) =
        booleanRoll
        |> toBoolArrays
        |> Seq.filter (fun ia -> not (CollectionProps.isSorted ia))
        |> Seq.distinct
        |> fromBoolArrays (booleanRoll |> getArrayLength)


    let isSorted (booleanRoll: booleanRoll) =
        let mutable i = 0
        let iIncr = booleanRoll.arrayLength |> UMX.untag
        let iMax = booleanRoll.data.Length
        let mutable looP = true

        while ((i < iMax) && looP) do
            looP <- CollectionProps.isSortedOffset booleanRoll.data i iIncr
            i <- i + iIncr
        looP



type uInt8Roll =
    private
        { arrayCount: int<arrayCount>
          arrayLength: int<arrayLength>
          bitsPerSymbol: int<bitsPerSymbol>
          data: uint8[] }

module Uint8Roll =

    let getArrayCount (uInt8Roll: uInt8Roll) = uInt8Roll.arrayCount

    let getArrayLength (uInt8Roll: uInt8Roll) = uInt8Roll.arrayLength

    let getData (uInt8Roll: uInt8Roll) = uInt8Roll.data

    let copy (uInt8Roll: uInt8Roll) =
        { arrayCount = uInt8Roll.arrayCount
          arrayLength = uInt8Roll.arrayLength
          bitsPerSymbol = uInt8Roll.bitsPerSymbol
          data = uInt8Roll.data |> Array.copy }


    let fromBitPack 
            (arrayLength: int<arrayLength>) 
            (bitPack: bitPack) 
            =
        result {
            let arrayCount =
                ((bitPack.symbolCount |> UMX.untag) / (arrayLength |> UMX.untag))
                |> UMX.tag<arrayCount>
            let data =
                bitPack
                |> BitPack.getData
                |> ByteUtils.getAllBitsFromByteSeq
                |> ByteUtils.bitsToSpBytePositions 
                                (bitPack |> BitPack.getBitsPerSymbol)
                                (bitPack |> BitPack.getSymbolCount)
                |> Seq.toArray

            return
                { uInt8Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitPack.bitsPerSymbol
                  data = data }
        }


    let toBitPack 
            (uInt8Roll: uInt8Roll) 
            =
        result {
            let symbolCt =
                ((uInt8Roll.arrayCount |> UMX.untag)
                 * (uInt8Roll.arrayLength |> UMX.untag))
                |> UMX.tag<symbolCount>

            let byteSeq, bitCt =
                uInt8Roll.data
                |> ByteUtils.bitsFromSpBytePositions uInt8Roll.bitsPerSymbol
                |> ByteUtils.storeBitSeqInBytes

            let data = byteSeq |> Seq.toArray

            return BitPack.create uInt8Roll.bitsPerSymbol symbolCt 0 data
        }


    let fromBoolArrays 
            (arrayLength: int<arrayLength>) 
            (aas: seq<bool[]>) 
            =
        result {
            let uint8s =
                aas |> Seq.concat 
                |> Seq.map (fun tf -> if tf then 1uy else 0uy) 
                |> Seq.toArray

            let arrayCount =
                (uint8s.Length / (UMX.untag arrayLength)) 
                |> UMX.tag<arrayCount>

            let bitsPerSymbol =  1 |> UMX.tag<bitsPerSymbol>
                                
            return
                { uInt8Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = uint8s }
        }


    let fromArrays 
            (arrayLength: int<arrayLength>)
            (bitsPerSymbol: int<bitsPerSymbol>)
            (aas: seq<uint8[]>) 
            =
        result {
            let uint8s = 
                aas |> Seq.concat 
                |> Seq.map (byte) 
                |> Seq.toArray

            let arrayCount =
                (uint8s.Length / (UMX.untag arrayLength)) 
                |> UMX.tag<arrayCount>

            return
                { uInt8Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = uint8s }
        }


    let toArrays (uInt8Roll: uInt8Roll) =
        uInt8Roll.data
        |> Seq.map (uint8)
        |> Seq.chunkBySize (uInt8Roll.arrayLength |> UMX.untag)

    let toBoolArrays (uInt8Roll: uInt8Roll) =
        uInt8Roll.data
        |> Seq.map (fun bv -> bv > 0uy)
        |> Seq.chunkBySize (uInt8Roll.arrayLength |> UMX.untag)


    let toIntArrays (uInt8Roll: uInt8Roll) =
        uInt8Roll.data
        |> Seq.map (int)
        |> Seq.chunkBySize (uInt8Roll.arrayLength |> UMX.untag)


    let uniqueMembers (uInt8Roll: uInt8Roll) =
        uInt8Roll
        |> toArrays
        |> Seq.distinct
        |> fromArrays 
                (uInt8Roll |> getArrayLength)
                uInt8Roll.bitsPerSymbol


    let uniqueUnsortedMembers (uInt8Roll: uInt8Roll) =
        uInt8Roll
        |> toArrays
        |> Seq.filter (fun ia -> not (CollectionProps.isSorted ia))
        |> Seq.distinct
        |> fromArrays 
            (uInt8Roll |> getArrayLength)
            uInt8Roll.bitsPerSymbol


    let isSorted (uInt8Roll: uInt8Roll) =
        let mutable i = 0
        let iIncr = uInt8Roll.arrayLength |> UMX.untag
        let iMax = uInt8Roll.data.Length
        let mutable looP = true

        while ((i < iMax) && looP) do
            looP <- CollectionProps.isSortedOffset uInt8Roll.data i iIncr
            i <- i + iIncr

        looP



type uInt16Roll =
    private
        { arrayCount: int<arrayCount>
          arrayLength: int<arrayLength>
          bitsPerSymbol: int<bitsPerSymbol>
          data: uint16[] }

module Uint16Roll =

    let getArrayCount (uInt16Roll: uInt16Roll) = uInt16Roll.arrayCount

    let getArrayLength (uInt16Roll: uInt16Roll) = uInt16Roll.arrayLength

    let getData (uInt16Roll: uInt16Roll) = uInt16Roll.data

    let copy 
            (uInt16Roll: uInt16Roll) 
            =
        { arrayCount = uInt16Roll.arrayCount
          arrayLength = uInt16Roll.arrayLength
          bitsPerSymbol = uInt16Roll.bitsPerSymbol
          data = uInt16Roll.data |> Array.copy }

    let fromBitPack 
            (arrayLength: int<arrayLength>) 
            (bitPack: bitPack) 
            =
        result {
            let arrayCount =
                ((bitPack.symbolCount |> UMX.untag) / (arrayLength |> UMX.untag))
                |> UMX.tag<arrayCount>

            let data =
                bitPack
                |> BitPack.getData
                |> ByteUtils.getAllBitsFromByteSeq
                |> ByteUtils.bitsToSpUint16Positions 
                        (bitPack |> BitPack.getBitsPerSymbol)
                        (bitPack |> BitPack.getSymbolCount)
                |> Seq.toArray

            return
                { uInt16Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitPack.bitsPerSymbol
                  data = data }
        }


    let toBitPack
            (uInt16Roll: uInt16Roll) 
            =
        result {
            let symbolCt =
                ((uInt16Roll.arrayCount |> UMX.untag)
                 * (uInt16Roll.arrayLength |> UMX.untag))
                |> UMX.tag<symbolCount>

            let byteSeq, bitCt =
                uInt16Roll.data
                |> ByteUtils.bitsFromSpUint16Positions uInt16Roll.bitsPerSymbol
                |> ByteUtils.storeBitSeqInBytes

            let data = byteSeq |> Seq.toArray
            
            return BitPack.create uInt16Roll.bitsPerSymbol symbolCt 0 data
        }


    let fromBoolArrays 
            (arrayLength: int<arrayLength>) 
            (aas: seq<bool[]>) 
            =
        result {
            let uint16s =
                aas |> Seq.concat 
                |> Seq.map (fun tf -> if tf then 1us else 0us) 
                |> Seq.toArray

            let arrayCount =
                (uint16s.Length / (UMX.untag arrayLength)) |> UMX.tag<arrayCount>
            
            let bitsPerSymbol =  1 |> UMX.tag<bitsPerSymbol>

            return
                { uInt16Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = uint16s }
        }


    let fromArrays 
            (arrayLength: int<arrayLength>)
            (bitsPerSymbol: int<bitsPerSymbol>)
            (aas: seq<uint16[]>) 
            =
        result {
            let uint16s = 
                aas |> Seq.concat 
                    |> Seq.map (uint16) 
                    |> Seq.toArray

            let arrayCount =
                (uint16s.Length / (UMX.untag arrayLength)) 
                |> UMX.tag<arrayCount>

            return
                { uInt16Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = uint16s }
        }

    let toArrays (uInt16Roll: uInt16Roll) =
        uInt16Roll.data
        |> Seq.map (uint16)
        |> Seq.chunkBySize (uInt16Roll.arrayLength |> UMX.untag)


    let toBoolArrays (uInt16Roll: uInt16Roll) =
        uInt16Roll.data
        |> Seq.map (fun bv -> bv > 0us)
        |> Seq.chunkBySize (uInt16Roll.arrayLength |> UMX.untag)


    let toIntArrays (uInt16Roll: uInt16Roll) =
        uInt16Roll.data
        |> Seq.map (int)
        |> Seq.chunkBySize (uInt16Roll.arrayLength |> UMX.untag)


    let uniqueMembers (uInt16Roll: uInt16Roll) =
        uInt16Roll
        |> toArrays
        |> Seq.distinct
        |> fromArrays
                (uInt16Roll |> getArrayLength)
                uInt16Roll.bitsPerSymbol


    let uniqueUnsortedMembers (uInt16Roll: uInt16Roll) =
        uInt16Roll
        |> toArrays
        |> Seq.filter (fun ia -> not (CollectionProps.isSorted ia))
        |> Seq.distinct
        |> fromArrays
                (uInt16Roll |> getArrayLength)
                uInt16Roll.bitsPerSymbol


    let isSorted (uInt16Roll: uInt16Roll) =
        let mutable i = 0
        let iIncr = uInt16Roll.arrayLength |> UMX.untag
        let iMax = uInt16Roll.data.Length
        let mutable looP = true

        while ((i < iMax) && looP) do
            looP <- CollectionProps.isSortedOffset uInt16Roll.data i iIncr
            i <- i + iIncr

        looP



type intRoll =
    private
        { arrayCount: int<arrayCount>
          arrayLength: int<arrayLength>
          bitsPerSymbol: int<bitsPerSymbol>
          data: int[] }

module IntRoll =

    let copy (intRoll: intRoll) =
        { arrayCount = intRoll.arrayCount
          arrayLength = intRoll.arrayLength
          bitsPerSymbol = intRoll.bitsPerSymbol
          data = intRoll.data |> Array.copy }

    let updateData (data: int[]) (intRoll: intRoll) = 
            { intRoll with data = data }

    let getArrayCount (uInt8Roll: intRoll) = uInt8Roll.arrayCount

    let getArrayLength (intRoll: intRoll) = intRoll.arrayLength

    let getRolloutLength (intRoll: intRoll) =
        (intRoll.arrayCount |> UMX.untag)
        * (intRoll.arrayLength |> UMX.untag)

    let getData (uInt8Roll: intRoll) = uInt8Roll.data

    let fromBitPack 
            (arrayLength: int<arrayLength>) 
            (bitPack: bitPack) 
            =
        result {
            let arrayCount =
                ((bitPack.symbolCount |> UMX.untag) / (arrayLength |> UMX.untag))
                |> UMX.tag<arrayCount>

            let data =
                bitPack
                |> BitPack.getData
                |> ByteUtils.getAllBitsFromByteSeq
                |> ByteUtils.bitsToSpIntPositions 
                        (bitPack |> BitPack.getBitsPerSymbol)
                        (bitPack |> BitPack.getSymbolCount)
                |> Seq.toArray

            return
                { intRoll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitPack.bitsPerSymbol
                  data = data }
        }

    let toBitPack
            (intRoll: intRoll) 
            =
        result {
            let symbolCt =
                ((intRoll.arrayCount |> UMX.untag)
                 * (intRoll.arrayLength |> UMX.untag))
                |> UMX.tag<symbolCount>

            let byteSeq, bitCt =
                intRoll.data
                |> ByteUtils.bitsFromSpIntPositions 
                        intRoll.bitsPerSymbol
                |> ByteUtils.storeBitSeqInBytes

            let data = byteSeq |> Seq.toArray
            return BitPack.create intRoll.bitsPerSymbol symbolCt 0 data
        }

    let fromBoolArrays 
            (arrayLength: int<arrayLength>) 
            (aas: seq<bool[]>) 
            =
        result {
            let uint8s =
                aas |> Seq.concat 
                |> Seq.map (fun tf -> if tf then 1 else 0) 
                |> Seq.toArray

            let arrayCount =
                (uint8s.Length / (UMX.untag arrayLength)) 
                |> UMX.tag<arrayCount>
            
            let bitsPerSymbol =  1 |> UMX.tag<bitsPerSymbol>

            return
                { intRoll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = uint8s }
        }

    let fromArrays 
            (arrayLength: int<arrayLength>)
            (bitsPerSymbol:int<bitsPerSymbol>)
            (aas: seq<int[]>) 
            =
        result {
            let intA = aas |> Seq.concat |> Seq.toArray
            let arrayCount =
                (intA.Length / (UMX.untag arrayLength)) 
                |> UMX.tag<arrayCount>
            return
                { intRoll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = intA }
        }


    let toArrays (intRoll: intRoll) =
        intRoll.data 
            |> Seq.chunkBySize (intRoll.arrayLength 
            |> UMX.untag)


    let toBoolArrays (intRoll: intRoll) =
        intRoll.data
        |> Seq.map (fun bv -> bv > 0)
        |> Seq.chunkBySize (intRoll.arrayLength |> UMX.untag)


    let uniqueMembers (intRoll: intRoll) =
        intRoll
        |> toArrays
        |> Seq.distinct
        |> fromArrays 
            (intRoll |> getArrayLength)
            (32 |> UMX.tag<bitsPerSymbol>)


    let uniqueUnsortedMembers (intRoll: intRoll) =
        intRoll
        |> toArrays
        |> Seq.filter (fun ia -> not (CollectionProps.isSorted ia))
        |> Seq.distinct
        |> fromArrays
            (intRoll |> getArrayLength)
            (31 |> UMX.tag<bitsPerSymbol>)


    let isSorted (intRoll: intRoll) =
        let mutable i = 0
        let iIncr = intRoll.arrayLength |> UMX.untag
        let iMax = intRoll.data.Length
        let mutable looP = true

        while ((i < iMax) && looP) do
            looP <- CollectionProps.isSortedOffset intRoll.data i iIncr
            i <- i + iIncr

        looP



type uint64Roll =
    private
        { arrayCount: int<arrayCount>
          arrayLength: int<arrayLength>
          bitsPerSymbol: int<bitsPerSymbol>
          data: uint64[] }

module Uint64Roll =

    let getArrayCount (uint64Roll: uint64Roll) = uint64Roll.arrayCount

    let getArrayLength (uint64Roll: uint64Roll) = uint64Roll.arrayLength

    let getData (uint64Roll: uint64Roll) = uint64Roll.data

    let stripeBlocksNeededForArrayCount (arrayCount: int<arrayCount>) =
        ((UMX.untag arrayCount) + 63) / 64

    let copy (uint64Roll: uint64Roll) =
        { arrayCount = uint64Roll.arrayCount
          arrayLength = uint64Roll.arrayLength
          bitsPerSymbol = uint64Roll.bitsPerSymbol
          data = uint64Roll.data |> Array.copy }

    let fromBitPack (arrayLength: int<arrayLength>) (bitPack: bitPack) =
        result {
            let arrayCount =
                ((bitPack.symbolCount |> UMX.untag) / (arrayLength |> UMX.untag))
                |> UMX.tag<arrayCount>

            let data =
                bitPack
                |> BitPack.getData
                |> ByteUtils.getAllBitsFromByteSeq
                |> ByteUtils.bitsToSpUint64Positions 
                        ( bitPack |> BitPack.getBitsPerSymbol)
                        ( bitPack |> BitPack.getSymbolCount)
                |> Seq.toArray

            return
                { uint64Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitPack.bitsPerSymbol
                  data = data }
        }


    let toBitPack
        (uint64Roll: uint64Roll) =
        result {
            let symbolCt =
                uint64Roll.arrayCount
                |> UMX.untag
                |> (*) (uint64Roll.arrayLength |> UMX.untag)
                |> UMX.tag<symbolCount>

            let byteSeq, bitCt =
                uint64Roll.data
                |> ByteUtils.bitsFromSpUint64Positions 
                        uint64Roll.bitsPerSymbol
                |> ByteUtils.storeBitSeqInBytes

            let data = byteSeq |> Seq.toArray
            return BitPack.create uint64Roll.bitsPerSymbol symbolCt 0 data
        }


    let fromArrays 
            (arrayLength: int<arrayLength>)
            (bitsPerSymbol:int<bitsPerSymbol>)
            (aas: seq<uint64[]>) 
            =
        result {
            let uint64s = aas |> Seq.concat |> Seq.toArray
            let arrayCount =
                (uint64s.Length / (UMX.untag arrayLength)) |> UMX.tag<arrayCount>
            return
                { uint64Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = uint64s }
        }


    let fromBoolArrays 
            (arrayLength: int<arrayLength>) 
            (aas: seq<bool[]>) 
            =
        result {
            let uint8s =
                aas |> Seq.concat 
                |> Seq.map (fun tf -> if tf then 1uL else 0uL) 
                |> Seq.toArray

            let arrayCount =
                (uint8s.Length / (UMX.untag arrayLength)) 
                |> UMX.tag<arrayCount>

            let bitsPerSymbol =  1 |> UMX.tag<bitsPerSymbol>

            return
                { uint64Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = uint8s }
        }

    let fromIntArrays 
            (arrayLength: int<arrayLength>)
            (bitsPerSymbol:int<bitsPerSymbol>)
            (aas: seq<int[]>) 
            =
        result {
            let uint64s = aas |> Seq.concat |> Seq.map (uint64) |> Seq.toArray
            let arrayCount =
                (uint64s.Length / (UMX.untag arrayLength)) |> UMX.tag<arrayCount>
            return
                { uint64Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  bitsPerSymbol = bitsPerSymbol
                  data = uint64s }
        }

    let toArrays 
            (uint64Roll: uint64Roll) 
            =
        uint64Roll.data
        |> Seq.chunkBySize 
                (uint64Roll.arrayLength |> UMX.untag)

    let toBoolArrays 
            (uint64Roll: uint64Roll) 
            =
        uint64Roll.data
        |> Seq.map (fun bv -> bv > 0uL)
        |> Seq.chunkBySize (uint64Roll.arrayLength |> UMX.untag)


    let toIntArrays 
            (uint64Roll: uint64Roll) 
            =
        uint64Roll.data
        |> Seq.map (int)
        |> Seq.chunkBySize (uint64Roll.arrayLength |> UMX.untag)


    let uniqueMembers 
            (uint64Roll: uint64Roll) 
            =
        uint64Roll
        |> toArrays
        |> Seq.distinct
        |> fromArrays
            (uint64Roll |> getArrayLength) 
            (64 |> UMX.tag<bitsPerSymbol>)


    let uniqueUnsortedMembers (uint64Roll: uint64Roll) =
        uint64Roll
        |> toArrays
        |> Seq.filter (fun ia -> not (CollectionProps.isSorted ia))
        |> Seq.distinct
        |> fromArrays 
            (uint64Roll |> getArrayLength) 
            (64 |> UMX.tag<bitsPerSymbol>)


    let isSorted (uint64Roll: uint64Roll) =
        let mutable i = 0
        let iIncr = uint64Roll.arrayLength |> UMX.untag
        let iMax = uint64Roll.data.Length
        let mutable looP = true
        while ((i < iMax) && looP) do
            looP <- CollectionProps.isSortedOffset uint64Roll.data i iIncr
            i <- i + iIncr

        looP


type bs64Roll =
    private
        { arrayCount: int<arrayCount>
          arrayLength: int<arrayLength>
          data: uint64[] }

module Bs64Roll =

    let getArrayCount (bs64Roll: bs64Roll) = bs64Roll.arrayCount

    let getArrayLength (bs64Roll: bs64Roll) = bs64Roll.arrayLength

    let getData (bs64Roll: bs64Roll) = bs64Roll.data

    let getDataArrayLength (bs64Roll: bs64Roll) = bs64Roll.data.Length

    let stripeBlocksNeededForArrayCount 
                (arrayCount: int<arrayCount>) 
        =
        ((UMX.untag arrayCount) + 63) / 64

    let createEmptyStripedSet 
                (arrayLength: int<arrayLength>) 
                (arrayCount: int<arrayCount>) 
        =
        let blocksNeeded = stripeBlocksNeededForArrayCount arrayCount
        let dataLength = (UMX.untag arrayLength) * blocksNeeded
        let data = Array.zeroCreate<uint64> dataLength

        { arrayCount = arrayCount
          arrayLength = arrayLength
          data = data }


    let copy (bs64Roll: bs64Roll) =
        { bs64Roll with data = bs64Roll.data |> Array.copy }


    let getUsedStripes (bs64Roll: bs64Roll) =
        let len = bs64Roll.data.Length
        let arrayLen = bs64Roll.arrayLength |> UMX.untag
        let q = bs64Roll.data.[len - arrayLen .. len - 1]
        let lastStripes = q |> ByteUtils.usedStripeCount
        let ww = (len - arrayLen) / arrayLen
        (ww * 64) + lastStripes


    let fromBoolArrays 
            (arrayLength: int<arrayLength>) 
            (aas: seq<bool[]>) 
        =
        result {
            let order = 
                arrayLength 
                |> UMX.untag
                |> UMX.tag<order>

            let! data, arrayCountV = 
                ByteUtils.makeStripedArraysFromBoolArrays order aas

            let arrayCount = 
                arrayCountV |> UMX.tag<arrayCount>

            return
                { bs64Roll.arrayCount = arrayCount
                  arrayLength = arrayLength
                  data = data }
        }


    let fromIntArrays 
            (arrayLength: int<arrayLength>) 
            (aas: seq<int[]>) 
        =
        let _aConv (ia: int[]) =
            Array.init ia.Length (fun dex -> ia[dex] = 1)

        fromBoolArrays arrayLength (aas |> Seq.map (_aConv))


    let fromBitPack 
            (arrayLength: int<arrayLength>) 
            (bitPack: bitPack) 
        =
        result {
            let bitsPerSymbol = bitPack |> BitPack.getBitsPerSymbol

            if (bitsPerSymbol |> UMX.untag) <> 1 then
                return! sprintf "bitsPerSymbol must be 1" |> Error
            else
                return!
                    bitPack
                    |> BitPack.getData
                    |> ByteUtils.getAllBitsFromByteSeq
                    |> Seq.take (bitPack.symbolCount |> UMX.untag)
                    |> Seq.chunkBySize (arrayLength |> UMX.untag)
                    |> fromBoolArrays arrayLength
        }


    let toBitPack (bs64Roll: bs64Roll) =
        result {
            let order = bs64Roll.arrayLength |> UMX.untag |> UMX.tag<order>
            let bitsPerSymbl = 1 |> UMX.tag<bitsPerSymbol>

            let symbolCt =
                bs64Roll.arrayCount
                |> UMX.untag
                |> (*) (bs64Roll.arrayLength |> UMX.untag)
                |> UMX.tag<symbolCount>

            let data, bitCt =
                ByteUtils.fromStripeArrays order bs64Roll.data
                |> Seq.take (bs64Roll.arrayCount |> UMX.untag)
                |> Seq.concat
                |> ByteUtils.storeBitSeqInBytes
            
            return BitPack.create bitsPerSymbl symbolCt 0 data
        }


    let toBoolArrays (bs64Roll: bs64Roll) =
        let order = bs64Roll.arrayLength |> UMX.untag |> UMX.tag<order>

        ByteUtils.fromStripeArrays order bs64Roll.data
        |> Seq.take (bs64Roll |> getArrayCount |> UMX.untag)


    let uniqueMembers (bs64Roll: bs64Roll) =
        bs64Roll
        |> toBoolArrays
        |> Seq.distinct
        |> fromBoolArrays (bs64Roll |> getArrayLength)


    let uniqueUnsortedMembers (bs64Roll: bs64Roll) =
        bs64Roll
        |> toBoolArrays
        |> Seq.filter (fun ia -> not (CollectionProps.isSorted ia))
        |> Seq.distinct
        |> fromBoolArrays (bs64Roll |> getArrayLength)


    let isSorted (bs64Roll: bs64Roll) =
        bs64Roll
        |> toBoolArrays
        |> Seq.forall (fun ia -> (CollectionProps.isSorted ia))


    let toIntArrays (bs64Roll: bs64Roll) =
        let _aConv (ba: bool[]) =
            Array.init ba.Length (fun dex -> if ba[dex] then 1 else 0)

        let order = bs64Roll.arrayLength |> UMX.untag |> UMX.tag<order>

        ByteUtils.fromStripeArrays order bs64Roll.data
        |> Seq.map (_aConv)
        |> Seq.take (bs64Roll |> getArrayCount |> UMX.untag)



type rolloutFormat =
    | RfB
    | RfU8
    | RfU16
    | RfI32
    | RfU64
    | RfBs64


module RolloutFormat =

    let toString (rf: rolloutFormat) =
        match rf with
        | rolloutFormat.RfB -> nameof rolloutFormat.RfB
        | rolloutFormat.RfU8 -> nameof rolloutFormat.RfU8
        | rolloutFormat.RfU16 -> nameof rolloutFormat.RfU16
        | rolloutFormat.RfI32 -> nameof rolloutFormat.RfI32
        | rolloutFormat.RfU64 -> nameof rolloutFormat.RfU64
        | rolloutFormat.RfBs64 -> nameof rolloutFormat.RfBs64


    let fromString str =
        match str with
        | nameof rolloutFormat.RfB -> rolloutFormat.RfB |> Ok
        | nameof rolloutFormat.RfU8 -> rolloutFormat.RfU8 |> Ok
        | nameof rolloutFormat.RfU16 -> rolloutFormat.RfU16 |> Ok
        | nameof rolloutFormat.RfI32 -> rolloutFormat.RfI32 |> Ok
        | nameof rolloutFormat.RfU64 -> rolloutFormat.RfU64 |> Ok
        | nameof rolloutFormat.RfBs64 -> rolloutFormat.RfBs64 |> Ok
        | _ -> Error(sprintf "no match for rolloutFormat: %s" str)



type rollout =
    | B of booleanRoll
    | U8 of uInt8Roll
    | U16 of uInt16Roll
    | I32 of intRoll
    | U64 of uint64Roll
    | Bs64 of bs64Roll


module Rollout =

    let getRolloutFormat (rollout: rollout) =
        match rollout with
        | B _uBRoll -> rolloutFormat.RfB
        | U8 _uInt8Roll -> rolloutFormat.RfU8
        | U16 _uInt16Roll -> rolloutFormat.RfU16
        | I32 _intRoll -> rolloutFormat.RfI32
        | U64 _uInt64Roll -> rolloutFormat.RfU64
        | Bs64 _bs64Roll -> rolloutFormat.RfBs64

    let getArrayLength (rollout: rollout) =
        match rollout with
        | B _uBRoll -> _uBRoll.arrayLength
        | U8 _uInt8Roll -> _uInt8Roll.arrayLength
        | U16 _uInt16Roll -> _uInt16Roll.arrayLength
        | I32 _intRoll -> _intRoll.arrayLength
        | U64 _uInt64Roll -> _uInt64Roll.arrayLength
        | Bs64 _bs64Roll -> _bs64Roll.arrayLength


    let getArrayCount (rollout: rollout) =
        match rollout with
        | B _uBRoll -> _uBRoll.arrayCount
        | U8 _uInt8Roll -> _uInt8Roll.arrayCount
        | U16 _uInt16Roll -> _uInt16Roll.arrayCount
        | I32 _intRoll -> _intRoll.arrayCount
        | U64 _uInt64Roll -> _uInt64Roll.arrayCount
        | Bs64 _bs64Roll -> _bs64Roll.arrayCount


    let getBitsPerSymbol (rollout: rollout) =
        match rollout with
        | B _uBRoll ->  1 |> UMX.tag<bitsPerSymbol>
        | U8 _uInt8Roll -> _uInt8Roll.bitsPerSymbol
        | U16 _uInt16Roll -> _uInt16Roll.bitsPerSymbol
        | I32 _intRoll -> _intRoll.bitsPerSymbol
        | U64 _uInt64Roll -> _uInt64Roll.bitsPerSymbol
        | Bs64 _bs64Roll ->  1 |> UMX.tag<bitsPerSymbol>


    let getDataBytes (rollout: rollout) =
        match rollout with
        | B _uBRoll ->  
            result {
                let! bp = _uBRoll |> BooleanRoll.toBitPack
                return bp |> BitPack.getData
            }
        | U8 _uInt8Roll -> 
            result {
                let! bp = _uInt8Roll |> Uint8Roll.toBitPack
                return bp |> BitPack.getData
            }
        | U16 _uInt16Roll -> 
            result {
                let! bp = _uInt16Roll |> Uint16Roll.toBitPack
                return bp |> BitPack.getData
            }
        | I32 _intRoll ->
            result {
                let! bp = _intRoll |> IntRoll.toBitPack
                return bp |> BitPack.getData
            }
        | U64 _uInt64Roll -> 
            result {
                let! bp = _uInt64Roll |> Uint64Roll.toBitPack
                return bp |> BitPack.getData
            }
        | Bs64 _bs64Roll ->  
            result {
                let! bp = _bs64Roll |> Bs64Roll.toBitPack
                return bp |> BitPack.getData
            }


    let copy (rollout: rollout) =
        match rollout with
        | B _uBRoll -> _uBRoll |> BooleanRoll.copy |> B
        | U8 _uInt8Roll -> _uInt8Roll |> Uint8Roll.copy |> U8
        | U16 _uInt16Roll -> _uInt16Roll |> Uint16Roll.copy |> U16
        | I32 _intRoll -> _intRoll |> IntRoll.copy |> I32
        | U64 _uInt64Roll -> _uInt64Roll |> Uint64Roll.copy |> U64
        | Bs64 _bs64Roll -> _bs64Roll |> Bs64Roll.copy |> Bs64


    let getRolloutLength (rollout: rollout) =
        (rollout |> getArrayCount |> UMX.untag)
        * (rollout |> getArrayLength |> UMX.untag)


    let fromBoolArrays 
            (rolloutFormat: rolloutFormat) 
            (arrayLength: int<arrayLength>)
            (aas: seq<bool[]>) 
            =
        match rolloutFormat with
        | RfB ->
            result {
                let! roll = BooleanRoll.fromBoolArrays arrayLength aas
                return roll |> rollout.B
            }

        | RfU8 ->
            result {
                let! roll = Uint8Roll.fromBoolArrays arrayLength aas
                return roll |> rollout.U8
            }
        | RfU16 ->
            result {
                let! roll = Uint16Roll.fromBoolArrays arrayLength aas
                return roll |> rollout.U16
            }
        | RfI32 ->
            result {
                let! roll = IntRoll.fromBoolArrays arrayLength aas
                return roll |> rollout.I32
            }
        | RfU64 ->
            result {
                let! roll = Uint64Roll.fromBoolArrays arrayLength aas
                return roll |> rollout.U64
            }
        | RfBs64 ->
            result {
                let! roll = Bs64Roll.fromBoolArrays arrayLength aas
                return roll |> rollout.Bs64
            }


    let fromIntArrays 
            (rolloutFormat: rolloutFormat) 
            (arrayLength: int<arrayLength>) 
            (bitsPerSymbol:int<bitsPerSymbol>)
            (aas: seq<int[]>) 
            =
        match rolloutFormat with
        | RfB ->
            result {
                if (bitsPerSymbol |> UMX.untag) > 1 then
                  return! ("1 bit per symbol max for bool" |> Error)
                else
                    let seq = aas |> Seq.map(fun ia -> ia |> Array.map(fun v -> v>0))
                    let! roll = BooleanRoll.fromBoolArrays arrayLength seq
                    return! roll |> rollout.B |> Ok
            }
        | RfU8 ->
            result {
                if (bitsPerSymbol |> UMX.untag) > 8 then
                  return! ("8 bits per symbol max for uint8" |> Error)
                else
                    let seq = aas |> Seq.map(fun ia -> ia |> Array.map(uint8))
                    let! roll = Uint8Roll.fromArrays arrayLength bitsPerSymbol seq
                    return! roll |> rollout.U8 |> Ok
            }
        | RfU16 ->
            result {
                if (bitsPerSymbol |> UMX.untag) > 16 then
                  return! ("16 bits per symbol max for bool" |> Error)
                else
                    let seq = aas |> Seq.map(fun ia -> ia |> Array.map(uint16))
                    let! roll = Uint16Roll.fromArrays arrayLength bitsPerSymbol seq
                    return! roll |> rollout.U16 |> Ok
            }
        | RfI32 ->
            result {
                if (bitsPerSymbol |> UMX.untag) > 31 then
                  return! ("31 bits per symbol max for Int32" |> Error)
                else
                    let! roll = IntRoll.fromArrays arrayLength bitsPerSymbol aas
                    return! roll |> rollout.I32 |> Ok
            }

        | RfU64 ->
            result {
                if (bitsPerSymbol |> UMX.untag) > 64 then
                  return! ("64 bits per symbol max for uint64" |> Error)
                else
                    let seq = aas |> Seq.map(fun ia -> ia |> Array.map(uint64))
                    let! roll = Uint64Roll.fromArrays arrayLength bitsPerSymbol seq
                    return! roll |> rollout.U64 |> Ok
            }
        | RfBs64 ->
            result {
                if (bitsPerSymbol |> UMX.untag) > 1 then
                  return! ("1 bits per symbol max for Bs64" |> Error)
                else
                    let seq = aas |> Seq.map(fun ia -> ia |> Array.map(fun v -> v>0))
                    let! roll = Bs64Roll.fromBoolArrays arrayLength seq
                    return! roll |> rollout.Bs64 |> Ok
            }


    let createEmpty =
        BooleanRoll.createEmpty |> rollout.B


    let isSorted (rollout: rollout) =
        match rollout with
        | B _uBRoll -> _uBRoll |> BooleanRoll.isSorted
        | U8 _uInt8Roll -> _uInt8Roll |> Uint8Roll.isSorted
        | U16 _uInt16Roll -> _uInt16Roll |> Uint16Roll.isSorted
        | I32 _intRoll -> _intRoll |> IntRoll.isSorted
        | U64 _uInt64Roll -> _uInt64Roll |> Uint64Roll.isSorted
        | Bs64 _bs64Roll -> _bs64Roll |> Bs64Roll.isSorted


    let toIntArrays (rollout: rollout) =
        match rollout with
        | B _uBRoll -> _uBRoll |> BooleanRoll.toIntArrays
        | U8 _uInt8Roll -> _uInt8Roll |> Uint8Roll.toIntArrays
        | U16 _uInt16Roll -> _uInt16Roll |> Uint16Roll.toIntArrays
        | I32 _intRoll -> _intRoll |> IntRoll.toArrays
        | U64 _uInt64Roll -> _uInt64Roll |> Uint64Roll.toIntArrays
        | Bs64 _bs64Roll -> _bs64Roll |> Bs64Roll.toIntArrays


    let toBoolArrays (rollout: rollout) =
        match rollout with
        | B _uBRoll -> _uBRoll |> BooleanRoll.toBoolArrays
        | U8 _uInt8Roll -> _uInt8Roll |> Uint8Roll.toBoolArrays
        | U16 _uInt16Roll -> _uInt16Roll |> Uint16Roll.toBoolArrays
        | I32 _intRoll -> _intRoll |> IntRoll.toBoolArrays
        | U64 _uInt64Roll -> _uInt64Roll |> Uint64Roll.toBoolArrays
        | Bs64 _bs64Roll -> _bs64Roll |> Bs64Roll.toBoolArrays


    let uniqueMembers (rollout: rollout) =
        match rollout with
        | B _uBRoll -> _uBRoll |> BooleanRoll.uniqueMembers |> Result.map B
        | U8 _uInt8Roll -> _uInt8Roll |> Uint8Roll.uniqueMembers |> Result.map U8
        | U16 _uInt16Roll -> _uInt16Roll |> Uint16Roll.uniqueMembers |> Result.map U16
        | I32 _intRoll -> _intRoll |> IntRoll.uniqueMembers |> Result.map I32
        | U64 _uInt64Roll -> _uInt64Roll |> Uint64Roll.uniqueMembers |> Result.map U64
        | Bs64 _bs64Roll -> _bs64Roll |> Bs64Roll.uniqueMembers |> Result.map Bs64


    let uniqueUnsortedMembers (rollout: rollout) =
        match rollout with
        | B _uBRoll -> _uBRoll |> BooleanRoll.uniqueUnsortedMembers |> Result.map B
        | U8 _uInt8Roll -> _uInt8Roll |> Uint8Roll.uniqueUnsortedMembers |> Result.map U8
        | U16 _uInt16Roll -> _uInt16Roll |> Uint16Roll.uniqueUnsortedMembers |> Result.map U16
        | I32 _intRoll -> _intRoll |> IntRoll.uniqueUnsortedMembers |> Result.map I32
        | U64 _uInt64Roll -> _uInt64Roll |> Uint64Roll.uniqueUnsortedMembers |> Result.map U64
        | Bs64 _bs64Roll -> _bs64Roll |> Bs64Roll.uniqueUnsortedMembers |> Result.map Bs64



    //let fromUInt64ArraySeq 
    //        (rolloutFormat: rolloutFormat)
    //        (arrayLength: arrayLength) 
    //        (aas: seq<uint64[]>) 
    //        =
    //    let intSeq =
    //        aas
    //        |> Seq.concat
    //        |> Seq.map (int)
    //        |> Seq.chunkBySize (arrayLength |> ArrayLength.value)

    //    match rolloutFormat with
    //    | RfB ->
    //        result {
    //            let! roll = BooleanRoll.fromIntArrays arrayLength intSeq
    //            return roll |> rollout.B
    //        }
    //    | RfU8 ->
    //        result {
    //            let! roll = Uint8Roll.fromIntArrays arrayLength intSeq
    //            return roll |> rollout.U8
    //        }
    //    | RfU16 ->
    //        result {
    //            let! roll = Uint16Roll.fromIntArrays arrayLength intSeq
    //            return roll |> rollout.U16
    //        }
    //    | RfI32 ->
    //        result {
    //            let! roll = IntRoll.fromIntArrays arrayLength intSeq
    //            return roll |> rollout.I32
    //        }
    //    | RfU64 ->
    //        result {
    //            let! roll = Uint64Roll.fromUint64Arrays arrayLength aas
    //            return roll |> rollout.U64
    //        }
    //    | RfBs64 -> failwith "not implemented"



    let fromBitPack 
        (rolloutFormat: rolloutFormat) 
        (arrayLength: int<arrayLength>) 
        (bitPack: bitPack) 
        =
        match rolloutFormat with
        | RfB ->
            result {
                let! roll = BooleanRoll.fromBitPack arrayLength bitPack
                return roll |> rollout.B
            }
        | RfU8 ->
            result {
                let! roll = Uint8Roll.fromBitPack arrayLength bitPack
                return roll |> rollout.U8
            }
        | RfU16 ->
            result {
                let! roll = Uint16Roll.fromBitPack arrayLength bitPack
                return roll |> rollout.U16
            }
        | RfI32 ->
            result {
                let! roll = IntRoll.fromBitPack arrayLength bitPack
                return roll |> rollout.I32
            }
        | RfU64 ->
            result {
                let! roll = Uint64Roll.fromBitPack arrayLength bitPack
                return roll |> rollout.U64
            }
        | RfBs64 ->
            result {
                let! roll = Bs64Roll.fromBitPack arrayLength bitPack
                return roll |> rollout.Bs64
            }


    let toBitPack 
        (bitsPerSymbl: int<bitsPerSymbol>) 
        (rollout: rollout) 
        =
        match rollout with
        | B _uBRoll -> _uBRoll |> BooleanRoll.toBitPack
        | U8 _uInt8Roll -> _uInt8Roll |> Uint8Roll.toBitPack
        | U16 _uInt16Roll -> _uInt16Roll |> Uint16Roll.toBitPack
        | I32 _intRoll -> _intRoll |> IntRoll.toBitPack
        | U64 _uInt64Roll -> _uInt64Roll |> Uint64Roll.toBitPack
        | Bs64 _bs64Roll -> _bs64Roll |> Bs64Roll.toBitPack
