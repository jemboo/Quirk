namespace Quirk.Sorting

open System
open FSharp.UMX
open Quirk.Core

type sorterSetMutator = 
    private { 
            id: Guid<sorterSetMutatorId>
            sorterMutator: sorterMutator
            sorterCountFinal: int<sorterCount> Option
        }

module SorterSetMutator =

    let load
            (id:Guid<sorterSetMutatorId>)
            (sorterMutator:sorterMutator) 
            (sorterCountFinal:int<sorterCount> option) 
        =
        { 
          id = id
          sorterMutator = sorterMutator
          sorterCountFinal = sorterCountFinal
        }

    let getId (sorterSetMutator: sorterSetMutator) = sorterSetMutator.id

    let getSorterMutator (sum: sorterSetMutator) = sum.sorterMutator

    let getSorterCountFinal (sum: sorterSetMutator) = 
         sum.sorterCountFinal

    let getMutantSorterSetId
            (sorterSetMutator:sorterSetMutator)
            (rngGen:rngGen) 
            (parentSetId:Guid<sorterSetId>)
        =
        [|  
            parentSetId :> obj;
            rngGen :> obj;
            (sorterSetMutator
                    |> getSorterMutator
                    |> SorterMutator.getMutatorId):> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> UMX.tag<sorterSetId>


    let createMutantSorterSetFromParentMap
            (sorterSetParentMap:sorterSetParentMap)
            (ssm:sorterSetMutator)
            (rngGen:rngGen) 
            (sorterSetToMutate:sorterSet)
        =
        result {
            let! mutants = 
                SorterMutator.makeMutants
                    (ssm |> getSorterMutator)
                    (rngGen |> Rando.fromRngGen)
                    (sorterSetParentMap |> SorterSetParentMap.getParentMap)
                    (sorterSetToMutate |> SorterSet.getSorters)

            return
                SorterSet.load
                    (sorterSetToMutate |> SorterSet.getOrder)
                    mutants
        }

