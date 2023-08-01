﻿open System
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
        



        Console.WriteLine($"//////batchB/////////")
        let tsStart = DateTime.Now
        
        let runFolder = "batchB"
        let runDir = System.IO.Path.Combine((rootDir |> List.head), runFolder)

        //let yow = Exp1Cfg.doReportPerfBins
        //            runDir
        //            (10 |> Generation.create)
        //            (Exp1Cfg.cfgsForCompleteRun())


        //let yow = Exp1Cfg.reportEmAll
        //            runDir
        //            (Exp1Cfg.cfgsForTestRun())

        let yow = Exp1Cfg.doRun 
                            runDir
                            (Exp1Cfg.cfgsForTestRun())

        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")



        match yow with
        | Ok msg -> Console.WriteLine($"done ... {msg}")
        | Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
