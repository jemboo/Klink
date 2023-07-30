open System
open Argu


module Program = 
    let [<EntryPoint>] main _ =

        let parser = ArgumentParser.Create<CliArguments>(programName = "gadget.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse [||] // [| "--detach" ; "--listener" ; "localhost" ; "8080" |]

        let all = argResults.GetAllResults()
        let rootDir = argResults.GetResults Working_Directory
        let logLevel = argResults.GetResults Log_Level
        let ears = argResults.GetResults Listener
        


        let rngGen0 = 10110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen1 = 20110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen2 = 22110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen3 = 12110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen4 = 13110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen5 = 31110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen6 = 14110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen7 = 15110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen8 = 16110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen9 = 17110 |> RandomSeed.create |> RngGen.createLcg
        let rngGen10 = 1811 |> RandomSeed.create |> RngGen.createLcg
        let rngGen11 = 1911 |> RandomSeed.create |> RngGen.createLcg
        let rngGen12 = 2011 |> RandomSeed.create |> RngGen.createLcg
        let rngGen13 = 2111 |> RandomSeed.create |> RngGen.createLcg
        let rngGen14 = 2211 |> RandomSeed.create |> RngGen.createLcg


        let nf0 = 0.001 |> NoiseFraction.create
        let nf1 = 0.025 |> NoiseFraction.create
        let nf2 = 0.050 |> NoiseFraction.create
        let nf3 = 0.250 |> NoiseFraction.create |> Some
        let nf4 = 0.500 |> NoiseFraction.create |> Some
        let nf5 = 1.000 |> NoiseFraction.create |> Some
        let nf6 = 2.000 |> NoiseFraction.create |> Some

        
        let sw0 = 0.05 |> StageWeight.create
        let sw1 = 0.10 |> StageWeight.create
        let sw2 = 0.25 |> StageWeight.create
        let sw3 = 0.50 |> StageWeight.create
        let sw4 = 1.00 |> StageWeight.create
        let sw5 = 2.00 |> StageWeight.create
        let sw6 = 4.00 |> StageWeight.create
        let sw7 = 8.00 |> StageWeight.create


        let mr0 = 0.0005 |> MutationRate.create
        let mr1 = 0.0010 |> MutationRate.create
        let mr2 = 0.0020 |> MutationRate.create
        let mr3 = 0.0030 |> MutationRate.create
        let mr4 = 0.0050 |> MutationRate.create
        let mr5 = 0.0075 |> MutationRate.create
        let mr6 = 0.0100 |> MutationRate.create
        let mr7 = 0.0250 |> MutationRate.create
        let mr8 = 0.0500 |> MutationRate.create
        let mr9 = 0.0750 |> MutationRate.create

        let sspm1 = sorterSetPruneMethod.Whole
        let sspm2 = sorterSetPruneMethod.Shc

        
        let rngGens = [rngGen0; rngGen1; rngGen2; rngGen3;]

        let stageWeights = [sw0;] // sw1; sw2;]

        let noiseFractions = [nf0;] // nf1; nf2; ]

        let mutationRates = [mr5; mr7; mr8; mr2; mr4; mr6]
        
        let sorterSetPruneMethods = [sspm1]

        let maxGen = 1000 |> Generation.create

        Console.WriteLine($"//////ppp78/////////")
        let tsStart = DateTime.Now

        
        let yow = Exp1Cfg.reportEmAllG
                    (rootDir |> List.head)
                    rngGens
                    stageWeights
                    noiseFractions
                    mutationRates
                    sorterSetPruneMethods


        //let yow = Exp1Cfg.makeEmAll 
        //                    (rootDir |> List.head)
        //                    rngGens
        //                    stageWeights
        //                    noiseFractions
        //                    mutationRates
        //                    maxGen
        //                    sorterSetPruneMethods

        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")



        match yow with
        | Ok msg -> Console.WriteLine($"done ... {msg}")
        | Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
