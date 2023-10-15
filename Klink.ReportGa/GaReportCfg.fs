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

    //let fromPlex
    //        (genMin:generation)
    //        (genMax:generation)
    //        (evalCompName:wsComponentName)
    //        (reportFilter:generationFilter)
    //        (reportFileName:string)
    //        (plex:gaCfgPlex)
    //    =
    //    let runIds =
    //         GaCfgPlex.toRunIds plex
    //         |> Seq.toArray
    //    {
    //        gaReportEvalsCfg.reportFileName = reportFileName
    //        runIds = runIds
    //        genMin = genMin
    //        genMax = genMax
    //        evalCompName = evalCompName
    //        reportFilter = reportFilter
    //    }


    let reportAllEvals
        (projectFolderPath:string)
        (forEvalsReader: string -> IWorkspaceStore)
        (forReportWriter: string -> IWorkspaceStore)
        (reportCfg:gaReportEvalsCfg)
        =
        let reportPath = Path.Combine(projectFolderPath, "Reports")
        let reportWriter =  forReportWriter reportPath

        let runDirs = reportCfg.runIds
                        |> Array.map(RunId.value >> string)
                        |> Array.map(fun fldr -> Path.Combine(projectFolderPath, fldr))
                        |> Array.toList

        result {
            return! 
                runDirs 
                |> List.map(GaReporting.reportEvals
                                reportWriter 
                                forEvalsReader
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
            (forEvalsReader: string -> IWorkspaceStore)
            (forReportWriter: string -> IWorkspaceStore)
            (reportCfg:gaReportBinsCfg)
            =
            let reportPath = Path.Combine(projectFolderPath, "Reports")
            let reportWriter =  forReportWriter reportPath

            let runDirs = reportCfg.runIds
                            |> Array.map(RunId.value >> string)
                            |> Array.map(fun fldr -> Path.Combine(projectFolderPath, fldr))
                            |> Array.toList

            result {
                return! 
                    runDirs 
                    |> List.map(GaReporting.reportBins 
                                    reportWriter 
                                    forEvalsReader
                                    reportCfg.reportFileName 
                                    reportCfg.genMin)
                    |> Result.sequence
                    |> Result.map(ignore)
            }




type gaReportCfg =
    | Evals of gaReportEvalsCfg
    | Bins of gaReportBinsCfg



module GaReportCfg =

    let procReportCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (gaReportCfg:gaReportCfg)
        =
            match gaReportCfg with
            | Evals a -> ()

            | Bins b -> ()