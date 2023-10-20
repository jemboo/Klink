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
            switchGenModes = [|switchGenMode.stageSymmetric|];
        } |> runCfgPlex.Shc

    let baseGenerationCount = 500 |> Generation.create
    let baseReportFilter = CommonParams.modulusFilter 5
    let initScriptName = "initScript"

    let baseDir = $"c:\Klink"
    let projectFolder  = $"o16\StageRfl"

    let writeInitScripts (maxRunsPerScript:int) = 
            KlinkScript.createInitRunScriptsFromRunCfgPlex 
                baseGenerationCount
                baseReportFilter
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