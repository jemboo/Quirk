namespace Quirk.Core
open FSharp.UMX

[<Measure>] type cfgPlexItemName
[<Measure>] type cfgPlexItemRank
[<Measure>] type cfgPlexName


[<Measure>] type replicaNumber



type gaCfgPlex =
   {
        name:string<cfgPlexName>
        orders:int<order>[]
        multiplicationRates:float<reproductionRate>[]
        mutationRates:float<mutationRate>[]
        noiseFractions:float<noiseFraction>[]
        rngTypes:rngType[]
        parentCounts:int<sorterCount>[]
        sorterSetPruneMethods:sorterSetPruneMethod[]
        stageWeights:float<stageWeight>[]
        switchGenModes:switchGenMode[]
    }