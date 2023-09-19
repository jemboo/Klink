namespace global
open System
open System.IO


type runCfgSet = { setName:string; 
                   runCfgs:runCfg[]    }


module RunCfgSet =

    let procRunCfgSet 
            (projectFolderPath:string)
            (up:useParallel)
            (rcs:runCfgSet)
        =
        result {
            let! yab = rcs.runCfgs
                      |> Array.map(RunCfg.procRunCfg projectFolderPath up)
                      |> Array.toList
                      |> Result.sequence
            return ()
        }



    let initRunFromPlex
            (newGenerations:generation)
            (reportFilter:generationFilter)
            (runSetName:string)
            (plex:shcCfgPlex)
        =
        let runCfgs = 
            plex |>
            ShcCfgPlex.toShcInitRunCfg (Some newGenerations) (Some reportFilter)
                  |> Seq.map(shcRunCfg.InitRun >> runCfg.Shc)
                  |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}


    let continueRunFromPlex 
            (newGenerations:generation)
            (runSetName:string)
            (plex:shcCfgPlex)
        =
        let runCfgs = 
            plex |>
            ShcCfgPlex.toShcContinueRunCfg newGenerations
                  |> Seq.map(shcRunCfg.Continue >> runCfg.Shc)
                  |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}


    let reportAllFromPlex 
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (runSetName:string)
            (reportFileName:string)
            (plex:shcCfgPlex)
        =
        let runCfg = 
            plex |>
            ShcCfgPlex.toShcReportEvalsCfg 
                    genMin
                    genMax 
                    evalCompName 
                    reportFilter 
                    reportFileName
                  |> shcReportCfg.Evals
                  |> shcRunCfg.Report
                  |> runCfg.Shc

        {setName = runSetName; runCfgs = [|runCfg|]}


    let reportAllFromPlexSeq 
            (genMin:generation)
            (genMax:generation)
            (evalCompName:wsComponentName)
            (reportFilter:generationFilter)
            (runSetName:string)
            (reportFileName:string)
            (plexes:shcCfgPlex seq)
        =
        let runCfgs = 
            plexes 
            |> Seq.map(
                ShcCfgPlex.toShcReportEvalsCfg 
                        genMin
                        genMax
                        evalCompName
                        reportFilter
                        reportFileName)
            |> Seq.map(shcReportCfg.Evals >> shcRunCfg.Report >> runCfg.Shc)
            |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}


    let reportBinsFromPlex 
            (genMin:generation)
            (genMax:generation)
            (runSetName:string)
            (reportFileName:string)
            (plex:shcCfgPlex)
        =
        let runCfg = 
            plex |>
            ShcCfgPlex.toShcReportBinsCfg 
                    genMin
                    genMax
                    reportFileName
                  |> shcReportCfg.Bins
                  |> shcRunCfg.Report
                  |> runCfg.Shc

        {setName = runSetName; runCfgs = [|runCfg|]}



    let reportBinsFromPlexSeq
            (genMin:generation)
            (genMax:generation)
            (runSetName:string)
            (reportFileName:string)
            (plexes:shcCfgPlex seq)
        =
        let runCfgs = 
            plexes |> Seq.map(
            ShcCfgPlex.toShcReportBinsCfg 
                    genMin
                    genMax
                    reportFileName)
            |> Seq.map(shcReportCfg.Bins >> shcRunCfg.Report  >> runCfg.Shc)
            |> Seq.toArray

        {setName = runSetName; runCfgs = runCfgs}