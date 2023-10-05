namespace global
open System
open System.IO


type gaReportCfg =
    | A of int
    | B of int


module GaReportCfg =

    let procRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (gaReportCfg:gaReportCfg)
        =
            match gaReportCfg with
            | A a -> ()

            | B b -> ()