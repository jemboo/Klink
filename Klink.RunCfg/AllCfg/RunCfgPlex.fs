namespace global

type runCfgPlex =
    | Ga of gaCfgPlex
    | Shc of shcCfgPlex


module RunCfgPlex =

   let toInitRunCfgs
            (generations:generation)
            (reportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (seqSplicer: (int*int) option)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toInitRunCfgs generations reportFilter fullReportFilter seqSplicer gcp
            |> Seq.map(gaRunCfg.InitRun >> runCfg.Ga)
        | Shc scp ->  
            ShcCfgPlex.toInitRunCfgs generations reportFilter fullReportFilter seqSplicer scp
            |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)

        
   let toContinueRunCfgs
            (newGenerations:generation)
            (reportGenFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (seqSplicer: (int*int) option)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toContinueRunCfgs newGenerations reportGenFilter fullReportFilter seqSplicer gcp 
            |> Seq.map(gaRunCfg.Continue >> runCfg.Ga)
        | Shc scp ->  
            ShcCfgPlex.toContinueRunCfgs newGenerations reportGenFilter fullReportFilter seqSplicer scp
            |> Seq.map(shcRunCfg.Continue >> runCfg.Shc)