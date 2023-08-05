open System
open Argu


module Program = 
    let [<EntryPoint>] main argv =

        let parser = ArgumentParser.Create<CliArguments>(programName = "Klink.Runner.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse argv

        let all = argResults.GetAllResults()
        let workingDirectory = argResults.GetResults Working_Directory
        let projectFolder = argResults.GetResults Project_Folder
        let runFolder = argResults.GetResults Run_Folder
        let controlFile = argResults.GetResults Control_File
        let startingConfigIndex = argResults.GetResults Starting_Config_Index
        let configCount = argResults.GetResults Config_Count
        let logLevel = argResults.GetResults Log_level



        Console.WriteLine($"//////testShcYab/////////")
        let tsStart = DateTime.Now

        let runPath = System.IO.Path.Combine((workingDirectory |> List.head), (projectFolder |> List.head), (runFolder |> List.head))
        Console.WriteLine($"runPath: {runPath}")
        Console.WriteLine($"controlFile: {controlFile}")
        Console.WriteLine($"startingConfigIndex: {startingConfigIndex}")
        Console.WriteLine($"configCount: {configCount}")
        Console.WriteLine($"logLevel: {logLevel}")


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
                            runPath
                            (Exp1Cfg.cfgsForTestRun(2) |> Seq.skip 0)


        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")

        //match yow with
        //| Ok msg -> Console.WriteLine($"done ... {msg}")
        //| Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
