namespace global
open System


module Exp1Causes = 

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
                let! switchCount = wsParams |> WorkspaceParams.getSwitchCount "sorterLength"
                let! sorterEvalMode =wsParams |> WorkspaceParams.getSorterEvalMode "sorterEvalMode"
                let! useParallel = wsParams |> WorkspaceParams.getUseParallel "useParallel"

                let sortableSetCfg = 
                    SortableSetCertainCfg.makeAllBitsReducedOneStage order
                           |> sortableSetCfg.Certain

                let wsParamsWithSortableSet = 
                    wsParams |> WorkspaceParams.setSortableSetId "sortableSet"
                                    (sortableSetCfg |> SortableSetCfg.getId)

                let sorterSetCfg = 
                    new sorterSetRndCfg(
                                wnSorterSetParent,
                                order,
                                switchGenMode,
                                switchCount,
                                sorterCount)


                let causeAddWorkspaceParams =  
                    new causeAddWorkspaceParams(
                            wsParamsWithSortableSet)


                let causeAddSortableSet =  
                    new causeAddSortableSet(
                            wnSortableSet, 
                            sortableSetCfg)


                let causeAddSorterSetRnd =  
                    new causeAddSorterSetRnd(
                            wnSorterSetParent,
                            sorterSetCfg,
                            rngGenCreate)


                let causeMakeSorterSetEvalParent = 
                    new causeMakeSorterSetEval(
                            wnSortableSet,
                            wnSorterSetParent,
                            sorterEvalMode,
                            wnSorterSetEvalParent,
                            useParallel)


                return
                   ( workspaceCfg 
                        |> WorkspaceCfg.addCauses 
                            [
                                causeAddWorkspaceParams;
                                causeAddSortableSet; 
                                causeAddSorterSetRnd;
                                causeMakeSorterSetEvalParent;
                            ],
                     wsParamsWithSortableSet )
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
                let! sorterSetPruneMethod = wsParams |> WorkspaceParams.getSorterSetPruneMethod "sorterSetPruneMethod" 
                let! stageWeight = wsParams |> WorkspaceParams.getStageWeight "stageWeight" 
                let! sorterEvalMode = wsParams |> WorkspaceParams.getSorterEvalMode "sorterEvalMode" 
                let! switchGenMode = wsParams |> WorkspaceParams.getSwitchGenMode "switchGenMode" 
                let! useParallel = wsParams |> WorkspaceParams.getUseParallel "useParallel" 

                let causeAddSorterSetMutator = 
                    new causeAddSorterSetMutator(
                            wnSorterSetMutator, 
                            order, 
                            switchGenMode,
                            sorterCountMutated, 
                            mutationRate) :> ICause |> Ok

                let causeMutateSorterSet = 
                    new causeMutateSorterSet(
                            wnSorterSetParent,
                            wnSorterSetMutated,
                            wnSorterSetMutator,
                            wnParentMap,
                            rngGenMutate
                            ) :> ICause |> Ok

                let causeMakeSorterSetEvalMutated = 
                    new causeMakeSorterSetEval(
                            wnSortableSet,
                            wnSorterSetMutated,
                            sorterEvalMode,
                            wnSorterSetEvalMutated,
                            useParallel) :> ICause |> Ok

                let causePruneSorterSets = 
                    match sorterSetPruneMethod with
                    | sorterSetPruneMethod.Whole ->
                        new causePruneSorterSetsWhole(
                                wnSorterSetParent,
                                wnSorterSetMutated,
                                wnSorterSetEvalParent,
                                wnSorterSetEvalMutated,
                                wnSorterSetPruned,
                                wnSorterSetEvalPruned,
                                rngGenPrune,
                                sorterCount,
                                noiseFraction,
                                stageWeight
                                ) :> ICause |> Ok

                    | sorterSetPruneMethod.Shc ->
                        new causePruneSorterSetsShc(
                                wnSorterSetParent,
                                wnSorterSetMutated,
                                wnSorterSetEvalParent,
                                wnSorterSetEvalMutated,
                                wnSorterSetPruned,
                                wnParentMap,
                                wnSorterSetEvalPruned,
                                rngGenPrune,
                                sorterCount,
                                noiseFraction,
                                stageWeight
                                ) :> ICause |> Ok
                    | _ ->
                        $"sorterSetPruneMethod:{sorterSetPruneMethod} 
                            not handled in makeMutantsAndPrune" |> Error

                let! causeList = 
                            [
                                causeAddSorterSetMutator;
                                causeMutateSorterSet;
                                causeMakeSorterSetEvalMutated;
                                causePruneSorterSets;
                            ] |> Result.sequence

                return
                        workspaceCfg
                        |> WorkspaceCfg.addCauses 
                            causeList

            }



    let assignToNextGen
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetPruned:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSetEvalPruned:wsComponentName)
            (wnParentMap:wsComponentName)
            (wsParams:workspaceParams)
            (workspaceCfg:workspaceCfg)
            =

            result {
                let causeSetupForNextGen = 
                    new setupForNextGen(
                            wnSortableSet,
                            wnSorterSetParent,
                            wnSorterSetPruned,
                            wnSorterSetEvalParent,
                            wnSorterSetEvalPruned,
                            wnParentMap,
                            wsParams
                            )

                return
                        workspaceCfg 
                        |> WorkspaceCfg.addCauses 
                            [
                                causeSetupForNextGen;
                            ]
            }
