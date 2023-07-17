namespace global
open System


module WsOpsLibA = 

    let initParentMapAndEval
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wsParams:workspaceParams)
            (workspaceCfg:workspaceCfg)
            =
            result {
                let! rngGenCreate = wsParams |> WorkspaceParams.getRngGen "rngGenCreate" 
                let! order = wsParams |> WorkspaceParams.getOrder "order"
                let! sorterCount = wsParams |> WorkspaceParams.getSorterCount "sorterCount"
                let! switchGenMode = wsParams |> WorkspaceParams.getSwitchGenMode "switchGenMode"
                let! switchCount = wsParams |> WorkspaceParams.getSwitchCount "switchCount"
                let! sorterEvalMode =wsParams |> WorkspaceParams.getSorterEvalMode "sorterEvalMode"
                let! useParallel = wsParams |> WorkspaceParams.getUseParallel "useParallel"

                let ssCfg = sortableSetCertainCfg.All_Bits order
                            |> sortableSetCfg.Certain

                let srCfg = new sorterSetRndCfg(
                                wnSorterSetParent,
                                order,
                                switchGenMode,
                                switchCount,
                                sorterCount)


                let causeAddWorkspaceParams =  
                    new causeAddWorkspaceParams(
                            wsParams)


                let causeAddSortableSet =  
                    new causeAddSortableSet(
                            wnSortableSet, 
                            ssCfg)

                let causeAddSorterSetRnd =  
                    new causeAddSorterSetRnd(
                            wnSorterSetParent,
                            srCfg,
                            rngGenCreate)

                let causeMakeSorterSetEvalParent = 
                    new causeMakeSorterSetEval(
                            wnSortableSet,
                            wnSorterSetParent,
                            sorterEvalMode,
                            wnSorterSetEvalParent,
                            useParallel)


                return
                        workspaceCfg 
                        |> WorkspaceCfg.addCauseCfgs 
                            [
                                causeAddWorkspaceParams;
                                causeAddSortableSet; 
                                causeAddSorterSetRnd;
                                causeMakeSorterSetEvalParent;
                            ]
            }


    let makeMutantsAndPrune
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetMutator:wsComponentName)
            (wnSorterSetMutated:wsComponentName)
            (wnSorterSetPruned:wsComponentName)
            (wnParentMap:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSetEvalMutated:wsComponentName)
            (wnSorterSetEvalPruned:wsComponentName)
            (wnSorterSetPruner:wsComponentName)            
            (wsParams:workspaceParams)
            (workspaceCfg:workspaceCfg)
            =
            result {
                let! mutationRate = wsParams |> WorkspaceParams.getMutationRate "mutationRate" 
                let! noiseFraction = wsParams |> WorkspaceParams.getNoiseFraction "noiseFraction" 
                let! rngGenMutate = wsParams |> WorkspaceParams.getRngGen "rngGenMutate" 
                let! rngGenPrune = wsParams |> WorkspaceParams.getRngGen "rngGenPrune"  
                let! order = wsParams |> WorkspaceParams.getOrder "order" 
                let! sorterCount = wsParams |> WorkspaceParams.getSorterCount "sorterCount" 
                let! sorterCountMutated = wsParams |> WorkspaceParams.getSorterCount "sorterCountMutated" 
                let! stageWeight = wsParams |> WorkspaceParams.getStageWeight "stageWeight" 
                let! sorterEvalMode = wsParams |> WorkspaceParams.getSorterEvalMode "sorterEvalMode" 
                let! switchGenMode = wsParams |> WorkspaceParams.getSwitchGenMode "switchGenMode" 
                let! useParallel = wsParams |> WorkspaceParams.getUseParallel "useParallel" 

                let ssCfg = sortableSetCertainCfg.All_Bits order
                            |> sortableSetCfg.Certain

                let causeAddSortableSet =  
                    new causeAddSortableSet(
                            wnSortableSet, 
                            ssCfg)


                let causeAddSorterSetMutator = 
                    new causeAddSorterSetMutator(
                            wnSorterSetMutator, 
                            order, 
                            switchGenMode,
                            sorterCountMutated, 
                            mutationRate)

                let causeMutateSorterSet = 
                    new causeMutateSorterSet(
                            wnSorterSetParent,
                            wnSorterSetMutated,
                            wnSorterSetMutator,
                            wnParentMap,
                            rngGenMutate
                            )

                let causeMakeSorterSetEvalMutated = 
                    new causeMakeSorterSetEval(
                            wnSortableSet,
                            wnSorterSetMutated,
                            sorterEvalMode,
                            wnSorterSetEvalMutated,
                            useParallel)

                let causePruneSorterSets = 
                    new causePruneSorterSetsWhole(
                            wnSorterSetParent,
                            wnSorterSetMutated,
                            wnSorterSetEvalParent,
                            wnSorterSetEvalMutated,
                            wnSorterSetPruner,
                            wnSorterSetPruned,
                            wnSorterSetEvalPruned,
                            rngGenPrune,
                            sorterCount,
                            noiseFraction,
                            stageWeight
                            )

                return
                        workspaceCfg 
                        |> WorkspaceCfg.addCauseCfgs 
                            [causeAddSortableSet;
                            causeAddSorterSetMutator;
                            causeMutateSorterSet;
                            causeMakeSorterSetEvalMutated;
                            causePruneSorterSets;
                            ]

            }



    let assignToNextGen
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetPruned:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSetEvalPruned:wsComponentName)
            (workspaceCfg:workspaceCfg)
            =

            result {
                let causePruneSorterSetsNextGen = 
                    new causePruneSorterSetsNextGen(
                            wnSorterSetParent,
                            wnSorterSetPruned,
                            wnSorterSetEvalParent,
                            wnSorterSetEvalPruned
                            )

                return
                        workspaceCfg 
                        |> WorkspaceCfg.addCauseCfgs 
                            [
                                causePruneSorterSetsNextGen;
                            ]

            }
