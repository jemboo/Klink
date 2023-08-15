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
        let reportFileName = argResults.GetResults Report_File_Name |> List.head
        let procedure = argResults.GetResults Procedure |> List.head
        let startingConfigIndex = argResults.GetResults Starting_Config_Index |> List.head
        let configCount = argResults.GetResults Config_Count |> List.head
        let logLevel = argResults.GetResults Log_level |> List.head
        let iterationCt = argResults.GetResults Iteration_Count |> List.head

        Console.WriteLine($"runPath: {projectFolder}")
        Console.WriteLine($"procedure: {procedure}")
        Console.WriteLine($"startingConfigIndex: {startingConfigIndex}")
        Console.WriteLine($"configCount: {configCount}")
        Console.WriteLine($"logLevel: {logLevel}")
        Console.WriteLine($"iterations: {iterationCt}")

        Console.WriteLine($"//////hi/////////")
        Console.WriteLine($"//////{projectFolder}/////////")

        let tsStart = DateTime.Now
        
        let runPath = System.IO.Path.Combine(workingDirectory, projectFolder) //, runFolder)


        if procedure = "doRun" then
           Exp1Cfg.doRunRun
                        runPath
                        (Exp1Cfg.cfgsForTestRun(iterationCt) 
                            |> Seq.skip startingConfigIndex
                            |> Seq.take configCount)
            |> ignore

        elif procedure = "continueRun" then
           Exp1Cfg.continueUpdating
                        runPath
                        startingConfigIndex
                        configCount
                        iterationCt
            |> ignore

        elif procedure = "reportAll" then
            Exp1Reporting.reportEmAll
                            runPath
                            reportFileName
                            startingConfigIndex
                            configCount
            |> ignore

        elif procedure = "reportBins" then
            Exp1Reporting.doReportPerfBins
                    runPath
                    (1 |> Generation.create)
            |> ignore

        else
            Console.WriteLine $"procedure: {procedure} is not handled"


        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")

        //match yow with
        //| Ok msg -> Console.WriteLine($"done ... {msg}")
        //| Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
