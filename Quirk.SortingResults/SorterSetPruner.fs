namespace Quirk.SortingResults

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open System.Text.RegularExpressions


type sorterSetPruner = 
    private 
        {
            id: Guid<sorterSetPrunerId>;
            prunedCount:int<sorterCount>;
            noiseFraction:float<noiseFraction> option;
            stageWeight:float<stageWeight>;
        }

module SorterSetPruner =

    let getId
            (sorterSetPruner:sorterSetPruner) 
         =
         sorterSetPruner.id

    let getPrunedCount
             (sorterSetPruner:sorterSetPruner) 
         =
         sorterSetPruner.prunedCount

    let getNoiseFraction
             (sorterSetPruner:sorterSetPruner) 
         =
         sorterSetPruner.noiseFraction

    let getStageWeight
                (sorterSetPruner:sorterSetPruner) 
         =
         sorterSetPruner.stageWeight

    let load
            (id:Guid<sorterSetPrunerId>)
            (prunedCount:int<sorterCount>)
            (noiseFraction:float<noiseFraction> option)
            (stageWeight:float<stageWeight>)
        =
        {   
            id=id
            prunedCount=prunedCount
            noiseFraction=noiseFraction
            stageWeight=stageWeight
        }


    let makeId
            (prunedCount:int<sorterCount>)
            (stageWeight:float<stageWeight>) 
            (noiseFraction:float<noiseFraction> option)
        =
        [|
            "sorterSetPrunerWhole" :> obj
            stageWeight |> UMX.untag :> obj; 
            noiseFraction |> Option.map(UMX.untag) :> obj; 
            prunedCount |> UMX.untag :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<sorterSetPrunerId>


    let make (prunedCount:int<sorterCount>)
             (noiseFraction:float<noiseFraction> option)
             (stageWeight:float<stageWeight>)
        =
        {
            id = makeId prunedCount stageWeight noiseFraction;
            prunedCount = prunedCount;
            noiseFraction = noiseFraction;
            stageWeight = stageWeight; 
        }


    let makePrunedSorterSetId
                (sorterSetPrunerId:Guid<sorterSetPrunerId>) 
                (sorterSetPruneMethod:sorterSetPruneMethod)
                (sorterSetIdParent:Guid<sorterSetId>) 
                (sorterSetIdChild:Guid<sorterSetId>)
                (stageWeight:float<stageWeight>)
                (noiseFraction:float<noiseFraction> option)
                (rngGen:rngGen) 
         =
        [|
            sorterSetPrunerId |> UMX.untag :> obj
            sorterSetPruneMethod |> SorterSetPruneMethod.toReport :> obj
            sorterSetIdParent |> UMX.untag :> obj;
            sorterSetIdChild |> UMX.untag :> obj;
            stageWeight |> UMX.untag :> obj
            noiseFraction :> obj
            rngGen :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<sorterSetId>


    let makePrunedSorterSetEvalId
                (sorterSetPrunerId:Guid<sorterSetPrunerId>)
                (sorterSetPruneMethod:sorterSetPruneMethod)
                (sorterSetEvalIdParent:Guid<sorterSetEvalId>) 
                (sorterSetEvalIdChild:Guid<sorterSetEvalId>)
                (rngGen:rngGen) 
         =
        [|
            sorterSetPrunerId |> UMX.untag :> obj;
            sorterSetPruneMethod |> SorterSetPruneMethod.toReport :> obj
            sorterSetEvalIdParent |> UMX.untag :> obj;
            sorterSetEvalIdChild |> UMX.untag :> obj;
            rngGen :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<sorterSetEvalId>


    let getSigmaSelection 
            (sorterEvalsWithFitness:(sorterEval*float<sorterFitness>)[])
            (sigmaRatio:float)
            (rngGen:rngGen)
        =
            let deviation = 
                sorterEvalsWithFitness 
                |> Array.map(snd >> UMX.untag)
                |> CollectionProps.stdDeviation

            let noiseLevel = deviation * sigmaRatio

            let randy = rngGen |> Rando.fromRngGen
            let noiseMaker = RandVars.gaussianDistribution 0.0 noiseLevel randy

            let yab =
                sorterEvalsWithFitness 
                |> Seq.zip noiseMaker 
                |> Seq.map(fun (n, (srtrEval, srtrFitness)) ->
                                ((srtrEval, srtrFitness), n + (srtrFitness |> UMX.untag)) )
                |> Seq.sortByDescending(snd)
                |> Seq.toArray
            yab |> Array.map(fun ((sEv, sFt), npFt) -> sEv)



    let runWholePrune
            (sorterSetPruner:sorterSetPruner)
            (rngGen:rngGen)
            (sorterEvalsToPrune:sorterEval[])
         =
            let stageWgt = getStageWeight sorterSetPruner
            let sorterEvalsWithFitness = 
                sorterEvalsToPrune 
                |> Array.filter(SorterEval.getSorterSpeed >> Option.isSome)
                |> Array.filter(SorterEval.failedForSure >> not)
                |> Array.map(fun sEv -> 
                     ( sEv,
                       sEv |> SorterEval.getSorterSpeed |> Option.get |> SorterFitness.fromSpeed stageWgt
                     )
                   )
            let rankedSorters =
                if (sorterEvalsWithFitness.Length = 0) then
                    [||]
                elif sorterSetPruner.noiseFraction |> Option.isSome then
                    getSigmaSelection 
                        sorterEvalsWithFitness 
                        (sorterSetPruner.noiseFraction |> NoiseFraction.toFloat) 
                        rngGen

                else
                    let yab = sorterEvalsWithFitness 
                              |> Array.map(fun (srtrEval, srtrFit) -> ((srtrEval, srtrFit), (srtrFit |> UMX.untag)))
                              |> Array.sortByDescending(snd)
                    yab |> Array.map(fun ((sEv, sFt), npFt) -> sEv)

            rankedSorters
            |> CollectionOps.takeUpto (sorterSetPruner.prunedCount |> UMX.untag)
            |> Seq.toArray


    let runWholeCappedPrune
            (sorterSetPruner:sorterSetPruner)
            (rngGen:rngGen)
            (sorterPhenotypeCount:int<sorterPhenotypeCount>)
            (sorterEvalsToPrune:sorterEval[])
         =
            let stageWgt = getStageWeight sorterSetPruner
            let sorterEvalsWithFitness = 
                sorterEvalsToPrune
                |> Array.filter(SorterEval.getSorterSpeed >> Option.isSome)
                |> Array.filter(SorterEval.failedForSure >> not)
                |> CollectionOps.getItemsUpToMaxTimes 
                            (SorterEval.getSortrPhenotypeId)
                            (sorterPhenotypeCount |> UMX.untag)
                |> Seq.toArray
                |> Array.map(fun sEv -> 
                     ( sEv,
                       sEv |> SorterEval.getSorterSpeed |> Option.get |> SorterFitness.fromSpeed stageWgt
                     )
                   )
            let rankedSorters =
                if (sorterEvalsWithFitness.Length = 0) then
                    [||]
                elif sorterSetPruner.noiseFraction |> Option.isSome then
                    getSigmaSelection 
                        sorterEvalsWithFitness 
                        (sorterSetPruner.noiseFraction |> NoiseFraction.toFloat) 
                        rngGen

                else
                    let yab = sorterEvalsWithFitness 
                              |> Array.map(fun (srtrEval, srtrFit) -> ((srtrEval, srtrFit), (srtrFit |> UMX.untag)))
                              |> Array.sortByDescending(snd)
                    yab |> Array.map(fun ((sEv, sFt), npFt) -> sEv)

            rankedSorters
            |> CollectionOps.takeUpto (sorterSetPruner.prunedCount |> UMX.untag)
            |> Seq.toArray


    let runShcPrune
            (sorterSetPruner:sorterSetPruner)
            (rngGen:rngGen)
            (sorterSetParentMap:sorterSetParentMap)
            (sorterEvalsToPrune:sorterEval[])
         =
            let extendedPm = sorterSetParentMap |> SorterSetParentMap.extendToParents
            let familySorterEvalGroups = 
                    sorterEvalsToPrune 
                            |> Array.map(fun sev -> (sev, extendedPm.[sev.sortrId]))
                            |> Array.groupBy(fun (sorterEv, sorterPid) -> sorterPid)
                            |> Array.map(snd)
                            |> Array.map(Array.map(fst))

            let familyCount = (familySorterEvalGroups |> Array.length)
            let familyPrunedCount = (sorterSetPruner.prunedCount |> UMX.untag) 
                                        / familyCount
                                     |> UMX.tag<sorterCount>

            let familyRngGens = 
                rngGen |> Rando.toMoreRngGens
                |> Seq.take(familyCount)
                |> Seq.toArray

            let familyPruner = make familyPrunedCount sorterSetPruner.noiseFraction sorterSetPruner.stageWeight

            familyRngGens
                    |> Array.mapi(fun dex rg -> runWholePrune familyPruner rg familySorterEvalGroups.[dex])
                    |> Array.concat



    let runPrune
            (sorterSetPruneMethod:sorterSetPruneMethod)
            (rngGen:rngGen)
            (sorterSetParentMap:sorterSetParentMap)
            (sorterSetPruner:sorterSetPruner)
            (sorterEvalsToPrune:sorterEval[])
         =
         match sorterSetPruneMethod with
         | Whole -> runShcPrune sorterSetPruner rngGen sorterSetParentMap sorterEvalsToPrune
         | Shc -> runShcPrune sorterSetPruner rngGen sorterSetParentMap sorterEvalsToPrune
         | PhenotypeCap spc -> runWholeCappedPrune sorterSetPruner rngGen spc sorterEvalsToPrune