namespace global

type runCfgPlex =
    | Ga of gaCfgPlex
    | Shc of shcCfgPlex


module RunCfgPlex =

   let toInitRunCfgs
            (generations:generation)
            (reportFilter:generationFilter option)
            (seqSplicer: (int*int) option)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toInitRunCfgs generations reportFilter seqSplicer gcp
            |> Seq.map(gaRunCfg.InitRun >> runCfg.Ga)
        | Shc scp ->  
            ShcCfgPlex.toInitRunCfgs generations reportFilter seqSplicer scp
            |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)

        
   let toContinueRunCfgs
            (newGenerations:generation)
            (seqSplicer: (int*int) option)
            (plex:runCfgPlex)
        =
        match plex with
        | Ga gcp -> 
            GaCfgPlex.toContinueRunCfgs newGenerations seqSplicer gcp 
            |> Seq.map(gaRunCfg.Continue >> runCfg.Ga)
        | Shc scp ->  
            ShcCfgPlex.toContinueRunCfgs newGenerations seqSplicer scp
            |> Seq.map(shcRunCfg.Continue >> runCfg.Shc)