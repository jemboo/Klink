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
        
        let nNoiseFractions = "noiseFractions" |> CfgPlexItemName.create
        let nStageWeights = "stageWeights" |> CfgPlexItemName.create
        let nOrders = "orders" |> CfgPlexItemName.create

        let r0 = 0 |> CfgPlexItemRank.create
        let r1 = 0 |> CfgPlexItemRank.create
        let r2 = 0 |> CfgPlexItemRank.create

        let cpiNoiseFracts = CfgPlexItem.create nNoiseFractions r0 noiseFracts
        let cpiStageWeights = CfgPlexItem.create nStageWeights r1 stageWgts
        let cpiOrders = CfgPlexItem.create nOrders r2 orders
        
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
        
        let nNoiseFractions = "noiseFractions" |> CfgPlexItemName.create
        let nStageWeights = "stageWeights" |> CfgPlexItemName.create
        let nOrders = "orders" |> CfgPlexItemName.create
        let nSwitchGenModes = "switchGenModes" |> CfgPlexItemName.create

        let r0 = 0 |> CfgPlexItemRank.create
        let r1 = 0 |> CfgPlexItemRank.create
        let r2 = 0 |> CfgPlexItemRank.create
        let r3 = 0 |> CfgPlexItemRank.create

        let cpiNoiseFracts = CfgPlexItem.create nNoiseFractions r0 noiseFracts
        let cpiStageWeights = CfgPlexItem.create nStageWeights r1 stageWgts
        let cpiOrders = CfgPlexItem.create nOrders r2 orders
        let cpiSwitchGenModes = CfgPlexItem.create nSwitchGenModes r3 switchGenModes
        
        let cfgPlexItems = [| cpiSwitchGenModes; cpiNoiseFracts; cpiStageWeights; cpiOrders; |]
        
        let combos =
                CfgPlexItem.enumerateItems cfgPlexItems
                |> Seq.toArray
        
        Assert.IsTrue(combos.Length = 81)


    [<TestMethod>]
    member this.TestMethodPassing () =


        Assert.IsTrue(true);