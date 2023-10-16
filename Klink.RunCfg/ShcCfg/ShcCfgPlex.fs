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
            (newGenerations:generation)
            (reportFilter:generationFilter option)
            (plex:shcCfgPlex)
            (daFunc: order -> generation -> generationFilter option -> 
                     rngGen -> (sorterCount*sorterCount) -> 
                     switchGenMode -> stageWeight -> 
                     noiseFraction -> mutationRate -> sorterSetPruneMethod -> 'a)
            (seqSplicer: (int*int) option)
            =
        let preSpliced =
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
                                                    order 
                                                    newGenerations 
                                                    reportFilter rngGen 
                                                    tupSorterSetSize switchGenMode 
                                                    stageWeight noiseFraction
                                                    mutationRate sorterSetPruneMethod
            }
        match seqSplicer with
        | Some (skp, tk) -> preSpliced |> Seq.skip skp |> Seq.take tk
        | None -> preSpliced



    let _fromFunc2<'a>
            (plex:shcCfgPlex)
            (daFunc: order ->
                     rngGen -> (sorterCount*sorterCount) -> 
                     switchGenMode -> stageWeight -> 
                     noiseFraction -> mutationRate -> sorterSetPruneMethod -> 'a)
            (seqSplicer: (int*int) option)
            =
        let preSpliced =
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
                                                    order 
                                                    rngGen 
                                                    tupSorterSetSize 
                                                    switchGenMode 
                                                    stageWeight
                                                    noiseFraction
                                                    mutationRate 
                                                    sorterSetPruneMethod
                }
        match seqSplicer with
        | Some (skp, tk) -> preSpliced |> Seq.skip skp |> Seq.take tk
        | None -> preSpliced



    let toInitRunCfgs
            (newGenerations:generation)
            (reportFilter:generationFilter option)
            (seqSplicer: (int*int) option)
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

        _fromFunc newGenerations reportFilter plex _toIr seqSplicer



    let toRunIds
            (seqSplicer: (int*int) option)
            (plex:shcCfgPlex)
        =
        let _toRunId 
                  order rngGen 
                  tupSorterSetSize switchGenMode stageWeight 
                  noiseFraction mutationRate sorterSetPruneMethod =

                ShcInitRunCfg.getRunId2
                    mutationRate
                    noiseFraction
                    order
                    rngGen
                    sorterEvalMode.DontCheckSuccess
                    (fst tupSorterSetSize)
                    (snd tupSorterSetSize)
                    sorterSetPruneMethod
                    stageWeight
                    (SwitchCount.orderTo999SwitchCount order)
                    switchGenMode

        _fromFunc2 plex _toRunId seqSplicer



    let toContinueRunCfgs
            (newGenerations:generation)
            (seqSplicer: (int*int) option)
            (plex:shcCfgPlex)
        =
        let _toCrc newGenerations (runId:runId) =
                { 
                    shcContinueRunCfg.runId = runId;
                    newGenerations = newGenerations
                }
        toRunIds seqSplicer plex
        |> Seq.map(_toCrc newGenerations)




    //let toReportEvalsCfg
    //        (genMin:generation)
    //        (genMax:generation)
    //        (evalCompName:wsComponentName)
    //        (reportFilter:generationFilter)
    //        (reportFileName:string)
    //        (plex:shcCfgPlex)
    //    =
    //    {
    //        shcReportEvalsCfg.reportFileName = reportFileName
    //        runIds = toRunIds plex |> Seq.toArray
    //        genMin = genMin
    //        genMax = genMax
    //        evalCompName = evalCompName
    //        reportFilter = reportFilter
    //    }




    //let toReportBinsCfg
    //        (genMin:generation)
    //        (genMax:generation)
    //        (reportFileName:string)
    //        (plex:shcCfgPlex)
    //    =
    //    {
    //        shcReportBinsCfg.reportFileName = reportFileName
    //        runIds = toRunIds plex |> Seq.toArray
    //        genMin = genMin
    //        genMax = genMax
    //    }
