namespace global
open System
open System.IO
open ShcReportEvalsCfg


type reportCfg =
    | Shc of shcReportCfg
    | Ga of gaReportCfg


module ReportCfg =

    let procReportCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (reportCfg:reportCfg)
        =
            match reportCfg with
            | Shc shc -> 
                    let res =  shc |> ShcReportCfg.procReportCfg projectFolderPath up workspaceFileStoreF
                    match res with
                    | Ok _ -> Console.WriteLine($"report completed")
                    | Error m -> Console.WriteLine($"report error: {m}")

            | Ga ga -> 
                    let res = ga |> GaReportCfg.procReportCfg projectFolderPath up workspaceFileStoreF
                    match res with
                    | Ok _ -> Console.WriteLine($"report completed")
                    | Error m -> Console.WriteLine($"report error: {m}")