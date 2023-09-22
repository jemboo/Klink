namespace global
open System
open System.IO


type shcInitRunCfg =
    {
        mutationRate:mutationRate
        newGenerations:generation
        noiseFraction:noiseFraction
        order:order
        rngGen:rngGen
        sorterEvalMode:sorterEvalMode
        sorterCount:sorterCount
        sorterCountMutated:sorterCount
        sorterSetPruneMethod:sorterSetPruneMethod
        stagesSkipped:stageCount
        stageWeight:stageWeight
        switchCount:switchCount
        switchGenMode:switchGenMode
        reportFilter:generationFilter option
    }


module ShcInitRunCfg =

    let getRunId (cfg:shcInitRunCfg) =
        [
            cfg.mutationRate |> MutationRate.value :> obj;
            cfg.noiseFraction |> NoiseFraction.value :> obj;
            cfg.order |> Order.value :> obj;
            cfg.rngGen :> obj;
            cfg.sorterEvalMode :> obj
            cfg.sorterCount |> SorterCount.value :> obj;
            cfg.sorterCountMutated |> SorterCount.value :> obj;
            cfg.sorterSetPruneMethod :> obj;
            cfg.stageWeight |> StageWeight.value :> obj;
            cfg.switchCount :> obj;
            cfg.switchGenMode :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create



    let toWorkspaceParams 
            (useParallel:useParallel)
            (gaCfg:shcInitRunCfg)
        =
        let _nextRngGen rng =
            rng
            |> Rando.fromRngGen
            |> Rando.toRngGen

        let rngGenCreate = (_nextRngGen gaCfg.rngGen)
        let rngGenMutate = (_nextRngGen rngGenCreate)
        let rngGenPrune = (_nextRngGen rngGenMutate)

        WorkspaceParams.make Map.empty
        |> WorkspaceParamsAttrs.setRunId ShcWsParamKeys.runId (gaCfg |> getRunId)
        |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_current (0 |> Generation.create)
        |> WorkspaceParamsAttrs.setGeneration ShcWsParamKeys.generation_max (gaCfg.newGenerations)
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenCreate rngGenCreate
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenMutate rngGenMutate
        |> WorkspaceParamsAttrs.setRngGen ShcWsParamKeys.rngGenPrune rngGenPrune
        |> WorkspaceParamsAttrs.setMutationRate ShcWsParamKeys.mutationRate gaCfg.mutationRate
        |> WorkspaceParamsAttrs.setNoiseFraction ShcWsParamKeys.noiseFraction (Some gaCfg.noiseFraction)
        |> WorkspaceParamsAttrs.setOrder ShcWsParamKeys.order gaCfg.order
        |> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCount gaCfg.sorterCount
        |> WorkspaceParamsAttrs.setSorterCount ShcWsParamKeys.sorterCountMutated gaCfg.sorterCountMutated
        |> WorkspaceParamsAttrs.setSorterEvalMode ShcWsParamKeys.sorterEvalMode gaCfg.sorterEvalMode
        |> WorkspaceParamsAttrs.setStageCount ShcWsParamKeys.stagesSkipped gaCfg.stagesSkipped
        |> WorkspaceParamsAttrs.setStageWeight ShcWsParamKeys.stageWeight gaCfg.stageWeight
        |> WorkspaceParamsAttrs.setSwitchCount ShcWsParamKeys.sorterLength gaCfg.switchCount
        |> WorkspaceParamsAttrs.setSwitchGenMode ShcWsParamKeys.switchGenMode gaCfg.switchGenMode
        |> WorkspaceParamsAttrs.setSorterSetPruneMethod ShcWsParamKeys.sorterSetPruneMethod gaCfg.sorterSetPruneMethod
        |> WorkspaceParamsAttrs.setGenerationFilter ShcWsParamKeys.generation_filter (gaCfg.reportFilter |> Option.get )
        |> WorkspaceParamsAttrs.setUseParallel ShcWsParamKeys.useParallel useParallel



type shcContinueRunCfg =
    {
        runId:runId
        newGenerations:generation
    }


type shcRunCfg =
    | InitRun of shcInitRunCfg
    | Continue of shcContinueRunCfg


module ShcRunCfg =

    let procShcRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (shcRunCfg:shcRunCfg)
        =
            match shcRunCfg with
            | InitRun irc -> 
                irc |> ShcInitRunCfg.toWorkspaceParams up
                    |> InterGenWsOps.doGenLoop projectFolderPath
            | Continue crc -> 
                 InterGenWsOps.continueUpdating 
                        projectFolderPath 
                        crc.runId 
                        crc.newGenerations
            //| Report rrc -> 
            //    match rrc with
            //    | Evals rac ->
            //        ShcReportEvalsCfg.reportAllEvals 
            //                projectFolderPath 
            //                rac
            //    | Bins rbc ->
            //        ShcReportBinsCfgs.reportAllBins
            //                projectFolderPath 
            //                rbc














//type shcRunCfgSet = {
//                        setName:string; 
//                        runCfgs:shcRunCfg[]
//                    }


//module ShcRunCfgSet =

//    let procRunCfgSet 
//            (projectFolderPath:string)
//            (up:useParallel)
//            (rcs:shcRunCfgSet)
//        =
//        result {
//            let! yab = rcs.runCfgs
//                      |> Array.map(ShcRunCfg.procShcRunCfg projectFolderPath up)
//                      |> Array.toList
//                      |> Result.sequence
//            return ()
//        }



//    let initRunFromPlex
//            (newGenerations:generation)
//            (reportFilter:generationFilter)
//            (runSetName:string)
//            (plex:shcInitRunCfgPlex)
//        =
//        let runCfgs = 
//            plex |>
//            ShcInitRunCfg.fromPlex (Some newGenerations) (Some reportFilter)
//                  |> Seq.map(shcRunCfg.InitRun)
//                  |> Seq.toArray

//        {setName = runSetName; runCfgs = runCfgs}


//    let continueRunFromPlex 
//            (newGenerations:generation)
//            (runSetName:string)
//            (plex:shcInitRunCfgPlex)
//        =
//        let runCfgs = 
//            plex |>
//            ShcContinueRunCfg.fromPlex newGenerations
//                  |> Seq.map(shcRunCfg.Continue)
//                  |> Seq.toArray

//        {setName = runSetName; runCfgs = runCfgs}


//    let reportAllFromPlex 
//            (genMin:generation)
//            (genMax:generation)
//            (evalCompName:wsComponentName)
//            (reportFilter:generationFilter)
//            (runSetName:string)
//            (reportFileName:string)
//            (plex:shcInitRunCfgPlex)
//        =
//        let runCfg = 
//            plex |>
//            ShcReportEvalsCfg.fromPlex 
//                    genMin
//                    genMax 
//                    evalCompName 
//                    reportFilter 
//                    reportFileName
//                  |> shcReportCfg.Evals
//                  |> shcRunCfg.Report

//        {setName = runSetName; runCfgs = [|runCfg|]}


//    let reportAllFromPlexSeq 
//            (genMin:generation)
//            (genMax:generation)
//            (evalCompName:wsComponentName)
//            (reportFilter:generationFilter)
//            (runSetName:string)
//            (reportFileName:string)
//            (plexes:shcInitRunCfgPlex seq)
//        =
//        let runCfgs = 
//            plexes |> Seq.map(
//            ShcReportEvalsCfg.fromPlex 
//                    genMin
//                    genMax 
//                    evalCompName 
//                    reportFilter 
//                    reportFileName)
//            |> Seq.map(shcReportCfg.Evals >> shcRunCfg.Report)
//            |> Seq.toArray

//        {setName = runSetName; runCfgs = runCfgs}


//    let reportBinsFromPlex 
//            (genMin:generation)
//            (genMax:generation)
//            (runSetName:string)
//            (reportFileName:string)
//            (plex:shcInitRunCfgPlex)
//        =
//        let runCfg = 
//            plex |>
//            ShcReportBinsCfgs.fromPlex 
//                    genMin
//                    genMax
//                    reportFileName
//                  |> shcReportCfg.Bins
//                  |> shcRunCfg.Report

//        {setName = runSetName; runCfgs = [|runCfg|]}



//    let reportBinsFromPlexSeq
//            (genMin:generation)
//            (genMax:generation)
//            (runSetName:string)
//            (reportFileName:string)
//            (plexes:shcInitRunCfgPlex seq)
//        =
//        let runCfgs = 
//            plexes |> Seq.map(
//            ShcReportBinsCfgs.fromPlex 
//                    genMin
//                    genMax
//                    reportFileName)
//            |> Seq.map(shcReportCfg.Bins >> shcRunCfg.Report)
//            |> Seq.toArray

//        {setName = runSetName; runCfgs = runCfgs}