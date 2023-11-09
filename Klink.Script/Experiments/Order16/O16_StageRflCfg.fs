namespace global
open System

module O16_StageRflCfg =

    open CommonParams

    //let runCfgPlex =
    //    {
    //        shcCfgPlex.orders = [| 16 |> Order.createNr |]
    //        sortableSetCfgs =  [| (sortableSetCfgType.All_Bits_Reduced, 1 |> StageCount.create) |]
    //        mutationRates = [|mr0;mr2;mr4|];
    //        noiseFractions = [|nf0;nf2;nf4;nf6|];
    //        rngGens = rndGens 0 4 ;
    //        tupSorterSetSizes = [|ssz7_8|];
    //        sorterSetPruneMethods = [|sspm1; sspm2|];
    //        stageWeights = [|sw0; sw1|];
    //        switchGenModes = [|switchGenMode.stageSymmetric|];
    //    } |> runCfgPlex.Shc


    let runCfgPlex =
        {
            shcCfgPlex.orders = [| 128 |> Order.createNr |]
            sortableSetCfgs =  
                [| 
                    (   sortableSetCfgType.MergeWithInts, 
                        0 |> StageCount.create, 
                        sorterEvalMode.DontCheckSuccess
                    )
                |]
            mutationRates = [|mr0;mr1;mr2|];
            noiseFractions = [|nf0;nf1;nf2;|];
            rngGens = rndGens 0 2 ;
            tupSorterSetSizes = [|ssz5_6|];
            sorterSetPruneMethodsOld = [|sspm1;|];
            stageWeights = [|sw0; sw1|];
            switchGenModes = [|switchGenMode.stageSymmetric|];
        } |> runCfgPlex.Shc


    let baseGenerationCount = 25000 |> Generation.create
    let baseReportFilter = CommonParams.modulusFilter 25
    let fullReportFilter = CommonParams.modulusFilter 25
    let initScriptName = "initScript"

    let baseDir = $"c:\Klink"
    let projectFolder  = $"o128\StageRfl"

    let writeInitScripts (maxRunsPerScript:int) = 
            KlinkScript.createInitRunScriptsFromRunCfgPlex 
                baseGenerationCount
                baseReportFilter
                fullReportFilter
                initScriptName
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
                baseGenerationCount
                evalScriptComponent
                baseReportFilter
                fullReportFilter
                reportEvalsFileName
                seqSplicer
                runCfgPlex
            
            |> ScriptFileMake.writeScript baseDir projectFolder


    let reportBinsFileName = "reportBinsReport"
    let reportBinsScriptFileName = "reportBinsReportScript"

    let writeReportBinsScript (seqSplicer:(int*int) option) = 
            KlinkScript.createReportBinsScriptFromRunCfgPlex 
                reportGenMin
                baseGenerationCount
                reportBinsFileName
                seqSplicer
                runCfgPlex
            
            |> ScriptFileMake.writeScript baseDir projectFolder