namespace Klink.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type PermutationFixture() =

    [<TestMethod>]
    member this.Permutation_Identity() =
        let ord = Order.createNr 16
        let expectedLen = (Order.value ord)
        let expectedSum = (expectedLen * (expectedLen - 1)) / 2
        let permutes = Permutation.identity ord
        Assert.AreEqual(expectedLen, permutes |> Permutation.getArray |> Array.length)
        Assert.AreEqual(expectedSum, permutes |> Permutation.getArray |> Array.sum)

    [<TestMethod>]
    member this.Permutation_switchVals() =
        let origPerm = [|2;1;3;0;4|] |> Permutation.createNr
        let aVal, bVal = 1, 3
        let expected = [2;3;1;0;4]
        let swPerm = origPerm |> Permutation.switchVals aVal bVal
        let aOut = swPerm |> Permutation.getArray |> Array.toList
        Assert.AreEqual(expected, aOut)


    [<TestMethod>]
    member this.Permutation_powers() =
        let ord = Order.createNr 16
        let perm = Permutation.rotate ord 1
        let arA = perm |> Permutation.powers None |> Seq.toArray
        Assert.AreEqual(arA.Length, 16)



    [<TestMethod>]
    member this.Permutation_stackedSource() =
        let ord = Order.createNr 16
        let stackedSources = 
            [|0 .. (ord |> Order.value) / 2|]
            |> Array.map (Permutation.stackedSource ord )

        Assert.AreEqual(stackedSources.Length, (ord |> Order.value) / 2 + 1)


    [<TestMethod>]
    member this.Permutation_PowerDist() =
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 32
        let permCount = 1000

        let randPerms =
            Permutation.createRandoms ord iRando
            |> CollectionOps.takeUpto permCount
            |> Seq.map ((Permutation.powers None) >> Seq.toArray)
            |> Seq.toArray

        let yabs = randPerms |> Array.countBy (fun po -> po.Length) |> Array.sortBy (fst)

        yabs
        |> Array.iter (fun tup -> Console.WriteLine(sprintf "%d\t%d" (fst tup) (snd tup)))

        Assert.IsTrue(randPerms.Length > 0)


    [<TestMethod>]
    member this.Permutation_PowerMemberDist() =
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 32
        let permCount = 10

        let randPerms =
            Permutation.createRandoms ord iRando
            |> CollectionOps.takeUpto permCount
            |> Seq.map ((Permutation.powers None) >> Seq.toArray)
            |> Seq.map(fun ap -> ap |> Array.map(fun perm -> perm |> Permutation.powers None |> Seq.length))
            |> Seq.toArray


        //yabs
        //|> Array.iter (fun tup -> Console.WriteLine(sprintf "%d\t%d" (fst tup) (snd tup)))


        Assert.IsTrue(randPerms.Length > 0)


    [<TestMethod>]
    member this.Permutation_Inverse() =
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 32
        let permCount = 10

        let randPerms =
            Permutation.createRandoms ord iRando
            |> CollectionOps.takeUpto permCount
            |> Seq.toList

        let invPerms = 
                randPerms |> List.map(Permutation.inverse)

        let testProds =
            randPerms |> List.mapi(fun dex perm -> Permutation.productNr perm invPerms.[dex])

        let idPerm = Permutation.identity ord


        for dex = 0 to (permCount - 1) do
            Assert.AreEqual(idPerm, testProds.[dex])



    [<TestMethod>]
    member this.Permutation_applyToInt32() =
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 12
        let modulus = 4096
        let permCount = 10

        let randPerms =
            Permutation.createRandoms ord iRando
            |> CollectionOps.takeUpto permCount
            |> Seq.toList

        let invPerms = 
                randPerms |> List.map(Permutation.inverse)

        let testInts = Array.init permCount (fun _ -> iRando.NextInt modulus )
            
        for dex = 0 to (permCount - 1) do
            let unScramb = testInts.[dex]
            let scramb = unScramb |> Permutation.applyToInt32 randPerms.[dex]
            let scrBak = scramb |> Permutation.applyToInt32 invPerms.[dex]

            Assert.AreEqual(unScramb, scrBak)



    [<TestMethod>]
    member this.getWeight() = 
        let aId = [|0;1;2;3|]
        let aMax = [|3;2;1;0|]
        let wId = aId |> Permutation.createNr |> Permutation.getWeight
        let wMax = aMax |> Permutation.createNr |> Permutation.getWeight
        Assert.AreEqual(wId, 0)
        Assert.AreEqual(wMax, 8)


    [<TestMethod>]
    member this.distance() = 
        let aId = [|0;1;2;3|] |> Permutation.createNr
        let aMax = [|3;2;1;0|] |> Permutation.createNr
        let aMed = [|2;3;0;1|] |> Permutation.createNr
        let wIdId = aId |> Permutation.getDistance aId
        let wMaxId = aMax |> Permutation.getDistance aId
        let wMaxMax = aMax |> Permutation.getDistance aMax
        let wIdMax = aId |> Permutation.getDistance aMax
        let wMaxMed = aMax |> Permutation.getDistance aMed
        let wMedMax = aMed |> Permutation.getDistance aMax
        Assert.AreEqual(wIdId, 0)
        Assert.AreEqual(wMedMax, 4)



    [<TestMethod>]
    member this.distanceDist() = 
        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 32
        let permCount = 100

        let rndPerms =
            Permutation.createRandoms ord iRando
            |> CollectionOps.takeUpto permCount
            |> Seq.toArray

        let sDex = seq { 0 .. (permCount - 1)}
        let aHay = 
            sDex 
            |> Seq.map(fun v -> sDex
                                |> Seq.map(fun w -> (v,w, (Permutation.getDistance rndPerms.[v] rndPerms.[w]))))
            |> Seq.concat |> Seq.toArray
        aHay |> Array.iter(fun v -> Console.WriteLine(sprintf "%A" v))
        Assert.AreEqual(8, 8)



    [<TestMethod>]
    member this.getRandomSeparated() =

        let seed = RandomSeed.fromNow ()
        let iRando = Rando.fromRngGen (RngGen.createNet seed)
        let ord = Order.createNr 32
        let permCount = 100

        let sepPerms = Permutation.getRandomSeparated ord 0 400 iRando
                       |> Seq.take 100
                       |> Seq.toArray
        let sDex = seq { 0 .. (permCount - 1)}
        let aHay = 
            sDex 
            |> Seq.map(fun v -> sDex
                                |> Seq.map(fun w -> (v,w, (Permutation.getDistance sepPerms.[v] sepPerms.[w]))))
            |> Seq.concat |> Seq.toArray
        aHay |> Array.iter(fun v -> Console.WriteLine(sprintf "%A" v))

        Assert.IsTrue(true)


    [<TestMethod>]
    member this.TestMethodPassing() = Assert.IsTrue(true)
