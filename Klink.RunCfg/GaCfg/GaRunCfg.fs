namespace global
open System
open System.IO


type gaInitRunCfg =
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
        stagesSkipped:stageCount
        stageWeight:stageWeight
        switchCount:switchCount
        switchGenMode:switchGenMode
        reportFilter:generationFilter option
    }


module GaInitRunCfg =


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
            stageWeight |> StageWeight.value :> obj;
            switchCount :> obj;
            switchGenMode :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create


    let getRunId (cfg:gaInitRunCfg) =
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
            cfg.stageWeight
            cfg.switchCount
            cfg.switchGenMode



    let toWorkspaceParams 
            (useParallel:useParallel)
            (gaInitRunCfg:gaInitRunCfg)
        =
        let _nextRngGen rng =
            rng
            |> Rando.fromRngGen
            |> Rando.toRngGen

        let rngGenCreate = (_nextRngGen gaInitRunCfg.rngGen)
        let rngGenMutate = (_nextRngGen rngGenCreate)
        let rngGenPrune = (_nextRngGen rngGenMutate)

        WorkspaceParams.make Map.empty
        |> WorkspaceParamsAttrs.setRunId GaWsParamKeys.runId (gaInitRunCfg |> getRunId)
        |> WorkspaceParamsAttrs.setSortableSetCfgType ShcWsParamKeys.sortableSetCfgType (gaInitRunCfg.sortableSetCfgType)
        |> WorkspaceParamsAttrs.setGeneration GaWsParamKeys.generation_current (0 |> Generation.create)
        |> WorkspaceParamsAttrs.setGeneration GaWsParamKeys.generation_max (gaInitRunCfg.newGenerations)
        |> WorkspaceParamsAttrs.setRngGen GaWsParamKeys.rngGenCreate rngGenCreate
        |> WorkspaceParamsAttrs.setRngGen GaWsParamKeys.rngGenMutate rngGenMutate
        |> WorkspaceParamsAttrs.setRngGen GaWsParamKeys.rngGenPrune rngGenPrune
        |> WorkspaceParamsAttrs.setMutationRate GaWsParamKeys.mutationRate gaInitRunCfg.mutationRate
        |> WorkspaceParamsAttrs.setNoiseFraction GaWsParamKeys.noiseFraction (Some gaInitRunCfg.noiseFraction)
        |> WorkspaceParamsAttrs.setOrder GaWsParamKeys.order gaInitRunCfg.order
        |> WorkspaceParamsAttrs.setSorterCount GaWsParamKeys.sorterCount gaInitRunCfg.sorterCount
        |> WorkspaceParamsAttrs.setSorterCount GaWsParamKeys.sorterCountMutated gaInitRunCfg.sorterCountMutated
        |> WorkspaceParamsAttrs.setSorterEvalMode GaWsParamKeys.sorterEvalMode gaInitRunCfg.sorterEvalMode
        |> WorkspaceParamsAttrs.setStageCount GaWsParamKeys.stagesSkipped gaInitRunCfg.stagesSkipped
        |> WorkspaceParamsAttrs.setStageWeight GaWsParamKeys.stageWeight gaInitRunCfg.stageWeight
        |> WorkspaceParamsAttrs.setSwitchCount GaWsParamKeys.sorterLength gaInitRunCfg.switchCount
        |> WorkspaceParamsAttrs.setSwitchGenMode GaWsParamKeys.switchGenMode gaInitRunCfg.switchGenMode
        |> WorkspaceParamsAttrs.setSorterSetPruneMethod GaWsParamKeys.sorterSetPruneMethod gaInitRunCfg.sorterSetPruneMethod
        |> WorkspaceParamsAttrs.setGenerationFilter GaWsParamKeys.generation_filter (gaInitRunCfg.reportFilter |> Option.get )
        |> WorkspaceParamsAttrs.setUseParallel GaWsParamKeys.useParallel useParallel





type gaContinueRunCfg =
    {
        runId:runId
        newGenerations:generation
        reportGenFilter:generationFilter
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
                    |> InterGenWsOps.runGenLoops projectFolderPath workspaceFileStoreF
            | Continue crc ->
                 InterGenWsOps.continueGenLoops 
                        projectFolderPath 
                        crc.runId 
                        crc.newGenerations
                        crc.reportGenFilter
                        workspaceFileStoreF