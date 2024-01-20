namespace Quirk.SortingOps

open System
open FSharp.UMX
open Quirk.Core
open Quirk.Sorting
open SysExt


type sorterOpOutput =
    private
        { sortr: sorter
          sortableSt: sortableSet
          transformedRollout: rollout
          sorterOpTracker: sorterOpTracker }

module SorterOpOutput =

    let make (sortr: sorter) 
             (sortableSt: sortableSet) 
             (transRollout: rollout) 
             (sorterOpTracker: sorterOpTracker) =
        { sorterOpOutput.sortr = sortr
          sorterOpOutput.sortableSt = sortableSt
          sorterOpOutput.transformedRollout = transRollout
          sorterOpOutput.sorterOpTracker = sorterOpTracker }

    let getSorter (sorterOpOutput: sorterOpOutput) = sorterOpOutput.sortr

    let getSortableSet (sorterOpOutput: sorterOpOutput) = sorterOpOutput.sortableSt

    let getTransformedRollout (sorterOpOutput: sorterOpOutput) = sorterOpOutput.transformedRollout

    let getSorterOpTracker (sorterOpOutput: sorterOpOutput) = sorterOpOutput.sorterOpTracker

    let isSorted (sorterOpOutput: sorterOpOutput) =
        sorterOpOutput.transformedRollout |> Rollout.isSorted

    let getRefinedSortableSet 
            (sortableSetId:Guid<sortableSetId>) 
            (sorterOpOutput: sorterOpOutput) 
            =
        result {
            let! refinedRollout =
                    sorterOpOutput.transformedRollout 
                    |> Rollout.uniqueUnsortedMembers
            
            let symbolSetSetSize = 
                sorterOpOutput.sortableSt
                |> SortableSet.getSymbolSetSize                    

            return SortableSet.make sortableSetId symbolSetSetSize refinedRollout
        }

    let getRefinedSortableCount (sorterOpOutput: sorterOpOutput) =
        sorterOpOutput
        |> getRefinedSortableSet (Guid.Empty |> UMX.tag<sortableSetId>) 
        |> Result.map(SortableSet.getRollout >> Rollout.getArrayCount >> 
              UMX.cast<arrayCount, sortableCount>)
