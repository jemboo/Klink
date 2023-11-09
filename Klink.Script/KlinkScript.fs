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
            name: scriptName;
            items: scriptItem[]
        }


module KlinkScript =

    let getName (ks:klinkScript) = ks.name

    let getScriptItems (ks:klinkScript) = ks.items

    let private _scriptFileName 
                    (prefix:string) 
                    (maxChunk:int) 
                    (dex:int) 
                =
        sprintf "%s_%d" prefix (maxChunk * dex)


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
            (scriptFileNamePfx:string)
            (maxRunCountPerScript:int)
            (seqSplicer: (int*int) option)
            (plex:runCfgPlex)
        =
        let runCfgs = 
            RunCfgPlex.toInitRunCfgs
                generations
                reportFilter
                fullReportFilter
                seqSplicer
                plex
            |> Seq.chunkBySize maxRunCountPerScript
            |> Seq.toArray


        runCfgs |> Array.mapi(
            fun dex cfgs -> 
                    { 
                        klinkScript.name = 
                            _scriptFileName scriptFileNamePfx maxRunCountPerScript dex
                            |> ScriptName.create
                        items = cfgs |> Array.map(scriptItem.Run)
                    }
                )


    let createContinueRunScriptsFromRunCfgPlex
            (newGenerations:generation)
            (reportGenFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (scriptFileNamePfx:string)
            (maxRunCountPerScript:int)
            (seqSplicer: (int*int) option)
            (plex:runCfgPlex)
        =
        let runCfgs = 
            RunCfgPlex.toContinueRunCfgs
                newGenerations
                reportGenFilter
                fullReportFilter
                seqSplicer
                plex
            |> Seq.chunkBySize maxRunCountPerScript
            |> Seq.toArray


        runCfgs |> Array.mapi(
            fun dex cfgs -> 
                    { 
                        klinkScript.name = 
                            _scriptFileName scriptFileNamePfx maxRunCountPerScript dex
                            |> ScriptName.create
                        items = cfgs |> Array.map(scriptItem.Run)
                    }
                )



    let createReportEvalsScriptFromRunCfgPlex
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (reportFileName:string)
            (seqSplicer: (int*int) option)
            (plex:runCfgPlex)
        =
        let scriptItem = 
            match plex with
            | runCfgPlex.Shc shcCfgPlex ->
                let runIds = shcCfgPlex |>  ShcCfgPlex.toRunIds seqSplicer |> Seq.toArray
                {
                    shcReportEvalsCfg.reportFileName = reportFileName
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                    evalCompName = evalCompName
                    reportFilter = reportFilter
                } |> shcReportCfg.Evals |> reportCfg.Shc |> scriptItem.Report

            | runCfgPlex.Ga gaCfgPlex ->
                let runIds = gaCfgPlex |> GaCfgPlex.toRunIds seqSplicer |> Seq.toArray
                {
                    gaReportEvalsCfg.reportFileName = reportFileName
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                    evalCompName = evalCompName
                    reportFilter = reportFilter
                } |> gaReportCfg.Evals |> reportCfg.Ga  |> scriptItem.Report
        {
            klinkScript.name = reportFileName |> ScriptName.create
            items = [| scriptItem |]
        }



    let createReportBinsScriptFromRunCfgPlex
            (genMin:generation)
            (genMax:generation)
            (reportFileName:string)
            (seqSplicer: (int*int) option)
            (plex:runCfgPlex)
        =
        let scriptItem = 
            match plex with
            | runCfgPlex.Shc shcCfgPlex ->
                let runIds = shcCfgPlex |> ShcCfgPlex.toRunIds seqSplicer |> Seq.toArray
                {
                    shcReportBinsCfg.reportFileName = reportFileName
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                } |> shcReportCfg.Bins |> reportCfg.Shc |> scriptItem.Report

            | runCfgPlex.Ga gaCfgPlex ->
                let runIds = gaCfgPlex |> GaCfgPlex.toRunIds seqSplicer |> Seq.toArray
                {
                    gaReportBinsCfg.reportFileName = reportFileName
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                } |> gaReportCfg.Bins |> reportCfg.Ga  |> scriptItem.Report
        {
            klinkScript.name = reportFileName |> ScriptName.create
            items = [| scriptItem |]
        }