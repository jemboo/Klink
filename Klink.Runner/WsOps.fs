namespace global
open System


module WsOps = 


    let makeEm (rootDir:string) =
        let res() =

            result {
                let order = 16 |> Order.createNr
                let rngGen = 123 |> RandomSeed.create |> RngGen.createLcg
                let randy = rngGen |> Rando.fromRngGen
                let nextRngGen () =
                    randy |> Rando.nextRngGen

                let switchCount = SwitchCount.orderTo999SwitchCount order
                let sorterCount = SorterCount.create 32
                let switchGenMode = switchGenMode.Stage
                let sorterCountMutated = SorterCount.create 64
                let mutationRate = 0.1 |> MutationRate.create
                //let noiseFraction = Some 0.5
                let noiseFraction = None
                let stageWeight = 1.0 |> StageWeight.create
                let sorterEvalMode = sorterEvalMode.DontCheckSuccess
                let useParallel = UseParallel.create true

                
                let wnRandoCreate = "randoCreate" |> WsComponentName.create
                let wnRandoMutate = "randoMutate" |> WsComponentName.create
                let wnRandoPrune = "randoPrune" |> WsComponentName.create
                let wnSortableSet = "sortableSet" |> WsComponentName.create
                let wnSorterSetParent = "sorterSetParent" |> WsComponentName.create
                let wnSorterSetMutator = "sorterSetMutator" |> WsComponentName.create
                let wnSorterSetMutated = "sorterSetMutated" |> WsComponentName.create
                let wnSorterSetPruned = "sorterSetPruned" |> WsComponentName.create


                let wnParentMap = "parentMap" |> WsComponentName.create

                let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
                let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
                let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create


                let wnSorterSetPruner = "sorterSetPruner" |> WsComponentName.create

     
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

                let causeAddSorterSet =  
                    new causeAddSorterSetRnd(
                            wnSorterSetParent, 
                            wnRandoCreate, 
                            srCfg,
                            nextRngGen ())

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



                let emptyWsCfg = WorkspaceCfg.Empty
                let fullWsCfg = 
                        emptyWsCfg 
                        |> WorkspaceCfg.addCauseCfgs 
                            [causeAddSortableSet; 
                            causeAddSorterSet; 
                            causeAddSorterSetMutator;
                            causeMutateSorterSet;
                            causeMakeSorterSetEvalParent;
                            causeMakeSorterSetEvalMutated;
                            causePruneSorterSets
                            ]

                let fs = new WorkspaceFileStore(rootDir)

                //let! workspace = 
                //        Workspace.empty 
                //            |> WorkspaceCfg.makeWorkspace fullWsCfg.history (fun s-> Console.WriteLine(s))

                let! workspace = 
                        fullWsCfg
                            |> WorkspaceCfg.updateWorkspace fs (fun s-> Console.WriteLine(s))


                return! fs.saveWorkSpace workspace
            }















        //    result {
        //        let wsEmpty = Workspace.empty

        //        let order = Order.createNr 10
        //        let ssRecId = Guid.NewGuid() |> SortableSetId.create
        //        let wcSS = SortableSet.makeAllBits ssRecId rolloutFormat.RfU8 order 
        //                    |> Result.ExtractOrThrow
        //                    |> workspaceComponent.SortableSet

        //        let wsCompName = "sortableSet" |> WsComponentName.create
        //        let ssWsId = Guid.NewGuid() |> WorkspaceId.create

        //        let! wsIn = wsEmpty 
        //                        |> Workspace.addComponent ssWsId wsCompName wcSS

        //        let fs = new WorkspaceFileStore(rootDir)

        //        return! fs.saveWorkSpace wsIn
        //    }


            //WsCfgs.allSortableSetCfgs ()
            //|> Array.map(FileStoreOps.getSortableSet)

            //WsCfgs.allSorterSetMutatedFromRndCfgs ()
            //|> Array.map(sorterSetCfg.RndMutated)
            //|> Array.map(getSorterSet)

            //WsCfgs.allSorterSetRndCfgs ()
            //|> Array.map(sorterSetCfg.Rnd)
            //|> Array.map(getSorterSet)

            //WsCfgs.allSorterSetSelfAppendCfgs ()
            //|> Array.map(sorterSetCfg.SelfAppend)
            //|> Array.map(getSorterSet)



            //WsCfgs.allSorterSetRnd_EvalAllBitsCfg ()
            //|> Array.map(sorterSet_EvalCfg.Rnd)
            //|> Array.map(getSorterSetEval ( true |> UseParallel.create))

            //WsCfgs.allSsMfr_EvalAllBitsCfg ()
            //|> Array.map(sorterSet_EvalCfg.RndMutated)
            //|> Array.map(getSorterSetEval ( true |> UseParallel.create))


            //WsCfgs.allSsAfr_EvalAllBitsCfg ()
            //|> Array.map(sorterSet_EvalCfg.RndAppended)
            //|> Array.map(getSorterSetEval ( true |> UseParallel.create))


            //WsCfgs.allSsMfr_EvalAllBitsCfg ()
            //|> Array.map(getSsmfr_EvalAllBits)


            //WsCfgs.allSorterSetEvalReportCfgs ()
            //|> Array.map(make_sorterSetRnd_Report)

            //WsCfgs.allssmfrEvalReportCfgs ()
            //|> Array.map(make_ssMfr_Report)

            //WsCfgs.allssAfrEvalReportCfgs ()
            //|> Array.map(make_ssAfr_Report)

            //WsCfgs.allssmfrEvalMergeReportCfgs ()
            //|> Array.map(make_ssmrMerge_Report)

        res()