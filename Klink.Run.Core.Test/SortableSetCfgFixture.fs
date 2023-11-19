namespace Klink.Run.Core.Test

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
    member this.CfgPlexItem () =

        let noiseFracts = [|0.1; 0.2; 0.3|] |> Array.map(MutationRate.create >> cfgPlexItemValue.MutationRate)
        let stageWgts = [|0.1; 0.2; 0.3|] |> Array.map(StageWeight.create >> cfgPlexItemValue.StageWeight)
        let orders = [|1; 2; 3|]  |> Array.map(Order.createNr >> cfgPlexItemValue.Order)
        
        let cpiNoiseFracts = CfgPlexItem.create "noiseFractions"  0 noiseFracts
        let cpiStageWeights = CfgPlexItem.create "stageWeights"  1 stageWgts
        let cpiOrders = CfgPlexItem.create "orders"  2 orders
        
        let cfgPlexItems = [| cpiNoiseFracts; cpiStageWeights; cpiOrders; |]
        
        let combos =
                CfgPlexItem.enumerateItems cfgPlexItems
                |> Seq.toArray
        
        Assert.IsTrue(combos.Length = 27)
        
        

    [<TestMethod>]
    member this.CfgPlexItem2 () =

        let noiseFracts = [|0.1; 0.2; 0.3|] |> Array.map(MutationRate.create >> cfgPlexItemValue.MutationRate)
        let stageWgts = [|0.1; 0.2; 0.3|] |> Array.map(StageWeight.create >> cfgPlexItemValue.StageWeight)
        let orders = [|1; 2; 3|]  |> Array.map(Order.createNr >> cfgPlexItemValue.Order)
        let switchGenModes = [|switchGenMode.stage ; switchGenMode.switch; switchGenMode.stageSymmetric|]  |> Array.map(cfgPlexItemValue.SwitchGenMode)
        
        
        let cpiNoiseFracts = CfgPlexItem.create "noiseFractions"  0 noiseFracts
        let cpiStageWeights = CfgPlexItem.create "stageWeights"  1 stageWgts
        let cpiOrders = CfgPlexItem.create "orders"  2 orders
        let cpiSwitchGenModes = CfgPlexItem.create "switchGenModes" 3 switchGenModes
        
        let cfgPlexItems = [| cpiSwitchGenModes; cpiNoiseFracts; cpiStageWeights; cpiOrders; |]
        
        let combos =
                CfgPlexItem.enumerateItems cfgPlexItems
                |> Seq.toArray
        
        Assert.IsTrue(combos.Length = 81)
        
        
        
        
        


    [<TestMethod>]
    member this.TestMethodPassing () =


        Assert.IsTrue(true);