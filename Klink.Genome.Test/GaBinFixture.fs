namespace Klink.Genome.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type GaBinFixture () =

    [<TestMethod>]
    member this.BinSet_make () =

        let keys = [0 ..999]
        let rngGenMake = RngGen.createLcg (123 |> RandomSeed.create)
        let rngGenUpdate = RngGen.createLcg (456 |> RandomSeed.create)
        let binSz = 5
        let binCt = 7


        let randy = rngGenMake |> Rando.fromRngGen
        let rnds = Array.init 10 (fun _ -> randy.NextUInt)

        let yab = BinSet.make rngGenMake binSz binCt keys
        
        let readout = yab |> BinSet.getBinIndexGroups

        Assert.IsTrue(readout.Length = binCt);



    [<TestMethod>]
    member this.BinSet_update () =
    
        let keys = [0 .. 5]
        let rngGenMake = RngGen.createLcg (123 |> RandomSeed.create)
        let rngGenUpdate = RngGen.createLcg (456 |> RandomSeed.create)
        let binSz = 5
        let binCt = 7


        let randy = rngGenUpdate |> Rando.fromRngGen

        let yow = Array.init 100 (fun _ -> 2 * (randy.NextInt 2) - 1)


        let rnds = Array.init 10 (fun _ -> randy.NextUInt)

        let yab = BinSet.make rngGenMake binSz binCt keys
        let readout1 = yab |> BinSet.getBinIndexGroups

        let yab2 = yab |> BinSet.update rngGenUpdate
        let yab3 = yab2 |> BinSet.update rngGenUpdate
        let yab4 = yab3 |> BinSet.update rngGenUpdate
        let yab5 = yab4 |> BinSet.update rngGenUpdate


        let readout2 = yab3 |> BinSet.getBinIndexGroups

        Assert.IsTrue(true);
