namespace Quirk.Iter
open FSharp.UMX
open Quirk.Core

[<Measure>] type generation
[<Measure>] type reproductionRate

module Generation =
    let next (gen:int<generation>) = ((gen |> UMX.untag) + 1) |> UMX.tag<generation>
    let add ct (gen:int<generation>) = ((gen |> UMX.untag) + ct) |> UMX.tag<generation>
    let addG 
            (a:int<generation>) 
            (b:int<generation>) 
        = 
        ((a |> UMX.untag) + (b |> UMX.untag)) |> UMX.tag<generation>

    let binnedValue 
            (binSz:int) 
            (gen:int<generation>) 
        =
        let gv = gen |> UMX.untag
        gv - 1 - ((gv - 1) % binSz)

type modGenerationFilter = {modulus:int}

type expGenerationFilter = {exp:float}

type generationFilter =
    | ModF of modGenerationFilter
    | ExpF of expGenerationFilter

module GenerationFilter =
    let passing 
            (gen:int<generation>) 
            (filt:generationFilter) 
        =
        match filt with
        | ModF mgf -> ((gen |> UMX.untag) % mgf.modulus) = 0
        | ExpF egf -> IntSeries.expoB egf.exp (gen |> UMX.untag)