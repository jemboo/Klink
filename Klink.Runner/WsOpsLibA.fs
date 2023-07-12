namespace global
open System


module WsOpsLibA = 


    let initParentMapAndEval
            (wnRandoCreate:wsComponentName)
            (wnSortableSet:wsComponentName)
            (wnSorterSetParent:wsComponentName)
            (wnSorterSetEvalParent:wsComponentName)
            (order:order)
            (rngGen:rngGen)
            (sorterCount:sorterCount)
            (switchGenMode:switchGenMode)
            (workspaceCfg:workspaceCfg)
            =
            result {
                let randy = rngGen |> Rando.fromRngGen
                let nextRngGen () =
                    randy |> Rando.nextRngGen

                let switchCount = SwitchCount.orderTo999SwitchCount order
                let sorterEvalMode = sorterEvalMode.DontCheckSuccess
                let useParallel = UseParallel.create true

                let ssCfg = sortableSetCertainCfg.All_Bits order
                            |> sortableSetCfg.Certain

                let srCfg = new sorterSetRndCfg(
                                wnSorterSetParent,
                                order,
                                switchGenMode,
                                switchCount,
                                sorterCount)

                let causeAddSortableSet =  
                    new causeAddSortableSet(
                            wnSortableSet, 
                            ssCfg)

                let causeAddSorterSetRnd =  
                    new causeAddSorterSetRnd(
                            wnSorterSetParent, 
                            wnRandoCreate, 
                            srCfg,
                            nextRngGen ())

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
                            [causeAddSortableSet; 
                            causeAddSorterSetRnd;
                            causeMakeSorterSetEvalParent;
                            ]
            }




    let makeMutantsAndPrune
            (wnRandoMutate:wsComponentName)
            (wnRandoPrune:wsComponentName)
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
            (order:order)
            (rngGen:rngGen)
            (sorterCount:sorterCount)
            (sorterCountMutated:sorterCount)
            (mutationRate:mutationRate)
            (noiseFraction:float option)
            (switchGenMode:switchGenMode)
            (stageWeight:stageWeight)
            (workspaceCfg:workspaceCfg)
            =

            result {
                let randy = rngGen |> Rando.fromRngGen
                let nextRngGen () =
                    randy |> Rando.nextRngGen

                let sorterEvalMode = sorterEvalMode.DontCheckSuccess
                let useParallel = UseParallel.create true

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
                            wnRandoMutate,
                            wnParentMap,
                            nextRngGen ()
                            )

                let causeMakeSorterSetEvalParent = 
                    new causeMakeSorterSetEval(
                            wnSortableSet,
                            wnSorterSetParent,
                            sorterEvalMode,
                            wnSorterSetEvalParent,
                            useParallel)

                let causeMakeSorterSetEvalMutated = 
                    new causeMakeSorterSetEval(
                            wnSortableSet,
                            wnSorterSetMutated,
                            sorterEvalMode,
                            wnSorterSetEvalMutated,
                            useParallel)

                let causePruneSorterSets = 
                    new causePruneSorterSetsWhole(
                            wnRandoPrune,
                            wnSorterSetParent,
                            wnSorterSetMutated,
                            wnSorterSetEvalParent,
                            wnSorterSetEvalMutated,
                            wnSorterSetPruner,
                            wnSorterSetPruned,
                            wnSorterSetEvalPruned,
                            nextRngGen (),
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
                            causeMakeSorterSetEvalParent;
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
