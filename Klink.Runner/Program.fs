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
        
        let nf0 = 0.001 |> NoiseFraction.create |> Some
        let nf1 = 0.025 |> NoiseFraction.create |> Some
        let nf2 = 0.05 |> NoiseFraction.create |> Some
        let nf3 = 0.25 |> NoiseFraction.create |> Some
        let nf4 = 0.5 |> NoiseFraction.create |> Some
        let nf5 = 1.0 |> NoiseFraction.create |> Some
        let nf6 = 2.0 |> NoiseFraction.create |> Some

        
        let sw0 = 0.05 |> StageWeight.create
        let sw1 = 0.1 |> StageWeight.create
        let sw2 = 0.25 |> StageWeight.create
        let sw3 = 0.5 |> StageWeight.create
        let sw4 = 1.0 |> StageWeight.create
        let sw5 = 2.0 |> StageWeight.create
        let sw6 = 4.0 |> StageWeight.create
        let sw7 = 8.0 |> StageWeight.create


        let runId1 = 1 |> RunId.create
        let runId2 = 2 |> RunId.create        
        let runId3 = 3 |> RunId.create
        let runId4 = 4 |> RunId.create
        let runId5 = 5 |> RunId.create
        let runId6 = 6 |> RunId.create
        let runId7 = 7 |> RunId.create
        let runId8 = 8 |> RunId.create
        let runId9 = 9 |> RunId.create

        let runId10 = 10 |> RunId.create
        let runId11 = 11 |> RunId.create        
        let runId12 = 12 |> RunId.create
        let runId13 = 13 |> RunId.create
        let runId14 = 14 |> RunId.create
        let runId15 = 15 |> RunId.create
        let runId16 = 16 |> RunId.create
        let runId17 = 17 |> RunId.create
        let runId18 = 18 |> RunId.create

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


        let runIds = [runId1; runId2; runId3; runId4; runId5; runId6; runId7; runId8;]

        let stageWeights = [sw0; sw1; sw2;]

        let noiseFractions = [nf0; nf1; nf2; ]

        let mutationRates = [mr2; mr5; mr8; ]
        
        Console.WriteLine($"//////747/////////")
        let tsStart = DateTime.Now

        
        let yow = WsOps.reportEmAllG
                    (rootDir |> List.head)
                    [runId1; runId2; runId3; runId4; runId5;]


        //let yow = WsOps.makeEmAll 
        //                    (rootDir |> List.head)
        //                    [runId1; runId2; runId3; runId4; runId5; runId6]
        //                    stageWeights
        //                    noiseFractions
        //                    mutationRates

        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")



        //match yow with
        //| Ok msg -> Console.WriteLine($"done ... {msg}")
        //| Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
