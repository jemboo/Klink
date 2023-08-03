namespace Klink.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type IntSeriesFixture() =

    [<TestMethod>]
    member this.IntSequence_expoB() =
        let aMax = 100
        let yab = Array.init aMax (fun dex -> (dex, IntSeries.expoB 5.0 dex))
        Assert.AreEqual(1, 1)

    [<TestMethod>]
    member this.IntSequence_logTics() =
        let ticsPerLog = 70.0
        let endVal = 5000
        let away = IntSeries.logTics ticsPerLog endVal |> Seq.toArray
        let mutable dex = 1
        while dex < away.Length do
            Assert.IsTrue(IntSeries.expoB ticsPerLog away.[dex])
            dex <- dex + 1


    [<TestMethod>]
    member this.TestMethodPassing() = Assert.IsTrue(true)
