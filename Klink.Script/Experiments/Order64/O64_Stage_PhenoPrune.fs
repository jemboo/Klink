namespace global
open System

module O64_Stage_PhenoPrune =

    open CommonParams

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
    //        noiseFractions = [|nf1;|];
    //        rngGens = rndGens 0 1 ;
    //        tupSorterSetSizes = [|ssz4_5|];
    //        sorterSetPruneMethods = [|(sspm2, 2 |> SorterCount.create |> Some); (sspm1, None);|];
    //        stageWeights = [|sw0;|];
    //        switchGenModes = [|switchGenMode.stage|];
    //    } |> runCfgPlex.Shc


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
    //        noiseFractions = [|nf1;nf3;|];
    //        rngGens = rndGens 0 2 ;
    //        tupSorterSetSizes = [|ssz5_6|];
    //        sorterSetPruneMethods = 
    //            [|
    //              (sspm2, 1 |> SorterCount.create |> Some);
    //              (sspm2, 2 |> SorterCount.create |> Some);
    //              (sspm2, 4 |> SorterCount.create |> Some);
    //            |];
    //        stageWeights = [|sw0;|];
    //        switchGenModes = [|switchGenMode.stage|];
    //    } |> runCfgPlex.Shc


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
    //        mutationRates = [|mr1;mr2;mr3|];
    //        noiseFractions = [|nf3;nf4;nf5|];
    //        rngGens = rndGens 0 8 ;
    //        tupSorterSetSizes = [|ssz5_6|];
    //        sorterSetPruneMethods = 
    //            [|
    //              (sspm2, 4 |> SorterCount.create |> Some);
    //              (sspm2, 16 |> SorterCount.create |> Some);
    //              (sspm2, 32 |> SorterCount.create |> Some);
    //            |];
    //        stageWeights = [|sw0;|];
    //        switchGenModes = [|switchGenMode.stage; switchGenMode.stageSymmetric|];
    //    } |> runCfgPlex.Shc



    let runCfgPlex =
        {
            shcCfgPlex.orders = [| 64 |> Order.createNr |]
            sortableSetCfgs =  
                    [| 
                        (
                            sortableSetCfgType.MergeWithInts, 
                            0 |> StageCount.create,
                            sorterEvalMode.CheckSuccess
                        )
                    |]
            mutationRates = [|mr2;|];
            noiseFractions = [|nf2;nf3;nf4;|];
            rngGens = rndGens 0 10 ;
            tupSorterSetSizes = [|ssz5_6|];
            sorterSetPruneMethodsOld = 
                [|
                    spc2;
                    spc4;
                    spc6;
                |];
            stageWeights = [|sw0;|];
            switchGenModes = [|switchGenMode.stageSymmetric|];
        } |> runCfgPlex.Shc



    let initGenerationCount = 5000 |> Generation.create
    let continueGenerationCount = 49500 |> Generation.create
    let totalGenerationCount = Generation.addG initGenerationCount continueGenerationCount

    let initReportFilter = CommonParams.modulusFilter 10
    let continueReportFilter = CommonParams.modulusFilter 10

    let initScriptName = "initScriptL_mr"
    let continueScriptName = "continueScriptLr"

    let baseDir = $"c:\Klink"
    let projectFolder  = $"o64\StagePhenoPrune"

    let writeInitScripts (maxRunsPerScript:int) = 
            KlinkScript.createInitRunScriptsFromRunCfgPlex 
                initGenerationCount
                initReportFilter
                initScriptName
                maxRunsPerScript
                None
                runCfgPlex
            |> Array.map(ScriptFileMake.writeScript baseDir projectFolder)


    let writeContinueScripts (maxRunsPerScript:int) = 
            KlinkScript.createContinueRunScriptsFromRunCfgPlex 
                continueGenerationCount
                continueReportFilter
                continueScriptName
                maxRunsPerScript
                None
                runCfgPlex
            |> Array.map(ScriptFileMake.writeScript baseDir projectFolder)



    let reportGenMin = 0 |> Generation.create
    let reportEvalsFileName = "reportEvalsReport"
    let reportEvalsScriptFileName = "reportEvalsReportScript"
    let evalScriptComponent = ("sorterSetEvalParent" |> WsComponentName.create)

    let writeReportEvalsScript (seqSplicer:(int*int) option) = 
            KlinkScript.createReportEvalsScriptFromRunCfgPlex 
                reportGenMin
                totalGenerationCount
                evalScriptComponent
                continueReportFilter
                reportEvalsFileName
                seqSplicer
                runCfgPlex
            
            |> ScriptFileMake.writeScript baseDir projectFolder


    let reportBinsFileName = "reportBinsReport"
    let reportBinsScriptFileName = "reportBinsReportScript"

    let writeReportBinsScript (seqSplicer:(int*int) option) = 
            KlinkScript.createReportBinsScriptFromRunCfgPlex 
                reportGenMin
                initGenerationCount
                reportBinsFileName
                seqSplicer
                runCfgPlex
            
            |> ScriptFileMake.writeScript baseDir projectFolder