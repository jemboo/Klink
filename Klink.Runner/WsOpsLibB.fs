namespace global
open System


module WsOpsLibB = ()


    //let pruneRoundA1
    //        (wnRandoCreate:wsComponentName)
    //        (wnRandoMutate:wsComponentName)
    //        (wnRandoPrune:wsComponentName)
    //        (wnSortableSet:wsComponentName)
    //        (wnSorterSetParent:wsComponentName)
    //        (wnSorterSetMutator:wsComponentName)
    //        (wnSorterSetMutated:wsComponentName)
    //        (wnSorterSetPruned:wsComponentName)
    //        (wnParentMap:wsComponentName)
    //        (wnSorterSetEvalParent:wsComponentName)
    //        (wnSorterSetEvalMutated:wsComponentName)
    //        (wnSorterSetEvalPruned:wsComponentName)
    //        (wnSorterSetPruner:wsComponentName)
    //        (fs:WorkspaceFileStore)
    //        (order:order)
    //        (rngGen:rngGen)
    //        (sorterCount:sorterCount)
    //        (sorterCountMutated:sorterCount)
    //        (mutationRate:mutationRate)
    //        (noiseFraction:noiseFraction option)
    //        (switchGenMode:switchGenMode)
    //        (stageWeight:stageWeight)
    //        (workspaceCfg:workspaceCfg)
    //        =

    //        result {
    //            let randy = rngGen |> Rando.fromRngGen
    //            let nextRngGen () =
    //                randy |> Rando.nextRngGen

    //            let switchCount = SwitchCount.orderTo999SwitchCount order
    //            let sorterEvalMode = sorterEvalMode.DontCheckSuccess
    //            let useParallel = UseParallel.create true

    //            let ssCfg = sortableSetCertainCfg.All_Bits order
    //                        |> sortableSetCfg.Certain
    //            let srCfg = new sorterSetRndCfg(
    //                            wnSorterSetParent,
    //                            order,
    //                            switchGenMode,
    //                            switchCount,
    //                            sorterCount)


    //            let causeAddSortableSet =  
    //                new causeAddSortableSet(
    //                        wnSortableSet, 
    //                        ssCfg)

    //            let causeAddSorterSetRnd =  
    //                new causeAddSorterSetRnd(
    //                        wnSorterSetParent, 
    //                        wnRandoCreate, 
    //                        srCfg,
    //                        nextRngGen ())

    //            let causeAddSorterSetMutator = 
    //                new causeAddSorterSetMutator(
    //                        wnSorterSetMutator, 
    //                        order, 
    //                        switchGenMode,
    //                        sorterCountMutated, 
    //                        mutationRate)

    //            let causeMutateSorterSet = 
    //                new causeMutateSorterSet(
    //                        wnSorterSetParent,
    //                        wnSorterSetMutated,
    //                        wnSorterSetMutator,
    //                        wnRandoMutate,
    //                        wnParentMap,
    //                        nextRngGen ()
    //                        )

    //            let causeMakeSorterSetEvalParent = 
    //                new causeMakeSorterSetEval(
    //                        wnSortableSet,
    //                        wnSorterSetParent,
    //                        sorterEvalMode,
    //                        wnSorterSetEvalParent,
    //                        useParallel)

    //            let causeMakeSorterSetEvalMutated = 
    //                new causeMakeSorterSetEval(
    //                        wnSortableSet,
    //                        wnSorterSetMutated,
    //                        sorterEvalMode,
    //                        wnSorterSetEvalMutated,
    //                        useParallel)

    //            let causePruneSorterSets = 
    //                new causePruneSorterSetsWhole(
    //                        wnRandoPrune,
    //                        wnSorterSetParent,
    //                        wnSorterSetMutated,
    //                        wnSorterSetEvalParent,
    //                        wnSorterSetEvalMutated,
    //                        wnSorterSetPruner,
    //                        wnSorterSetPruned,
    //                        wnSorterSetEvalPruned,
    //                        nextRngGen (),
    //                        sorterCount,
    //                        noiseFraction,
    //                        stageWeight
    //                        )

    //            let causePruneSorterSetsNextGen = 
    //                new causePruneSorterSetsNextGen(
    //                        wnSorterSetParent,
    //                        wnSorterSetPruned,
    //                        wnSorterSetEvalParent,
    //                        wnSorterSetEvalPruned
    //                        )


    //            let fullWsCfg = 
    //                    workspaceCfg 
    //                    |> WorkspaceCfg.addCauseCfgs 
    //                        [causeAddSortableSet; 
    //                        causeAddSorterSetRnd; 
    //                        causeAddSorterSetMutator;
    //                        causeMutateSorterSet;
    //                        causeMakeSorterSetEvalParent;
    //                        causeMakeSorterSetEvalMutated;
    //                        causePruneSorterSets;
    //                        causePruneSorterSetsNextGen;
    //                        ]

    //            return!
    //                    fullWsCfg
    //                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

    //        }



