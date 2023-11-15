namespace global

type runCfgPlex =
    | Ga of gaCfgPlex
    | Shc of shcCfgPlex


module RunCfgPlex =

   let projectFolder 
        (plex:runCfgPlex) 
       =
        match plex with
        | Ga gcp -> 
            gcp.projectFolder
        | Shc scp ->  
            scp.projectFolder


   let name 
        (plex:runCfgPlex) 
       =
        match plex with
        | Ga gcp -> 
            gcp.name
        | Shc scp ->  
            scp.name


   let toInitRunCfgs
            (generations:generation)
            (reportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (selectedIndexes: int[] option)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toInitRunCfgs generations reportFilter fullReportFilter selectedIndexes gcp
            |> Seq.map(gaRunCfg.InitRun >> runCfg.Ga)
        | Shc scp ->  
            ShcCfgPlex.toInitRunCfgs generations reportFilter fullReportFilter selectedIndexes scp
            |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)

        
   let toContinueRunCfgs
            (newGenerations:generation)
            (reportGenFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (selectedIndexes: int[] option)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toContinueRunCfgs newGenerations reportGenFilter fullReportFilter selectedIndexes gcp 
            |> Seq.map(gaRunCfg.Continue >> runCfg.Ga)
        | Shc scp ->  
            ShcCfgPlex.toContinueRunCfgs newGenerations reportGenFilter fullReportFilter selectedIndexes scp
            |> Seq.map(shcRunCfg.Continue >> runCfg.Shc)

