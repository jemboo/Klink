namespace Klink.SortingResults.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type SorterSetAncestryFixture() =

    [<TestMethod>]
    member this.sorterAncestryP() =

        let gen1 = 1 |> Generation.create
        let gen2 = 2 |> Generation.create
        let gen3 = 3 |> Generation.create
        
        let sorterId1 = Guid.NewGuid() |> SorterId.create
        let sorterId2 = Guid.NewGuid() |> SorterId.create
        let sorterId3 = Guid.NewGuid() |> SorterId.create
        let sorterId4 = Guid.NewGuid() |> SorterId.create
        let sorterId5 = Guid.NewGuid() |> SorterId.create

        let sorterPhenoId1 = Guid.NewGuid() |> SorterPhenotypeId.create
        let sorterPhenoId2 = Guid.NewGuid() |> SorterPhenotypeId.create
        let sorterPhenoId3 = Guid.NewGuid() |> SorterPhenotypeId.create
        let sorterPhenoId4 = Guid.NewGuid() |> SorterPhenotypeId.create
        let sorterPhenoId5 = Guid.NewGuid() |> SorterPhenotypeId.create

        let sorterFitness1 = 0.1 |> SorterFitness.create
        let sorterFitness2 = 0.2 |> SorterFitness.create
        let sorterFitness3 = 0.3 |> SorterFitness.create
        let sorterFitness4 = 0.4 |> SorterFitness.create
        let sorterFitness5 = 0.5 |> SorterFitness.create


        let genInfo1_1 = GenInfo.create gen1 sorterId1 sorterPhenoId1 sorterFitness1
        let genInfo1_2 = GenInfo.create gen1 sorterId2 sorterPhenoId2 sorterFitness2
        let genInfo1_3 = GenInfo.create gen1 sorterId3 sorterPhenoId3 sorterFitness3
        let genInfo1_4 = GenInfo.create gen1 sorterId4 sorterPhenoId4 sorterFitness4

        
        let genInfo2_1 = GenInfo.create gen2 sorterId1 sorterPhenoId1 sorterFitness1
        let genInfo2_2 = GenInfo.create gen2 sorterId2 sorterPhenoId2 sorterFitness2
        let genInfo2_3 = GenInfo.create gen2 sorterId3 sorterPhenoId3 sorterFitness3
        let genInfo2_4 = GenInfo.create gen2 sorterId4 sorterPhenoId4 sorterFitness4


        let sorterAncestry1 = SorterAncestryP.create sorterId1 gen1 sorterPhenoId1 sorterFitness1
        

        let sorterAncestry1un = 
                sorterAncestry1 
                    |> SorterAncestryP.update sorterId2 gen2 sorterPhenoId1 sorterFitness1

        let sorterAncestry1uy = 
                sorterAncestry1 
                    |> SorterAncestryP.update sorterId2 gen2 sorterPhenoId2 sorterFitness2

        Assert.IsTrue(CollectionProps.areEqual 1 1)

