namespace Klink.Sorting.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type SorterSetFixture() =

    [<TestMethod>]
    member this.makeSorterSet() = 
      let sorterSetId = Guid.NewGuid() |> SorterSetId.create
      let ordr = 64 |> Order.createNr
      let wPfx = Seq.empty<switch>
      let switchCt = 100 |> SwitchCount.create
      let sorterCt = 10 |> SorterCount.create
      let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
      let rndGn () = 
        randy |> Rando.toRngGen

      let sorterSt = SorterSet.createRandomSwitches 
                        sorterSetId sorterCt ordr wPfx switchCt rndGn

      let sorterSt2 = SorterSet.load 
                        (sorterSt |> SorterSet.getId)
                        (sorterSt |> SorterSet.getOrder)
                        (sorterSt |> SorterSet.getSorters)
        
      Assert.IsTrue(CollectionProps.areEqual sorterSt sorterSt2)
      Assert.AreEqual(sorterSt, sorterSt2)


    [<TestMethod>]
    member this.createMutationSet() = 
      let parentSorterSetId = Guid.NewGuid() |> SorterSetId.create
      let ordr = 16 |> Order.createNr
      let wPfx = Seq.empty<switch>
      let swFreq = 1.0 |> SwitchFrequency.create
      let switchCt = 20 |> SwitchCount.create
      let parentSorterCt = 2 |> SorterCount.create
      let mutantSorterCt = 6 |> SorterCount.create
      let rngGen = RngGen.createLcg (123 |> RandomSeed.create)
      let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
      let rndGn () = 
        randy |> Rando.toRngGen
      let mutationRate = MutationRate.create 0.15

      let sorterSetParent = 
        SorterSet.createRandomStages 
             parentSorterSetId parentSorterCt swFreq ordr wPfx switchCt rndGn

      let sorterMutator = 
        SorterUniformMutator.create 
             None None switchGenMode.Stage mutationRate
        |> sorterMutator.Uniform


      let sorterSetMutator = 
            SorterSetMutator.load
                (Guid.NewGuid() |> SorterSetMutatorId.create)
                sorterMutator
                (Some mutantSorterCt)

      let mutantSorterSetId = parentSorterSetId
                              |> SorterSetMutator.getMutantSorterSetId  sorterSetMutator rngGen

      let parentMap = SorterSetParentMap.create
                            mutantSorterSetId
                            parentSorterSetId
                            mutantSorterCt
                            ( sorterSetParent 
                                |> SorterSet.getSorters 
                                |> Array.map(Sorter.getSorterId) )

      let mutantSet =  
        sorterSetParent |> SorterSetMutator.createMutantSorterSetFromParentMap
                            parentMap sorterSetMutator rngGen
                        |> Result.ExtractOrThrow

      Assert.AreEqual(mutantSet |> SorterSet.getSorterCount, mutantSorterCt)


    [<TestMethod>]
    member this.createAppendSet() = 
      let sorterSetBaseId = Guid.NewGuid() |> SorterSetId.create
      let sorterSetAppendedId = Guid.NewGuid() |> SorterSetId.create
      let ordr = 16 |> Order.createNr
      let wPfx = Seq.empty<switch>
      let swFreq = 1.0 |> SwitchFrequency.create
      let switchCt = 20 |> SwitchCount.create
      let baseSorterCt = 2 |> SorterCount.create
      let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
      let rndGn () = 
        randy |> Rando.toRngGen

      let sorterStBase = 
        SorterSet.createRandomStages 
            sorterSetBaseId 
            baseSorterCt 
            swFreq 
            ordr 
            wPfx 
            switchCt 
            rndGn
                    
      let appendSetMap = 
            SorterSetConcatMap.createSorterSetAppend
                sorterStBase
                sorterSetAppendedId
           |> Result.ExtractOrThrow

      Assert.AreEqual(1,1)