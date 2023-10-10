namespace global
open System
open System.IO


type gaInitRunCfg =
    {
        mutationRate:mutationRate
        newGenerations:generation
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


    let getRunId2 
            (mutationRate:mutationRate) 
            (noiseFraction:noiseFraction) 
            (order:order) 
            (rngGen:rngGen) 
            (sorterEvalMode:sorterEvalMode) 
            (sorterCount:sorterCount) 
            (sorterCountMutated:sorterCount) 
            (sorterSetPruneMethod:sorterSetPruneMethod) 
            (stageWeight:stageWeight) 
            (switchCount:switchCount) 
            (switchGenMode:switchGenMode)
        =
        [
            mutationRate |> MutationRate.value :> obj;
            noiseFraction |> NoiseFraction.value :> obj;
            order |> Order.value :> obj;
            rngGen :> obj;
            sorterEvalMode :> obj
            sorterCount |> SorterCount.value :> obj;
            sorterCountMutated |> SorterCount.value :> obj;
            sorterSetPruneMethod :> obj;
            stageWeight |> StageWeight.value :> obj;
            switchCount :> obj;
            switchGenMode :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create


    let getRunId (cfg:gaInitRunCfg) =
        getRunId2 
            cfg.mutationRate
            cfg.noiseFraction
            cfg.order
            cfg.rngGen
            cfg.sorterEvalMode
            cfg.sorterCount
            cfg.sorterCountMutated
            cfg.sorterSetPruneMethod
            cfg.stageWeight
            cfg.switchCount
            cfg.switchGenMode



    let toWorkspaceParams 
            (useParallel:useParallel)
            (gaCfg:gaInitRunCfg)
        =
        let _nextRngGen rng =
            rng
            |> Rando.fromRngGen
            |> Rando.toRngGen

        let rngGenCreate = (_nextRngGen gaCfg.rngGen)
        let rngGenMutate = (_nextRngGen rngGenCreate)
        let rngGenPrune = (_nextRngGen rngGenMutate)

        WorkspaceParams.make Map.empty
        |> WorkspaceParamsAttrs.setRunId GaWsParamKeys.runId (gaCfg |> getRunId)
        |> WorkspaceParamsAttrs.setGeneration GaWsParamKeys.generation_current (0 |> Generation.create)
        |> WorkspaceParamsAttrs.setGeneration GaWsParamKeys.generation_max (gaCfg.newGenerations)
        |> WorkspaceParamsAttrs.setRngGen GaWsParamKeys.rngGenCreate rngGenCreate
        |> WorkspaceParamsAttrs.setRngGen GaWsParamKeys.rngGenMutate rngGenMutate
        |> WorkspaceParamsAttrs.setRngGen GaWsParamKeys.rngGenPrune rngGenPrune
        |> WorkspaceParamsAttrs.setMutationRate GaWsParamKeys.mutationRate gaCfg.mutationRate
        |> WorkspaceParamsAttrs.setNoiseFraction GaWsParamKeys.noiseFraction (Some gaCfg.noiseFraction)
        |> WorkspaceParamsAttrs.setOrder GaWsParamKeys.order gaCfg.order
        |> WorkspaceParamsAttrs.setSorterCount GaWsParamKeys.sorterCount gaCfg.sorterCount
        |> WorkspaceParamsAttrs.setSorterCount GaWsParamKeys.sorterCountMutated gaCfg.sorterCountMutated
        |> WorkspaceParamsAttrs.setSorterEvalMode GaWsParamKeys.sorterEvalMode gaCfg.sorterEvalMode
        |> WorkspaceParamsAttrs.setStageCount GaWsParamKeys.stagesSkipped gaCfg.stagesSkipped
        |> WorkspaceParamsAttrs.setStageWeight GaWsParamKeys.stageWeight gaCfg.stageWeight
        |> WorkspaceParamsAttrs.setSwitchCount GaWsParamKeys.sorterLength gaCfg.switchCount
        |> WorkspaceParamsAttrs.setSwitchGenMode GaWsParamKeys.switchGenMode gaCfg.switchGenMode
        |> WorkspaceParamsAttrs.setSorterSetPruneMethod GaWsParamKeys.sorterSetPruneMethod gaCfg.sorterSetPruneMethod
        |> WorkspaceParamsAttrs.setGenerationFilter GaWsParamKeys.generation_filter (gaCfg.reportFilter |> Option.get )
        |> WorkspaceParamsAttrs.setUseParallel GaWsParamKeys.useParallel useParallel





type gaContinueRunCfg =
    {
        runId:runId
        newGenerations:generation
    }


type gaRunCfg =
    | InitRun of gaInitRunCfg
    | Continue of gaContinueRunCfg


module GaRunCfg =

    let procGaRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (runCfg:gaRunCfg)
        =
            match runCfg with
            | InitRun irc -> 
                irc |> GaInitRunCfg.toWorkspaceParams up
                    |> InterGenWsOps.startGenLoops projectFolderPath workspaceFileStoreF
            | Continue crc ->
                 InterGenWsOps.continueGenLoops 
                        projectFolderPath 
                        crc.runId 
                        crc.newGenerations
                        workspaceFileStoreF
            //| Report rrc -> ()
            //    //match rrc with
            //    //| Evals rac ->
            //    //    GaReportEvalsCfg.reportAllEvals 
            //    //            projectFolderPath 
            //    //            rac
            //    //| Bins rbc ->
            //    //    GaReportBinsCfgs.reportAllBins
            //    //            projectFolderPath 
            //    //            rbc