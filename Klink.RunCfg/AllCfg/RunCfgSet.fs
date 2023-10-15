namespace global


type runCfgSet = { setName:string; 
                   runCfgs:runCfg[]  }


module RunCfgSet =
    
    let private _runName 
                    (prefix:string) 
                    (maxChunk:int) 
                    (dex:int) 
                =
        sprintf "%s_%d" prefix (maxChunk * dex)


    let procRunCfgSet 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (rcs:runCfgSet)
        =
        rcs.runCfgs
        |> Array.map(RunCfg.procRunCfg projectFolderPath up workspaceFileStoreF)



    let initShcRunFromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (runSetName:string)
            (plex:shcCfgPlex)
        =
        let runCfgs = 
            plex |>
            ShcCfgPlex.toInitRunCfgs newGenerations (Some reportFilter)
                  |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)
                  |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}


    let initGaRunFromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (runSetName:string)
            (plex:gaCfgPlex)
        =
        let runCfgs = 
            plex |>
            GaCfgPlex.toInitRunCfgs newGenerations (Some reportFilter)
                  |> Seq.map(gaRunCfg.InitRun >> runCfg.Ga)
                  |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}


    let initShcRunsFromPlex
            (generations:generation)
            (reportFilter:generationFilter)
            (runSetPfx:string)
            (maxRunCount:int)
            (plex:shcCfgPlex)
        =
        let runCfgs = 
            plex |>
            ShcCfgPlex.toInitRunCfgs generations (Some reportFilter)
                  |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)
                  |> Seq.chunkBySize maxRunCount
                  |> Seq.toArray

        runCfgs |> Array.mapi(
            fun dex cfgs -> 
                { setName = _runName runSetPfx maxRunCount dex;
                  runCfgs = cfgs}
                )


    let initGaRunsFromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (runSetPfx:string)
            (maxRunCount:int)
            (plex:gaCfgPlex)
        =
        let runCfgs = 
            plex |>
            GaCfgPlex.toInitRunCfgs newGenerations (Some reportFilter)
                  |> Seq.map(gaRunCfg.InitRun >> runCfg.Ga)
                  |> Seq.chunkBySize maxRunCount
                  |> Seq.toArray

        runCfgs |> Array.mapi(
            fun dex cfgs -> 
                { setName = _runName runSetPfx maxRunCount dex;
                  runCfgs = cfgs}
                )


    let initRunFromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (runSetName:string)
            (plex:runCfgPlex)
        =
        match plex with
        | Shc shcCfgPlex -> 
            shcCfgPlex |> initShcRunFromPlex newGenerations reportFilter runSetName
        | Ga gaCfgPlex -> 
            gaCfgPlex |> initGaRunFromPlex newGenerations reportFilter runSetName




    let initRunsFromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (runSetName:string)
            (maxRunCount:int)
            (plex:runCfgPlex)
        =
        match plex with
        | Shc shcCfgPlex -> 
            shcCfgPlex |> initShcRunsFromPlex newGenerations reportFilter runSetName maxRunCount
        | Ga gaCfgPlex -> 
            gaCfgPlex |> initGaRunsFromPlex newGenerations reportFilter runSetName maxRunCount




    let continueShcRunFromPlex 
            (newGenerations:generation)
            (runSetName:string)
            (plex:shcCfgPlex)
        =
        let runCfgs = 
            plex |>
            ShcCfgPlex.toContinueRunCfgs newGenerations
                  |> Seq.map(shcRunCfg.Continue >> runCfg.Shc)
                  |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}



    let continueGaRunFromPlex 
            (newGenerations:generation)
            (runSetName:string)
            (plex:gaCfgPlex)
        =
        let runCfgs = 
            plex |>
            GaCfgPlex.toContinueRunCfgs newGenerations
                  |> Seq.map(gaRunCfg.Continue >> runCfg.Ga)
                  |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}



    let continueRunFromPlex 
            (newGenerations:generation)
            (runSetName:string)
            (plex:runCfgPlex)
        =
        match plex with
        | Shc shcCfgPlex -> 
            shcCfgPlex |> continueShcRunFromPlex newGenerations runSetName
        | Ga gaCfgPlex -> 
            gaCfgPlex |> continueGaRunFromPlex newGenerations runSetName




    let reportAllFromPlex 
            (newGenerations:generation)
            (runSetName:string)
            (plex:runCfgPlex)
        =
        match plex with
        | Shc shcCfgPlex -> 
            shcCfgPlex |> continueShcRunFromPlex newGenerations runSetName
        | Ga gaCfgPlex -> 
            gaCfgPlex |> continueGaRunFromPlex newGenerations runSetName





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