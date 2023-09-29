namespace Klink.Sorting.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type SorterSetParentMapFixture() =

    [<TestMethod>]
    member this.sorterSetAncestry() =
      let parentSorterSetId = Guid.NewGuid() |> SorterSetId.create
      let childSorterSetId = Guid.NewGuid() |> SorterSetId.create
      let parentSorterSetCt = 10 |> SorterCount.create
      let childSorterSetCt = 10 |> SorterCount.create

      let parentSorterIds = 
            SorterSet.generateSorterIds parentSorterSetId
            |> Seq.take (parentSorterSetCt |> SorterCount.value)
            |> Seq.toArray

      let sorterSetParentMap = 
            SorterSetParentMap.create
                childSorterSetId
                parentSorterSetId
                childSorterSetCt
                parentSorterIds

      let extendedMap = sorterSetParentMap |> SorterSetParentMap.extendToParents

      let sorterSetAncestryTag = Guid.NewGuid()
      let sorterSetAncestry = 
            SorterSetAncestry.create
                sorterSetAncestryTag
                parentSorterIds
                (0 |> Generation.create)
      
      let ssaUpdate = 
            sorterSetAncestry |>
                SorterSetAncestry.update
                    extendedMap


      let extendedMapR = extendedMap 
                            |> Map.toSeq
                            |> Seq.skip 3
                            |> Seq.take 10
                            |> Map.ofSeq

      let ssaUpdate = 
            sorterSetAncestry |>
                SorterSetAncestry.update
                    extendedMapR


      Assert.IsTrue(true)