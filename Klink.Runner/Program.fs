open System
open Argu


module Program = 
    let [<EntryPoint>] main argv =

        let parser = ArgumentParser.Create<CliArguments>(programName = "Klink.Runner.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse argv

        let all = argResults.GetAllResults()
        let workingDirectory = argResults.GetResults Working_Directory |> List.head
        let projectFolder = argResults.GetResults Project_Folder |> List.head
        let projectFolder = argResults.GetResults Project_Folder |> List.head
        let controlFile = argResults.GetResults Control_File |> List.head
        let startingConfigIndex = argResults.GetResults Starting_Config_Index |> List.head
        let configCount = argResults.GetResults Config_Count |> List.head
        let logLevel = argResults.GetResults Log_level |> List.head
        let iterationCt = argResults.GetResults Iteration_Count |> List.head

        Console.WriteLine($"runPath: {projectFolder}")
        Console.WriteLine($"controlFile: {controlFile}")
        Console.WriteLine($"startingConfigIndex: {startingConfigIndex}")
        Console.WriteLine($"configCount: {configCount}")
        Console.WriteLine($"logLevel: {logLevel}")
        Console.WriteLine($"iterations: {iterationCt}")

        Console.WriteLine($"//////{projectFolder}/////////")
        let tsStart = DateTime.Now
        
        let runPath = System.IO.Path.Combine(workingDirectory, projectFolder) //, runFolder)



        //let yow = GaReporting.doReportPerfBins
        //            runPath
        //            (1 |> Generation.create)


        //let yow = GaReporting.reportEmAll
        //            runPath


        let yow = Exp1Cfg.doRunRun
                        runPath
                        (Exp1Cfg.cfgsForTestRun(0) 
                            |> Seq.skip startingConfigIndex
                            |> Seq.take configCount)


        //let yow = Exp1Cfg.continueUpdating
        //            runPath
        //            startingConfigIndex
        //            configCount
        //            iterationCt


        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")

        //match yow with
        //| Ok msg -> Console.WriteLine($"done ... {msg}")
        //| Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
