namespace global
open System

module O16_StageRflCfg =

    open CommonParams

    let runCfgPlex =
        {
            shcCfgPlex.orders = [|16 |> Order.createNr |]
            mutationRates = [|mr0;mr2;mr4|];
            noiseFractions = [|nf0;nf2;nf4;nf6|];
            rngGens = rndGens 0 4 ;
            tupSorterSetSizes = [|ssz7_8|];
            sorterSetPruneMethods = [|sspm1; sspm2|];
            stageWeights = [|sw0; sw1|];
            switchGenModes = [|switchGenMode.StageSymmetric|];
        } |> runCfgPlex.Shc

    let baseGenerationCount = 500 |> Generation.create
    let baseReportFilter = CommonParams.modulusFilter 5
    let initScriptName = "nf"
    //let initScriptSet_nf = 
    //        InitScriptSet.make 
    //            initScriptName
    //            baseGenerationCount 
    //            baseReportFilter 
    //            runCfgPlex

    
    let baseDir = $"c:\Klink"
    let projectFolder  = $"o16\StageRfl"
    let itemSplicer = Some (1,2)

    let writeInitScripts (maxRunsPerScript:int) = 
            KlinkScript.createInitRunScriptsFromRunCfgPlex 
                baseGenerationCount
                baseReportFilter
                initScriptName
                maxRunsPerScript
                itemSplicer
                runCfgPlex
            |> Array.map(ScriptFileMake.writeScript baseDir projectFolder)


    let reportGenMin = 0 |> Generation.create
    let reportFileName = "reportBinsReport"
    let scriptFileName = "reportBinsReportScript"
    let evalScriptComponent = ("sorterSetEvalParent" |> WsComponentName.create)

    let reportAllScripts (maxRunsPerScript:int) = 
            KlinkScript.createReportEvalsScriptFromRunCfgPlex 
                reportGenMin
                baseGenerationCount
                evalScriptComponent
                baseReportFilter
                reportFileName