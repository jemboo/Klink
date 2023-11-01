namespace global
open System
open System.IO


type shcInitRunCfg =
    {
        runId:runId
        sortableSetCfgType:sortableSetCfgType
        mutationRate:mutationRate
        newGenerations:generation
        noiseFraction:noiseFraction
        order:order
        rngGen:rngGen
        sorterEvalMode:sorterEvalMode
        sorterCount:sorterCount
        sorterCountMutated:sorterCount
        sorterSetPruneMethod:sorterSetPruneMethod
        maxPhenotypeForPrune:sorterCount option
        stagesSkipped:stageCount
        stageWeight:stageWeight
        switchCount:switchCount
        switchGenMode:switchGenMode
        reportFilter:generationFilter option
    }


module ShcInitRunCfg =


    let getRunId2 
            (sortableSetCfgType:sortableSetCfgType)
            (mutationRate:mutationRate) 
            (noiseFraction:noiseFraction) 
            (order:order) 
            (rngGen:rngGen) 
            (sorterEvalMode:sorterEvalMode) 
            (sorterCount:sorterCount) 
            (sorterCountMutated:sorterCount) 
            (sorterSetPruneMethod:sorterSetPruneMethod) 
            (maxPhenotypeForPrune: sorterCount option)
            (stageWeight:stageWeight) 
            (switchCount:switchCount) 
            (switchGenMode:switchGenMode)
        =
        [
            sortableSetCfgType :> obj
            mutationRate |> MutationRate.value :> obj;
            noiseFraction |> NoiseFraction.value :> obj;
            order |> Order.value :> obj;
            rngGen :> obj;
            sorterEvalMode :> obj
            sorterCount |> SorterCount.value :> obj;
            sorterCountMutated |> SorterCount.value :> obj;
            sorterSetPruneMethod :> obj;
            maxPhenotypeForPrune :> obj;
            stageWeight |> StageWeight.value :> obj;
            switchCount :> obj;
            switchGenMode :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create


    let getRunId (cfg:shcInitRunCfg) =
        getRunId2 
            cfg.sortableSetCfgType
            cfg.mutationRate
            cfg.noiseFraction
            cfg.order
            cfg.rngGen
            cfg.sorterEvalMode
            cfg.sorterCount
            cfg.sorterCountMutated
            cfg.sorterSetPruneMethod
            cfg.maxPhenotypeForPrune
            cfg.stageWeight
            cfg.switchCount
            cfg.switchGenMode



    let toWorkspaceParams 
            (useParallel:useParallel)
            (shcInitRunCfg:shcInitRunCfg)
        =
        let _nextRngGen rng =
            rng
            |> Rando.fromRngGen
            |> Rando.toRngGen

        let rngGenCreate = (_nextRngGen shcInitRunCfg.rngGen)
        let rngGenMutate = (_nextRngGen rngGenCreate)
        let rngGenPrune = (_nextRngGen rngGenMutate)

        WorkspaceParams.make Map.empty
        |> WorkspaceParamsAttrs.setRunId ShcWsParamKeys.runId (shcInitRunCfg |> getRunId)
        |> WorkspaceParamsAttrs.setSortableSetCfgType ShcWsParamKeys.sortableSetCfgType (shcInitRunCfg.sortableSetCfgType)
        |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_current (0 |> Generation.create)
        |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_max (shcInitRunCfg.newGenerations)
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenCreate rngGenCreate
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenMutate rngGenMutate
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenPrune rngGenPrune
        |> WorkspaceParamsAttrs.setMutationRate ShcWsParamKeys.mutationRate shcInitRunCfg.mutationRate
        |> WorkspaceParamsAttrs.setNoiseFraction ShcWsParamKeys.noiseFraction (Some shcInitRunCfg.noiseFraction)
        |> WorkspaceParamsAttrs.setOrder ShcWsParamKeys.order shcInitRunCfg.order
        |> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCount shcInitRunCfg.sorterCount
        |> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCountMutated shcInitRunCfg.sorterCountMutated
        |> WorkspaceParamsAttrs.setSorterEvalMode ShcWsParamKeys.sorterEvalMode shcInitRunCfg.sorterEvalMode
        |> WorkspaceParamsAttrs.setStageCount ShcWsParamKeys.stagesSkipped shcInitRunCfg.stagesSkipped
        |> WorkspaceParamsAttrs.setStageWeight ShcWsParamKeys.stageWeight shcInitRunCfg.stageWeight
        |> WorkspaceParamsAttrs.setSwitchCount ShcWsParamKeys.sorterLength shcInitRunCfg.switchCount
        |> WorkspaceParamsAttrs.setSwitchGenMode ShcWsParamKeys.switchGenMode shcInitRunCfg.switchGenMode
        |> WorkspaceParamsAttrs.setSorterSetPruneMethod ShcWsParamKeys.sorterSetPruneMethod shcInitRunCfg.sorterSetPruneMethod
        |> WorkspaceParamsAttrs.setSorterCountOption ShcWsParamKeys.maxPrunedPhenotypeCount shcInitRunCfg.maxPhenotypeForPrune
        |> WorkspaceParamsAttrs.setGenerationFilter ShcWsParamKeys.generation_filter (shcInitRunCfg.reportFilter |> Option.get )
        |> WorkspaceParamsAttrs.setUseParallel ShcWsParamKeys.useParallel useParallel



type shcContinueRunCfg =
    {
        runId:runId
        newGenerations:generation
        reportGenFilter:generationFilter
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
            | Continue shcContinueRunCfg -> 
                InterGenWsOps.continueGenLoops
                    projectFolderPath
                    shcContinueRunCfg.runId
                    shcContinueRunCfg.newGenerations
                    shcContinueRunCfg.reportGenFilter
                    workspaceFileStoreF