﻿namespace global
open System

module O128_StageRflCfg =

    open CommonParams
    let baseDir = $"c:\Klink"
    let projectFolder  = $"o128\StageRfl" |> ProjectFolder.create


    let runCfgPlex =
        {
            shcCfgPlex.orders = [| 128 |> Order.createNr |]
            sortableSetCfgs =  
                    [| 
                        (
                            sortableSetCfgType.MergeWithInts, 
                            0 |> StageCount.create,
                            sorterEvalMode.CheckSuccess
                        )
                    |]
            mutationRates = [|mr1;|];
            noiseFractions = [|nf0;nf1;nf2;nf3|];
            rngGens = rndGens 0 4 ;
            tupSorterSetSizes = [|ssz5_6|];
            sorterSetPruneMethodsOld = [|sspm1;|];
            stageWeights = [|sw0; sw1|];
            switchGenModes = [|switchGenMode.stageSymmetric|];
            projectFolder = projectFolder
        } |> runCfgPlex.Shc

    //let runCfgPlex =
    //    {
    //        shcCfgPlex.orders = [| 128 |> Order.createNr |]
    //        sortableSetCfgs =  
    //                [| 
    //                    (
    //                        sortableSetCfgType.MergeWithInts, 
    //                        0 |> StageCount.create,
    //                        sorterEvalMode.CheckSuccess
    //                    )
    //                |]
    //        mutationRates = [|mr1;|];
    //        noiseFractions = [|nf3;|];
    //        rngGens = rndGens 0 2 ;
    //        tupSorterSetSizes = [|ssz5_6|];
    //        sorterSetPruneMethods = [|sspm1;|];
    //        stageWeights = [|sw0; sw1|];
    //        switchGenModes = [|switchGenMode.stageSymmetric|];
    //    } |> runCfgPlex.Shc


    let baseGenerationCount = 25000 |> Generation.create
    let baseReportFilter = CommonParams.modulusFilter 25
    let fullReportFilter = CommonParams.modulusFilter 25
    let initScriptName = "patchScript"


    let writeInitScripts (maxRunsPerScript:int) = 
            KlinkScript.createInitRunScriptsFromRunCfgPlex 
                baseGenerationCount
                baseReportFilter
                fullReportFilter
                initScriptName
                projectFolder
                maxRunsPerScript
                None
                runCfgPlex
            |> Array.map(ScriptFileMake.writeScript baseDir)


    let reportGenMin = 0 |> Generation.create
    let reportEvalsFileName = "reportEvalsReport"
    let reportEvalsScriptFileName = "reportEvalsReportScript"
    let evalScriptComponent = ("sorterSetEvalParent" |> WsComponentName.create)

    let writeReportEvalsScript (seqSplicer:(int*int) option) = 
            KlinkScript.createReportEvalsScriptFromRunCfgPlex 
                reportGenMin
                baseGenerationCount
                evalScriptComponent
                baseReportFilter
                reportEvalsFileName
                projectFolder
                seqSplicer
                runCfgPlex
            
         |> ScriptFileMake.writeScript baseDir


    let reportBinsFileName = "reportBinsReport"
    let reportBinsScriptFileName = "reportBinsReportScript"

    let writeReportBinsScript (seqSplicer:(int*int) option) = 
            KlinkScript.createReportBinsScriptFromRunCfgPlex 
                reportGenMin
                baseGenerationCount
                reportBinsFileName
                projectFolder
                seqSplicer
                runCfgPlex
            
            |> ScriptFileMake.writeScript baseDir