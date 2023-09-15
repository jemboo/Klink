namespace Klink.Genome.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Counter () =

        let ctr = Counter.create 0
        let maxCounterValue = 3
        let updateBatchLength = 5
        let mutable errorRept = ""

        let gaCounter = 
                Ga.create
                    (fun c -> c |> Counter.getId)
                    ctr
                    (Counter.update maxCounterValue)
                    (Counter.terminate updateBatchLength)
                    (Counter.archive)


        let res = Ga.update gaCounter (fun m -> errorRept <- m)

        Assert.IsTrue(true);



    [<TestMethod>]
    member this.epiSelect () =

        let lhs = Array.init 10 (fun dex -> dex * 10)
        let rhs = Array.init 10 (id)
        let activations = Array.init 10 (fun dex -> (2.5 - (float dex) / 2.0) |> BoundedFloat.create)
        let epiSites = 
                   Array.zip3 lhs rhs activations
                   |> Array.map(fun (l,r,a) -> EpiSite.create l r a)

        let yipsLow = epiSites |> Array.map(fun ste ->  EpiSite._selectA ste (- 0.5) ( 0.3))
        let yipsMed = epiSites |> Array.map(fun ste ->  EpiSite._selectA ste ( 0.0)  ( 0.3))
        let yipsHi  = epiSites |> Array.map(fun ste ->  EpiSite._selectA ste ( 0.5)  ( 0.3))

        Assert.IsTrue(true);





    [<TestMethod>]
    member this.epiSelectASeq () =
    
        let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
        let selectionNoiseDev = 0.75
        let selectDelta = 0.1

        let lhs = Array.init 200 (fun dex -> dex * 10)
        let rhs = Array.init 200 (id)
        let activations = Array.init 200 (fun dex -> (1.0 - (float dex) / 100.0) |> BoundedFloat.create)
        let mutable epiSites = 
                   Array.zip3 lhs rhs activations
                   |> Array.map(fun (l,r,a) -> EpiSite.create l r a)

        let mutable ctr = 0

        while ctr < 5 do
            let yab = EpiSite.epiSelectA randy selectionNoiseDev selectDelta epiSites
            let biases = yab |> Array.map(fun (_, site) -> site |> EpiSite.getActivation |> BoundedFloat.value)
            let rept = biases |> StringUtil.toCsvLine (fun fv -> fv |> string)
            Console.WriteLine(rept)
            epiSites <- yab |> Array.map(snd)
            ctr <- ctr + 1


        Assert.IsTrue(true);




    [<TestMethod>]
    member this.epiSelectSeq () =
    
        let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
        let selectionNoiseDev = 0.75
        let selectDelta = 0.1

        let lhs = Array.init 200 (fun dex -> dex * 10)
        let rhs = Array.init 200 (id)
        let activations = Array.init 200 (fun dex -> (1.0 - (float dex) / 100.0) |> BoundedFloat.create)
        let mutable epiSites = 
                   Array.zip3 lhs rhs activations
                   |> Array.map(fun (l,r,a) -> EpiSite.create l r a)

        let mutable ctr = 0


        let rept = epiSites |> StringUtil.toCsvLine (fun fv -> fv |> EpiSite.getActivation |> BoundedFloat.value |> string)
        Console.WriteLine(rept)




        while ctr < 5000 do
            let yab = EpiSite.epiSelectB randy selectionNoiseDev selectDelta epiSites
            let biases = yab |> Array.map(fun (_, site) -> site |> EpiSite.getActivation |> BoundedFloat.value)
            let rept = biases |> StringUtil.toCsvLine (fun fv -> fv |> string)
            Console.WriteLine(rept)
            epiSites <- yab |> Array.map(snd)
            ctr <- ctr + 1


        Assert.IsTrue(true);


    [<TestMethod>]
    member this.epiSelectASample () =
    
        let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
        let selectionNoiseDev = 0.75
        let selectDelta = 0.1

        let lhs = Array.init 200 (fun dex -> dex * 10)
        let rhs = Array.init 200 (id)
        let activations = Array.init 200 (fun dex -> (1.0 - (float dex) / 100.0) |> BoundedFloat.create)
        let epiSites = 
                   Array.zip3 lhs rhs activations
                   |> Array.map(fun (l,r,a) -> EpiSite.create l r a)

        let mutable ctr = 0



        let rept = epiSites |> StringUtil.toCsvLine (fun fv -> fv |> EpiSite.getActivation |> BoundedFloat.value |> string)
        Console.WriteLine(rept)



        while ctr < 5000 do
            let yab = EpiSite.epiSelectA randy selectionNoiseDev selectDelta epiSites
            let biases = yab |> Array.map(fun (_, site) -> site |> EpiSite.getActivation |> BoundedFloat.value)
            let rept = biases |> StringUtil.toCsvLine (fun fv -> fv |> string)
            Console.WriteLine(rept)
            ctr <- ctr + 1


        Assert.IsTrue(true);


    [<TestMethod>]
    member this.epiSelectSample () =
    
        let randy = Rando.create rngType.Lcg (123 |> RandomSeed.create)
        let selectionNoiseDev = 0.75
        let selectDelta = 0.1

        let lhs = Array.init 200 (fun dex -> dex * 10)
        let rhs = Array.init 200 (id)
        let activations = Array.init 200 (fun dex -> (1.0 - (float dex) / 100.0) |> BoundedFloat.create)
        let epiSites = 
                   Array.zip3 lhs rhs activations
                   |> Array.map(fun (l,r,a) -> EpiSite.create l r a)

        let mutable ctr = 0

        let rept = epiSites |> StringUtil.toCsvLine (fun fv -> fv |> EpiSite.getActivation |> BoundedFloat.value |> string)
        Console.WriteLine(rept)


        while ctr < 5000 do
            let yab = EpiSite.epiSelectB randy selectionNoiseDev selectDelta epiSites
            let biases = yab |> Array.map(fun (_, site) -> site |> EpiSite.getActivation |> BoundedFloat.value)
            let rept = biases |> StringUtil.toCsvLine (fun fv -> fv |> string)
            Console.WriteLine(rept)
            ctr <- ctr + 1


        Assert.IsTrue(true);
