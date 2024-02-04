namespace Quirk.Cfg

open FSharp.UMX
open Quirk.Core
open Quirk.Sorting

type sorterSetRndCfg = private {
            order: int<order>
            switchGenMode: switchGenMode
            switchCount: int<switchCount>
            sorterCount: int<sorterCount>
        }



module SorterSetRndCfg =
    let make
            (order: int<order>)
            (switchGenMode: switchGenMode)
            (switchCount: int<switchCount>)
            (sorterCount: int<sorterCount>)
        =
        {
            order=order
            switchGenMode=switchGenMode
            switchCount=switchCount
            sorterCount=sorterCount
        }

    let getOrder 
            (sorterSetRndCfg:sorterSetRndCfg)
        =
        sorterSetRndCfg.order

    let getSwitchGenMode
            (sorterSetRndCfg:sorterSetRndCfg)
        =
        sorterSetRndCfg.order

    let getSwitchCount
            (sorterSetRndCfg:sorterSetRndCfg)
        =
        sorterSetRndCfg.order

    let getSorterCount
            (sorterSetRndCfg:sorterSetRndCfg)
        =
        sorterSetRndCfg.order

    let getConfigName
            (rdsg:sorterSetRndCfg) 
        =
        sprintf "%d_%s"
            (rdsg.order |> UMX.untag)
            (rdsg.switchGenMode |> string)


    //let makeSorterSet
    //        (rngGenProvider: rngGenProvider)
    //        (rdsg: sorterSetRndCfg) 
    //    =
    //    let rndGenF () = 
    //        rngGenProvider |> RngGenProvider.nextRngGen

    //    result {
    //        let ssRet =
    //            match rdsg.switchGenMode with
    //            | switchGenMode.switch -> 
    //                SorterSet.createRandomSwitches
    //                    rdsg.sorterSetId
    //                    rdsg.sorterCount
    //                    rdsg.order
    //                    []
    //                    rdsg.switchCount
    //                    rndGenF
    //                    |> Ok
    //            | switchGenMode.stage -> 
    //                SorterSet.createRandomStages2
    //                    rdsg.sorterSetId
    //                    rdsg.sorterCount
    //                    rdsg.order
    //                    []
    //                    rdsg.switchCount
    //                    rndGenF
    //                    |> Ok

    //            | switchGenMode.stageSymmetric -> 
    //                SorterSet.createRandomSymmetric
    //                    rdsg.sorterSetId
    //                    rdsg.sorterCount
    //                    rdsg.order
    //                    []
    //                    rdsg.switchCount
    //                    rndGenF
    //                    |> Ok

    //            | _ -> $"switchGenMode:{rdsg.switchGenMode} not handled" |> Error
    //        return! ssRet
    //    }