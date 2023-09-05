namespace global
open System



module Exp1Run =

    let wnSortableSet = "sortableSet" |> WsComponentName.create
    let wnSorterSetParent = "sorterSetParent" |> WsComponentName.create
    let wnSorterSetMutator = "sorterSetMutator" |> WsComponentName.create
    let wnSorterSetMutated = "sorterSetMutated" |> WsComponentName.create
    let wnSorterSetPruned = "sorterSetPruned" |> WsComponentName.create
    let wnParentMap = "parentMap" |> WsComponentName.create
    let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
    let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
    let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
    let wnSorterSpeedBinSet = "sorterSpeedBinSet" |> WsComponentName.create
       

    let doRun
            (projectDir:string)
            (wsParamsS: workspaceParams seq)
            (maxGen:generation)
        = 
        result {
            for wsParams in wsParamsS do
                let! runId = wsParams |> WorkspaceParamsAttrs.getRunId "runId"

                let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
                let fs = new WorkspaceFileStore(runDir)

                let! wsCfg_params, _ = 
                        Exp1WsOps.setupWorkspace
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wnSorterSpeedBinSet
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))


                let mutable curGen = 0
                let mutable curCfg = wsCfg_params |> fst
                let mutable curParams = wsCfg_params |> snd

                while curGen < (maxGen |> Generation.value) do
                    let! wsCfgN, wsPramsN = 
                        Exp1WsOps.doGen
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetMutator
                            wnSorterSetMutated
                            wnSorterSetPruned
                            wnParentMap
                            wnSorterSetEvalParent
                            wnSorterSetEvalMutated
                            wnSorterSetEvalPruned
                            wnSorterSpeedBinSet
                            fs
                            (fun s-> Console.WriteLine(s))
                            curParams
                            curCfg

                    curCfg <- wsCfgN
                    curParams <- wsPramsN
                    curGen <- curGen + 1

            return "success"
        }



    let doRunRun
            (projectFolderPath:string)
            (wsParams: workspaceParams)
        = 
        result {

            let! runId = wsParams |> WorkspaceParamsAttrs.getRunId "runId"
            let runDir = IO.Path.Combine(projectFolderPath, runId |> RunId.value |> string)
            let fs = new WorkspaceFileStore(runDir)

            let! wsCfg_params, ws = 
                    Exp1WsOps.setupWorkspace
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetEvalParent
                            wnSorterSpeedBinSet
                            wsParams
                            fs
                            (fun s-> Console.WriteLine(s))
            let! maxGen = 
                    wsParams |> WorkspaceParamsAttrs.getGeneration "generation_max"
                                |> Result.map(Generation.value)


            let! cg = wsCfg_params |> snd |> WorkspaceParamsAttrs.getGeneration "generation_current"
                    
            let mutable curGen = cg |> Generation.value
            let mutable curParams = wsCfg_params |> snd
            let mutable curWorkspace = ws

            while curGen < maxGen do
                let! wsN, wsPramsN =
                    Exp1WsOps.doGenOnWorkspace
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetMutator
                        wnSorterSetMutated
                        wnSorterSetPruned
                        wnParentMap
                        wnSorterSetEvalParent
                        wnSorterSetEvalMutated
                        wnSorterSetEvalPruned
                        wnSorterSpeedBinSet
                        fs
                        (fun s-> Console.WriteLine(s))
                        curParams
                        curWorkspace

                curWorkspace <- wsN
                curParams <- wsPramsN
                curGen <- curGen + 1

            return ()
        }


    let continueUpdating
            (projectDir:string)
            (runId:runId)
            (newGenerations:generation)
        =
        let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
        result {
            Console.WriteLine(runDir)

            let fs = new WorkspaceFileStore(runDir)

            let! workspaceId = fs.getLastWorkspaceId
                
            Console.WriteLine($" wsId: {workspaceId}")

            let! wsLoaded = fs.loadWorkSpace workspaceId
            let! paramsLoaded = wsLoaded 
                                |> Workspace.getComponent ("workspaceParams" |> WsComponentName.create)
                                |> Result.bind(WorkspaceComponent.asWorkspaceParams)
            let! genLoaded = paramsLoaded |> WorkspaceParamsAttrs.getGeneration "generation_current"
                                |> Result.map(Generation.value)

            let mutable curGen = genLoaded
            let mutable curParams = paramsLoaded
            let mutable curWorkspace = wsLoaded


            let maxGen = curGen + (newGenerations |> Generation.value)
            while curGen < maxGen do
                let! wsN, wsPramsN = 
                    Exp1WsOps.doGenOnWorkspace
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetMutator
                        wnSorterSetMutated
                        wnSorterSetPruned
                        wnParentMap
                        wnSorterSetEvalParent
                        wnSorterSetEvalMutated
                        wnSorterSetEvalPruned
                        wnSorterSpeedBinSet
                        fs
                        (fun s-> Console.WriteLine(s))
                        curParams
                        curWorkspace

                curWorkspace <- wsN
                curParams <- wsPramsN
                curGen <- curGen + 1

            return ()
        }



    let procShcRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (runCfg:shcRunCfg)
        =
            match runCfg with
            | InitRun irc -> 
                irc |> ShcInitRunCfgs.toWorkspaceParams up
                    |> doRunRun projectFolderPath
            | Continue crc -> 
                 continueUpdating 
                        projectFolderPath 
                        crc.runId 
                        crc.newGenerations
            | Report rrc -> 
                match rrc with
                | Evals rac ->
                    Exp1Reporting.reportAllEvals 
                            projectFolderPath 
                            rac
                | Bins rbc ->
                    Exp1Reporting.reportAllBins
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
