﻿namespace global
open System
open System.IO

type shcReportEvalsCfg =
    {
        reportFileName:reportName
        runIds:runId array
        genMin:generation
        genMax:generation
        evalCompName:wsComponentName
        reportFilter:generationFilter
    }


module ShcReportEvalsCfg =

    let reportAllEvals
        (projectFolderPath:string)
        (forEvalsReader: string -> IWorkspaceStore)
        (forReportWriter: string -> IWorkspaceStore)
        (reportCfg:shcReportEvalsCfg)
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
                |> List.map(ShcReporting.reportEvals
                                reportWriter 
                                forEvalsReader
                                reportCfg.reportFileName 
                                reportCfg.evalCompName 
                                reportCfg.genMin)
                |> Result.sequence
                |> Result.map(ignore)
        }



type shcReportBinsCfg =
    {
        reportFileName:reportName
        runIds:runId array
        genMin:generation
        genMax:generation
    }

module ShcReportBinsCfgs =

    let reportAllBins
        (projectFolderPath:string)
        (forEvalsReader: string -> IWorkspaceStore)
        (forReportWriter: string -> IWorkspaceStore)
        (reportCfg:shcReportBinsCfg)
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
                |> List.map(ShcReporting.reportBins 
                                reportWriter 
                                forEvalsReader
                                reportCfg.reportFileName 
                                reportCfg.genMin)
                |> Result.sequence
                |> Result.map(ignore)
        }



type shcReportCfg =
    | Evals of shcReportEvalsCfg
    | Bins of shcReportBinsCfg

module ShcReportCfg =

    let procReportCfg 
            (projectFolderPath:string)
            (useParallel:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (shcReportCfg:shcReportCfg)
        =
            match shcReportCfg with
            | Evals shcReportEvalsCfg ->
                ShcReportEvalsCfg.reportAllEvals
                    projectFolderPath
                    workspaceFileStoreF
                    workspaceFileStoreF
                    shcReportEvalsCfg

            | Bins shcReportBinsCfg -> 
                ShcReportBinsCfgs.reportAllBins
                    projectFolderPath
                    workspaceFileStoreF
                    workspaceFileStoreF
                    shcReportBinsCfg


    let reportName
            (shcReportCfg:shcReportCfg)
        =
            match shcReportCfg with
            | Evals shcReportEvalsCfg ->
                shcReportEvalsCfg.reportFileName

            | Bins shcReportBinsCfg -> 
                shcReportBinsCfg.reportFileName