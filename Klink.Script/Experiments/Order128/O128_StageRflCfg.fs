namespace global
open System

module O128_StageCfg =

    open CommonParams
    
    let baseDir = $"c:\Klink"
    let projectFolder  = $"o128\Stage" |> ProjectFolder.create

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
            mutationRates = [|mr1; mr2; mr3;|]
            noiseFractions = [|nf1; nf2; nf3|];
            rngGens = rndGens 0 4 ;
            tupSorterSetSizes = [|ssz4_5|];
            sorterSetPruneMethods = [|sspm1;|];
            stageWeights = [|sw0; sw1|];
            switchGenModes = [|switchGenMode.stage|];
            projectFolder = projectFolder
        } |> runCfgPlex.Shc


    let baseGenerationCount = 5000 |> Generation.create
    let baseReportFilter = CommonParams.modulusFilter 10
    let fullReportFilter = CommonParams.modulusFilter 10
    let initScriptName = "initScript"


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