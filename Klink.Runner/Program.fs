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

        let logLevel = argResults.GetResults Log_level |> List.head




        let useParallel = argResults.GetResults Use_Parallel |> List.head
                          |> UseParallel.create


        Console.WriteLine($"workingDirectory: {workingDirectory}")
        Console.WriteLine($"projectFolder: {projectFolder}")
        Console.WriteLine($"logLevel: {logLevel}")

        Console.WriteLine($"//////hi/////////")
        let projectFolderPath = System.IO.Path.Combine(workingDirectory, projectFolder)
        Console.WriteLine($"//////{ projectFolderPath }/////////")


        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create

        let tsStart = DateTime.Now



        let reportError (errorMsg:string) =
            Console.WriteLine(errorMsg)
            () |> Ok



        let (scriptFileName, shcRunCfgSet) =
                ShcRun.getNextRunCfgSet projectFolderPath
                |> Result.ExtractOrThrow

        let yab =
                Exp1Run.procRunCfgSet projectFolderPath useParallel shcRunCfgSet
                |> Result.ExtractOrThrow

        let boink = ShcRun.finishScript scriptFileName projectFolderPath
                    |> Result.ExtractOrThrow



        //if procedure = "doRun" then

        //   Exp1Run.doRunRun
        //        runPath
        //        (Exp1Run.cfgsForTestRun(iterationCt) 
        //            |> Seq.skip 0
        //            |> Seq.take configCount
        //            |> Seq.map (Exp1CfgOld.toWorkspaceParams useParallel))
        //        (iterationCt |> Generation.create)
        //    |> ignore

        //elif procedure = "continueRun" then
        //   Exp1Run.continueUpdating
        //        runPath
        //        startingConfigIndex
        //        configCount
        //        iterationCt
        //    |> ignore

        //elif procedure = "reportAll" then
        //    Exp1Reporting.reportEmAll
        //        runPath
        //        reportFileName
        //        wnSorterSetEvalParent
        //        startingConfigIndex
        //        configCount
        //    |> ignore

        //elif procedure = "reportBins" then
        //    Exp1Reporting.doReportPerfBins
        //            runPath
        //            (1 |> Generation.create)
        //    |> ignore

        //else
        //    Console.WriteLine $"procedure: {procedure} is not handled"


        let tsEnd = DateTime.Now

        let tSpan = tsEnd - tsStart 

        Console.WriteLine($"{tSpan.ToString()}")



        Console.Read() |> ignore
        0
