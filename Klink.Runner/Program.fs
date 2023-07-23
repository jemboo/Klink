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

        let nf1 = 0.025 |> NoiseFraction.create |> Some
        let nf2 = 0.05 |> NoiseFraction.create |> Some
        let nf3 = 0.25 |> NoiseFraction.create |> Some
        let nf4 = 0.5 |> NoiseFraction.create |> Some
        let nf5 = 1.0 |> NoiseFraction.create |> Some
        let nf6 = 2.0 |> NoiseFraction.create |> Some


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



        let runIds = [runId1;] // runId2; runId3; runId4; runId5; runId6; runId7; runId8; runId9;]

        let stageWeights = [sw1; sw2; sw3; sw4; sw5; sw6; sw7]

        let noiseFractions = [nf1; nf2; nf3; nf4; nf5; nf6; ]


        //let yow = WsOps.reportEmAll 
        //            (rootDir |> List.head)
        //            runIds


        let tsStart = DateTime.Now

        let yow = WsOps.makeEmAll 
                            (rootDir |> List.head)
                            runIds
                            stageWeights
                            noiseFractions


        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")



        //let yow = WsOps.makeEm 
        //                (rootDir |> List.head)
        //                runId3
        //                sw3
        //                nf1

        //let yow = WsOps.reportEm 
        //            (rootDir |> List.head)
        //            runId1

        //match yow with
        //| Ok msg -> Console.WriteLine($"done ... {msg}")
        //| Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
