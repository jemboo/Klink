namespace global
open System
open System.IO


type shcInitRunCfg =
    {
        mutationRate:mutationRate
        newGenerations:generation option
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
            (newGenerations:generation option)
            (reportFilter:generationFilter option)
            (plex:shcInitRunCfgPlex)
            (daFunc: order -> generation option -> generationFilter option -> 
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
            (newGenerations:generation option)
            (reportFilter:generationFilter option)
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
                    stagesSkipped = 1 |> StageCount.create
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
        |> WorkspaceParamsAttrs.setRunId "runId" (gaCfg |> getRunId)
        |> WorkspaceParamsAttrs.setGeneration "generation_current" (0 |> Generation.create)
        |> WorkspaceParamsAttrs.setGeneration "generation_max" (gaCfg.newGenerations |> Option.get )
        |> WorkspaceParamsAttrs.setRngGen "rngGenCreate" rngGenCreate
        |> WorkspaceParamsAttrs.setRngGen "rngGenMutate" rngGenMutate
        |> WorkspaceParamsAttrs.setRngGen "rngGenPrune" rngGenPrune
        |> WorkspaceParamsAttrs.setMutationRate "mutationRate" gaCfg.mutationRate
        |> WorkspaceParamsAttrs.setNoiseFraction "noiseFraction" (Some gaCfg.noiseFraction)
        |> WorkspaceParamsAttrs.setOrder "order" gaCfg.order
        |> WorkspaceParamsAttrs.setSorterCount "sorterCount" gaCfg.sorterCount
        |> WorkspaceParamsAttrs.setSorterCount "sorterCountMutated" gaCfg.sorterCountMutated
        |> WorkspaceParamsAttrs.setSorterEvalMode "sorterEvalMode" gaCfg.sorterEvalMode
        |> WorkspaceParamsAttrs.setStageCount "stagesSkipped" gaCfg.stagesSkipped
        |> WorkspaceParamsAttrs.setStageWeight "stageWeight" gaCfg.stageWeight
        |> WorkspaceParamsAttrs.setSwitchCount "sorterLength" gaCfg.switchCount
        |> WorkspaceParamsAttrs.setSwitchGenMode "switchGenMode" gaCfg.switchGenMode
        |> WorkspaceParamsAttrs.setSorterSetPruneMethod "sorterSetPruneMethod" gaCfg.sorterSetPruneMethod
        |> WorkspaceParamsAttrs.setGenerationFilter "generation_filter" (gaCfg.reportFilter |> Option.get )
        |> WorkspaceParamsAttrs.setUseParallel "useParallel" useParallel



type shcContinueRunCfgs =
    {
        runId:runId
        newGenerations:generation
    }


module ShcContinueRunCfgs =

    let fromPlex
            (newGenerations:generation)
            (plex:shcInitRunCfgPlex)
        =

        let _toCrc newGenerations (gaCfg:shcInitRunCfg) =
                { 
                    shcContinueRunCfgs.runId = (gaCfg |> ShcInitRunCfgs.getRunId);
                    newGenerations = newGenerations
                }
        ShcInitRunCfgs.fromPlex None None plex
        |> Seq.map(_toCrc newGenerations)



type shcReportEvalsCfg =
    {
        reportFileName:string
        runIds:runId array
        genMin:generation
        genMax:generation
        evalCompName:wsComponentName
        reportFilter:generationFilter
    }

module ShcReportEvalsCfgs =

    let fromPlex
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (reportFileName:string)
            (plex:shcInitRunCfgPlex)
        =
        let runIds =
             ShcInitRunCfgs.fromPlex None None plex
             |> Seq.map(ShcInitRunCfgs.getRunId)
             |> Seq.toArray
        {
            shcReportEvalsCfg.reportFileName = reportFileName
            runIds = runIds
            genMin = genMin
            genMax = genMax
            evalCompName = evalCompName
            reportFilter = reportFilter
        }


    let reportAllEvals
        (projectFolderPath:string)
        (reportCfg:shcReportEvalsCfg)
        =
        let fsReporter = new WorkspaceFileStore(Path.Combine(projectFolderPath, "Reports"))

        let runDirs = reportCfg.runIds
                        |> Array.map(RunId.value >> string)
                        |> Array.map(fun fldr -> Path.Combine(projectFolderPath, fldr))
                        |> Array.toList

        result {
            return! 
                runDirs 
                |> List.map(Reporting.reportEvals 
                                fsReporter 
                                reportCfg.reportFileName 
                                reportCfg.evalCompName 
                                reportCfg.genMin)
                |> Result.sequence
                |> Result.map(ignore)
        }







type shcReportBinsCfg =
    {
        reportFileName:string
        runIds:runId array
        genMin:generation
        genMax:generation
    }

module ShcReportBinsCfgs =

    let fromPlex
            (genMin:generation)
            (genMax:generation)
            (reportFileName:string)
            (plex:shcInitRunCfgPlex)
        =
        let runIds =
             ShcInitRunCfgs.fromPlex None None plex
             |> Seq.map(ShcInitRunCfgs.getRunId)
             |> Seq.toArray
        {
            shcReportBinsCfg.reportFileName = reportFileName
            runIds = runIds
            genMin = genMin
            genMax = genMax
        }



    let reportAllBins
        (projectFolderPath:string)
        (reportCfg:shcReportBinsCfg)
        =
        let fsReporter = new WorkspaceFileStore(Path.Combine(projectFolderPath, "Reports"))

        let runDirs = reportCfg.runIds
                        |> Array.map(RunId.value >> string)
                        |> Array.map(fun fldr -> Path.Combine(projectFolderPath, fldr))
                        |> Array.toList

        result {
            return! 
                runDirs 
                |> List.map(Reporting.reportBins 
                                fsReporter 
                                reportCfg.reportFileName 
                                reportCfg.genMin)
                |> Result.sequence
                |> Result.map(ignore)
        }




type shcReportCfg =
    | Evals of shcReportEvalsCfg
    | Bins of shcReportBinsCfg









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
            ShcInitRunCfgs.fromPlex (Some newGenerations) (Some reportFilter)
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
            ShcContinueRunCfgs.fromPlex newGenerations
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
            ShcReportEvalsCfgs.fromPlex 
                    genMin
                    genMax 
                    evalCompName 
                    reportFilter 
                    reportFileName
                  |> shcReportCfg.Evals
                  |> shcRunCfg.Report

        {setName = runSetName; runCfgs = [|runCfg|]}


    let reportAllFromPlexSeq 
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (runSetName:string)
            (reportFileName:string)
            (plexes:shcInitRunCfgPlex seq)
        =
        let runCfgs = 
            plexes |> Seq.map(
            ShcReportEvalsCfgs.fromPlex 
                    genMin
                    genMax 
                    evalCompName 
                    reportFilter 
                    reportFileName)
            |> Seq.map(shcReportCfg.Evals >> shcRunCfg.Report)
            |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}


    let reportBinsFromPlex 
            (genMin:generation)
            (genMax:generation)
            (runSetName:string)
            (reportFileName:string)
            (plex:shcInitRunCfgPlex)
        =
        let runCfg = 
            plex |>
            ShcReportBinsCfgs.fromPlex 
                    genMin
                    genMax
                    reportFileName
                  |> shcReportCfg.Bins
                  |> shcRunCfg.Report

        {setName = runSetName; runCfgs = [|runCfg|]}



    let reportBinsFromPlexSeq
            (genMin:generation)
            (genMax:generation)
            (runSetName:string)
            (reportFileName:string)
            (plexes:shcInitRunCfgPlex seq)
        =
        let runCfgs = 
            plexes |> Seq.map(
            ShcReportBinsCfgs.fromPlex 
                    genMin
                    genMax
                    reportFileName)
            |> Seq.map(shcReportCfg.Bins >> shcRunCfg.Report)
            |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}