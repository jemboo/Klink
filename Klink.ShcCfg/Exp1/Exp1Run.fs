namespace global
open System



module Exp1Run =


    let procShcRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (runCfg:shcRunCfg)
        =
            match runCfg with
            | InitRun irc -> 
                irc |> ShcInitRunCfgs.toWorkspaceParams up
                    |> InterGenWsOps.doGenLoop projectFolderPath
            | Continue crc -> 
                 InterGenWsOps.continueUpdating 
                        projectFolderPath 
                        crc.runId 
                        crc.newGenerations
            | Report rrc -> 
                match rrc with
                | Evals rac ->
                    ShcReportEvalsCfgs.reportAllEvals 
                            projectFolderPath 
                            rac
                | Bins rbc ->
                    ShcReportBinsCfgs.reportAllBins
                            projectFolderPath 
                            rbc


    let procRunCfgSet 
            (projectFolderPath:string)
            (up:useParallel)
            (rcs:shcRunCfgSet)
        =
        result {
            let! yab = rcs.runCfgs
                      |> Array.map(procShcRunCfg projectFolderPath up)
                      |> Array.toList
                      |> Result.sequence
            return ()
        }
