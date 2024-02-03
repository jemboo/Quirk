namespace Quirk.Core
open Microsoft.FSharp.Core
open System
open System.Security.Cryptography

module GuidUtils =

    let fromUint32s 
            (g1: uint32) 
            (g2: uint32) 
            (g3: uint32) 
            (g4: uint32) =
        let pc0 = System.BitConverter.GetBytes(g1)
        let pc1 = System.BitConverter.GetBytes(g2)
        let pc2 = System.BitConverter.GetBytes(g3)
        let pc3 = System.BitConverter.GetBytes(g4)

        let woof =
            seq {
                pc0.[0]; pc0.[1]; pc0.[2]; pc0.[3]
                pc1.[0]; pc1.[1]; pc1.[2]; pc1.[3]
                pc2.[0]; pc2.[1]; pc2.[2]; pc2.[3]
                pc3.[0]; pc3.[1]; pc3.[2]; pc3.[3]
            }
            |> Seq.toArray

        new System.Guid(woof)


    let fromUint64s 
            (g1: uint64) 
            (g2: uint64) =
        let pc0 = System.BitConverter.GetBytes(g1)
        let pc1 = System.BitConverter.GetBytes(g2)

        let woof =
            seq {
                pc0.[0]; pc0.[1]; pc0.[2]; pc0.[3]
                pc0.[4]; pc0.[5]; pc0.[6]; pc0.[7]
                pc1.[0]; pc1.[1]; pc1.[2]; pc1.[3]
                pc1.[4]; pc1.[5]; pc1.[6]; pc1.[7]
            }
            |> Seq.toArray

        new System.Guid(woof)

    let toUint32s (guy:Guid) =
        let bytes = guy.ToByteArray()
        let uints = Array.zeroCreate<uint32> 4
        Buffer.BlockCopy(bytes, 0, uints, 0, 16)
        uints


    let toUint64s (guy:Guid) =
        let bytes = guy.ToByteArray()
        let uints = Array.zeroCreate<uint64> 2
        Buffer.BlockCopy(bytes, 0, uints, 0, 16)
        uints

    let toUint64 (guy:Guid) =
        let uints = toUint64s guy
        uints.[0] + uints.[1]


    let guidFromBytes (ba: byte[]) = new Guid(ba)


    let addGuids (g1: Guid) (g2: Guid) =
        let pcs1 = g1.ToByteArray()
        let pcs2 = g2.ToByteArray()
        let pcsS = Array.init 16 (fun i -> pcs1.[i] + pcs2.[i])
        new System.Guid(pcsS)


    let addGuidsOpt (g1: Guid option) (g2: Guid option) =
        match g1, g2 with
        | Some v1, Some v2 -> addGuids v1 v2
        | None, Some v2 -> v2
        | Some v1, None -> v1
        | None, None -> Guid.Empty


    let hashBytes (objs: seq<byte>) =
        let md5 = MD5.Create()
        System.Guid(md5.ComputeHash(objs |> Seq.toArray))


    // structural equality
    let guidFromObjs (objs: seq<obj>) = 
        System.Guid(ByteUtils.hashObjs objs)


    let guidFromStringR (gstr: string) =
        let mutable gv = Guid.NewGuid()

        match Guid.TryParse(gstr, &gv) with
        | true -> gv |> Ok
        | false -> "not a guid: " + gstr |> Result.Error


    let guidFromStringOpt (gstr: string) =
        let mutable gv = Guid.NewGuid()

        match Guid.TryParse(gstr, &gv) with
        | true -> gv |> Some
        | false -> None





    /// ***********************************************************
    /// ******************** Guid[] <-> byte[] ********************
    /// ***********************************************************

    let getGuidFromBytes (offset: int) (blob: byte[]) =
        try
            guidFromBytes blob.[offset .. offset + 15] |> Ok
        with ex ->
            ("error in getGuidFromBytes: (*16)" + ex.Message) |> Result.Error


    let mapBytesToGuids (blob_offset: int) (guidA: Guid[]) (guidA_offset: int) (guid_ct: int) (blob: byte[]) =
        try
            for i = guidA_offset to (guidA_offset + guid_ct - 1) do
                let gu = guidFromBytes blob.[(i * 16 + blob_offset) .. (i * 16 + 15 + blob_offset)]
                guidA.[i] <- gu

            guidA |> Ok
        with ex ->
            ("error in mapBytesToGuids: (*17)" + ex.Message) |> Result.Error


    let mapGuidsToBytes (guidA_offset: int) (guid_ct: int) (blob: byte[]) (blob_offset: int) (guidA: Guid[]) =
        try
            for i = guidA_offset to (guidA_offset + guid_ct - 1) do
                let gByts = guidA.[i].ToByteArray()
                let blobOffset = blob_offset + (i - guidA_offset) * 16
                Buffer.BlockCopy(gByts, 0, blob, blobOffset, 16)

            blob |> Ok
        with ex ->
            ("error in mapGuidsToBytes: (*18)" + ex.Message) |> Result.Error


    let convertBytesToGuids (blob: byte[]) =
        try
            let guidA = Array.zeroCreate<Guid> (blob.Length / 16)
            blob |> mapBytesToGuids 0 guidA 0 (blob.Length / 16)
        with ex ->
            ("error in convertBytesToGuids: (*19)" + ex.Message) |> Result.Error


    let convertGuidsToBytes (guidA: Guid[]) =
        try
            let blob = Array.zeroCreate<byte> (guidA.Length * 16)
            guidA |> mapGuidsToBytes 0 guidA.Length blob 0
        with ex ->
            ("error in convertGuidsToBytes: (*20)" + ex.Message) |> Result.Error
