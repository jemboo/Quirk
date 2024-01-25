namespace Quirk.SortingResults

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open Quirk.Iter

type genInfo =
        private {
        generation:int<generation>
        sorterId: Guid<sorterId>
        sorterPhenotypeId:Guid<sorterPhenotypeId>
        sorterFitness:float<sorterFitness>
        }


module GenInfo =

    let create 
        (gen:int<generation>)
        (sorterId:Guid<sorterId>)
        (sorterPhenotypeId:Guid<sorterPhenotypeId>)
        (sorterFitness:float<sorterFitness>)
        =
        {
            generation = gen
            sorterId = sorterId
            sorterPhenotypeId = sorterPhenotypeId
            sorterFitness = sorterFitness
        }

    let getGeneration (sa:genInfo) =
        sa.generation

    let getSorterId (sa:genInfo) =
        sa.sorterId

    let getSorterPhenotypeId (sa:genInfo) =
        sa.sorterPhenotypeId

    let getSorterFitness (sa:genInfo) =
        sa.sorterFitness


type sorterAncestry =
        private {
        sorterId: Guid<sorterId>
        ancestors: List<genInfo>
        }


module SorterAncestry =

    let getSorterId (sa:sorterAncestry) =
        sa.sorterId

    let getAncestors (sa:sorterAncestry) =
        sa.ancestors

    let load (sorterId: Guid<sorterId>)
             (ancestors: genInfo[])
        =
        {
            sorterId =sorterId
            ancestors = ancestors |> Array.toList
        }

    let create
            (sorterId:Guid<sorterId>) 
            (generation:int<generation>) 
            (sorterPhenotypeId:Guid<sorterPhenotypeId>) 
            (sorterFitness:float<sorterFitness>) 
        =
        {
            sorterAncestry.sorterId = sorterId;
            ancestors = 
                [
                    {
                        genInfo.sorterId = sorterId;
                        generation = generation
                        sorterPhenotypeId = sorterPhenotypeId
                        sorterFitness = sorterFitness
                    }
                ]
        }

    let update 
            (sorterId:Guid<sorterId>) 
            (generation:int<generation>) 
            (sorterPhenotypeId:Guid<sorterPhenotypeId>) 
            (sorterFitness:float<sorterFitness>) 
            (parentSorterAncestry:sorterAncestry) 
        =
        let _replace newInfo last history =
            if newInfo.sorterPhenotypeId = last.sorterPhenotypeId then
                match history with
                | [] -> [last]
                | _ ->  newInfo::history
            else
                newInfo::last::history

        let newStuff = 
            {
                genInfo.sorterId = sorterId;
                generation = generation
                sorterPhenotypeId = sorterPhenotypeId
                sorterFitness = sorterFitness
            }

        let updatedAncestors =
            match parentSorterAncestry.ancestors with
            | [] -> 
                [newStuff]
            | last::history -> 
                let yab = _replace newStuff last history
                yab

        {
            sorterAncestry.sorterId = sorterId;
            ancestors = updatedAncestors
        }



type sorterSetAncestry =
        private {
            id: Guid<sorterSetAncestryId>;
            generation:int<generation>;
            ancestorMap:Map<Guid<sorterId>, sorterAncestry>
            tag:Guid
        }


module SorterSetAncestry =

    let getId (sa:sorterSetAncestry) =
        sa.id

    let getGeneration (sa:sorterSetAncestry) =
        sa.generation

    let getAncestorMap (sa:sorterSetAncestry) =
        sa.ancestorMap

    let getTag (sa:sorterSetAncestry) =
        sa.tag

    let load (id:Guid<sorterSetAncestryId>) 
             (generation:int<generation>)
             (ancestors:sorterAncestry[])
             (tag:Guid) 
        =
        let ancestorMap = 
                ancestors
                |> Array.map(fun am -> (am.sorterId, am))
                |> Map.ofArray
        {
            sorterSetAncestry.id = id;
            generation = generation;
            ancestorMap = ancestorMap
            tag = tag
        }


    let create (sorterSetEval:sorterSetEval)
               (stageWeight:float<stageWeight>)
               (generation:int<generation>)
               (tag:Guid)
        =
        let sorterSetAncestryId = 
              Guid.NewGuid() |> UMX.tag<sorterSetAncestryId>

        let _makeSorterAncestry 
                (sev:sorterEval)
                (gen:int<generation>)
                (sw:float<stageWeight>)
            =
            SorterAncestry.create
                sev.sortrId
                gen
                (sev.sortrPhenotypeId |> Option.get)
                (sev.sorterSpeed |> Option.get |> SorterFitness.fromSpeed sw)

        {
            id = sorterSetAncestryId;
            generation = generation
            ancestorMap = 
                sorterSetEval.sorterEvals
                |> Map.toArray
                |> Array.map(fun (srtrId, sev) -> (srtrId, _makeSorterAncestry sev generation stageWeight))
                |> Map.ofSeq
            tag = tag
        }


    let update
            (generation:int<generation>)
            (stageWeight:float<stageWeight>)
            (sorterSetEval:sorterSetEval)
            (parentMap:Map<Guid<sorterId>, Guid<sorterParentId>>)
            (sorterSetAncestry:sorterSetAncestry)
        =
        let newId = 
              Guid.NewGuid() |> UMX.tag<sorterSetAncestryId>

        let _updateSorterAncestry 
                (parentSorterAncestry:sorterAncestry)
                (sev:sorterEval)
                (gen:int<generation>)
                (stageWeight:float<stageWeight>)
            =
            parentSorterAncestry
                |> SorterAncestry.update
                    sev.sortrId
                    gen
                    (sev.sortrPhenotypeId |> Option.get)
                    (sev.sorterSpeed |> Option.get |> SorterFitness.fromSpeed stageWeight)

        let evalMap = sorterSetEval |> SorterSetEval.getSorterEvalsMap

        let _update  (sorterId:Guid<sorterId>, sorterParentId:Guid<sorterParentId>) =
            let asSorterId = sorterParentId |> UMX.cast<sorterParentId,sorterId>
            if (sorterId |> UMX.untag) = (sorterParentId |> UMX.untag) 
               then
                (sorterId, sorterSetAncestry.ancestorMap.[asSorterId])

               else
                let parentSorterAncestry = 
                    sorterSetAncestry.ancestorMap.[asSorterId]
                let updatedSorterAncestry = 
                     _updateSorterAncestry
                        parentSorterAncestry 
                        evalMap.[sorterId]
                        generation
                        stageWeight
                (
                    sorterId,
                    updatedSorterAncestry
                )

        // if the sorter is not in the parent map, then it is it's own parent
        let _lookupParentSorter (sorterId:Guid<sorterId>) =
            if (parentMap.ContainsKey sorterId) then
                parentMap.[sorterId]
            else
                sorterId |> UMX.cast<sorterId, sorterParentId>

        let newAncestorMap = 
            sorterSetEval
            |> SorterSetEval.getSorterEvalsMap
            |> Map.toArray
            |> Array.map(fst)
            |> Array.map(fun sorterId -> (sorterId, _lookupParentSorter sorterId))
            |> Seq.map(_update)
            |> Map.ofSeq

        {
            id = newId;
            generation = generation; 
            ancestorMap = newAncestorMap
            tag = sorterSetAncestry.tag
        }