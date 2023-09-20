namespace global

type shcCfgPlex =
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


module ShcCfgPlex =

    let _fromFunc<'a>
            (newGenerations:generation option)
            (reportFilter:generationFilter option)
            (plex:shcCfgPlex)
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




    let toInitRunCfgs
            (newGenerations:generation option)
            (reportFilter:generationFilter option)
            (plex:shcCfgPlex)
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

        _fromFunc newGenerations reportFilter plex _toIr




    let toContinueRunCfgs
            (newGenerations:generation)
            (plex:shcCfgPlex)
        =

        let _toCrc newGenerations (gaCfg:shcInitRunCfg) =
                { 
                    shcContinueRunCfg.runId = (gaCfg |> ShcInitRunCfg.getRunId);
                    newGenerations = newGenerations
                }
        toInitRunCfgs None None plex
        |> Seq.map(_toCrc newGenerations)




    let toReportEvalsCfg
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (reportFileName:string)
            (plex:shcCfgPlex)
        =
        let runIds =
             toInitRunCfgs None None plex
             |> Seq.map(ShcInitRunCfg.getRunId)
             |> Seq.toArray
        {
            shcReportEvalsCfg.reportFileName = reportFileName
            runIds = runIds
            genMin = genMin
            genMax = genMax
            evalCompName = evalCompName
            reportFilter = reportFilter
        }




    let toReportBinsCfg
            (genMin:generation)
            (genMax:generation)
            (reportFileName:string)
            (plex:shcCfgPlex)
        =
        let runIds =
             toInitRunCfgs None None plex
             |> Seq.map(ShcInitRunCfg.getRunId)
             |> Seq.toArray
        {
            shcReportBinsCfg.reportFileName = reportFileName
            runIds = runIds
            genMin = genMin
            genMax = genMax
        }
