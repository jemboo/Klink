﻿namespace global
open System


module IntraGenWsOps = 

    let setupWorkspace
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSpeedBinSet:wsComponentName)       
            (wsParams:workspaceParams)
            (fs:WorkspaceFileStore)
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
                                |> WorkspaceParamsAttrs.getGeneration ShcWsParamKeys.generation_current
                                |> Result.map(Generation.value)


                let! res = fs.saveWorkSpace wsGenZero
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
            (fs:WorkspaceFileStore)
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
                        wsParams |> WorkspaceParamsAttrs.incrGeneration ShcWsParamKeys.generation_current
                                 |> Result.bind(WorkspaceParamsAttrs.updateRngGen ShcWsParamKeys.rngGenMutate)
                                 |> Result.bind(WorkspaceParamsAttrs.updateRngGen ShcWsParamKeys.rngGenPrune)

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
                        |> WorkspaceParamsAttrs.getGeneration ShcWsParamKeys.generation_current

                let! gf = wsParamsNextGen |> WorkspaceParamsAttrs.getGenerationFilter ShcWsParamKeys.generation_filter
                if (gf |> GenerationFilter.passing nextGen) then
                    let! res = fs.saveWorkSpace wsNextGen
                    logger ($"Saved Gen {nextGen |> Generation.value} to { wsNextGen |> Workspace.getId |> WorkspaceId.value}")
                    let causeAddSorterSpeedBinSet =
                        new causeAddSorterSpeedBinSet(wnSorterSpeedBinSet, wsParams)

                    let causeAddSorterSetAncestry =
                        new causeAddSorterSetAncestry(wnSorterSetAncestry, wsParams, wnSorterSetEvalPruned)


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