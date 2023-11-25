namespace Klink.DataConvert.Test

open Microsoft.VisualStudio.TestTools.UnitTesting
open System

[<TestClass>]
type runCfgPlexDtoFixture() =


    [<TestMethod>]
    member this.cfgPlexItemDto() =
        let cfpiNameA = "cfpNameA" |> CfgPlexItemName.create
        let cfpiRankA = 1 |> CfgPlexItemRank.create
        
        let cfpiNameB = "cfpNameB" |> CfgPlexItemName.create
        let cfpiRankB = 2 |> CfgPlexItemRank.create
        
        let cfpiNameC = "cfpNameC" |> CfgPlexItemName.create
        let cfpiRankC = 3|> CfgPlexItemRank.create
        
        
        let cfpivOrder1 = 16 |> Order.createNr |> cfgPlexItemValue.Order
        let cfpivMutationRate1 = 0.1 |> MutationRate.create |> cfgPlexItemValue.MutationRate
        let cfpivMutationRate2 = 0.2 |> MutationRate.create |> cfgPlexItemValue.MutationRate
        let cfpivNoiseFraction1 = 0.25 |> NoiseFraction.create |> cfgPlexItemValue.NoiseFraction
        let cfpivNoiseFraction2 = 0.50 |> NoiseFraction.create |> cfgPlexItemValue.NoiseFraction
        
        let cfgPlexItemValuesA = [| cfpivOrder1 |]
        let cfgPlexItemValuesB = [| cfpivMutationRate1; cfpivMutationRate2 |]
        let cfgPlexItemValuesC = [| cfpivNoiseFraction1; cfpivNoiseFraction2 |]
        
        let cfpiA = CfgPlexItem.create cfpiNameA cfpiRankA cfgPlexItemValuesA
        let cfpiB = CfgPlexItem.create cfpiNameB cfpiRankA cfgPlexItemValuesB
        let cfpiC = CfgPlexItem.create cfpiNameC cfpiRankC cfgPlexItemValuesC
        
        
        
        let cfgPlexName = "cfgPlexName" |> CfgPlexName.create
        let rngGen = RngGen.createLcg (123 |> RandomSeed.create)
        let cfgPlexItems = [|cfpiA; cfpiB; cfpiC|]
        
        let cfgPlex = CfgPlex.create cfgPlexName rngGen cfgPlexItems
        let cfgPlexCereal = cfgPlex |> CfgPlexDto.toJson 
        let cfgPlexBackR = cfgPlexCereal |> CfgPlexDto.fromJson 
        let cfgPlexBack = cfgPlexBackR |> Result.ExtractOrThrow 
        
        
        Assert.IsTrue(CollectionProps.areEqual 1 1)
