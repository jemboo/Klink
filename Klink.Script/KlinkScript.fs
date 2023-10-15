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
            (scriptFileNamePfx:string)
            (maxRunCountPerScript:int)
            (plex:runCfgPlex)
        =
        let runCfgs = 
            match plex with
            | runCfgPlex.Shc shcCfgPlex ->
                shcCfgPlex |>
                ShcCfgPlex.toInitRunCfgs generations (Some reportFilter)
                      |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)
                      |> Seq.chunkBySize maxRunCountPerScript
                      |> Seq.toArray
            | runCfgPlex.Ga gaCfgPlex ->
                gaCfgPlex |>
                GaCfgPlex.toInitRunCfgs generations (Some reportFilter)
                      |> Seq.map(gaRunCfg.InitRun >> runCfg.Ga)
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
            (nreGenerations:generation)
            (scriptFileNamePfx:string)
            (maxRunCountPerScript:int)
            (plex:runCfgPlex)
        =
        let runCfgs = 
            match plex with
            | runCfgPlex.Shc shcCfgPlex ->
                shcCfgPlex |>
                ShcCfgPlex.toContinueRunCfgs nreGenerations
                      |> Seq.map(shcRunCfg.Continue >> runCfg.Shc)
                      |> Seq.chunkBySize maxRunCountPerScript
                      |> Seq.toArray
            | runCfgPlex.Ga gaCfgPlex ->
                gaCfgPlex |>
                GaCfgPlex.toContinueRunCfgs nreGenerations
                      |> Seq.map(gaRunCfg.Continue >> runCfg.Ga)
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
            (reportFileName:string)
            (scriptFileName:string)
            (plex:runCfgPlex)
        =
        let scriptItem = 
            match plex with
            | runCfgPlex.Shc shcCfgPlex ->
                let runIds = shcCfgPlex |>  ShcCfgPlex.toRunIds |> Seq.toArray
                {
                    shcReportEvalsCfg.reportFileName = reportFileName
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                    evalCompName = evalCompName
                    reportFilter = reportFilter
                } |> shcReportCfg.Evals |> reportCfg.Shc |> scriptItem.Report

            | runCfgPlex.Ga gaCfgPlex ->
                let runIds = gaCfgPlex |> GaCfgPlex.toRunIds |> Seq.toArray
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
            (plex:runCfgPlex)
        =
        let scriptItem = 
            match plex with
            | runCfgPlex.Shc shcCfgPlex ->
                let runIds = shcCfgPlex |> ShcCfgPlex.toRunIds |> Seq.toArray
                {
                    shcReportBinsCfg.reportFileName = reportFileName
                    runIds = runIds
                    genMin = genMin
                    genMax = genMax
                } |> shcReportCfg.Bins |> reportCfg.Shc |> scriptItem.Report

            | runCfgPlex.Ga gaCfgPlex ->
                let runIds = gaCfgPlex |> GaCfgPlex.toRunIds |> Seq.toArray
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