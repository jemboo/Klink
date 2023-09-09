namespace Klink.SortingResults.Test
open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type SorterSpeedBinFixture() =


    [<TestMethod>]
    member this.SorterSpeedBinKey() =
        let ss = SorterSpeed.create 
                            (1 |> SwitchCount.create)
                            (1 |> StageCount.create)

        let ssBt = "Mutants" |> SorterSpeedBinType.create

        let ssfl = true |> Some

        let ssBk = SorterSpeedBinKey.make ssfl ssBt ss
        let cereal = ssBk |> SorterSpeedBinKeyDto.toJson
        let ssBkb = cereal |> SorterSpeedBinKeyDto.fromJson
                            |> Result.ExtractOrThrow
        Assert.IsTrue(true)




    [<TestMethod>]
    member this.SorterSpeedBinSet() =

        let ordr = 16 |> Order.createNr
        let switchCt = SwitchCount.orderTo900SwitchCount ordr
        let sorterCt = SorterCount.create 20

        let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
        let rndGn () = 
                randy |> Rando.toRngGen

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
            |> CollectionOps.infinteLoop
            |> Seq.take 70
            |> Seq.toArray


        let binType = "Mutants" |> SorterSpeedBinType.create
        let ssBins = sorterEvls |> Array.map(SorterSpeedBin.fromSorterEval ordr binType)
        let binSet = 
            ssBins 
            |> SorterSpeedBinSet.addBins
                (SorterSpeedBinSet.create (Map.empty)  (0 |> Generation.create) (Guid.NewGuid()))
                (0 |> Generation.create)

        Assert.IsTrue(ssBins.Length > 1)

        let cereal = binSet |> SorterSpeedBinSetDto.toJson
        let bsBack = cereal |> SorterSpeedBinSetDto.fromJson
                            |> Result.ExtractOrThrow

        Assert.AreEqual(binSet, bsBack)



    [<TestMethod>]
    member this.getHeader() =
        let ss = SpeedBinProps.getHeader()

        Assert.IsTrue(true)
