namespace Quirk.Core
open FSharp.UMX
open System

// Rando
[<Measure>] type randomSeed
module RandomSeed =
    let fromNow () = DateTime.Now.Ticks |> uint64 |> UMX.tag<randomSeed>
    let fromGuid (gu:Guid) = 
        gu |> GuidUtils.toUint64 |> UMX.tag<randomSeed>



type rngType =
    | Lcg
    | Net

module RngType =
    let fromString (cereal: string) =
        match cereal with
        | "Lcg" -> rngType.Lcg |> Ok
        | "Net" -> rngType.Net |> Ok
        | _ -> $"Invalid string: { cereal } for rngType" |> Error
        
    let toString (rngType: rngType) =
        rngType |> string



type rngGen = private { rngType:rngType; seed:uint64<randomSeed> }
module RngGen =
    let create (rngTyp: rngType) (seed: uint64<randomSeed>) = { rngType = rngTyp; seed = seed }
    let getType (rgn:rngGen) =  rgn.rngType
    let getSeed (rgn:rngGen) =  rgn.seed
    let createLcg (seed: uint64<randomSeed>) = create rngType.Lcg seed
    let createNet (seed: uint64<randomSeed>) = create rngType.Net seed
    let fromNow (rngType:rngType) = 
        match rngType with
        | Lcg -> RandomSeed.fromNow () |> createLcg
        | Net -> RandomSeed.fromNow () |> createNet

    let fromGuid (rngType:rngType) (gu:Guid) =
        let seed = gu |> GuidUtils.toUint64 |> UMX.tag<randomSeed>
        match rngType with
        | Lcg -> seed |> createLcg
        | Net -> seed |> createNet


    let toStringArray (rg:rngGen) =
        [
            "RngGen"
            rg |> getType |> string
            rg |> getSeed |> string
        ]

    let fromStrings rgt seed =
        result {
            let! rngType = rgt |> RngType.fromString
            let! seedVal = StringUtil.parseUint64 seed
            let rndSeed = seedVal |> UMX.tag<randomSeed>
            return create rngType rndSeed
        }
        

type IRando =
    abstract member Count: int
    abstract member Seed: uint64<randomSeed>
    abstract member NextInt: int -> int
    abstract member NextUInt: unit -> uint32
    abstract member NextPositiveInt: unit -> int32
    abstract member NextULong: unit -> uint64
    abstract member NextFloat: unit -> float
    abstract member NextGuid: unit -> Guid
    abstract member rngType: rngType



type randomNet(seed: uint64<randomSeed>) =
    let mutable _count = 0
    let rnd = new System.Random(seed |> UMX.untag |> int)

    interface IRando with
        member this.Seed = seed
        member this.Count = _count

        member this.NextUInt () =
            _count <- _count + 2
            let vv = (uint32 (rnd.Next()))
            vv + (uint32 (rnd.Next()))

        member this.NextPositiveInt () = 
            _count <- _count + 1
            rnd.Next()

        member this.NextInt (modulus:int) =
            let r = this :> IRando
            (r.NextPositiveInt()) % modulus

        member this.NextULong () =
            let r = this :> IRando
            let vv = uint64 (r.NextUInt())
            (vv <<< 32) + (uint64 (r.NextUInt()))

        member this.NextGuid () =
            let r = this :> IRando
            let vv0 = r.NextUInt()
            let vv1 = r.NextUInt()
            let vv2 = r.NextUInt()
            let vv3 = r.NextUInt()
            GuidUtils.fromUint32s vv0 vv1 vv2 vv3


        member this.NextFloat () =
            _count <- _count + 1
            rnd.NextDouble()

        member this.rngType = rngType.Net


type randomLcg(seed: uint64<randomSeed>) =
    let _a = 6364136223846793005UL
    let _c = 1442695040888963407UL
    let mutable _last = (_a * (uint64 (seed |> UMX.untag)) + _c)
    let mutable _count = 0
    member this.Seed = seed
    member this.Count = _count

    member this.NextUInt =
        _count <- _count + 1
        _last <- ((_a * _last) + _c)
        (uint32 (_last >>> 32))

    member this.NextULong =
        let mm = ((_a * _last) + _c)
        _last <- ((_a * mm) + _c)
        _count <- _count + 2
        _last + (mm >>> 32)

    member this.NextFloat =
        (float this.NextUInt) / (float Microsoft.FSharp.Core.uint32.MaxValue)

    interface IRando with
        member this.Seed = this.Seed
        member this.Count = _count
        member this.NextUInt () = this.NextUInt
        member this.NextPositiveInt () = int (this.NextUInt >>> 1)
        member this.NextInt (modulus:int) =
            ( int (this.NextUInt >>> 1)) % modulus
        member this.NextULong () = this.NextULong
        
        member this.NextGuid () =
            let r = this :> IRando
            let vv0 = r.NextULong()
            let vv1 = r.NextULong()
            GuidUtils.fromUint64s vv0 vv1

        member this.NextFloat () = this.NextFloat
        member this.rngType = rngType.Lcg


module Rando =

    let create rngtype seed =
        match rngtype with
        | rngType.Lcg -> new randomLcg(seed) :> IRando
        | rngType.Net -> new randomNet(seed) :> IRando
        | _ -> failwith "not handled in Rando.create"

    let fromRngGen (rg: rngGen) = create rg.rngType rg.seed

    let nextRando (randy: IRando) =
          create randy.rngType ((randy.NextULong () ) |> UMX.tag<randomSeed> )

    let toRngGen (randy: IRando) =
          RngGen.create randy.rngType ((randy.NextULong () ) |> UMX.tag<randomSeed> )

    let nextRngGen (rg: rngGen) =
        rg |> fromRngGen |> toRngGen


    let toMoreRngGens (rg: rngGen) =
        let randy = fromRngGen rg
        seq {
            while true do
                randy |> toRngGen
        }

    let indexedRngGen 
            (index:int) 
            (rg: rngGen) 
        =
        let randy = fromRngGen rg
        for dex = 0 to index do
            let discard = randy.NextPositiveInt
            ()
        toRngGen randy



type rngGenProviderId = private RngGenProviderId of Guid
module RngGenProviderId =
    let value (RngGenProviderId v) = v
    let create vl = RngGenProviderId vl

type rngGenProvider = 
    private {
            id: rngGenProviderId;
            rngGen: rngGen
            randy: IRando
        }

module RngGenProvider =

    let load (id:rngGenProviderId) 
             (rngGen:rngGen)
        =
        {
            id = id;
            rngGen = rngGen;
            randy = rngGen |> Rando.fromRngGen
        }

    let makeId (rngGen:rngGen) = 
        [|
            rngGen :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> RngGenProviderId.create

    let make (rngGen:rngGen) = 
        load (makeId rngGen) rngGen

    let getId (rngGenProvider:rngGenProvider) 
        =  rngGenProvider.id

    let getFixedRngGen(rngGenProvider:rngGenProvider) 
        =  rngGenProvider.rngGen

    let nextRngGen (rngGenProvider:rngGenProvider) 
        =  rngGenProvider.randy |> Rando.toRngGen













