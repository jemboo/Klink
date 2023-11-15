namespace global
open System


type scriptName = private ScriptName of string
module ScriptName =
    let value (ScriptName v) = v

    let create (value: string) =
        value |> ScriptName



type scriptItem = 
     | Run of runCfg
     | Report of reportCfg


module ScriptItem =

    let procScriptItem
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (scriptItem:scriptItem)
        =
            match scriptItem with
            | Run runCfg -> 
                 runCfg |> RunCfg.procRunCfg projectFolderPath up workspaceFileStoreF

            | Report reportCfg -> 
                 reportCfg |> ReportCfg.procReportCfg projectFolderPath up workspaceFileStoreF



type klinkScript = 
    private 
        {
            scriptName: scriptName;
            projectFolder:projectFolder
            items: scriptItem[]
        }


module KlinkScript =

    let getName (ks:klinkScript) = ks.scriptName

    let getScriptItems (ks:klinkScript) = ks.items

    let private _initScriptFileName 
                    (prefix:cfgPlexName) 
                    (maxChunk:int) 
                    (dex:int) 
                =
        sprintf "i_%s_%d" (prefix |> CfgPlexName.value)  (maxChunk * dex)
        |> ScriptName.create


    let private _continueScriptFileName 
                    (prefix:cfgPlexName) 
                    (newGenerations:generation)
                    (maxChunk:int) 
                    (dex:int) 
                =
        sprintf "c_%s_%d_%d"
                    (prefix |> CfgPlexName.value)
                    (newGenerations |> Generation.value) 
                    (maxChunk * dex)
        |> ScriptName.create



    let private _reportEvalScriptFileName 
                    (prefix:cfgPlexName)
                =
        sprintf "res_%s" (prefix |> CfgPlexName.value)
        |> ScriptName.create


    let private _reportBinsScriptFileName 
                    (prefix:cfgPlexName)
                =
        sprintf "rbs_%s" (prefix |> CfgPlexName.value)
        |> ScriptName.create


    let private _reportEvalFileName 
                    (prefix:cfgPlexName)
                =
        sprintf "re_%s" (prefix |> CfgPlexName.value)
        |> ReportName.create


    let private _reportBinsFileName 
                    (prefix:cfgPlexName)
                =
        sprintf "rb_%s" (prefix |> CfgPlexName.value)
        |> ReportName.create



    let procScript 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (klinkScript:klinkScript) 
        =
        klinkScript.items 
            |> Array.iter(ScriptItem.procScriptItem projectFolderPath up workspaceFileStoreF)


    
    let createInitRunScriptsFromRunCfgPlex
            (generations:generation)
            (reportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (maxRunCountPerScript:int)
            (selectedIndexes: int[] option)
            (plex:runCfgPlex)
        =
        let runCfgs = 
            RunCfgPlex.toInitRunCfgs
                generations
                reportFilter
                fullReportFilter
                selectedIndexes
                plex
            |> Seq.chunkBySize maxRunCountPerScript
            |> Seq.toArray


        runCfgs |> Array.mapi(
            fun dex cfgs -> 
                    { 
                        klinkScript.scriptName = 
                            _initScriptFileName 
                                    (plex |> RunCfgPlex.name) 
                                    maxRunCountPerScript 
                                    dex
                        projectFolder = plex |> RunCfgPlex.projectFolder;
                        items = cfgs |> Array.map(scriptItem.Run)
                    }
                )


    let createContinueRunScriptsFromRunCfgPlex
            (newGenerationCount:generation)
            (freqReportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (maxRunCountPerScript:int)
            (selectedIndexes: int[] option)
            (plex:runCfgPlex)
        =
        let runCfgs = 
            RunCfgPlex.toContinueRunCfgs
                newGenerationCount
                freqReportFilter
                fullReportFilter
                selectedIndexes
                plex
            |> Seq.chunkBySize maxRunCountPerScript
            |> Seq.toArray


        runCfgs |> Array.mapi(
            fun dex cfgs -> 
                    { 
                        klinkScript.scriptName = 
                            _continueScriptFileName 
                                (plex |> RunCfgPlex.name)  
                                newGenerationCount 
                                maxRunCountPerScript
                                dex
                        projectFolder = plex |> RunCfgPlex.projectFolder;
                        items = cfgs |> Array.map(scriptItem.Run)
                    }
                )



    let createReportEvalsScriptFromRunCfgPlex
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (selectedIndexes: int[] option)
            (plex:runCfgPlex)
        =
        let scriptItem = 
            match plex with
            | runCfgPlex.Shc shcCfgPlex ->
                let runIds = shcCfgPlex |>  ShcCfgPlex.toRunIds selectedIndexes |> Seq.toArray
                {
                    shcReportEvalsCfg.reportFileName = 
                            _reportEvalFileName (plex |> RunCfgPlex.name)
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                    evalCompName = evalCompName
                    reportFilter = reportFilter
                } |> shcReportCfg.Evals |> reportCfg.Shc |> scriptItem.Report

            | runCfgPlex.Ga gaCfgPlex ->
                let runIds = gaCfgPlex |> GaCfgPlex.toRunIds selectedIndexes |> Seq.toArray
                {
                    gaReportEvalsCfg.reportFileName = 
                            _reportEvalFileName (plex |> RunCfgPlex.name)
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                    evalCompName = evalCompName
                    reportFilter = reportFilter
                } |> gaReportCfg.Evals |> reportCfg.Ga  |> scriptItem.Report
        {
            klinkScript.scriptName = (plex |> RunCfgPlex.name)  |> _reportEvalScriptFileName
            projectFolder = plex |> RunCfgPlex.projectFolder;
            items = [| scriptItem |]
        }



    let createReportBinsScriptFromRunCfgPlex
            (genMin:generation)
            (genMax:generation)
            (selectedIndexes: int[] option)
            (plex:runCfgPlex)
        =
        let scriptItem = 
            match plex with
            | runCfgPlex.Shc shcCfgPlex ->
                let runIds = shcCfgPlex |> ShcCfgPlex.toRunIds selectedIndexes |> Seq.toArray
                {
                    shcReportBinsCfg.reportFileName = 
                            _reportBinsFileName (plex |> RunCfgPlex.name)
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                } |> shcReportCfg.Bins |> reportCfg.Shc |> scriptItem.Report

            | runCfgPlex.Ga gaCfgPlex ->
                let runIds = gaCfgPlex |> GaCfgPlex.toRunIds selectedIndexes |> Seq.toArray
                {
                    gaReportBinsCfg.reportFileName = 
                            _reportBinsFileName (plex |> RunCfgPlex.name)
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                } |> gaReportCfg.Bins |> reportCfg.Ga  |> scriptItem.Report
        {
            klinkScript.scriptName = (plex |> RunCfgPlex.name)  |> _reportBinsScriptFileName
            projectFolder =  plex |> RunCfgPlex.projectFolder;
            items = [| scriptItem |]
        }