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

    let private _runName 
                    (prefix:string) 
                    (maxChunk:int) 
                    (dex:int) 
                =
        sprintf "%s_%d" prefix (maxChunk * dex)


    let procScript 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (klinkScript:klinkScript) =

        klinkScript.items 
            |> Array.iter(ScriptItem.procScriptItem projectFolderPath up workspaceFileStoreF)


    let createInitRunScriptsFromRunCfgPlex
            (generations:generation)
            (reportFilter:generationFilter)
            (runSetPfx:string)
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
                            _runName runSetPfx maxRunCountPerScript dex
                            |> ScriptName.create
                        items = cfgs |> Array.map(scriptItem.Run)
                    }
                )





    //let createReportScriptsFromRunCfgPlex
    //        (generations:generation)
    //        (reportFilter:generationFilter)
    //        (runSetPfx:string)
    //        (maxRunCountPerScript:int)
    //        (plex:runCfgPlex)
    //    =
    //    let runCfgs = 
    //        match plex with
    //        | runCfgPlex.Shc shcCfgPlex ->
    //            shcCfgPlex |>
    //            ShcCfgPlex.toContinueRunCfgs generations (Some reportFilter)
    //                  |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)
    //                  |> Seq.chunkBySize maxRunCountPerScript
    //                  |> Seq.toArray
    //        | runCfgPlex.Ga gaCfgPlex ->
    //            gaCfgPlex |>
    //            GaCfgPlex.toInitRunCfgs generations (Some reportFilter)
    //                  |> Seq.map(gaRunCfg.InitRun >> runCfg.Ga)
    //                  |> Seq.chunkBySize maxRunCountPerScript
    //                  |> Seq.toArray


    //    runCfgs |> Array.mapi(
    //        fun dex cfgs -> 
    //                { 
    //                    klinkScript.name = 
    //                        _runName runSetPfx maxRunCountPerScript dex
    //                        |> ScriptName.create
    //                    items = cfgs |> Array.map(scriptItem.Run)
    //                }
    //            )
