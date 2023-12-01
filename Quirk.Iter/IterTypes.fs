namespace Quirk.Core
open FSharp.UMX

[<Measure>] type generation
[<Measure>] type reproductionRate


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