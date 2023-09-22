namespace global
open System
open System.IO


type reportScript =
    | Shc of shcRunCfg
    | Ga of gaRunCfg


module ReportScript =

    let procRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (runCfg:runCfg)
        =
           ()
