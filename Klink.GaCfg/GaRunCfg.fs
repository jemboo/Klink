namespace global
open System
open System.IO



type gaInitRunCfg =
    {
        mutationRate:mutationRate
        newGenerations:generation option
        noiseFraction:noiseFraction
        order:order
        rngGen:rngGen
        sorterEvalMode:sorterEvalMode
        sorterCount:sorterCount
        sorterCountMutated:sorterCount
        sorterSetPruneMethod:sorterSetPruneMethod
        stagesSkipped:stageCount
        stageWeight:stageWeight
        switchCount:switchCount
        switchGenMode:switchGenMode
        reportFilter:generationFilter option
    }


module GaInitRunCfg =

    let getRunId (cfg:gaInitRunCfg) =
        [
            cfg.mutationRate |> MutationRate.value :> obj;
            cfg.noiseFraction |> NoiseFraction.value :> obj;
            cfg.order |> Order.value :> obj;
            cfg.rngGen :> obj;
            cfg.sorterEvalMode :> obj
            cfg.sorterCount |> SorterCount.value :> obj;
            cfg.sorterCountMutated |> SorterCount.value :> obj;
            cfg.sorterSetPruneMethod :> obj;
            cfg.stageWeight |> StageWeight.value :> obj;
            cfg.switchCount :> obj;
            cfg.switchGenMode :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create




type gaContinueRunCfg =
    {
        runId:runId
        newGenerations:generation
    }


module GaContinueRunCfg =
    let yab = ()
    //let fromPlex
    //        (newGenerations:generation)
    //        (plex:gaInitRunCfgPlex)
    //    =

    //    let _toCrc newGenerations (gaCfg:gaInitRunCfg) =
    //            { 
    //                gaContinueRunCfg.runId = (gaCfg |> GaInitRunCfg.getRunId);
    //                newGenerations = newGenerations
    //            }
    //    GaInitRunCfg.fromPlex None None plex
    //    |> Seq.map(_toCrc newGenerations)



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
    let yab = ()
    //let fromPlex
    //        (genMin:generation)
    //        (genMax:generation)
    //        (evalCompName:wsComponentName)
    //        (reportFilter:generationFilter)
    //        (reportFileName:string)
    //        (plex:gaInitRunCfgPlex)
    //    =
    //    let runIds =
    //         GaInitRunCfg.fromPlex None None plex
    //         |> Seq.map(GaInitRunCfg.getRunId)
    //         |> Seq.toArray
    //    {
    //        gaReportEvalsCfg.reportFileName = reportFileName
    //        runIds = runIds
    //        genMin = genMin
    //        genMax = genMax
    //        evalCompName = evalCompName
    //        reportFilter = reportFilter
    //    }


    //let reportAllEvals
    //    (projectFolderPath:string)
    //    (reportCfg:gaReportEvalsCfg)
    //    =
    //    let fsReporter = new WorkspaceFileStore(Path.Combine(projectFolderPath, "Reports"))

    //    let runDirs = reportCfg.runIds
    //                    |> Array.map(RunId.value >> string)
    //                    |> Array.map(fun fldr -> Path.Combine(projectFolderPath, fldr))
    //                    |> Array.toList

    //    result {
    //        return! 
    //            runDirs 
    //            |> List.map(Reporting.reportEvals 
    //                            fsReporter 
    //                            reportCfg.reportFileName 
    //                            reportCfg.evalCompName 
    //                            reportCfg.genMin)
    //            |> Result.sequence
    //            |> Result.map(ignore)
    //    }



type gaReportBinsCfg =
    {
        reportFileName:string
        runIds:runId array
        genMin:generation
        genMax:generation
    }

module GaReportBinsCfgs =
    let yab = ()
    //let reportAllBins
    //    (projectFolderPath:string)
    //    (reportCfg:gaReportBinsCfg)
    //    =
    //    let fsReporter = new WorkspaceFileStore(Path.Combine(projectFolderPath, "Reports"))

    //    let runDirs = reportCfg.runIds
    //                    |> Array.map(RunId.value >> string)
    //                    |> Array.map(fun fldr -> Path.Combine(projectFolderPath, fldr))
    //                    |> Array.toList

    //    result {
    //        return! 
    //            runDirs 
    //            |> List.map(Reporting.reportBins 
    //                            fsReporter 
    //                            reportCfg.reportFileName 
    //                            reportCfg.genMin)
    //            |> Result.sequence
    //            |> Result.map(ignore)
    //    }




type gaReportCfg =
    | Evals of gaReportEvalsCfg
    | Bins of gaReportBinsCfg






type gaRunCfg =
    | InitRun of gaInitRunCfg
    | Continue of gaContinueRunCfg
    | Report of gaReportCfg


module GaRunCfg =

    let procGaRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (runCfg:gaRunCfg)
        =
            match runCfg with
            | InitRun irc -> ()
                //irc |> GaInitRunCfg.toWorkspaceParams up
                //    |> InterGenWsOps.doGenLoop projectFolderPath
            | Continue crc -> ()
                 //InterGenWsOps.continueUpdating 
                 //       projectFolderPath 
                 //       crc.runId 
                 //       crc.newGenerations
            | Report rrc -> ()
                //match rrc with
                //| Evals rac ->
                //    GaReportEvalsCfg.reportAllEvals 
                //            projectFolderPath 
                //            rac
                //| Bins rbc ->
                //    GaReportBinsCfgs.reportAllBins
                //            projectFolderPath 
                //            rbc