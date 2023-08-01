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
                return wsCfgNParams
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
                let! pruneCfg = 
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

                let! nextGenCfg = 
                     WsCfgLib.assignToNextGen
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetPruned
                        wnSorterSetEvalParent
                        wnSorterSetEvalPruned
                        wnParentMap
                        wsParamsNextGen
                        pruneCfg

                let! wsNextGen = 
                        nextGenCfg
                            |> WorkspaceCfg.runWorkspaceCfg fs logger

                let! res = fs.saveWorkSpace wsNextGen

                let! nextGenNumber = 
                            wsParams 
                                |> WorkspaceParams.getGeneration "generation" 
                                |> Result.map(Generation.value)

                logger ($"Saved Gen {nextGenNumber} to { wsNextGen |> Workspace.getId |> WorkspaceId.value}")


                let aggregatedCause = new causeLoadWorkspace (wsNextGen |> Workspace.getId)
                let truncWsCfg = aggregatedCause.makeTruncatedWorkspaceCfg()

                return truncWsCfg, wsParamsNextGen
             }

