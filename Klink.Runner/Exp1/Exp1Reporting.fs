namespace global
open System


module Exp1Reporting =

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
            "sorterSetPruneMethod";
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


    let reportEm (projectDir:string) (runId:runId) 
        =
        let runDir = System.IO.Path.Combine(projectDir, runId |> RunId.value |> string)
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
            (projectDir:string)
            (fileName:string)
            (firstFolderIndex:int)
            (folderNum:int)
        =
        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        let wnToQuery = wnSorterSetEvalParent

        let fsReporter = new WorkspaceFileStore(projectDir)

        fsReporter.appendLines None fileName [reportHeaderStandard ()] 
                |> Result.ExtractOrThrow
                |> ignore


        let runDirs = IO.Directory.EnumerateDirectories(projectDir)

        result {
        
            for runDir in (runDirs |> Seq.skip firstFolderIndex |> Seq.take folderNum) do

                let fsForRun = new WorkspaceFileStore(runDir)
                let fs = new WorkspaceFileStore(runDir)
                let! ssEvalnPrams = fs.getAllSorterSetEvalsWithParamsByName wnToQuery

                let! yab = ssEvalnPrams 
                                |> List.map (fun (ssEval, wsPram) -> 
                                    reportLines 
                                            ssEval 
                                            wsPram 
                                            fileName
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
            (projectDir:string)
            (genBinSz: generation)
        =

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        let wnToQuery = wnSorterSetEvalParent


        let reportFileName = wnToQuery |> WsComponentName.value |> string
        let fsReporter = new WorkspaceFileStore(projectDir)


        fsReporter.appendLines None reportFileName [reportHeaderBinned ()] 
                |> Result.ExtractOrThrow
                |> ignore

        let runDirs = IO.Directory.EnumerateDirectories(projectDir)

        result {
            for runDir in runDirs do

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