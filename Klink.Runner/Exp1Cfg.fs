﻿namespace global
open System

type exp1Cfg =
    {
        generation:generation
        mutationRate:mutationRate
        noiseFraction:noiseFraction
        order:order
        rngGen:rngGen
        sorterCount:sorterCount
        sorterCountMutated:sorterCount
        sorterSetPruneMethod:sorterSetPruneMethod
        stageWeight:stageWeight
        switchGenMode:switchGenMode
        useParallel:useParallel
    }

module Exp1Cfg =


    let defltCfg = 
        {
            exp1Cfg.generation = 0 |> Generation.create
            exp1Cfg.mutationRate = 0.01 |> MutationRate.create
            exp1Cfg.noiseFraction = 0.01 |> NoiseFraction.create
            exp1Cfg.order = 16 |> Order.createNr
            exp1Cfg.rngGen = 1234 |> RandomSeed.create |> RngGen.createLcg
            exp1Cfg.sorterCount = 32 |> SorterCount.create
            exp1Cfg.sorterCountMutated = 64 |> SorterCount.create
            exp1Cfg.sorterSetPruneMethod = sorterSetPruneMethod.Whole
            exp1Cfg.stageWeight = 0.1 |> StageWeight.create
            exp1Cfg.switchGenMode = switchGenMode.Stage
            exp1Cfg.useParallel = true |> UseParallel.create
        }


    let getRunId (cfg:exp1Cfg) =
        [
            cfg.generation |> Generation.value :> obj;
            cfg.mutationRate |> MutationRate.value :> obj;
            cfg.noiseFraction |> NoiseFraction.value :> obj;
            cfg.order |> Order.value :> obj;
            cfg.rngGen :> obj;
            cfg.sorterCount |> SorterCount.value :> obj;
            cfg.sorterCountMutated |> SorterCount.value :> obj;
            cfg.sorterSetPruneMethod :> obj;
            cfg.stageWeight |> StageWeight.value :> obj;
            cfg.switchGenMode :> obj;
            cfg.useParallel :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create


    let wnSortableSet = "sortableSet" |> WsComponentName.create
    let wnSorterSetParent = "sorterSetParent" |> WsComponentName.create
    let wnSorterSetMutator = "sorterSetMutator" |> WsComponentName.create
    let wnSorterSetMutated = "sorterSetMutated" |> WsComponentName.create
    let wnSorterSetPruned = "sorterSetPruned" |> WsComponentName.create
    let wnParentMap = "parentMap" |> WsComponentName.create
    let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
    let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
    let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
    let wnSorterSetPruner = "sorterSetPruner" |> WsComponentName.create
        
    let forGa 
            (exp1Cfg:exp1Cfg)
        =
        let runId = exp1Cfg |> getRunId

        let randy = exp1Cfg.rngGen
                            |> Rando.fromRngGen

        let nextRngGen () =
            randy |> Rando.toRngGen

        let rngGenCreate = (nextRngGen ())
        let rngGenMutate = (nextRngGen ())
        let rngGenPrune = (nextRngGen ())
        let switchCount = SwitchCount.orderTo999SwitchCount exp1Cfg.order 
        let sorterEvalMode = sorterEvalMode.DontCheckSuccess

        let workspaceParams = 
            WorkspaceParams.make Map.empty
            |> WorkspaceParams.setRunId "runId" runId
            |> WorkspaceParams.setRngGen "rngGenCreate" rngGenCreate
            |> WorkspaceParams.setRngGen "rngGenMutate" rngGenMutate
            |> WorkspaceParams.setRngGen "rngGenPrune" rngGenPrune
            |> WorkspaceParams.setGeneration "generation" exp1Cfg.generation
            |> WorkspaceParams.setMutationRate "mutationRate" exp1Cfg.mutationRate
            |> WorkspaceParams.setNoiseFraction "noiseFraction" (Some exp1Cfg.noiseFraction)
            |> WorkspaceParams.setOrder "order" exp1Cfg.order
            |> WorkspaceParams.setSorterCount "sorterCount" exp1Cfg.sorterCount
            |> WorkspaceParams.setSorterCount "sorterCountMutated" exp1Cfg.sorterCountMutated
            |> WorkspaceParams.setSorterEvalMode "sorterEvalMode" sorterEvalMode
            |> WorkspaceParams.setStageWeight "stageWeight" exp1Cfg.stageWeight
            |> WorkspaceParams.setSwitchCount "sorterLength" switchCount
            |> WorkspaceParams.setSwitchGenMode "switchGenMode" exp1Cfg.switchGenMode
            |> WorkspaceParams.setUseParallel "useParallel" exp1Cfg.useParallel
            |> WorkspaceParams.setSorterSetPruneMethod "sorterSetPruneMethod" exp1Cfg.sorterSetPruneMethod

        (workspaceParams, runId)


    let runFolder = "testRnd"

    let makeEmAll
            (rootDir:string) 
            (randomSeeds:rngGen seq)
            (stageWeights:stageWeight seq) 
            (noiseFractions:noiseFraction seq) 
            (mutationRates:mutationRate seq)
            (maxGen:generation)
            (sorterSetPruneMethods:sorterSetPruneMethod seq)
        =
        
        let wnSortableSet = "sortableSet" |> WsComponentName.create
        let wnSorterSetParent = "sorterSetParent" |> WsComponentName.create
        let wnSorterSetMutator = "sorterSetMutator" |> WsComponentName.create
        let wnSorterSetMutated = "sorterSetMutated" |> WsComponentName.create
        let wnSorterSetPruned = "sorterSetPruned" |> WsComponentName.create
        let wnParentMap = "parentMap" |> WsComponentName.create
        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create

        result {

            for randomSeed in randomSeeds do
                for stageWeight in stageWeights do
                    for noiseFraction in noiseFractions do
                        for mutationRate in mutationRates do
                            for sorterSetPruneMethod in sorterSetPruneMethods do
                                let curCfg = 
                                    { defltCfg with 
                                        rngGen = randomSeed;
                                        stageWeight = stageWeight;
                                        noiseFraction = noiseFraction;
                                        mutationRate = mutationRate;
                                        sorterSetPruneMethod = sorterSetPruneMethod
                                        }

                                let (wsParams, runId) = curCfg |> forGa

                                let runDir = IO.Path.Combine(rootDir, runFolder, runId |> RunId.value |> string)
                                let fs = new WorkspaceFileStore(runDir)

                                let! wsCfg = WsOpsLib.genZero
                                                    wnSortableSet
                                                    wnSorterSetParent
                                                    wnSorterSetEvalParent
                                                    wsParams
                                                    fs
                                                    (fun s-> Console.WriteLine(s))

                                let mutable curGen = 0
                                let mutable curCfg = wsCfg
                                let mutable curParams = wsParams

                                while curGen < (maxGen |> Generation.value) do
                                    let! wsCfgN, wsPramsN = 
                                        WsOpsLib.doGen
                                            wnSortableSet
                                            wnSorterSetParent
                                            wnSorterSetMutator
                                            wnSorterSetMutated
                                            wnSorterSetPruned
                                            wnParentMap
                                            wnSorterSetEvalParent
                                            wnSorterSetEvalMutated
                                            wnSorterSetEvalPruned
                                            fs
                                            (fun s-> Console.WriteLine(s))
                                            curParams
                                            curCfg

                                    curCfg <- wsCfgN
                                    curParams <- wsPramsN
                                    curGen <- curGen + 1

            return "success"
        }




    let standardSorterEvalProps =
        [
            sorterEvalProps.ErrorMsg;
            sorterEvalProps.Phenotype;
            sorterEvalProps.SortableSetId;
            sorterEvalProps.SortedSetSize;
            sorterEvalProps.SorterId;
            sorterEvalProps.StageCount;
            sorterEvalProps.Success;
            sorterEvalProps.SwitchCount;
        ]

    let standardSorterEvalHeaders () = 
        standardSorterEvalProps
            |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""


    let standardSorterEvalValues (sev:sorterEval) =
        let yab = sev |> SorterEval.getSorterEvalProps
        standardSorterEvalProps
            |> List.fold(fun st t -> sprintf "%s\t%s" st yab.[t]) ""


    let binnedSorterEvalProps =
        [
            sorterEvalProps.SortableSetId;
        ]

    let binnedSorterEvalHeaders () = 
        binnedSorterEvalProps
            |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""


    let binnedSorterEvalValues (sev:sorterEval) =
        let yab = sev |> SorterEval.getSorterEvalProps
        binnedSorterEvalProps
            |> List.fold(fun st t -> sprintf "%s\t%s" st yab.[t]) ""


    let standardParamProps =
        [
            "generation";
            "runId";
            "mutationRate";
            "noiseFraction";
            "order";
            "sorterCount";
            "sorterCountMutated";
            "stageWeight";
            "sorterLength";
            "switchGenMode";
        ]
        

    let standardParamValues (wsps:workspaceParams) =
        let paramMap t = (wsps |> WorkspaceParams.getMap).[t]
        standardParamProps |> StringUtil.toCsvLine paramMap

    let binParamProps =
        [
            "runId";
            "mutationRate";
            "noiseFraction";
            "order";
            "sorterCount";
            "sorterCountMutated";
            "stageWeight";
            "sorterLength";
            "switchGenMode";
        ]

    let binnedParamValues (wsps:workspaceParams) (genBinSz:int) =
        let paramMap t = (wsps |> WorkspaceParams.getMap).[t]
        result {
            let! genBin = wsps |> WorkspaceParams.getGeneration "generation"
                               |> Result.map(Generation.binnedValue genBinSz)
            return $"{genBin}\t{binParamProps |> StringUtil.toCsvLine paramMap}"
        }


    let reportHeaderStandard () =
        sprintf "%s%s" 
            (standardParamProps |> List.reduce(fun st t -> sprintf "%s\t%s" st t))
            (standardSorterEvalHeaders ())


    let reportHeaderBinned () =
        sprintf "%s%s%s%s"
            "gen_bin\t"
            (binParamProps |> List.reduce(fun st t -> sprintf "%s\t%s" st t))
            (binnedSorterEvalHeaders ())
            "\tSwitchCt\tStageCt\tRecordCt"


    let reportLines
            (sorterSetEval:sorterSetEval) 
            (wsps:workspaceParams)
            (fileName:string)
            (fs:WorkspaceFileStore)
        =
        let lines = 
            sorterSetEval 
                |> SorterSetEval.getSorterEvals 
                |> Array.map(fun sev ->
                                    sprintf "%s%s" 
                                        (standardParamValues wsps)
                                        (standardSorterEvalValues sev))
        fs.appendLines None fileName lines


    let reportEm (rootDir:string) (runId:runId) 
        =
        let runDir = System.IO.Path.Combine(rootDir, runFolder, runId |> RunId.value |> string)
        let reportFileName = "SorterEvalReport"
        let fs = new WorkspaceFileStore(runDir)

        fs.appendLines None reportFileName [reportHeaderStandard ()] 
                |> Result.ExtractOrThrow
                |> ignore

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        result {
            let! compTupes = fs.getAllSorterSetEvalsWithParamsByName wnSorterSetEvalMutated
            let yab = compTupes 
                        |> List.map (fun (ssEval, wsPram) -> 
                            reportLines ssEval wsPram reportFileName fs)
            return "success"
        }


    let reportEmAll
            (rootDir:string) 
            (randomSeeds:rngGen seq)
            (stageWeights:stageWeight seq) 
            (noiseFractions:noiseFraction seq) 
            (mutationRates:mutationRate seq)
            (sorterSetPruneMethods:sorterSetPruneMethod seq)
        =
        let runDir = System.IO.Path.Combine(rootDir, runFolder)

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        let wnToQuery = wnSorterSetEvalParent


        let reportFileName = wnToQuery |> WsComponentName.value |> string
        let fsReporter = new WorkspaceFileStore(runDir)

        fsReporter.appendLines None reportFileName [reportHeaderStandard ()] 
                |> Result.ExtractOrThrow
                |> ignore




        result {

            for randomSeed in randomSeeds do
                for stageWeight in stageWeights do
                    for noiseFraction in noiseFractions do
                        for mutationRate in mutationRates do
                            for sorterSetPruneMethod in sorterSetPruneMethods do
                                let curCfg = 
                                    { defltCfg with 
                                        rngGen = randomSeed;
                                        stageWeight = stageWeight;
                                        noiseFraction = noiseFraction;
                                        mutationRate = mutationRate;
                                        sorterSetPruneMethod = sorterSetPruneMethod
                                        }

                                let runId = curCfg |> getRunId

                                let runDir = IO.Path.Combine(rootDir, runFolder, runId |> RunId.value |> string)
                                let fs = new WorkspaceFileStore(runDir)
                                let! ssEvalnPrams = fs.getAllSorterSetEvalsWithParamsByName wnToQuery

                                let! yab = ssEvalnPrams 
                                                |> List.map (fun (ssEval, wsPram) -> 
                                                    reportLines 
                                                            ssEval 
                                                            wsPram 
                                                            reportFileName
                                                            fsReporter)
                                           |> Result.sequence
                                return ()

            return "success"
        }

















    let paramGroup (genBinSz:int) (wsPram:workspaceParams)  =
        result {
            let! gen = wsPram |> WorkspaceParams.getGeneration "generation"
                              |> Result.map(Generation.binnedValue genBinSz)
            let! mr = wsPram |> WorkspaceParams.getMutationRate "mutationRate"
            let! nf = wsPram |> WorkspaceParams.getNoiseFraction "noiseFraction"
            let! order = wsPram |> WorkspaceParams.getOrder "order"
            let! runId = wsPram |> WorkspaceParams.getRunId "runId"
            let! sct = wsPram |> WorkspaceParams.getSorterCount "sorterCount"
            let! scM = wsPram |> WorkspaceParams.getSorterCount "sorterCountMutated"
            let! sLen = wsPram |> WorkspaceParams.getSorterCount "sorterLength"
            let! stw = wsPram |> WorkspaceParams.getStageWeight "stageWeight"
            let! sgm = wsPram |> WorkspaceParams.getSwitchGenMode "switchGenMode"
            return [
                        gen :> obj; 
                        mr :> obj; 
                        nf :> obj; 
                        order :> obj; 
                        runId :> obj; 
                        sct :> obj; 
                        scM :> obj; 
                        sLen :> obj; 
                        stw :> obj; 
                        sgm :> obj;
                   ] |> GuidUtils.guidFromObjs
        }


    let addToGroupReport 
            (fsReporting:WorkspaceFileStore)
            (fsRun:WorkspaceFileStore)
            (fileName:string)
            (genBinSz:int)
            (tupL:(workspaceDescription*workspaceParams) list) =

        let getSorterSetEvals worspaceDescr = 
            let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
            result {
                return! fsRun.getComponent wnSorterSetEvalParent worspaceDescr
                              |> Result.bind(WorkspaceComponent.asSorterSetEval)
            }
        result {
            let wsPramsFirst = tupL |> List.head |> snd
            let! ssEvals = 
                tupL |> List.map(fst >> getSorterSetEvals)
                     |> Result.sequence

            let sorterEvalFirst = 
                        ssEvals 
                        |> List.head |> SorterSetEval.getSorterEvals |> Array.head

            let! speedBins = 
                        ssEvals 
                        |> SorterSetEval.getAllSorterSpeedBins
                        |> Result.map(Seq.toList)

            let! bpvs = binnedParamValues wsPramsFirst genBinSz
            let bsevs = binnedSorterEvalValues sorterEvalFirst

            let newLines =
                speedBins |> List.map(
                    fun (srtrSpeed, ct) -> 

                        let switchCt = srtrSpeed 
                                        |> SorterSpeed.getSwitchCount
                                        |> SwitchCount.value
                        let stageCt = srtrSpeed 
                                        |> SorterSpeed.getStageCount
                                        |> StageCount.value
                        
                        $"{bpvs}{bsevs}\t{switchCt}\t{stageCt}\t{ct}"
                    )

            let! res = fsReporting.appendLines None fileName newLines
            return ()
        }



    let reportEmAllG
            (rootDir:string)
            (randomSeeds:rngGen seq)
            (stageWeights:stageWeight seq) 
            (noiseFractions:noiseFraction seq) 
            (mutationRates:mutationRate seq)
            (sorterSetPruneMethods:sorterSetPruneMethod seq)
        =
        let genBinSz = 10
        let runDir = System.IO.Path.Combine(rootDir, runFolder)

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        let wnToQuery = wnSorterSetEvalParent


        let reportFileName = wnToQuery |> WsComponentName.value |> string
        let fsReporter = new WorkspaceFileStore(runDir)

        fsReporter.appendLines None reportFileName [reportHeaderBinned ()] 
                |> Result.ExtractOrThrow
                |> ignore


        result {


            for randomSeed in randomSeeds do
                for stageWeight in stageWeights do
                    for noiseFraction in noiseFractions do
                        for mutationRate in mutationRates do
                            for sorterSetPruneMethod in sorterSetPruneMethods do
                                let curCfg = 
                                    { defltCfg with 
                                        rngGen = randomSeed;
                                        stageWeight = stageWeight;
                                        noiseFraction = noiseFraction;
                                        mutationRate = mutationRate;
                                        sorterSetPruneMethod = sorterSetPruneMethod
                                        }

                                let runId = curCfg |> getRunId

                                let runDir = IO.Path.Combine(rootDir, runFolder, runId |> RunId.value |> string)
                                let fsForRun = new WorkspaceFileStore(runDir)
                                let! compTupes = fsForRun.getAllWorkspaceDescriptionsWithParams()
                                let dscrs = compTupes |> List.filter(
                                    fun (descr, prams) -> descr |> WorkspaceDescription.getLastCauseName = "setupForNextGen" )
                                let! taggedTupes = 
                                        dscrs |> List.map(fun (descr, prams) -> ((descr, prams), prams |> paramGroup genBinSz))
                                              |> List.map(Result.tupRight)
                                              |> Result.sequence
                                let gps = taggedTupes |> List.groupBy(snd) |> List.map(snd >> List.map(fst))
                                let repLs = gps |> List.map(addToGroupReport fsReporter fsForRun reportFileName genBinSz)
                                ()

            return ()
        }