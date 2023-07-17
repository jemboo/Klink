namespace Klink.DataConvert.Test
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type jsonUtilsFixture() =

    [<TestMethod>]
    member this.deserialize() =
        let stageWeight = 1.0 |> StageWeight.create
        let cereal = Json.serialize stageWeight
        let stageWeightBack = Json.deserialize<stageWeight>(cereal)

        Assert.AreEqual(stageWeight, stageWeightBack)