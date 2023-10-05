namespace global
open System
open System.IO
open ShcReportEvalsCfg


type reportCfg =
    | Shc of shcReportCfg
    | Ga of gaReportCfg


module ReportCfg =

    let procRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (reportCfg:reportCfg)
        =
            match reportCfg with
            | Shc shc -> 
                    shc |> ShcReportCfg.procRunCfg projectFolderPath up workspaceFileStoreF

            | Ga ga -> 
                    ga |> GaReportCfg.procRunCfg projectFolderPath up workspaceFileStoreF