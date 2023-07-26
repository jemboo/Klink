namespace global
open System


module WsOps = 


    let runFolder = "testRnd"

    let makeEmAll
            (rootDir:string) 
            (runIds:runId seq)
            (stageWeights:stageWeight seq) 
            (noiseFractions:noiseFraction option seq) 
            (mutationRates:mutationRate seq) 
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
        let wnSorterSetPruner = "sorterSetPruner" |> WsComponentName.create


        result {

            for runId in runIds do
                for stageWeight in stageWeights do
                    for noiseFraction in noiseFractions do
                        for mutationRate in mutationRates do
                            let runDir = IO.Path.Combine(rootDir, runFolder, runId |> RunId.value |> string)
                            let fs = new WorkspaceFileStore(runDir)
                            let wsParams = WsParamsGen.forGa runId stageWeight noiseFraction mutationRate


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

                            while curGen < 500 do
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
                                        wnSorterSetPruner
                                        fs
                                        (fun s-> Console.WriteLine(s))
                                        curParams
                                        curCfg

                                curCfg <- wsCfgN
                                curParams <- wsPramsN
                                curGen <- curGen + 1

            return ()
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
            return ()
        }



    let reportEmAll 
            (rootDir:string)
            (runIds:runId seq)
        =
        let runDir = System.IO.Path.Combine(rootDir, runFolder)
        let reportFileName = "SorterSetEvalPruned"
        let fsReporter = new WorkspaceFileStore(runDir)

        fsReporter.appendLines None reportFileName [reportHeaderStandard ()] 
                |> Result.ExtractOrThrow
                |> ignore

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        result {

            for runId in runIds do
                let runDir = IO.Path.Combine(rootDir, runFolder, runId |> RunId.value |> string)
                let fs = new WorkspaceFileStore(runDir)
                let! ssEvalnPrams = fs.getAllSorterSetEvalsWithParamsByName wnSorterSetEvalPruned

                let! yab = ssEvalnPrams 
                                |> List.map (fun (ssEval, wsPram) -> 
                                    reportLines ssEval wsPram reportFileName fsReporter)
                           |> Result.sequence
                ()

            return ()
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
                        
                        $"{bpvs}\t{bsevs}{switchCt}\t{stageCt}\t{ct}"
                    )

            let! res = fsReporting.appendLines None fileName newLines
            return ()
        }


    let reportEmAllG
            (rootDir:string)
            (runIds:runId seq)
        =
        let genBinSz = 10
        let runDir = System.IO.Path.Combine(rootDir, runFolder)
        let reportFileName = "SorterSetEvalPruned_G"
        let fsReporter = new WorkspaceFileStore(runDir)

        fsReporter.appendLines None reportFileName [reportHeaderBinned ()] 
                |> Result.ExtractOrThrow
                |> ignore

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        result {

            for runId in runIds do
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
                let yab = 5
                ()

            return ()
        }