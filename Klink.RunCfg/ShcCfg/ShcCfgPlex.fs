namespace global

type shcCfgPlex =
    {
        orders:order[]
        sortableSetCfgs:(sortableSetCfgType*stageCount*sorterEvalMode) []
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
            (daFunc: 
                     order -> 
                     generation -> 
                     generationFilter option -> 
                     rngGen -> 
                     sorterEvalMode ->
                     sortableSetCfgType -> 
                     stageCount -> 
                     (sorterCount*sorterCount) -> 
                     switchGenMode -> 
                     stageWeight -> 
                     noiseFraction -> 
                     mutationRate ->
                     sorterSetPruneMethod -> 
                     'a)
            (seqSplicer: (int*int) option)
            =
        let preSpliced =
            seq {
            for order in plex.orders do
                for (sortableSetCfgType, stagesSkipped, sorterEvalMode) in plex.sortableSetCfgs do
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
                                                        reportFilter
                                                        rngGen
                                                        sorterEvalMode
                                                        sortableSetCfgType
                                                        stagesSkipped
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



    let _fromFunc2<'a>
            (plex:shcCfgPlex)
            (daFunc: 
                sortableSetCfgType ->
                order ->
                rngGen ->
                sorterEvalMode ->
                (sorterCount*sorterCount) -> 
                switchGenMode -> 
                stageWeight -> 
                noiseFraction -> 
                mutationRate -> 
                sorterSetPruneMethod -> 
                'a)
            (seqSplicer: (int*int) option)
        =
        let preSpliced =
            seq {
            for order in plex.orders do
                for (sortableSetCfgType, stagesSkipped, sorterEvalMode) in plex.sortableSetCfgs do
                    for rngGen in plex.rngGens do
                        for tupSorterSetSize in plex.tupSorterSetSizes do
                            for switchGenMode in plex.switchGenModes do
                                for stageWeight in plex.stageWeights do
                                    for noiseFraction in plex.noiseFractions do
                                        for mutationRate in plex.mutationRates do
                                            for sorterSetPruneMethod in plex.sorterSetPruneMethods do
                                                yield
                                                    daFunc 
                                                        sortableSetCfgType
                                                        order 
                                                        rngGen
                                                        sorterEvalMode
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

        let _toIr 
                order 
                newGenerations 
                reportFilter 
                rngGen 
                sorterEvalMode
                sortableSetCfgType
                stagesSkipped
                tupSorterSetSize 
                switchGenMode 
                stageWeight 
                noiseFraction 
                mutationRate 
                sorterSetPruneMethod
            =
                let runId = 
                    ShcInitRunCfg.getRunId2
                        sortableSetCfgType
                        mutationRate
                        noiseFraction
                        order
                        rngGen
                        sorterEvalMode
                        (fst tupSorterSetSize)
                        (snd tupSorterSetSize)
                        sorterSetPruneMethod
                        stageWeight
                        (SwitchCount.orderTo999SwitchCount order)
                        switchGenMode

                                
                let shcInitRunCfg =
                        { 
                            shcInitRunCfg.runId = runId
                            mutationRate = mutationRate;
                            sortableSetCfgType = sortableSetCfgType;
                            newGenerations = newGenerations
                            noiseFraction = noiseFraction
                            order = order
                            rngGen = rngGen
                            sorterEvalMode = sorterEvalMode
                            sorterCount = fst tupSorterSetSize
                            sorterCountMutated = snd tupSorterSetSize
                            sorterSetPruneMethod = sorterSetPruneMethod;
                            stagesSkipped = stagesSkipped
                            stageWeight = stageWeight
                            switchCount = (SwitchCount.orderTo999SwitchCount order)
                            switchGenMode = switchGenMode
                            reportFilter = reportFilter
                        }
                shcInitRunCfg

        _fromFunc newGenerations reportFilter plex _toIr seqSplicer



    let toRunIds
            (seqSplicer: (int*int) option)
            (plex:shcCfgPlex)
        =
        let _toRunId
                  sortableSetCfgType
                  order 
                  rngGen
                  sorterEvalMode
                  tupSorterSetSize 
                  switchGenMode 
                  stageWeight 
                  noiseFraction 
                  mutationRate 
                  sorterSetPruneMethod =

                ShcInitRunCfg.getRunId2
                    sortableSetCfgType
                    mutationRate
                    noiseFraction
                    order
                    rngGen
                    sorterEvalMode
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


