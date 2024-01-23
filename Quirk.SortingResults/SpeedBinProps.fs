namespace Quirk.SortingResults

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting


type sorterSpeedBinProps =
     | ErrorMsg 
     | BestSwitchCt
     | BestStageCt
     | BestFitness
     | BestEntropy
     | AveSwitchCt
     | AveStageCt
     | AveFitness
     | DevSwitchCt
     | DevStageCt
     | DevFitness
     | PhenotypeEntropy
     | BestSwitchCtM
     | BestStageCtM
     | BestFitnessM
     | BestEntropyM
     | AveSwitchCtM
     | AveStageCtM
     | AveFitnessM
     | DevSwitchCtM
     | DevStageCtM
     | DevFitnessM
     | PhenotypeEntropyM





module SpeedBinProps 
       =
    let getMapForSpeedBinType 
            (ssbs:sorterSpeedBinSet) 
            (ssbt:sorterSpeedBinType) 
        = 
        ssbs |> SorterSpeedBinSet.getBinMap
             |> Map.toSeq
             |> Seq.filter(fun (k, v) -> k.sorterSpeedBinType = ssbt)
             |> Map.ofSeq

    let getKvpWithMinStages
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>, int<sorterCount>>>) =
        binMap |> Map.toSeq |> Seq.minBy(fun (k,v) -> k.sorterSpeed.stageCt |> UMX.untag) 


    let getKvpWithMinSwitches
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>) =
        binMap |> Map.toSeq |> Seq.minBy(fun (k,v) -> k.sorterSpeed.switchCt |> UMX.untag)


    let orderByFitnessDesc
            (stageWeight: float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>) 
        =
        binMap |> Map.toArray
               |> Array.map(
               fun (k,v) -> ((k,v), k.sorterSpeed |> SorterFitness.fromSpeed stageWeight))
               |> Array.sortByDescending(snd >> UMX.untag)

    let getBestSorterSpeed
            (stageWeight: float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        orderByFitnessDesc stageWeight binMap  
        |> Seq.head |> fst |> fst |> SorterSpeedBinKey.getSorterSpeed


    let getBestFitness
            (stageWeight: float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
        if yab.Length = 0 then
            0.0
        else
            yab |> Seq.head |> snd |> UMX.untag


    let getBestStageCount
            (stageWeight: float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
        if yab.Length = 0 then
            0
        else
            yab |> Seq.head |> fst |> fst |> SorterSpeedBinKey.getSorterSpeed 
                |> SorterSpeed.getStageCount |> UMX.untag


    let getBestSwitchCount
            (stageWeight: float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
        if yab.Length = 0 then
            0
        else
            yab |> Seq.head |> fst |> fst |> SorterSpeedBinKey.getSorterSpeed 
                |> SorterSpeed.getSwitchCount |> UMX.untag



    let getBestSpeedBinEntropy
            (stageWeight: float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
        if yab.Length = 0 then
            0.0
        else
            yab |> Seq.head |> fst |> snd |> Map.toArray |> Array.map(snd >> UMX.untag)
                |> Combinatorics.entropyOfInts



    let getAllSorterSpeedBinKeysWithCounts
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        binMap |> Map.toArray
               |> Array.map(
               fun (k,v) -> ((k, v |> Map.toSeq |> Seq.sumBy(snd >> UMX.untag))))


    let getAveFitness
            (stageWeight:float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterFitness.fromSpeed stageWeight 
                 |> UMX.untag,
                ct))
            |> CollectionProps.weightedAverage


    let getAveSwitchCount
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterSpeed.getSwitchCount 
                 |> UMX.untag, ct))
        |> CollectionProps.weightedAverage


    let getAveStageCount
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterSpeed.getStageCount 
                 |> UMX.untag, ct))
        |> CollectionProps.weightedAverage


    let getStDevSwitchCount
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterSpeed.getSwitchCount 
                 |> UMX.untag, ct))
        |> CollectionProps.weightedStdDeviationS


    let getStDevStageCount
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterSpeed.getStageCount 
                 |> UMX.untag, ct))
        |> CollectionProps.weightedStdDeviationS


    let getStDevFitness
            (stageWeight: float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterFitness.fromSpeed stageWeight 
                 |> UMX.untag,
                ct))
            |> CollectionProps.weightedStdDeviationS


    let getPhenotypeEntropy
            (stageWeight:float<stageWeight>)
            (binMap:Map<sorterSpeedBinKey, Map<Guid<sorterPhenotypeId>,int<sorterCount>>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
                    |> Seq.map(fst >> snd >> Map.toArray >> Array.map(snd >> UMX.untag))
                    |> Seq.concat |> Seq.toArray
        yab |> Combinatorics.entropyOfInts



    
    let getAllProps =
        [
            sorterSpeedBinProps.AveFitness;
            sorterSpeedBinProps.AveStageCt;
            sorterSpeedBinProps.AveSwitchCt;
            sorterSpeedBinProps.BestEntropy;
            sorterSpeedBinProps.BestFitness;
            sorterSpeedBinProps.BestStageCt;
            sorterSpeedBinProps.BestSwitchCt;
            sorterSpeedBinProps.DevFitness;
            sorterSpeedBinProps.DevStageCt;
            sorterSpeedBinProps.DevSwitchCt;
            sorterSpeedBinProps.PhenotypeEntropy;
            sorterSpeedBinProps.AveFitnessM;
            sorterSpeedBinProps.AveStageCtM;
            sorterSpeedBinProps.AveSwitchCtM;
            sorterSpeedBinProps.BestEntropyM;
            sorterSpeedBinProps.BestFitnessM;
            sorterSpeedBinProps.BestStageCtM;
            sorterSpeedBinProps.BestSwitchCtM;
            sorterSpeedBinProps.DevFitnessM;
            sorterSpeedBinProps.DevStageCtM;
            sorterSpeedBinProps.DevSwitchCtM;
            sorterSpeedBinProps.PhenotypeEntropyM;
        ]
        
    let getHeader () =
        getAllProps 
            |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""



    let getAllProperties
            (stageWeight:float<stageWeight>)
            (ssbs:sorterSpeedBinSet) 
        =
        let parentType = "sorterSetParent" |> SorterSpeedBinType.create
        let mutantType = "sorterSetMutated" |> SorterSpeedBinType.create
        let parentBins = getMapForSpeedBinType ssbs parentType
        let mutantBins = getMapForSpeedBinType ssbs mutantType

        let aveFitness = getAveFitness stageWeight parentBins
        let aveStageCt = getAveStageCount parentBins
        let aveSwitchCt = getAveSwitchCount parentBins
        let bestEntropy = getBestSpeedBinEntropy stageWeight parentBins
        let bestFitness = getBestFitness stageWeight parentBins
        let bestStageCt = getBestStageCount stageWeight parentBins
        let bestSwitchCt = getBestSwitchCount stageWeight parentBins
        let devFitness = getStDevFitness stageWeight parentBins
        let devStageCt = getStDevStageCount parentBins
        let devSwitchCt = getStDevSwitchCount parentBins
        let phenotypeEntropy = getPhenotypeEntropy stageWeight parentBins
        let aveFitnessM = getAveFitness stageWeight mutantBins
        let aveStageCtM = getAveStageCount mutantBins
        let aveSwitchCtM = getAveSwitchCount mutantBins
        let bestEntropyM = getBestSpeedBinEntropy stageWeight mutantBins
        let bestFitnessM = getBestFitness stageWeight mutantBins
        let bestStageCtM = getBestStageCount stageWeight mutantBins
        let bestSwitchCtM = getBestSwitchCount stageWeight mutantBins
        let devFitnessM = getStDevFitness stageWeight mutantBins
        let devStageCtM = getStDevStageCount mutantBins
        let devSwitchCtM = getStDevSwitchCount mutantBins
        let phenotypeEntropyM = getPhenotypeEntropy stageWeight mutantBins

        [
         aveFitness;
         aveStageCt;
         aveSwitchCt;
         bestEntropy;
         bestFitness;
         bestStageCt;
         bestSwitchCt;
         devFitness;
         devStageCt;
         devSwitchCt;
         phenotypeEntropy;
         aveFitnessM;
         aveStageCtM;
         aveSwitchCtM;
         bestEntropyM;
         bestFitnessM;
         bestStageCtM;
         bestSwitchCtM;
         devFitnessM;
         devStageCtM;
         devSwitchCtM;
         phenotypeEntropyM;
        ] |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""
