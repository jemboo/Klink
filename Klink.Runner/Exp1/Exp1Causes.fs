namespace global
open System


module Exp1Causes = 

    let initParentMapAndEval
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSpeedBinSet:wsComponentName)
            (wsParams:workspaceParams)
            (workspaceCfg:workspaceCfg)
            =
            result {
                let! rngGenCreate = wsParams |> WorkspaceParamsAttrs.getRngGen "rngGenCreate" 
                let! order = wsParams |> WorkspaceParamsAttrs.getOrder "order"
                let! sorterCount = wsParams |> WorkspaceParamsAttrs.getSorterCount "sorterCount"
                let! switchGenMode = wsParams |> WorkspaceParamsAttrs.getSwitchGenMode "switchGenMode"
                let! switchCount = wsParams |> WorkspaceParamsAttrs.getSwitchCount "sorterLength"
                let! sorterEvalMode =wsParams |> WorkspaceParamsAttrs.getSorterEvalMode "sorterEvalMode"
                let! useParallel = wsParams |> WorkspaceParamsAttrs.getUseParallel "useParallel"
                let! stagesSkipped = wsParams |> WorkspaceParamsAttrs.getStageCount "stagesSkipped"

                let sortableSetCfg = 
                    SortableSetCertainCfg.makeAllBitsReducedOneStage stagesSkipped order
                           |> sortableSetCfg.Certain

                let wsParamsWithSortableSet = 
                    wsParams |> WorkspaceParamsAttrs.setSortableSetId "sortableSet"
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
                            wnSorterSpeedBinSet,
                            useParallel)

                let causeAddSorterSpeedBinSet =
                    new causeAddSorterSpeedBinSet(wsParams, wnSorterSpeedBinSet)

                return
                   ( workspaceCfg 
                        |> WorkspaceCfg.addCauses 
                            [
                                causeAddWorkspaceParams;
                                causeAddSortableSet; 
                                causeAddSorterSetRnd;
                                causeAddSorterSpeedBinSet;
                                causeMakeSorterSetEvalParent;
                            ],
                     wsParamsWithSortableSet )
            }


    let resetSpeedBins
            (wnSorterSpeedBinSet:wsComponentName)
            (wsParams:workspaceParams)
            (workspaceCfg:workspaceCfg)
            =
            result {

                let causeAddSorterSpeedBinSet =
                    new causeAddSorterSpeedBinSet(wsParams, wnSorterSpeedBinSet)

                return
                   workspaceCfg 
                        |> WorkspaceCfg.addCauses 
                            [
                                causeAddSorterSpeedBinSet;
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
            (wnSorterSpeedBinSet:wsComponentName)       
            (wsParams:workspaceParams)
            (workspaceCfg:workspaceCfg)
         =
            result {
                let! mutationRate = wsParams |> WorkspaceParamsAttrs.getMutationRate "mutationRate" 
                let! noiseFraction = wsParams |> WorkspaceParamsAttrs.getNoiseFraction "noiseFraction" 
                let! rngGenMutate = wsParams |> WorkspaceParamsAttrs.getRngGen "rngGenMutate" 
                let! rngGenPrune = wsParams |> WorkspaceParamsAttrs.getRngGen "rngGenPrune"  
                let! order = wsParams |> WorkspaceParamsAttrs.getOrder "order" 
                let! sorterCount = wsParams |> WorkspaceParamsAttrs.getSorterCount "sorterCount" 
                let! sorterCountMutated = wsParams |> WorkspaceParamsAttrs.getSorterCount "sorterCountMutated" 
                let! sorterSetPruneMethod = wsParams |> WorkspaceParamsAttrs.getSorterSetPruneMethod "sorterSetPruneMethod" 
                let! stageWeight = wsParams |> WorkspaceParamsAttrs.getStageWeight "stageWeight" 
                let! sorterEvalMode = wsParams |> WorkspaceParamsAttrs.getSorterEvalMode "sorterEvalMode" 
                let! switchGenMode = wsParams |> WorkspaceParamsAttrs.getSwitchGenMode "switchGenMode" 
                let! useParallel = wsParams |> WorkspaceParamsAttrs.getUseParallel "useParallel" 

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
                            wnSorterSpeedBinSet,
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
            (wnSorterSpeedBinSet:wsComponentName)
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
                            wnSorterSpeedBinSet,
                            wsParams
                            )

                return
                    workspaceCfg 
                    |> WorkspaceCfg.addCauses 
                        [
                            causeSetupForNextGen;
                        ]
            }
