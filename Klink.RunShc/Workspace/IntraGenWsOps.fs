namespace global
open System


module IntraGenWsOps = 

    let setupWorkspace
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSpeedBinSet:wsComponentName)    
            (wnSorterSetAncestry:wsComponentName)   
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
                        wnSorterSetAncestry
                        wsParams
                        emptyWsCfg

                let! wsGenZero =
                        wsCfg |> History.runWorkspaceCfg fs logger

                let! curGenNumber = 
                            wsParams
                                |> WorkspaceParamsAttrs.getGeneration ShcWsParamKeys.generation_current
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
            (wnSorterSetAncestry:wsComponentName)       
            (fs:IWorkspaceStore)
            (logger: string -> unit)            
            (wsParams:workspaceParams)
            (ws:workspace)
         =
            result {
                
                let baseWsHistory = History.Empty

                let! wsHistoryWithPrune = 
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
                        wnSorterSetAncestry
                        wsParams
                        baseWsHistory

                let! wsParamsNextGen = 
                        wsParams |> WorkspaceParamsAttrs.incrGeneration ShcWsParamKeys.generation_current
                                 |> Result.bind(WorkspaceParamsAttrs.updateRngGen ShcWsParamKeys.rngGenMutate)
                                 |> Result.bind(WorkspaceParamsAttrs.updateRngGen ShcWsParamKeys.rngGenPrune)

                let! wsHistoryNextGen = 
                     CauseSets.addNextGenCauses
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetPruned
                        wnSorterSetEvalParent
                        wnSorterSetEvalPruned
                        wnParentMap
                        wnSorterSpeedBinSet
                        wnSorterSetAncestry
                        wsParamsNextGen
                        wsHistoryWithPrune

                let! wsNextGen =
                        ws
                        |> History.runWorkspaceCfgOnWorkspace
                                logger
                                (wsHistoryNextGen.causes) 

                let! nextGen = 
                        wsParamsNextGen 
                        |> WorkspaceParamsAttrs.getGeneration ShcWsParamKeys.generation_current

                let! gfLong = wsParamsNextGen |> WorkspaceParamsAttrs.getGenerationFilter ShcWsParamKeys.generation_filter_long
                let! gfShort = wsParamsNextGen |> WorkspaceParamsAttrs.getGenerationFilter ShcWsParamKeys.generation_filter_short


                if (gfLong |> GenerationFilter.passing nextGen) then
                    let! res = fs.SaveWorkSpace wsNextGen (fun wsc -> true)
                    logger ($"Saved Gen {nextGen |> Generation.value} to { wsNextGen |> Workspace.getId |> WorkspaceId.value}")
                    let causeAddSorterSpeedBinSet =
                        new causeAddSorterSpeedBinSet(wnSorterSpeedBinSet, wsParams)

                    let causeAddSorterSetAncestry =
                        new causeAddSorterSetAncestry(wnSorterSetAncestry, wsParams, wnSorterSetEvalParent)


                    let! resetWs =
                        wsNextGen
                        |> History.runWorkspaceCfgOnWorkspace
                                logger
                                [
                                    causeAddSorterSpeedBinSet;
                                    causeAddSorterSetAncestry
                                ]

                    return resetWs, wsParamsNextGen



                else if (gfShort |> GenerationFilter.passing nextGen) then
                    let! res = fs.SaveWorkSpace wsNextGen (fun wsc -> (wsc = wnSorterSpeedBinSet) || (wsc = wnSorterSetEvalParent))
                    logger ($"Saved Gen {nextGen |> Generation.value} to { wsNextGen |> Workspace.getId |> WorkspaceId.value}")
                    let causeAddSorterSpeedBinSet =
                        new causeAddSorterSpeedBinSet(wnSorterSpeedBinSet, wsParams)

                    let causeAddSorterSetAncestry =
                        new causeAddSorterSetAncestry(wnSorterSetAncestry, wsParams, wnSorterSetEvalParent)


                    let! resetWs =
                        wsNextGen
                        |> History.runWorkspaceCfgOnWorkspace
                                logger
                                [
                                    causeAddSorterSpeedBinSet;
                                    causeAddSorterSetAncestry
                                ]

                    return resetWs, wsParamsNextGen


                else
                    return wsNextGen, wsParamsNextGen
             }