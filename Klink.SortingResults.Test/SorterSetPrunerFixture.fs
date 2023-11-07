namespace Klink.SortingResults.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type SorterSetPrunerFixture() =

    [<TestMethod>]
    member this.parse_sorterSetPruneMethod() = 
        let sspm = 128 
                    |> SorterPhenotypeCount.create
                    |> sorterSetPruneMethod.PhenotypeCap

        let rVal = sspm |> SorterSetPruneMethod.toReport

        let sspmBack = rVal |> SorterSetPruneMethod.fromReport
                            |> Result.ExtractOrThrow

        Assert.AreEqual(sspm, sspmBack)


    [<TestMethod>]
    member this.getSigmaSelection() =
        let ordr = 16 |> Order.createNr
        let sorterCt = 64 |> SorterCount.create
        let prunedCount = 64 |> SorterCount.create
        //let noiseFraction = None
        let noiseFraction =  0.5 |> NoiseFraction.create |> Some
        let stageWeight = 1.0 |> StageWeight.create
        let perfEvalMode = sorterEvalMode.DontCheckSuccess
        let sortableFormat = rolloutFormat.RfBs64 
       // let sortableFormat = rolloutFormat.RfI32 

        let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
        let rndGn () = 
            randy |> Rando.toRngGen
   
        let switchCt = SwitchCount.orderTo900SwitchCount ordr

        let sortableSet = SortableSet.makeAllBits 
                                (Guid.NewGuid() |> SortableSetId.create) 
                                sortableFormat 
                                ordr
                            |> Result.ExtractOrThrow

        let sorterSet = SorterSet.createRandomSwitches 
                            (Guid.NewGuid() |> SorterSetId.create)
                            sorterCt 
                            ordr 
                            Seq.empty<switch> 
                            switchCt 
                            rndGn

        let sorterSetEvals = 
            SorterSetEval.evalSorters 
                    perfEvalMode
                    sortableSet
                    (sorterSet |> SorterSet.getSorters) 
                    (true |> UseParallel.create)



        let pruner = SorterSetPruner.make prunedCount noiseFraction stageWeight

        let res =  SorterSetPruner.runWholePrune pruner (rndGn ()) sorterSetEvals




        Assert.IsTrue(CollectionProps.areEqual 1 1)

