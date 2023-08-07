namespace global
open System


module WsOpsLib = 


    let genZero
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)           
            (wsParams:workspaceParams)
            (fs:WorkspaceFileStore)
            (logger: string -> unit)
         =
            result {
                let emptyWsCfg = WorkspaceCfg.Empty
                let! wsCfgNParams = 
                    WsCfgLib.initParentMapAndEval
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetEvalParent
                        wsParams
                        emptyWsCfg

                let! wsGenZero = 
                        wsCfgNParams |> fst |> WorkspaceCfg.runWorkspaceCfg fs logger

                let! res = fs.saveWorkSpace wsGenZero
                logger ($"Saved Gen 0 to {wsGenZero |> Workspace.getId |> WorkspaceId.value}")
                return wsCfgNParams, wsGenZero
             }




    let doGen
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetMutator:wsComponentName)
            (wnSorterSetMutated:wsComponentName)
            (wnSorterSetPruned:wsComponentName)
            (wnParentMap:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSetEvalMutated:wsComponentName)
            (wnSorterSetEvalPruned:wsComponentName)
            (fs:WorkspaceFileStore)
            (logger: string -> unit)            
            (wsParams:workspaceParams)
            (workspaceCfg:workspaceCfg)
         =
            result {
                let! workspaceCfgPrune = 
                     WsCfgLib.makeMutantsAndPrune
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetMutator
                        wnSorterSetMutated
                        wnSorterSetPruned
                        wnParentMap
                        wnSorterSetEvalParent
                        wnSorterSetEvalMutated
                        wnSorterSetEvalPruned
                        wsParams
                        workspaceCfg

                let! wsParamsNextGen = 
                        wsParams |> WorkspaceParams.incrGeneration "generation"
                                 |> Result.bind(WorkspaceParams.updateRngGen "rngGenMutate")
                                 |> Result.bind(WorkspaceParams.updateRngGen "rngGenPrune")

                let! workspaceCfgNextGen = 
                     WsCfgLib.assignToNextGen
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetPruned
                        wnSorterSetEvalParent
                        wnSorterSetEvalPruned
                        wnParentMap
                        wsParamsNextGen
                        workspaceCfgPrune

                let! wsNextGen = 
                        workspaceCfgNextGen
                            |> WorkspaceCfg.runWorkspaceCfg fs logger

                let! res = fs.saveWorkSpace wsNextGen

                let! nextGenNumber = 
                            wsParamsNextGen 
                                |> WorkspaceParams.getGeneration "generation" 
                                |> Result.map(Generation.value)

                logger ($"Saved Gen {nextGenNumber} to { wsNextGen |> Workspace.getId |> WorkspaceId.value}")


                let aggregatedCause = new causeLoadWorkspace (wsNextGen |> Workspace.getId)
                let truncWsCfg = aggregatedCause.makeTruncatedWorkspaceCfg()

                return truncWsCfg, wsParamsNextGen
             }


    let doGenOnWorkspace
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetMutator:wsComponentName)
            (wnSorterSetMutated:wsComponentName)
            (wnSorterSetPruned:wsComponentName)
            (wnParentMap:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSetEvalMutated:wsComponentName)
            (wnSorterSetEvalPruned:wsComponentName)
            (fs:WorkspaceFileStore)
            (logger: string -> unit)            
            (wsParams:workspaceParams)
            (ws:workspace)
         =
            result {
                
                let baseWsCfg = WorkspaceCfg.Empty

                let! workspaceCfgPrune = 
                     WsCfgLib.makeMutantsAndPrune
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetMutator
                        wnSorterSetMutated
                        wnSorterSetPruned
                        wnParentMap
                        wnSorterSetEvalParent
                        wnSorterSetEvalMutated
                        wnSorterSetEvalPruned
                        wsParams
                        baseWsCfg

                let! wsParamsNextGen = 
                        wsParams |> WorkspaceParams.incrGeneration "generation"
                                 |> Result.bind(WorkspaceParams.updateRngGen "rngGenMutate")
                                 |> Result.bind(WorkspaceParams.updateRngGen "rngGenPrune")

                let! nextGenCfg = 
                     WsCfgLib.assignToNextGen
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetPruned
                        wnSorterSetEvalParent
                        wnSorterSetEvalPruned
                        wnParentMap
                        wsParamsNextGen
                        workspaceCfgPrune

                let! wsNextGen =
                        ws
                        |> WorkspaceCfg.runWorkspaceCfgOnWorkspace
                                logger
                                (nextGenCfg.history) 



                let! nextGenNumber = 
                            wsParams 
                                |> WorkspaceParams.getGeneration "generation" 
                                |> Result.map(Generation.value)

                if (IntSeries.expoB 100.0 nextGenNumber) then
                    let! res = fs.saveWorkSpace wsNextGen
                    logger ($"Saved Gen {nextGenNumber} to { wsNextGen |> Workspace.getId |> WorkspaceId.value}")

                return wsNextGen, wsParamsNextGen
             }