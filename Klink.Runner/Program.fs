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
        



        Console.WriteLine($"//////testShcYab/////////")
        let tsStart = DateTime.Now
        
        let runFolder = "testShc"
        let runDir = System.IO.Path.Combine((rootDir |> List.head), runFolder)

        //let yow = Exp1Cfg.doReportPerfBins
        //            runDir
        //            (1 |> Generation.create)
        //            (Exp1Cfg.cfgsForTestRun(8))


        //let yow = Exp1Cfg.doReportPerfBins2
        //            runDir
        //            (1 |> Generation.create)


        //let yow = Exp1Cfg.reportEmAll2
        //            runDir

        let yow = Exp1Cfg.doRunRun
                            runDir
                            (Exp1Cfg.cfgsForTestRun(2) |> Seq.skip 0)


        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")



        match yow with
        | Ok msg -> Console.WriteLine($"done ... {msg}")
        | Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
