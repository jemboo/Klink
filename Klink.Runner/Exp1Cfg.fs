namespace global
open System


module Exp1Cfg =

    let rngGen0 = 10110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen1 = 20110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen2 = 22110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen3 = 12110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen4 = 13110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen5 = 31110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen6 = 14110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen7 = 15110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen8 = 16110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen9 = 17110 |> RandomSeed.create |> RngGen.createLcg
    let rngGen10 = 1811 |> RandomSeed.create |> RngGen.createLcg
    let rngGen11 = 1911 |> RandomSeed.create |> RngGen.createLcg
    let rngGen12 = 2011 |> RandomSeed.create |> RngGen.createLcg
    let rngGen13 = 2111 |> RandomSeed.create |> RngGen.createLcg
    let rngGen14 = 2211 |> RandomSeed.create |> RngGen.createLcg


    let scP0 = 1     |> SorterCount.create
    let scP1 = 2     |> SorterCount.create
    let scP2 = 4     |> SorterCount.create
    let scP3 = 8     |> SorterCount.create
    let scP4 = 16    |> SorterCount.create
    let scP5 = 32    |> SorterCount.create
    let scP6 = 64    |> SorterCount.create
    let scP7 = 128   |> SorterCount.create
    let scP8 = 256   |> SorterCount.create
    let scP9 = 512   |> SorterCount.create
    let scP10 = 1024 |> SorterCount.create


    let scM0 = 1     |> SorterCount.create
    let scM1 = 2     |> SorterCount.create
    let scM2 = 4     |> SorterCount.create
    let scM3 = 8     |> SorterCount.create
    let scM4 = 16    |> SorterCount.create
    let scM5 = 32    |> SorterCount.create
    let scM6 = 64    |> SorterCount.create
    let scM7 = 128   |> SorterCount.create
    let scM8 = 256   |> SorterCount.create
    let scM9 = 512   |> SorterCount.create
    let scM10 = 1024 |> SorterCount.create



    let nf0 = 0.001 |> NoiseFraction.create
    let nf1 = 0.025 |> NoiseFraction.create
    let nf2 = 0.050 |> NoiseFraction.create
    let nf3 = 0.100 |> NoiseFraction.create
    let nf4 = 0.250 |> NoiseFraction.create
    let nf5 = 0.500 |> NoiseFraction.create |> Some
    let nf6 = 1.000 |> NoiseFraction.create |> Some
    let nf7 = 2.000 |> NoiseFraction.create |> Some

        
    let sw0 = 0.05 |> StageWeight.create
    let sw1 = 0.10 |> StageWeight.create
    let sw2 = 0.25 |> StageWeight.create
    let sw3 = 0.50 |> StageWeight.create
    let sw4 = 1.00 |> StageWeight.create
    let sw5 = 2.00 |> StageWeight.create
    let sw6 = 4.00 |> StageWeight.create
    let sw7 = 8.00 |> StageWeight.create


    let mr0 = 0.0005 |> MutationRate.create
    let mr1 = 0.0010 |> MutationRate.create
    let mr2 = 0.0020 |> MutationRate.create
    let mr3 = 0.0030 |> MutationRate.create
    let mr4 = 0.0050 |> MutationRate.create
    let mr5 = 0.0075 |> MutationRate.create
    let mr6 = 0.0100 |> MutationRate.create
    let mr7 = 0.0250 |> MutationRate.create
    let mr8 = 0.0500 |> MutationRate.create
    let mr9 = 0.0750 |> MutationRate.create
    let mr10 = 0.250 |> MutationRate.create

    let sspm1 = sorterSetPruneMethod.Whole
    let sspm2 = sorterSetPruneMethod.Shc
        
    let rngGens = [rngGen0; rngGen1; rngGen2; rngGen3; rngGen4; rngGen5; rngGen6; rngGen7;]

    //let sorterSetSizes = [(scP0, scM0); (scP0, scM1); (scP1, scM1); (scP1, scM2)]
    let sorterSetSizes = [(scP9, scM9);]

    let stageWeights = [sw0; sw1; sw2;]

    let noiseFractions = [nf0; nf2; nf3; nf4; ]

    let mutationRates = [mr10; mr4; mr5; mr6; mr7;]
        
    let sorterSetPruneMethods = [sspm2;] // sspm2]

    let switchGenModes = [switchGenMode.Switch; switchGenMode.Stage; switchGenMode.StageSymmetric]

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
        

    let doRun
            (rootDir:string)
            (gaCfgs: gaCfg seq)
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
            for gaCfg in gaCfgs do
                let wsParams = gaCfg |> GaCfg.getWorkspaceParams
                let runId = gaCfg |> GaCfg.getRunId

                let runDir = IO.Path.Combine(rootDir, runId |> RunId.value |> string)
                let fs = new WorkspaceFileStore(runDir)

                let! wsCfg_params, _ = 
                        WsOpsLib.genZero
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))

                let mutable curGen = gaCfg.curGen |> Generation.value
                let mutable curCfg = wsCfg_params |> fst
                let mutable curParams = wsCfg_params |> snd
                let maxGen = gaCfg.maxGen |> Generation.value

                while curGen < maxGen do
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



    let doRunRun
            (rootDir:string)
            (gaCfgs: gaCfg seq)
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
            for gaCfg in gaCfgs do
                let wsParams = gaCfg |> GaCfg.getWorkspaceParams
                let runId = gaCfg |> GaCfg.getRunId

                let runDir = IO.Path.Combine(rootDir, runId |> RunId.value |> string)
                let fs = new WorkspaceFileStore(runDir)

                let! wsCfg_params, ws = 
                        WsOpsLib.genZero
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))

                let mutable curGen = gaCfg.curGen |> Generation.value
                let mutable curCfg = wsCfg_params |> fst
                let mutable curParams = wsCfg_params |> snd
                let mutable curWorkspace = ws


                let maxGen = gaCfg.maxGen |> Generation.value

                while curGen < maxGen do
                    let! wsN, wsPramsN = 
                        WsOpsLib.doGenOnWorkspace
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
                            curWorkspace

                    curWorkspace <- wsN
                    curParams <- wsPramsN
                    curGen <- curGen + 1

            return "success"
        }


    let cfgsForTestRun (rndSkip:int) = 
        GaCfg.enumerate 
                (GaCfg.rndGens |> Seq.skip(rndSkip) |> Seq.take 8)
                [(scP10, scM10)]
                [switchGenMode.StageSymmetric]
                [sw0] 
                [nf0; nf4;]
                [mr5; mr7] 
                [sspm1]
                (2500 |> Generation.create)
         


    let cfgsForCompleteRun () = 
        GaCfg.enumerate 
                rngGens
                sorterSetSizes
                switchGenModes
                stageWeights 
                noiseFractions
                mutationRates 
                sorterSetPruneMethods
                (50 |> Generation.create)


    let standardSorterEvalProps =
        [
            sorterEvalProps.ErrorMsg;
            sorterEvalProps.Phenotype;
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


    //let binnedSorterEvalProps =
    //    [
    //        sorterEvalProps.SortableSetId;
    //    ]

    //let binnedSorterEvalHeaders () = 
    //    binnedSorterEvalProps
    //        |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""


    //let binnedSorterEvalValues (sev:sorterEval) =
    //    let yab = sev |> SorterEval.getSorterEvalProps
    //    binnedSorterEvalProps
    //        |> List.fold(fun st t -> sprintf "%s\t%s" st yab.[t]) ""


    let standardParamProps =
        [
            "generation";
            "runId";
            "mutationRate";
            "noiseFraction";
            "order";
            "sortableSet";
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
            "sortableSet";
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
            return $"{ genBin }\t{ binParamProps |> StringUtil.toCsvLine paramMap }"
        }


    let reportHeaderStandard () =
        sprintf "%s%s" 
            (standardParamProps |> List.reduce(fun st t -> sprintf "%s\t%s" st t))
            (standardSorterEvalHeaders ())


    let reportHeaderBinned () =
        sprintf "%s%s%s"
            "gen_bin\t"
            (binParamProps |> List.reduce(fun st t -> sprintf "%s\t%s" st t))
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
        let runDir = System.IO.Path.Combine(rootDir, runId |> RunId.value |> string)
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
            (gaCfgs: gaCfg seq)
        =
        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        let wnToQuery = wnSorterSetEvalParent

        let reportFileName = wnToQuery |> WsComponentName.value |> string
        let fsReporter = new WorkspaceFileStore(rootDir)

        fsReporter.appendLines None reportFileName [reportHeaderStandard ()] 
                |> Result.ExtractOrThrow
                |> ignore


        result {

            for gaCfg in gaCfgs do

                let runId = gaCfg |> GaCfg.getRunId

                let runDir = IO.Path.Combine(rootDir, runId |> RunId.value |> string)
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




    let paramGroup (genBinSz:generation) 
                   (wsPram:workspaceParams)  =
        result {
            let! gen = wsPram |> WorkspaceParams.getGeneration "generation"
                              |> Result.map(Generation.binnedValue (genBinSz |> Generation.value))
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
            (genBinSz:generation)
            (tupL:(workspaceDescription*workspaceParams) list) =

        let _getSorterSetEvals worspaceDescr = 
            let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
            result {
                return! fsRun.getComponent wnSorterSetEvalParent worspaceDescr
                              |> Result.bind(WorkspaceComponent.asSorterSetEval)
            }

        result {
            let wsPramsFirst = tupL |> List.head |> snd
            let! ssEvals = 
                tupL |> List.map(fst >> _getSorterSetEvals)
                     |> Result.sequence

            let! speedBins = 
                        ssEvals 
                        |> SorterSetEval.getAllSorterSpeedBins
                        |> Result.map(Seq.toList)

            let! bpvs = binnedParamValues wsPramsFirst (genBinSz |> Generation.value)

            let newLines =
                speedBins |> List.map(
                    fun (srtrSpeed, ct) -> 

                        let switchCt = srtrSpeed 
                                        |> SorterSpeed.getSwitchCount
                                        |> SwitchCount.value
                        let stageCt = srtrSpeed 
                                        |> SorterSpeed.getStageCount
                                        |> StageCount.value
                        
                        $"{bpvs}\t{switchCt}\t{stageCt}\t{ct}"
                    )

            let! res = fsReporting.appendLines None fileName newLines
            return ()
        }



    let doReportPerfBins
            (rootDir:string)
            (genBinSz: generation)
            (gaCfgs: gaCfg seq)
        =

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        let wnToQuery = wnSorterSetEvalParent


        let reportFileName = wnToQuery |> WsComponentName.value |> string
        let fsReporter = new WorkspaceFileStore(rootDir)


        fsReporter.appendLines None reportFileName [reportHeaderBinned ()] 
                |> Result.ExtractOrThrow
                |> ignore


        result {
            for gaCfg in gaCfgs do

                let runId = gaCfg |> GaCfg.getRunId

                let runDir = IO.Path.Combine(rootDir, runId |> RunId.value |> string)
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



    let doReportPerfBins2
            (rootDir:string)
            (genBinSz: generation)
        =

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        let wnToQuery = wnSorterSetEvalParent


        let reportFileName = wnToQuery |> WsComponentName.value |> string
        let fsReporter = new WorkspaceFileStore(rootDir)


        fsReporter.appendLines None reportFileName [reportHeaderBinned ()] 
                |> Result.ExtractOrThrow
                |> ignore

        let runDirs = IO.Directory.EnumerateDirectories(rootDir)

        result {
            for runDir in runDirs do

                //let runId = gaCfg |> GaCfg.getRunId
                //let runDir = IO.Path.Combine(rootDir, runId |> RunId.value |> string)

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