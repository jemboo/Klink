namespace global
open System

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
        stageWeight:stageWeight
        switchCount:switchCount
        switchGenMode:switchGenMode
        reportFilter:generationFilter
    }

type shcInitRunCfgPlex =
    {
        orders:order[]
        mutationRates:mutationRate[]
        noiseFractions:noiseFraction[]
        rngGens:rngGen[]
        tupSorterSetSizes:(sorterCount*sorterCount)[]
        sorterSetPruneMethods:sorterSetPruneMethod[]
        stageWeights:stageWeight[]
        switchGenModes:switchGenMode[]
    }


module ShcInitRunCfgPlex =

    let fromFunc<'a>
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (plex:shcInitRunCfgPlex)
            (daFunc: order -> generation -> generationFilter -> 
                     rngGen -> (sorterCount*sorterCount) -> 
                     switchGenMode -> stageWeight -> 
                     noiseFraction -> mutationRate -> sorterSetPruneMethod -> 'a)
            =
        seq {
        for order in plex.orders do
            for rngGen in plex.rngGens do
                for tupSorterSetSize in plex.tupSorterSetSizes do
                    for switchGenMode in plex.switchGenModes do
                        for stageWeight in plex.stageWeights do
                            for noiseFraction in plex.noiseFractions do
                                for mutationRate in plex.mutationRates do
                                    for sorterSetPruneMethod in plex.sorterSetPruneMethods do
                                        yield
                                            daFunc 
                                                order newGenerations 
                                                reportFilter rngGen 
                                                tupSorterSetSize switchGenMode 
                                                stageWeight noiseFraction
                                                mutationRate sorterSetPruneMethod
        }


module ShcInitRunCfgs =

    let getRunId (cfg:shcInitRunCfg) =
        [
            cfg.mutationRate |> MutationRate.value :> obj;
            cfg.noiseFraction |> NoiseFraction.value :> obj;
            cfg.order |> Order.value :> obj;
            cfg.rngGen :> obj;
            cfg.sorterEvalMode :> obj
            cfg.sorterCount |> SorterCount.value :> obj;
            cfg.sorterCountMutated |> SorterCount.value :> obj;
            cfg.sorterSetPruneMethod :> obj;
            cfg.stageWeight |> StageWeight.value :> obj;
            cfg.switchCount :> obj;
            cfg.switchGenMode :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create



    let fromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (plex:shcInitRunCfgPlex)
        =

        let _toIr order newGenerations reportFilter rngGen 
                  tupSorterSetSize switchGenMode stageWeight 
                  noiseFraction mutationRate sorterSetPruneMethod =
                { 
                    shcInitRunCfg.mutationRate = mutationRate;
                    newGenerations = newGenerations
                    noiseFraction = noiseFraction
                    order = order
                    rngGen = rngGen
                    sorterEvalMode = sorterEvalMode.DontCheckSuccess
                    sorterCount = fst tupSorterSetSize
                    sorterCountMutated = snd tupSorterSetSize
                    sorterSetPruneMethod = sorterSetPruneMethod;
                    stageWeight = stageWeight
                    switchCount = (SwitchCount.orderTo999SwitchCount order)
                    switchGenMode = switchGenMode
                    reportFilter = reportFilter
                }

        ShcInitRunCfgPlex.fromFunc newGenerations reportFilter plex _toIr



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
        |> WorkspaceParams.setRunId "runId" (gaCfg |> getRunId)
        |> WorkspaceParams.setGeneration "generation_current" (0 |> Generation.create)
        |> WorkspaceParams.setGeneration "generation_max" gaCfg.newGenerations
        |> WorkspaceParams.setRngGen "rngGenCreate" rngGenCreate
        |> WorkspaceParams.setRngGen "rngGenMutate" rngGenMutate
        |> WorkspaceParams.setRngGen "rngGenPrune" rngGenPrune
        |> WorkspaceParams.setMutationRate "mutationRate" gaCfg.mutationRate
        |> WorkspaceParams.setNoiseFraction "noiseFraction" (Some gaCfg.noiseFraction)
        |> WorkspaceParams.setOrder "order" gaCfg.order
        |> WorkspaceParams.setSorterCount "sorterCount" gaCfg.sorterCount
        |> WorkspaceParams.setSorterCount "sorterCountMutated" gaCfg.sorterCountMutated
        |> WorkspaceParams.setSorterEvalMode "sorterEvalMode" gaCfg.sorterEvalMode
        |> WorkspaceParams.setStageWeight "stageWeight" gaCfg.stageWeight
        |> WorkspaceParams.setSwitchCount "sorterLength" gaCfg.switchCount
        |> WorkspaceParams.setSwitchGenMode "switchGenMode" gaCfg.switchGenMode
        |> WorkspaceParams.setSorterSetPruneMethod "sorterSetPruneMethod" gaCfg.sorterSetPruneMethod
        |> WorkspaceParams.setUseParallel "useParallel" useParallel



type shcContinueRunCfgs =
    {
        runId:runId
        newGenerations:generation
    }


module ShcContinueRunCfgs =

    let fromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (plex:shcInitRunCfgPlex)
        =

        let _toCrc newGenerations (gaCfg:shcInitRunCfg) =
                { 
                    shcContinueRunCfgs.runId = (gaCfg |> ShcInitRunCfgs.getRunId);
                    newGenerations = newGenerations
                }
        ShcInitRunCfgs.fromPlex newGenerations reportFilter plex
        |> Seq.map(_toCrc newGenerations)



type shcReportCfg =
    {
        reportFileName:string
        runIds:runId array
        genMin:generation
        genMax:generation
        evalCompName:wsComponentName
        reportFilter:generationFilter
    }

module ShcReportRunCfgs =

    let fromPlex
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (reportFileName:string)
            (plex:shcInitRunCfgPlex)
        =
        let runIds =
             ShcInitRunCfgs.fromPlex genMin reportFilter plex
             |> Seq.map(ShcInitRunCfgs.getRunId)
             |> Seq.toArray
        {
            shcReportCfg.reportFileName = reportFileName
            runIds = runIds
            genMin = genMin
            genMax = genMax
            evalCompName = evalCompName
            reportFilter = reportFilter
        }




type shcRunCfg =
    | InitRun of shcInitRunCfg
    | Continue of shcContinueRunCfgs
    | Report of shcReportCfg




type shcRunCfgSet = {setName:string; runCfgs:shcRunCfg[]}

module ShcRunCfgSet =

    let initRunFromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (runSetName:string)
            (plex:shcInitRunCfgPlex)
        =
        let runCfgs = 
            plex |>
            ShcInitRunCfgs.fromPlex newGenerations reportFilter
                  |> Seq.map(shcRunCfg.InitRun)
                  |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}


    let continueRunFromPlex 
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (runSetName:string)
            (plex:shcInitRunCfgPlex)
        =
        let runCfgs = 
            plex |>
            ShcContinueRunCfgs.fromPlex newGenerations reportFilter
                  |> Seq.map(shcRunCfg.Continue)
                  |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}


    let reportAllFromPlex 
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (runSetName:string)
            (reportFileName:string)
            (plex:shcInitRunCfgPlex)
        =
        let runCfg = 
            plex |>
            ShcReportRunCfgs.fromPlex 
                    genMin
                    genMax evalCompName 
                    reportFilter reportFileName
                  |> shcRunCfg.Report

        {setName = runSetName; runCfgs = [|runCfg|]}