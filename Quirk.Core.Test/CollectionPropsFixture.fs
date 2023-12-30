namespace Quirk.Core.Test

open System
open Xunit
open FSharp.UMX
open Quirk.Core

type scriptItemT = 
    private 
        { 
            id: Guid
            index:int
        }

module ScriptItemT =

    let create (index:int) =
        {
            scriptItemT.id = Guid.NewGuid()
            index = index
        }


module CollectionPropsFixture =



    [<Fact>]
    let ``filterByIndexes``() =
        
        let infSeq = 
           Seq.initInfinite(ScriptItemT.create)

        let dexes = [|1;333|]

        let filtered = 
            CollectionProps.filterByIndexes dexes infSeq
            |> Seq.toArray


        Assert.Equal(84, 84)
