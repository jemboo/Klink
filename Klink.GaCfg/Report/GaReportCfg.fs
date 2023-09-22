namespace global
open System
open System.IO


type gaReportEvalsCfg =
    {
        reportFileName:string
        runIds:runId array
        genMin:generation
        genMax:generation
        evalCompName:wsComponentName
        reportFilter:generationFilter
    }

module GaReportEvalsCfg =
    let fromPlex
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (reportFileName:string)
            (plex:gaCfgPlex)
        =
        let runIds =
             GaCfgPlex.toInitRunCfgs None None plex
             |> Seq.map(GaInitRunCfg.getRunId)
             |> Seq.toArray
        {
            gaReportEvalsCfg.reportFileName = reportFileName
            runIds = runIds
            genMin = genMin
            genMax = genMax
            evalCompName = evalCompName
            reportFilter = reportFilter
        }


    let reportAllEvals
        (projectFolderPath:string)
        (reportCfg:gaReportEvalsCfg)
        =
        let fsReporter = new WorkspaceFileStore(Path.Combine(projectFolderPath, "Reports"))

        let runDirs = reportCfg.runIds
                        |> Array.map(RunId.value >> string)
                        |> Array.map(fun fldr -> Path.Combine(projectFolderPath, fldr))
                        |> Array.toList

        result {
            return! 
                runDirs 
                |> List.map(Reporting.reportEvals 
                                fsReporter 
                                reportCfg.reportFileName 
                                reportCfg.evalCompName 
                                reportCfg.genMin)
                |> Result.sequence
                |> Result.map(ignore)
        }



type gaReportBinsCfg =
    {
        reportFileName:string
        runIds:runId array
        genMin:generation
        genMax:generation
    }

module GaReportBinsCfgs =
    let reportAllBins
        (projectFolderPath:string)
        (reportCfg:gaReportBinsCfg)
        =
        let fsReporter = new WorkspaceFileStore(Path.Combine(projectFolderPath, "Reports"))

        let runDirs = reportCfg.runIds
                        |> Array.map(RunId.value >> string)
                        |> Array.map(fun fldr -> Path.Combine(projectFolderPath, fldr))
                        |> Array.toList

        result {
            return! 
                runDirs 
                |> List.map(Reporting.reportBins 
                                fsReporter 
                                reportCfg.reportFileName 
                                reportCfg.genMin)
                |> Result.sequence
                |> Result.map(ignore)
        }




type gaReportCfg =
    | Evals of gaReportEvalsCfg
    | Bins of gaReportBinsCfg
