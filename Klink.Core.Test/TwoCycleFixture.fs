namespace Klink.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TwoCycleFixture() =

    [<TestMethod>]
    member this.TwoCyclePerm_makeFromTupleSeq() =
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 16
        let stageTupes = seq { (0, 1) }
        let twoCycle = TwoCycle.makeFromTupleSeq ord stageTupes
        Assert.AreEqual((TwoCycle.getArray twoCycle).Length, ord |> Order.value)
        Assert.IsTrue(twoCycle |> TwoCycle.isATwoCycle)

        let stageTupes2 =
            seq {
                (0, 1)
                (2, 1)
            }

        let twoCycle2 = TwoCycle.makeFromTupleSeq ord stageTupes2
        Assert.AreEqual(twoCycle |> TwoCycle.getArray |> Array.toList, twoCycle2 |> TwoCycle.getArray |> Array.toList)


    [<TestMethod>]
    member this.TwoCyclePerm_MakeAllMonoCycles() =
        let seed = RandomSeed.fromNow ()
        let ord = Order.createNr 16
        let tcA = TwoCycle.makeAllMonoCycles ord |> Seq.map (TwoCycle.toPerm) |> Seq.toArray
        let allgood = tcA |> Array.forall (Permutation.isTwoCycle)
        Assert.AreEqual(Order.switchCount ord, tcA.Length)
        Assert.IsTrue(allgood)


    [<TestMethod>]
    member this.TwoCyclePerm_rndTwoCycle() =
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 16
        let switchCt = 4

        for i in { 0..20 } do
            let tcp = TwoCycle.rndPartialTwoCycle ord switchCt iRando
            Assert.IsTrue(tcp |> TwoCycle.toPerm |> Permutation.isTwoCycle)


    [<TestMethod>]
    member this.TwoCyclePerm_makeMode1() =
        let ord = Order.createNr 16
        let aa = TwoCycle.oddMode ord
        let ac = TwoCycle.oddModeWithCap ord
        let acv = TwoCycle.getArray ac
        Assert.IsTrue(acv.Length > 0)


    [<TestMethod>]
    member this.TwoCyclePerm_makeCoConjugateEvenOdd() =
        let ordV = 6
        let ord = Order.createNr ordV
        let permI = Permutation.identity ord
        let permC = Permutation.create  [|1;2;3;4;5;0|]
                    |> Result.ExtractOrThrow
        let conjI = TwoCycle.coConjugate permI |> Seq.toArray
        let conjC = TwoCycle.coConjugate permC |> Seq.toArray
        Assert.AreEqual(conjI.[0], conjC.[1])
        Assert.AreEqual(conjI.[1], conjC.[0])


    [<TestMethod>]
    member this.TwoCyclePerm_makeReflSymmetric() =
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 16
        let symTwoCs = Array.init 100 (fun _ -> TwoCycle.rndSymmetric ord iRando)
        let rr = symTwoCs |> Array.map (TwoCycle.isRflSymmetric) |> Array.countBy (id)
        Assert.AreEqual(rr.Length, 1)

        let rflCmps =
            symTwoCs
            |> Array.map (fun tc -> tc = (TwoCycle.reflect tc))
            |> Seq.countBy (id)
            |> Seq.toArray

        Assert.AreEqual(rflCmps.Length, 1)



    [<TestMethod>]
    member this.TwoCyclePerm_mutateReflSymmetric() =
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 16
        let sc = ord |> Order.value |> uint64 |> SymbolSetSize.createNr
        let refSyms = Array.init 1000 (fun _ -> TwoCycle.rndSymmetric ord iRando)

        let refMuts =
            refSyms
            |> Array.map (fun p ->
                let tcp =
                    seq {
                        while true do
                            yield RandVars.drawTwoWithoutRep sc iRando
                    }

                TwoCycle.mutateByReflPair tcp p)

        let mutReflBins =
            refMuts
            |> Array.map (fun tcp -> (tcp, tcp = (tcp |> TwoCycle.reflect)))
            |> Array.countBy (snd)

        Assert.AreEqual(mutReflBins.Length, 1)


    [<TestMethod>]
    member this.TwoCycleGen_evenMode() =
        let evenDegree = Order.createNr 16
        let resE = TwoCycle.evenMode evenDegree
        Assert.IsTrue((TwoCycle.getArray resE).Length = (Order.value evenDegree))
        let oddDegree = Order.createNr 15
        let resO = TwoCycle.evenMode oddDegree
        Assert.IsTrue((TwoCycle.getArray resO).Length = (Order.value oddDegree))


    [<TestMethod>]
    member this.TwoCyclePerm_conjugateBitRep() = 
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let orderLog = 8 |> Order.createNr
        let order = 256 |> Order.createNr

        let perms =
                Permutation.createRandoms orderLog iRando
                |> Seq.take 100 |> Seq.toArray

        let tcConjs = 
                TwoCycle.bitConjugateEvenMode order perms
                |> Seq.zip perms
                |> Seq.sortBy(fun tc -> tc |> snd |> TwoCycle.getArray |> Array.head)
                |> Seq.toArray

        Assert.IsTrue(true)
        


    [<TestMethod>]
    member this.totalSwitchBarLengths() = 
        let ordr = 16 |> Order.createNr
        let sbItentity = TwoCycle.identity ordr
        let tsblIdent = TwoCycle.totalSwitchBarLengths sbItentity
        Assert.AreEqual(tsblIdent, 0)

        let sbEvenMod = TwoCycle.evenMode ordr
        let tsblevenM = TwoCycle.totalSwitchBarLengths sbEvenMod
        Assert.AreEqual(tsblevenM, 8)

        Assert.IsTrue(true)

    [<TestMethod>]
    member this.TestMethodPassing() = Assert.IsTrue(true)
