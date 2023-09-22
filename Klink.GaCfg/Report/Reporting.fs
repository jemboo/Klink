namespace global
open System


module Reporting =

    let selectedSorterEvalProps =
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
        selectedSorterEvalProps
            |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""


    let standardSorterEvalValues (sev:sorterEval) =
        let yab = sev |> SorterEval.getSorterEvalProps
        selectedSorterEvalProps
            |> List.fold(fun st t -> sprintf "%s\t%s" st yab.[t]) ""


    let speedBinProps =
        [
            sorterEvalProps.ErrorMsg;
            sorterEvalProps.Phenotype;
            sorterEvalProps.SortedSetSize;
            sorterEvalProps.SorterId;
            sorterEvalProps.StageCount;
            sorterEvalProps.Success;
            sorterEvalProps.SwitchCount;
        ]

    let standardSpeedBinHeaders () = 
        speedBinProps
            |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""


    let paramPropsSorterEval =
        [
            GaWsParamKeys.generation_current;
            GaWsParamKeys.runId;
            GaWsParamKeys.mutationRate;
            GaWsParamKeys.noiseFraction;
            GaWsParamKeys.order;
            GaWsParamKeys.sortableSetId;
            GaWsParamKeys.sorterCount;
            GaWsParamKeys.sorterCountMutated;
            GaWsParamKeys.sorterSetPruneMethod;
            GaWsParamKeys.stageWeight;
            GaWsParamKeys.sorterLength;
            GaWsParamKeys.switchGenMode;
        ]

        
    let paramPropsSpeedBins =
        [
            GaWsParamKeys.generation_current;
            GaWsParamKeys.runId;
            GaWsParamKeys.mutationRate;
            GaWsParamKeys.noiseFraction;
            GaWsParamKeys.order;
            GaWsParamKeys.sortableSetId;
            GaWsParamKeys.sorterCount;
            GaWsParamKeys.sorterCountMutated;
            GaWsParamKeys.sorterSetPruneMethod;
            GaWsParamKeys.stageWeight;
            GaWsParamKeys.sorterLength;
            GaWsParamKeys.switchGenMode;
        ]
        


    let standardParamValues (wsps:workspaceParams) =
        let paramMap t = (wsps |> WorkspaceParams.getMap).[t]
        paramPropsSorterEval
            |> StringUtil.toCsvLine paramMap


    let binParamProps =
        [
            GaWsParamKeys.runId;
            GaWsParamKeys.mutationRate;
            GaWsParamKeys.noiseFraction;
            GaWsParamKeys.order;
            GaWsParamKeys.sortableSetId;
            GaWsParamKeys.sorterCount;
            GaWsParamKeys.sorterCountMutated;
            GaWsParamKeys.stageWeight;
            GaWsParamKeys.sorterLength;
            GaWsParamKeys.switchGenMode;
        ]

    let binnedParamValues (wsps:workspaceParams) (genBinSz:int) =
        let paramMap t = (wsps |> WorkspaceParams.getMap).[t]
        result {
            let! genBin = wsps |> WorkspaceParamsAttrs.getGeneration GaWsParamKeys.generation_current
                               |> Result.map(Generation.binnedValue genBinSz)



            return $"{ genBin }\t{ binParamProps |> StringUtil.toCsvLine paramMap }"
        }


    let reportHeaderSorterEvals () =
        sprintf "%s%s" 
            (paramPropsSorterEval 
              |> List.map(WorkspaceParamsKey.value) 
              |> List.reduce(fun st t -> sprintf "%s\t%s" st t))
            (standardSorterEvalHeaders ())


    let reportHeaderSpeedBins () =
        sprintf "%s%s" 
            (paramPropsSpeedBins
              |> List.map(WorkspaceParamsKey.value) 
              |> List.reduce(fun st t -> sprintf "%s\t%s" st t))
            (SpeedBinProps.getHeader())


    let reportHeaderBinned () =
        sprintf "%s%s%s"
            "gen_bin\t"
            (binParamProps
              |> List.map(WorkspaceParamsKey.value) 
              |> List.reduce(fun st t -> sprintf "%s\t%s" st t))
            "\tSwitchCt\tStageCt\tRecordCt"


    let reportSorterSetEvalLines
            (sorterSetEval:sorterSetEval) 
            (wsps:workspaceParams)
            (lineAppender:seq<string> -> Result<bool,string>)
        =
        let lines = 
            sorterSetEval 
                |> SorterSetEval.getSorterEvals 
                |> Array.map(fun sev ->
                                    sprintf "%s%s" 
                                        (standardParamValues wsps)
                                        (standardSorterEvalValues sev))
        lineAppender lines



    let reportSpeedBinLines
            (sorterSpeedBinSet:sorterSpeedBinSet) 
            (wsps:workspaceParams)
            (lineAppender:seq<string> -> Result<bool,string>)
        =
        let stageW1 = (wsps |> WorkspaceParams.getMap).[GaWsParamKeys.stageWeight]

        let stageW2 = stageW1
                     |> Double.Parse

        let stageW3 = stageW2 |> StageWeight.create

        let yab = SpeedBinProps.getAllProperties stageW3 sorterSpeedBinSet
        let lines = 
            [|
                sprintf "%s%s" 
                    (standardParamValues wsps)
                    yab
        
            |]

        lineAppender lines





    let reportEvals 
                 (fsReporter:WorkspaceFileStore) 
                 (reportFileName:string) 
                 (evalCompName:wsComponentName)
                 (minGen:generation)
                 (runFolderPath:string) 
        =
        let _lineWriter (lines:string seq) =
            fsReporter.writeLinesEnsureHeader None reportFileName [reportHeaderSorterEvals ()] lines

        result {
            let fsRunReader = new WorkspaceFileStore(runFolderPath)
            let! compTupes = 
                fsRunReader.getAllSorterSetEvalsWithParams
                    evalCompName 
                    (WorkspaceParamsAttrs.generationIsGte GaWsParamKeys.generation_current minGen)
            return!
                compTupes 
                |> List.map (fun (ssEval, wsPram) -> 
                    reportSorterSetEvalLines ssEval wsPram _lineWriter)
                |> Result.sequence
        }


    let reportBins 
            (fsReporter:WorkspaceFileStore) 
            (reportFileName:string)
            (minGen:generation)
            (runFolderPath:string) 
        =
        let _lineWriter (lines:string seq) =
            fsReporter.writeLinesEnsureHeader None reportFileName [reportHeaderSpeedBins ()] lines

        result {
            let fsRunReader = new WorkspaceFileStore(runFolderPath)
            let perfBinCompName = "sorterSpeedBinSet" |> WsComponentName.create
            let! compTupes = 
                fsRunReader.getAllSpeedSetBinsWithParams 
                    perfBinCompName
                    (WorkspaceParamsAttrs.generationIsGte GaWsParamKeys.generation_current minGen)
            return!
                compTupes 
                |> List.map (fun (ssBins, wsPram) -> 
                    reportSpeedBinLines ssBins wsPram _lineWriter)
                |> Result.sequence
        }




    let paramGroup (genBinSz:generation) 
                   (wsPram:workspaceParams)  =
        result {
            let! gen = wsPram |> WorkspaceParamsAttrs.getGeneration GaWsParamKeys.generation_current
                              |> Result.map(Generation.binnedValue (genBinSz |> Generation.value))
            let! mr = wsPram |> WorkspaceParamsAttrs.getMutationRate GaWsParamKeys.mutationRate
            let! nf = wsPram |> WorkspaceParamsAttrs.getNoiseFraction GaWsParamKeys.noiseFraction
            let! order = wsPram |> WorkspaceParamsAttrs.getOrder GaWsParamKeys.order
            let! runId = wsPram |> WorkspaceParamsAttrs.getRunId GaWsParamKeys.runId
            let! sct = wsPram |> WorkspaceParamsAttrs.getSorterCount GaWsParamKeys.sorterCount
            let! scM = wsPram |> WorkspaceParamsAttrs.getSorterCount GaWsParamKeys.sorterCountMutated
            let! sLen = wsPram |> WorkspaceParamsAttrs.getSorterCount GaWsParamKeys.sorterLength
            let! stw = wsPram |> WorkspaceParamsAttrs.getStageWeight GaWsParamKeys.stageWeight
            let! sgm = wsPram |> WorkspaceParamsAttrs.getSwitchGenMode GaWsParamKeys.switchGenMode
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
            (lineAppender:seq<string> -> Result<bool,string>)
            (fsRun:WorkspaceFileStore)
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

            let! res = lineAppender newLines
            return ()
        }


    let doReportPerfBins
            (projectDir:string)
            (genBinSz: generation)
        =

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnToQuery = wnSorterSetEvalParent


        let reportFileName = wnToQuery |> WsComponentName.value |> string
        let fsReporter = new WorkspaceFileStore(projectDir)

        let _lineWriter (lines:string seq) =
            fsReporter.writeLinesEnsureHeader None reportFileName [reportHeaderBinned ()] lines

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
                let repLs = gps |> List.map(addToGroupReport _lineWriter fsForRun genBinSz)
                ()

            return ()
        }