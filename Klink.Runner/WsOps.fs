namespace global
open System


module WsOps = 


    let makeEm (rootDir:string) =
        let res() =

            result {
                let order = 16 |> Order.createNr
                let rngGen = 123 |> RandomSeed.create |> RngGen.createLcg
                let switchCount = SwitchCount.orderTo999SwitchCount order
                let sorterCount = SorterCount.create 32
                let switchGenMode = switchGenMode.Stage

                let sorterCountMutated = SorterCount.create 64
                let mutationRate = 0.1 |> MutationRate.create

                let wnRando = "rando" |> WsComponentName.create
                let wnSortableSet = "sortableSet" |> WsComponentName.create
                let wnSorterSetParent = "sorterSetParent" |> WsComponentName.create
                let wnSorterSetMutator = "sorterSetMutator" |> WsComponentName.create
                let wnSorterSetMutated = "sorterSetMutated" |> WsComponentName.create
                let wnParentMap = "parentMap" |> WsComponentName.create

     
                let ssCfg = sortableSetCertainCfg.All_Bits order
                            |> sortableSetCfg.Certain
                let srCfg = new sorterSetRndCfg(
                                wnSorterSetParent,
                                order,
                                switchGenMode,
                                switchCount,
                                sorterCount)
                            |> sorterSetCfg.Rnd

               // let sorterSetMutatorCfg = new sorterSetMutatorCfg()


                let causeAddRando =  new causeAddRndGenProvider(wnRando, rngGen)
                let causeAddSortableSet =  new causeAddSortableSet(wnSortableSet, ssCfg)
                let causeAddSorterSet =  new causeAddSorterSet(wnSorterSetParent, wnRando, srCfg)
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
                            wnRando,
                            wnParentMap)



                let emptyWsCfg = WorkspaceCfg.Empty
                let fullWsCfg = 
                        emptyWsCfg 
                        |> WorkspaceCfg.addCauseCfgs 
                            [causeAddRando; 
                            causeAddSortableSet; 
                            causeAddSorterSet; 
                            causeAddSorterSetMutator;
                            causeMutateSorterSet]

                let! workspace = 
                        Workspace.empty 
                            |> WorkspaceCfg.makeWorkspace fullWsCfg.history

                let fs = new WorkspaceFileStore(rootDir)
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