namespace global

type runCfgPlex =
    | Ga of gaCfgPlex
    | Shc of shcCfgPlex


module RunCfgPlex =

   let toInitRunCfgs
            (newGenerations:generation option)
            (reportFilter:generationFilter option)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toInitRunCfgs newGenerations reportFilter gcp
            |> Seq.map(gaRunCfg.InitRun >> runCfg.Ga)
        | Shc scp ->  
            ShcCfgPlex.toInitRunCfgs newGenerations reportFilter scp
            |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)

        

   let toContinueRunCfgs
            (newGenerations:generation)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toContinueRunCfgs newGenerations gcp
            |> Seq.map(gaRunCfg.Continue >> runCfg.Ga)
        | Shc scp ->  
            ShcCfgPlex.toContinueRunCfgs newGenerations scp
            |> Seq.map(shcRunCfg.Continue >> runCfg.Shc)

        

   let toReportEvalsCfg
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (reportFileName:string)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
                GaCfgPlex.toReportEvalsCfg genMin genMax evalCompName reportFilter reportFileName gcp
                |> gaReportCfg.Evals |> gaRunCfg.Report |> runCfg.Ga
        | Shc scp ->  
            ShcCfgPlex.toReportEvalsCfg genMin genMax evalCompName reportFilter reportFileName scp
            |> shcReportCfg.Evals |> shcRunCfg.Report |> runCfg.Shc



   let toReportBinsCfg
            (genMin:generation)
            (genMax:generation)
            (reportFileName:string)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toReportBinsCfg genMin genMax reportFileName gcp
            |> gaReportCfg.Bins |> gaRunCfg.Report |> runCfg.Ga
        | Shc scp ->  
            ShcCfgPlex.toReportBinsCfg genMin genMax reportFileName scp
            |> shcReportCfg.Bins |> shcRunCfg.Report |> runCfg.Shc