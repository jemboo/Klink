namespace global
open System
open System.IO


type gaCfgPlexOld =
    {
        name:cfgPlexName
        orders:order[]
        sortableSetCfgs:(sortableSetCfgType*stageCount*sorterEvalMode) []
        mutationRates:mutationRate[]
        noiseFractions:noiseFraction[]
        rngGens:rngGen[]
        parentAndChildCounts:(sorterCount*sorterCount)[]
        sorterSetPruneMethods:sorterSetPruneMethod[]
        stageWeights:stageWeight[]
        switchGenModes:switchGenMode[]
        projectFolder:projectFolder
    }


module GaCfgPlex =

    let _fromFunc<'a>
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (plex:gaCfgPlexOld)
            (daFunc:
                order -> 
                generation -> 
                generationFilter -> 
                generationFilter -> 
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
            (selectedIndexes: int[] option)
            =
        let preSpliced =
            seq {
            for order in plex.orders do
                for (sortableSetCfgType, stagesSkipped, sorterEvalMode) in plex.sortableSetCfgs do
                    for rngGen in plex.rngGens do
                        for tupSorterSetSize in plex.parentAndChildCounts do
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
                                                        fullReportFilter
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
        match selectedIndexes with
        | Some dexes -> preSpliced |> CollectionOps.getItemsAtIndexes dexes
        | None -> preSpliced 



    let _fromFunc2<'a>
            (plex:gaCfgPlexOld)
            (daFunc: 
                sortableSetCfgType ->
                order ->
                rngGen ->
                (sorterCount*sorterCount) -> 
                switchGenMode -> 
                stageWeight -> 
                noiseFraction -> 
                mutationRate -> 
                sorterSetPruneMethod -> 
                'a)
            (selectedIndexes: int[] option)
        =
        let preSpliced =
            seq {
            for order in plex.orders do
                for (sortableSetCfgType, stagesSkipped, sorterEvalMode) in plex.sortableSetCfgs do
                    for rngGen in plex.rngGens do
                        for tupSorterSetSize in plex.parentAndChildCounts do
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
                                                        tupSorterSetSize 
                                                        switchGenMode 
                                                        stageWeight
                                                        noiseFraction
                                                        mutationRate 
                                                        sorterSetPruneMethod
            }
        match selectedIndexes with
        | Some dexes -> preSpliced |> CollectionOps.getItemsAtIndexes dexes
        | None -> preSpliced 




    let toInitRunCfgs
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (selectedIndexes: int[] option)
            (plex:gaCfgPlexOld)
        =

        let _toIr 
                order 
                newGenerations 
                reportFilter 
                fullReportFilter
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
                    GaInitRunCfg.getRunId2
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
                { 
                    gaInitRunCfg.runId = runId
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
                    fullReportFilter = fullReportFilter
                }

        _fromFunc newGenerations reportFilter fullReportFilter plex _toIr selectedIndexes




    let toRunIds 
            (selectedIndexes: int[] option)
            (plex:gaCfgPlexOld)
        =
        let _toRunId 
                sortableSetCfgType
                order 
                rngGen 
                tupSorterSetSize 
                switchGenMode 
                stageWeight 
                noiseFraction
                mutationRate 
                sorterSetPruneMethod
            =

                GaInitRunCfg.getRunId2
                    sortableSetCfgType
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

        _fromFunc2 plex _toRunId selectedIndexes



    let toContinueRunCfgs
            (newGenerations:generation)
            (reportGenFilter:generationFilter)
            (fullReportGenFilter:generationFilter)
            (selectedIndexes: int[] option)
            (plex:gaCfgPlexOld)
        =

        let _toCrc (runId:runId) =
                { 
                    gaContinueRunCfg.runId = runId;
                    newGenerations = newGenerations;
                    reportGenFilter = reportGenFilter
                    fullReportGenFilter = fullReportGenFilter
                }
        toRunIds selectedIndexes plex
        |> Seq.map(_toCrc)