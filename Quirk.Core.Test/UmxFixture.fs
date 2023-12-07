namespace Quirk.Core.Test

open System
open Xunit
open FSharp.UMX
open Quirk.Core


module UomFixture =


    [<Measure>] type skuId
    [<Measure>] type km
    [<Measure>] type foo

    [<Fact>]
    let ``multIntUomByInt``() =
        let fmv = 42
        let fm = UMX.tag<km> fmv
        let m = 2
        let fmp = fm |> Uoms.multIntUomByInt m
        let fmpv = fmp |> UMX.untag
        Assert.Equal(84, fmpv)


    type Record =
        {
            skuId : string<skuId>
            foo : string<foo>
            distance : int<km>
        }

    // Tests intentionally without assertions

    [<Fact>]
    let ``Simple unit of measure conversions``() =
        let x = Guid.NewGuid() |> UMX.tag<skuId>
        let y = (UMX.untag x).ToString("N") |> UMX.tag<skuId>
        let z = UMX.tag<km> 42
        let f = UMX.tag<km>  10.0f
        let w = sprintf "%O %s %d %f" (UMX.untag x) (UMX.untag y) z f
        let w2 = sprintf "%O %O %d %f" x y z f
        ()

    [<Fact>]
    let ``Simple unit of measure conversions with cast operator``() =
        let x : Guid<skuId> = % Guid.NewGuid()
        let y : string<skuId> = % (%x).ToString()
        let z : int<km> = % 42
        let w : string<foo> = % sprintf "%O %s %d" %x %y %z
        let b : byte<foo> = % 1uy
        let s : int16<foo> = % 1s
        let f : float32<foo>  = % 10.0f
        let d : DateTime<foo> = % DateTime.Now
        //let don : DateOnly<foo> = % DateOnly.FromDateTime(DateTime.Now)
        //let ton : TimeOnly<foo> = % TimeOnly.FromDateTime(DateTime.Now)
        ()

    [<Fact>]
    let ``Simple unit of measure conversions with UMX.tag function``() =
        let x = UMX.tag<skuId> (Guid.NewGuid())              
        let y = UMX.tag<skuId> ((%x).ToString())             
        let z = UMX.tag<km> (42)                          
        let w = UMX.tag<foo> (sprintf "%O %s %d" %x %y %z) 
        let b = UMX.tag<foo> (1uy)                         
        let s = UMX.tag<foo> (1s)                         
        let f = UMX.tag<foo> (10.0f)
        let d = UMX.tag<foo> DateTime.Now
        //let don = UMX.tag<foo> (DateOnly.FromDateTime(DateTime.Now))
        //let ton = UMX.tag<foo> (TimeOnly.FromDateTime(DateTime.Now))
        ()

    [<Fact>]
    let ``Record with units of measure``() =
        let record = { skuId = % "skuId" ; foo = % "foo" ; distance = % 42 }
        let x = sprintf "%s %s %d" %record.skuId %record.foo %record.distance
        ()


    [<Measure>] type Meter
    [<Measure>] type Kilometer

    let createArray () =
        let sizeInMeters = 100.0<Meter>
        let arraySize = int sizeInMeters // Convert size to int for array creation
        Array.zeroCreate<float> arraySize

    let setArrayValue (array: float[]) (index: int<Meter>) (value: float) =
        array.[(UMX.untag index)] <- value

    let getArrayValue (array: float[]) (index: int<Meter>) =
        array.[(UMX.untag index)]