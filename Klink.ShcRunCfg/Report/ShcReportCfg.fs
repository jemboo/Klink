namespace global
open System
open System.IO



type shcReportEvalsCfg =
    {
        reportFileName:string
        runIds:runId array
        genMin:generation
        genMax:generation
        evalCompName:wsComponentName
        reportFilter:generationFilter
    }

module ShcReportEvalsCfg =

    let reportAllEvals
        (projectFolderPath:string)
        (reportCfg:shcReportEvalsCfg)
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



type shcReportBinsCfg =
    {
        reportFileName:string
        runIds:runId array
        genMin:generation
        genMax:generation
    }

module ShcReportBinsCfgs =

    let reportAllBins
        (projectFolderPath:string)
        (reportCfg:shcReportBinsCfg)
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




type shcReportCfg =
    | Evals of shcReportEvalsCfg
    | Bins of shcReportBinsCfg

