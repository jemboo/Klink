namespace Klink.Run.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type SortableSetCfgFixture () =

    [<TestMethod>]
    member this.SortableSetCfg_Id () =
        let order = Order.createNr 16
        let cfgMergeWithBits = sortableSetCertainCfg.MergeWithBits order |> sortableSetCfg.Certain
        let cfgMergeWithInts = sortableSetCertainCfg.MergeWithInts order |> sortableSetCfg.Certain

        let bitsId = SortableSetCfg.getId cfgMergeWithBits

        let intsId = SortableSetCfg.getId cfgMergeWithInts

        Assert.IsTrue(true);




    [<TestMethod>]
    member this.TestMethodPassing () =


        Assert.IsTrue(true);