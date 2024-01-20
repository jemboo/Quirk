namespace Quirk.Sorting

open System
open FSharp.UMX
open Quirk.Core

type sorterUniformMutator =
    private
        {
          switchGenMode: switchGenMode
          mutationRate: float<mutationRate>
          switchCountPfx: int<switchCount> Option
          switchCountFinal: int<switchCount> Option
          mutatorFunc: sorter -> Guid<sorterId> -> IRando -> Result<sorter, string> 
        }

module SorterUniformMutator =

    let private _create sorterUniformMutatorTyp mutationRat
                        switchCtPfx switchCtFinal mutatorFunc =
        { 
          sorterUniformMutator.switchGenMode = sorterUniformMutatorTyp
          mutationRate = mutationRat
          mutatorFunc = mutatorFunc
          switchCountPfx = switchCtPfx
          switchCountFinal = switchCtFinal
        }

    let getSwitchGenMode (sum: sorterUniformMutator) = sum.switchGenMode

    let getMutationRate (sum: sorterUniformMutator) = sum.mutationRate

    let getMutatorFunc (sum: sorterUniformMutator) = sum.mutatorFunc
    
    let getSwitchCountPrefix (sum: sorterUniformMutator) = sum.switchCountPfx
    
    let getSwitchCountFinal (sum: sorterUniformMutator) = sum.switchCountFinal

    let getMutatorId (sum: sorterUniformMutator) 
        = 
        [| (sum.GetType()) :> obj;
           (sum.switchGenMode) :> obj;
           (sum.mutationRate) :> obj;
           (sum.switchCountPfx) :> obj;
           (sum.switchCountFinal) :> obj;
        |] 
            |> GuidUtils.guidFromObjs
            |> UMX.tag<sortableSetId>


    let _switchMutator
            (mutRate:float<mutationRate>) 
            (sorter: sorter)
            (sorterD:Guid<sorterId>)
            (randy: IRando)  
        =
        result {
            let newSwitches =
                Switch.mutateSwitches sorter.order mutRate randy sorter.switches
                |> Seq.toArray

            return Sorter.fromSwitches sorterD (sorter |> Sorter.getOrder) newSwitches
        }

    let _stageMutator
            (mutRate:float<mutationRate>)
            (sorter: sorter)
            (sorterD:Guid<sorterId>)
            (randy: IRando)
        =
        result {
            let mutantStages =
                sorter.switches
                |> Stage.fromSwitches sorter.order
                |> Seq.toArray
                |> Array.map (Stage.randomMutate randy mutRate)

            let newSwitches =
                [| for stage in mutantStages do
                        yield! stage.switches |]
                |> Seq.toArray

            return Sorter.fromSwitches sorterD (sorter |> Sorter.getOrder) newSwitches
        }

    let _stageRflMutator            
            (mutRate:float<mutationRate>) 
            (sorter: sorter)          
            (sorterD:Guid<sorterId>)  
            (randy: IRando)  
        =
        result {
            let mutantStages =
                sorter.switches
                |> Stage.fromSwitches sorter.order
                |> Seq.toArray
                |> Array.map (Stage.randomReflMutate randy mutRate)

            let newSwitches =
                [| for stage in mutantStages do
                        yield! stage.switches |]
                |> Seq.toArray

            return Sorter.fromSwitches sorterD (sorter |> Sorter.getOrder) newSwitches
        }


    let _makeMutant
            (mFunc: sorter -> Guid<sorterId> -> IRando -> Result<sorter, string>)
            (switchCtPrefix: int<switchCount> Option)
            (switchCtTarget: int<switchCount> Option)
            (sorter: sorter)          
            (sorterD:Guid<sorterId>)  
            (randy: IRando)  
        =
        result {
          let! mutantSortr = mFunc sorter sorterD randy
          let targetSwitchCt = 
            match switchCtTarget with
            | Some swct -> swct
            | None -> sorter |> Sorter.getSwitches 
                      |> Array.length |> UMX.tag<switchCount>

          let prefixSwitchCt = 
            match switchCtPrefix with
            | Some swct -> swct
            | None -> 0 |> UMX.tag<switchCount>

          return Sorter.crossOver prefixSwitchCt targetSwitchCt sorter mutantSortr sorterD randy
        }


    let create 
            (switchCtPrefix: int<switchCount> Option)
            (switchCountFinal: int<switchCount> Option)
            (switchGenMode:switchGenMode)  
            (mutRate: float<mutationRate>) 
        =
        match switchGenMode with
        | switchGenMode.switch ->
            _create switchGenMode mutRate switchCtPrefix switchCountFinal (_makeMutant (_switchMutator mutRate) switchCtPrefix switchCountFinal)
        | switchGenMode.stage ->
            _create switchGenMode mutRate switchCtPrefix switchCountFinal (_makeMutant (_stageMutator mutRate) switchCtPrefix switchCountFinal)
        | switchGenMode.stageSymmetric ->
            _create switchGenMode mutRate switchCtPrefix switchCountFinal (_makeMutant (_stageRflMutator mutRate) switchCtPrefix switchCountFinal)
        | _ -> failwith $"{switchGenMode} not matched in SorterUniformMutator.create"


type sorterMutator = 
      | Uniform of sorterUniformMutator

module SorterMutator =
    
    let getMutatorFunc 
            (sorterMutator:sorterMutator)
        =
        match sorterMutator with
        | Uniform sum -> sum |> SorterUniformMutator.getMutatorFunc


    let getMutatorId 
            (sorterMutator:sorterMutator)
        =
        match sorterMutator with
        | Uniform sum -> sum |> SorterUniformMutator.getMutatorId


    let getSwitchCountPfx
            (sorterMutator:sorterMutator)
        =
        match sorterMutator with
        | Uniform sum -> sum |> SorterUniformMutator.getMutatorFunc


    let getSwitchCountFinal
            (sorterMutator:sorterMutator)
        =
        match sorterMutator with
        | Uniform sum -> sum |> SorterUniformMutator.getSwitchCountFinal


    let makeMutants
            (sorterMutator:sorterMutator) 
            (randy:IRando)
            (parentIdMap:Map<Guid<sorterId>, Guid<sorterParentId>>)
            (sortersToMutate:sorter seq)
        =
        let _sortersToMutateLookup = 
            sortersToMutate 
            |> Seq.map(fun s -> 
                (s |> Sorter.getSorterId |> UMX.cast<sorterId, sorterParentId>, s))
            |> Map.ofSeq

        let _pid_mutant (childId:Guid<sorterId>) =
            result {
                
                let parentSorterId = parentIdMap.Item childId
                let parentSorter = _sortersToMutateLookup.Item parentSorterId
                let! mutant =
                    (sorterMutator |> getMutatorFunc)
                            parentSorter
                            childId
                            randy
                return mutant
            }

        parentIdMap 
        |> Map.toSeq
        |> Seq.map (fst >> _pid_mutant)
        |> Seq.toList
        |> Result.sequence