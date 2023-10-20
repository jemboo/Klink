namespace global
open System
open System.IO


type shcInitRunCfg =
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


module ShcInitRunCfg =


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


    let getRunId (cfg:shcInitRunCfg) =
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
            (gaCfg:shcInitRunCfg)
        =
        let _nextRngGen rng =
            rng
            |> Rando.fromRngGen
            |> Rando.toRngGen

        let rngGenCreate = (_nextRngGen gaCfg.rngGen)
        let rngGenMutate = (_nextRngGen rngGenCreate)
        let rngGenPrune = (_nextRngGen rngGenMutate)

        WorkspaceParams.make Map.empty
        |> WorkspaceParamsAttrs.setRunId ShcWsParamKeys.runId (gaCfg |> getRunId)
        |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_current (0 |> Generation.create)
        |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_max (gaCfg.newGenerations)
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenCreate rngGenCreate
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenMutate rngGenMutate
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenPrune rngGenPrune
        |> WorkspaceParamsAttrs.setMutationRate ShcWsParamKeys.mutationRate gaCfg.mutationRate
        |> WorkspaceParamsAttrs.setNoiseFraction ShcWsParamKeys.noiseFraction (Some gaCfg.noiseFraction)
        |> WorkspaceParamsAttrs.setOrder ShcWsParamKeys.order gaCfg.order
        |> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCount gaCfg.sorterCount
        |> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCountMutated gaCfg.sorterCountMutated
        |> WorkspaceParamsAttrs.setSorterEvalMode ShcWsParamKeys.sorterEvalMode gaCfg.sorterEvalMode
        |> WorkspaceParamsAttrs.setStageCount ShcWsParamKeys.stagesSkipped gaCfg.stagesSkipped
        |> WorkspaceParamsAttrs.setStageWeight ShcWsParamKeys.stageWeight gaCfg.stageWeight
        |> WorkspaceParamsAttrs.setSwitchCount ShcWsParamKeys.sorterLength gaCfg.switchCount
        |> WorkspaceParamsAttrs.setSwitchGenMode ShcWsParamKeys.switchGenMode gaCfg.switchGenMode
        |> WorkspaceParamsAttrs.setSorterSetPruneMethod ShcWsParamKeys.sorterSetPruneMethod gaCfg.sorterSetPruneMethod
        |> WorkspaceParamsAttrs.setGenerationFilter ShcWsParamKeys.generation_filter (gaCfg.reportFilter |> Option.get )
        |> WorkspaceParamsAttrs.setUseParallel ShcWsParamKeys.useParallel useParallel



type shcContinueRunCfg =
    {
        runId:runId
        newGenerations:generation
    }


type shcRunCfg =
    | InitRun of shcInitRunCfg
    | Continue of shcContinueRunCfg


module ShcRunCfg =

    let procShcRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (shcRunCfg:shcRunCfg)
        =
            match shcRunCfg with
            | InitRun irc -> 
                irc |> ShcInitRunCfg.toWorkspaceParams up
                    |> InterGenWsOps.runGenLoops projectFolderPath workspaceFileStoreF
            | Continue crc -> 
                InterGenWsOps.continueGenLoops
                    projectFolderPath
                    crc.runId
                    crc.newGenerations
                    workspaceFileStoreF