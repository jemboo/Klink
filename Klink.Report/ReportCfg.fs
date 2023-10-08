namespace global
open System
open System.IO
open ShcReportEvalsCfg


type reportCfg =
    | Shc of shcReportCfg
    | Ga of gaReportCfg


module ReportCfg =

    let procReportCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (reportCfg:reportCfg)
        =
            match reportCfg with
            | Shc shc -> 
                    shc |> ShcReportCfg.procReportCfg projectFolderPath up workspaceFileStoreF

            | Ga ga -> 
                    ga |> GaReportCfg.procReportCfg projectFolderPath up workspaceFileStoreF



module ReportCfgSet =
    let yab = ()

    //let reportAllFromPlex 
    //        (genMin:generation)
    //        (genMax:generation)
    //        (evalCompName:wsComponentName)
    //        (reportFilter:generationFilter)
    //        (runSetName:string)
    //        (reportFileName:string)
    //        (plex:shcCfgPlex)
    //    =
    //    let runCfg = 
    //        plex |>
    //        ShcCfgPlex.toReportEvalsCfg 
    //                genMin
    //                genMax 
    //                evalCompName 
    //                reportFilter 
    //                reportFileName
    //              |> shcReportCfg.Evals
    //              |> shcRunCfg.Report
    //              |> runCfg.Shc

    //    {setName = runSetName; runCfgs = [|runCfg|]}



    //let reportAllFromPlexSeq 
    //        (genMin:generation)
    //        (genMax:generation)
    //        (evalCompName:wsComponentName)
    //        (reportFilter:generationFilter)
    //        (runSetName:string)
    //        (reportFileName:string)
    //        (plexes:shcCfgPlex seq)
    //    =
    //    let runCfgs = 
    //        plexes 
    //        |> Seq.map(
    //            ShcCfgPlex.toReportEvalsCfg 
    //                    genMin
    //                    genMax
    //                    evalCompName
    //                    reportFilter
    //                    reportFileName)
    //        |> Seq.map(shcReportCfg.Evals >> shcRunCfg.Report >> runCfg.Shc)
    //        |> Seq.toArray

    //    {setName = runSetName; runCfgs = runCfgs}



    //let reportBinsFromPlex 
    //        (genMin:generation)
    //        (genMax:generation)
    //        (runSetName:string)
    //        (reportFileName:string)
    //        (plex:shcCfgPlex)
    //    =
    //    let runCfg = 
    //        plex |>
    //        ShcCfgPlex.toReportBinsCfg 
    //                genMin
    //                genMax
    //                reportFileName
    //              |> shcReportCfg.Bins
    //              |> shcRunCfg.Report
    //              |> runCfg.Shc

    //    {setName = runSetName; runCfgs = [|runCfg|]}



    //let reportBinsFromPlexSeq
    //        (genMin:generation)
    //        (genMax:generation)
    //        (runSetName:string)
    //        (reportFileName:string)
    //        (plexes:shcCfgPlex seq)
    //    =
    //    let runCfgs = 
    //        plexes |> Seq.map(
    //        ShcCfgPlex.toReportBinsCfg 
    //                genMin
    //                genMax
    //                reportFileName)
    //        |> Seq.map(shcReportCfg.Bins >> shcRunCfg.Report  >> runCfg.Shc)
    //        |> Seq.toArray

    //    {setName = runSetName; runCfgs = runCfgs}