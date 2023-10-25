namespace global
open System


module CauseSets = 

    let addInitShcCauses
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (wnSorterSpeedBinSet:wsComponentName)
            (wsParams:workspaceParams)
            (history:history)
            =
            result {
                let! rngGenCreate = wsParams |> WorkspaceParamsAttrs.getRngGen GaWsParamKeys.rngGenCreate
                let! order = wsParams |> WorkspaceParamsAttrs.getOrder GaWsParamKeys.order
                let! sorterCount = wsParams |> WorkspaceParamsAttrs.getSorterCount GaWsParamKeys.sorterCount
                let! switchGenMode = wsParams |> WorkspaceParamsAttrs.getSwitchGenMode GaWsParamKeys.switchGenMode
                let! switchCount = wsParams |> WorkspaceParamsAttrs.getSwitchCount GaWsParamKeys.sorterLength
                let! sorterEvalMode = wsParams |> WorkspaceParamsAttrs.getSorterEvalMode GaWsParamKeys.sorterEvalMode
                let! useParallel = wsParams |> WorkspaceParamsAttrs.getUseParallel GaWsParamKeys.useParallel
                let! stagesSkipped = wsParams |> WorkspaceParamsAttrs.getStageCount GaWsParamKeys.stagesSkipped
                let! sortableSetCfgType = wsParams |> WorkspaceParamsAttrs.getSortableSetCfgType GaWsParamKeys.sortableSetCfgType

                let sortableSetCfg = 
                        SortableSetCfg.make
                            sortableSetCfgType
                            order
                            None

                let wsParamsWithSortableSet = 
                    wsParams |> WorkspaceParamsAttrs.setSortableSetId GaWsParamKeys.sortableSetId
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
                    new causeAddSorterSpeedBinSet(wsParams, wnSorterSpeedBinSet)

                return
                   ( history 
                        |> History.addCauses 
                            [
                                causeAddWorkspaceParams;
                                causeAddSortableSet; 
                                causeAddSorterSetRnd;
                                causeAddSorterSpeedBinSet;
                                causeMakeSorterSetEvalParent;
                            ],
                     wsParamsWithSortableSet )
            }


    let addResetSpeedBinsCauses
            (wnSorterSpeedBinSet:wsComponentName)
            (wsParams:workspaceParams)
            (history:history)
            =
            result {

                let causeAddSorterSpeedBinSet =
                    new causeAddSorterSpeedBinSet(wsParams, wnSorterSpeedBinSet)

                return
                   history 
                        |> History.addCauses 
                            [
                                causeAddSorterSpeedBinSet;
                            ]
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
            (wsParams:workspaceParams)
            (history:history)
         =
            result {
                let! mutationRate = wsParams |> WorkspaceParamsAttrs.getMutationRate GaWsParamKeys.mutationRate
                let! noiseFraction = wsParams |> WorkspaceParamsAttrs.getNoiseFraction GaWsParamKeys.noiseFraction
                let! rngGenMutate = wsParams |> WorkspaceParamsAttrs.getRngGen GaWsParamKeys.rngGenMutate
                let! rngGenPrune = wsParams |> WorkspaceParamsAttrs.getRngGen GaWsParamKeys.rngGenPrune
                let! order = wsParams |> WorkspaceParamsAttrs.getOrder GaWsParamKeys.order
                let! sorterCount = wsParams |> WorkspaceParamsAttrs.getSorterCount GaWsParamKeys.sorterCount
                let! sorterCountMutated = wsParams |> WorkspaceParamsAttrs.getSorterCount GaWsParamKeys.sorterCountMutated
                let! sorterSetPruneMethod = wsParams |> WorkspaceParamsAttrs.getSorterSetPruneMethod GaWsParamKeys.sorterSetPruneMethod
                let! stageWeight = wsParams |> WorkspaceParamsAttrs.getStageWeight GaWsParamKeys.stageWeight
                let! sorterEvalMode = wsParams |> WorkspaceParamsAttrs.getSorterEvalMode GaWsParamKeys.sorterEvalMode
                let! switchGenMode = wsParams |> WorkspaceParamsAttrs.getSwitchGenMode GaWsParamKeys.switchGenMode
                let! generation = wsParams |> WorkspaceParamsAttrs.getGeneration GaWsParamKeys.generation_current

                let! useParallel = wsParams |> WorkspaceParamsAttrs.getUseParallel GaWsParamKeys.useParallel

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
                                causeUpdateSorterSpeedBinSetParent;
                                causeUpdateSorterSpeedBinSetMutant;
                                causePruneSorterSets;
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
                            wsParams
                            )

                return
                    history 
                    |> History.addCauses 
                        [
                            causeSetupForNextGen;
                        ]
            }
