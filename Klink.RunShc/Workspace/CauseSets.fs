namespace global
open System


module CauseSets = 

    let addInitShcCauses
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSpeedBinSet:wsComponentName)
            (wnSorterSetAncestry:wsComponentName)
            (wsParams:workspaceParams)
            (history:history)
            =
            result {
                let! rngGenCreate = wsParams |> WorkspaceParamsAttrs.getRngGen ShcWsParamKeys.rngGenCreate
                let! order = wsParams |> WorkspaceParamsAttrs.getOrder ShcWsParamKeys.order
                let! sorterCount = wsParams |> WorkspaceParamsAttrs.getSorterCount ShcWsParamKeys.sorterCount
                let! switchGenMode = wsParams |> WorkspaceParamsAttrs.getSwitchGenMode ShcWsParamKeys.switchGenMode
                let! switchCount = wsParams |> WorkspaceParamsAttrs.getSwitchCount ShcWsParamKeys.sorterLength
                let! sorterEvalMode = wsParams |> WorkspaceParamsAttrs.getSorterEvalMode ShcWsParamKeys.sorterEvalMode
                let! useParallel = wsParams |> WorkspaceParamsAttrs.getUseParallel ShcWsParamKeys.useParallel
                let! sortableSetCfgType = wsParams |> WorkspaceParamsAttrs.getSortableSetCfgType ShcWsParamKeys.sortableSetCfgType

                let sortableSetCfg = 
                        SortableSetCfg.make
                            sortableSetCfgType
                            order
                            None

                let wsParamsWithSortableSet = 
                    wsParams |> WorkspaceParamsAttrs.setSortableSetId ShcWsParamKeys.sortableSetId
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

                let causeAddSorterSpeedBinSet =
                    new causeAddSorterSpeedBinSet(wnSorterSpeedBinSet, wsParams)


                let causeAddSorterSetAncestry =
                    new causeAddSorterSetAncestry(wnSorterSetAncestry, wsParams, wnSorterSetEvalParent)


                return
                   ( history 
                        |> History.addCauses 
                            [
                                causeAddWorkspaceParams;
                                causeAddSortableSet; 
                                causeAddSorterSetRnd;
                                causeAddSorterSpeedBinSet;
                                causeMakeSorterSetEvalParent;
                                causeAddSorterSetAncestry;
                            ],
                     wsParamsWithSortableSet )
            }


    let addMutantsAndPruneCauses
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
            (wsParams:workspaceParams)
            (history:history)
         =
            result {
                let! mutationRate = wsParams |> WorkspaceParamsAttrs.getMutationRate ShcWsParamKeys.mutationRate
                let! noiseFraction = wsParams |> WorkspaceParamsAttrs.getNoiseFraction ShcWsParamKeys.noiseFraction
                let! rngGenMutate = wsParams |> WorkspaceParamsAttrs.getRngGen ShcWsParamKeys.rngGenMutate
                let! rngGenPrune = wsParams |> WorkspaceParamsAttrs.getRngGen ShcWsParamKeys.rngGenPrune
                let! order = wsParams |> WorkspaceParamsAttrs.getOrder ShcWsParamKeys.order
                let! sorterCount = wsParams |> WorkspaceParamsAttrs.getSorterCount ShcWsParamKeys.sorterCount
                let! sorterCountMutated = wsParams |> WorkspaceParamsAttrs.getSorterCount ShcWsParamKeys.sorterCountMutated
                let! sorterSetPruneMethod = wsParams |> WorkspaceParamsAttrs.getSorterSetPruneMethod ShcWsParamKeys.sorterSetPruneMethod
                let! stageWeight = wsParams |> WorkspaceParamsAttrs.getStageWeight ShcWsParamKeys.stageWeight
                let! sorterEvalMode = wsParams |> WorkspaceParamsAttrs.getSorterEvalMode ShcWsParamKeys.sorterEvalMode
                let! switchGenMode = wsParams |> WorkspaceParamsAttrs.getSwitchGenMode ShcWsParamKeys.switchGenMode
                let! generation = wsParams |> WorkspaceParamsAttrs.getGeneration ShcWsParamKeys.generation_current

                let! useParallel = wsParams |> WorkspaceParamsAttrs.getUseParallel ShcWsParamKeys.useParallel

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


                let causeUpdateSorterSpeedBinSetParent = 
                    new causeUpdateSorterSpeedBinSet(
                        wnSorterSpeedBinSet,
                        wnSorterSetEvalParent,
                        order,
                        wnSorterSetParent |> WsComponentName.value |> SorterSpeedBinType.create,
                        generation
                    ) :> ICause |> Ok


                let causeUpdateSorterSpeedBinSetMutant = 
                    new causeUpdateSorterSpeedBinSet(
                        wnSorterSpeedBinSet,
                        wnSorterSetEvalMutated,
                        order,
                        wnSorterSetMutated |> WsComponentName.value |> SorterSpeedBinType.create,
                        generation
                    ) :> ICause |> Ok



                let causePruneSorterSets = 
                    new causePruneSorterSets(
                            wnSorterSetParent,
                            wnSorterSetMutated,
                            wnSorterSetEvalParent,
                            wnSorterSetEvalMutated,
                            wnSorterSetPruned,
                            wnSorterSetEvalPruned,
                            wnParentMap,
                            rngGenPrune,
                            sorterCount,
                            noiseFraction,
                            stageWeight,
                            sorterSetPruneMethod
                            ) :> ICause |> Ok


                let causeUpdateSorterSetAncestry =
                    new causeUpdateSorterSetAncestry(
                        wnSorterSetAncestry,
                        wnSorterSetEvalPruned,
                        wnParentMap,
                        wsParams
                    ) :> ICause |> Ok


                let! causeList = 
                            [
                                causeAddSorterSetMutator;
                                causeMutateSorterSet;
                                causeMakeSorterSetEvalMutated;
                                causeUpdateSorterSpeedBinSetParent;
                                causeUpdateSorterSpeedBinSetMutant;
                                causePruneSorterSets;
                                causeUpdateSorterSetAncestry
                            ] |> Result.sequence

                return
                    history
                    |> History.addCauses causeList

            }



    let addNextGenCauses
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetPruned:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSetEvalPruned:wsComponentName)
            (wnParentMap:wsComponentName)
            (wnSorterSpeedBinSet:wsComponentName)
            (wnSorterSetAncestry:wsComponentName)
            (wsParams:workspaceParams)
            (history:history)
            =
            result {
                let causeSetupForNextGen = 
                    new causeSetupForNextGen(
                            wnSortableSet,
                            wnSorterSetParent,
                            wnSorterSetPruned,
                            wnSorterSetEvalParent,
                            wnSorterSetEvalPruned,
                            wnParentMap,
                            wnSorterSpeedBinSet,
                            wnSorterSetAncestry,
                            wsParams
                            )

                return
                    history 
                    |> History.addCauses 
                        [
                            causeSetupForNextGen;
                        ]
            }
