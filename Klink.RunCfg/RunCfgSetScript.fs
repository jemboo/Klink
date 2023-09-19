namespace global
open System
open System.IO


type runCfgSetScript =
    | Shc of shcRunCfg
    | Ga of gaRunCfg


module RunCfgSetScript =

    let procRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (runCfg:runCfg)
        =
           ()
