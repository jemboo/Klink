namespace global
open System
open System.IO


type runCfg =
    | Shc of shcRunCfg
    | Ga of gaRunCfg


module RunCfg =

    let procRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (workspaceFileStoreF: string -> IWorkspaceStore)
            (runCfg:runCfg)
        =
            match runCfg with
            | Shc shc -> 
                    shc |> ShcRunCfg.procShcRunCfg projectFolderPath up workspaceFileStoreF

            | Ga ga -> 
                    ga |> GaRunCfg.procGaRunCfg projectFolderPath up workspaceFileStoreF