namespace global
open System


module WsOps = 


    let makeEm (rootDir:string) =
        let res() =

            result {
                let order = 16 |> Order.createNr
                let wsCompName1 = "test1" |> WsComponentName.create
                let wsCompName2 = "test2" |> WsComponentName.create
                let ssCfg = sortableSetCertainCfg.All_Bits order
                            |> sortableSetCfg.Certain
                let causeCfg1 =  new causeCfgAddSortableSet(wsCompName1, ssCfg)
                let causeCfg2 =  new causeCfgAddSortableSet(wsCompName2, ssCfg)

                let emptyWsCfg = WorkspaceCfg.Empty
                let firstWsCfg = emptyWsCfg |> WorkspaceCfg.addCauseCfg causeCfg1
                let secondWsCfg = firstWsCfg |> WorkspaceCfg.addCauseCfg causeCfg2

                let! workspace = 
                        Workspace.empty 
                            |> WorkspaceCfg.makeWorkspace secondWsCfg.history

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