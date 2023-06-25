namespace global
open System


module WsOps = 


    let makeEm () =
        let res =
            WsCfgs.allSortableSetCfgs ()
            |> Array.map(FileStoreOps.getSortableSet)

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

        res