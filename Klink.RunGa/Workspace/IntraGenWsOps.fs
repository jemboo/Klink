namespace global
open System


module IntraGenWsOps = 

    let setupWorkspace
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSpeedBinSet:wsComponentName)       
            (wsParams:workspaceParams)
            (fs:IWorkspaceStore)
            (logger: string -> unit)
         =
            result {
                let emptyWsCfg = History.Empty
                let! (wsCfg, wsParams) = 
                    CauseSets.addInitShcCauses
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetEvalParent
                        wnSorterSpeedBinSet
                        wsParams
                        emptyWsCfg

                let! wsGenZero =
                        wsCfg |> History.runWorkspaceCfg fs logger

                let! curGenNumber = 
                            wsParams
                                |> WorkspaceParamsAttrs.getGeneration GaWsParamKeys.generation_current
                                |> Result.map(Generation.value)


                let! res = fs.SaveWorkSpace wsGenZero (fun _ -> true)
                logger ($"Saved Gen {curGenNumber} to {wsGenZero |> Workspace.getId |> WorkspaceId.value}")
                return (wsCfg, wsParams), wsGenZero
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
            (wnSorterSpeedBinSet:wsComponentName) 
            (fs:IWorkspaceStore)
            (logger: string -> unit)            
            (wsParams:workspaceParams)
            (ws:workspace)
         =
            result {
                
                let baseWsCfg = History.Empty

                let! workspaceCfgPrune = 
                     CauseSets.addMutantsAndPruneCauses
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
                        wsParams
                        baseWsCfg

                let! wsParamsNextGen = 
                        wsParams |> WorkspaceParamsAttrs.incrGeneration GaWsParamKeys.generation_current
                                 |> Result.bind(WorkspaceParamsAttrs.updateRngGen GaWsParamKeys.rngGenMutate)
                                 |> Result.bind(WorkspaceParamsAttrs.updateRngGen GaWsParamKeys.rngGenPrune)

                let! nextGenCfg = 
                     CauseSets.addNextGenCauses
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetPruned
                        wnSorterSetEvalParent
                        wnSorterSetEvalPruned
                        wnParentMap
                        wnSorterSpeedBinSet
                        wsParamsNextGen
                        workspaceCfgPrune

                let! wsNextGen =
                        ws
                        |> History.runWorkspaceCfgOnWorkspace
                                logger
                                (nextGenCfg.causes) 

                let! nextGen = 
                        wsParamsNextGen 
                        |> WorkspaceParamsAttrs.getGeneration GaWsParamKeys.generation_current

                let! gfLong = wsParamsNextGen |> WorkspaceParamsAttrs.getGenerationFilter GaWsParamKeys.generation_filter_long
                let! gfShort = wsParamsNextGen |> WorkspaceParamsAttrs.getGenerationFilter GaWsParamKeys.generation_filter_short


                if (gfLong |> GenerationFilter.passing nextGen) then
                    let! res = fs.SaveWorkSpace wsNextGen (fun _ -> true)
                    logger ($"Saved Gen {nextGen |> Generation.value} to { wsNextGen |> Workspace.getId |> WorkspaceId.value}")
                    let causeAddSorterSpeedBinSet =
                        new causeAddSorterSpeedBinSet(wsParams, wnSorterSpeedBinSet)
                    let! resetWs =
                            wsNextGen
                            |> History.runWorkspaceCfgOnWorkspace
                                    logger
                                    [causeAddSorterSpeedBinSet]

                    return resetWs, wsParamsNextGen


                else if (gfShort |> GenerationFilter.passing nextGen) then
                    let! res = fs.SaveWorkSpace wsNextGen (fun wsc -> wsc <> wnSorterSetParent)
                    logger ($"Saved Gen {nextGen |> Generation.value} to { wsNextGen |> Workspace.getId |> WorkspaceId.value}")
                    let causeAddSorterSpeedBinSet =
                        new causeAddSorterSpeedBinSet(wsParams, wnSorterSpeedBinSet)
                    let! resetWs =
                        wsNextGen
                        |> History.runWorkspaceCfgOnWorkspace
                                logger
                                [causeAddSorterSpeedBinSet]

                    return resetWs, wsParamsNextGen



                else
                    return wsNextGen, wsParamsNextGen
             }