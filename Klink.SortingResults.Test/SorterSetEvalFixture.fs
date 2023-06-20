namespace Klink.SortingResults.Test
open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type SorterSetEvalFixture() =

    [<TestMethod>]
    member this.getResults() =
        let ordr = 16 |> Order.createNr
        let switchCt = SwitchCount.orderTo900SwitchCount ordr
        let sorterCt = SorterCount.create 2000

        let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
        let rndGn () = 
                randy |> Rando.nextRngGen

        let sorterSetId = Guid.NewGuid() |> SorterSetId.create
        let sorterSt = SorterSet.createRandomSwitches sorterSetId sorterCt ordr [||] switchCt rndGn
        let rolloutFormt = rolloutFormat.RfBs64
        let sortableStId = SortableSetId.create (Guid.NewGuid())

        let sortableSt =
            SortableSet.makeAllBits sortableStId rolloutFormt ordr |> Result.ExtractOrThrow

        let sorterEvalMod = sorterEvalMode.DontCheckSuccess
        let useParalll = true |> UseParallel.create

        let sorterEvls =
            SorterSetEval.evalSorters 
                sorterEvalMod 
                sortableSt 
                (sorterSt |> SorterSet.getSorters)
                useParalll

        Assert.AreEqual(sorterEvls.Length, (sorterCt |> SorterCount.value))


    [<TestMethod>]
    member this.tt() =


        Assert.AreEqual(1, 1)
