﻿namespace global
open System
open System.IO

type gaCfgPlex =
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


module GaCfgPlex =

    let _fromFunc<'a>
            (newGenerations:generation option)
            (reportFilter:generationFilter option)
            (plex:gaCfgPlex)
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
            (plex:gaCfgPlex)
        =

        let _toIr order newGenerations reportFilter rngGen 
                  tupSorterSetSize switchGenMode stageWeight 
                  noiseFraction mutationRate sorterSetPruneMethod =

                { 
                    gaInitRunCfg.mutationRate = mutationRate;
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
            (plex:gaCfgPlex)
        =

        let _toCrc newGenerations (gaCfg:gaInitRunCfg) =
                { 
                    gaContinueRunCfg.runId = (gaCfg |> GaInitRunCfg.getRunId);
                    newGenerations = newGenerations
                }
        toInitRunCfgs None None plex
        |> Seq.map(_toCrc newGenerations)




    //let toReportEvalsCfg
    //        (genMin:generation)
    //        (genMax:generation)
    //        (evalCompName:wsComponentName)
    //        (reportFilter:generationFilter)
    //        (reportFileName:string)
    //        (plex:gaCfgPlex)
    //    =
    //    let runIds =
    //         toInitRunCfgs None None plex
    //         |> Seq.map(GaInitRunCfg.getRunId)
    //         |> Seq.toArray
    //    {
    //        gaReportEvalsCfg.reportFileName = reportFileName
    //        runIds = runIds
    //        genMin = genMin
    //        genMax = genMax
    //        evalCompName = evalCompName
    //        reportFilter = reportFilter
    //    }






    //let toReportBinsCfg
    //        (genMin:generation)
    //        (genMax:generation)
    //        (reportFileName:string)
    //        (plex:gaCfgPlex)
    //    =
    //    let runIds =
    //         toInitRunCfgs None None plex
    //         |> Seq.map(GaInitRunCfg.getRunId)
    //         |> Seq.toArray
    //    {
    //        gaReportBinsCfg.reportFileName = reportFileName
    //        runIds = runIds
    //        genMin = genMin
    //        genMax = genMax
    //    }
