﻿namespace global
open System


module WsOps = 

    //********  SortableSet ****************

    let saveSortableSet 
            (cfg:sortableSetCfg)
            (sst:sortableSet) 
        =
        let fileName = cfg |> SortableSetCfg.getFileName 
        WsFile.writeToFile wsFile.SortableSet fileName (sst |> SortableSetDto.toJson )


    let loadSortableSet (cfg:sortableSetCfg) =
          result {
            let! txtD = WsFile.readAllText  wsFile.SortableSet
                            (cfg |> SortableSetCfg.getFileName)
            return! txtD |> SortableSetDto.fromJson
          }


    let makeSortableSet (cfg:sortableSetCfg) =
        result {
            let! sortableSet = SortableSetCfg.makeSortableSet cfg
            let res = sortableSet 
                        |> saveSortableSet cfg
                        |> Result.map(ignore)
            return sortableSet
        }


    let getSortableSet (cfg:sortableSetCfg) =
        result {
            let loadRes = loadSortableSet cfg
            match loadRes with
            | Ok ss -> return ss
            | Error _ -> return! (makeSortableSet cfg)
        }


    //********  ParentMap  ****************
    
    let saveSorterSetParentMap
            (cfg:sorterSetParentMapCfg)
            (sst:sorterSetParentMap) 
        =
        WsFile.writeToFile wsFile.SorterSetParentMap
            (cfg |> SorterSetParentMapCfg.getFileName) 
            (sst |> SorterSetParentMapDto.toJson)


    let loadSorterSetParentMap
            (cfg:sorterSetParentMapCfg) 
          =
          result {
            let! txtD = 
                WsFile.readAllText  
                    wsFile.SorterSetParentMap
                    (cfg |> SorterSetParentMapCfg.getFileName)
            return! txtD |> SorterSetParentMapDto.fromJson
          }


    let makeSorterSetParentMap
            (cfg:sorterSetParentMapCfg) 
        =
        result {
            let parentMap = 
                    SorterSetParentMapCfg.makeParentMap
                        cfg

            let! resSs = parentMap |> saveSorterSetParentMap cfg
            return parentMap
        }


    let getParentMap
            (cfg:sorterSetParentMapCfg) 
        =
        result {
            let loadRes  = 
                result {
                    let! mut = loadSorterSetParentMap cfg
                    return mut
                }

            match loadRes with
            | Ok mut -> return mut
            | Error _ -> return! (makeSorterSetParentMap cfg)
        }



    //********  ConcatMap  ****************
    
    let saveSorterSetConcatMap
            (cfg:sorterSetSelfAppendCfg)
            (sst:sorterSetConcatMap) 
        =
        WsFile.writeToFile wsFile.SorterSetConcatMap
            (cfg |> SorterSetSelfAppendCfg.getSorterSetConcatFileName) 
            (sst |> SorterSetConcatMapDto.toJson)



    let loadSorterSetConcatMap
            (cfg:sorterSetSelfAppendCfg) 
          =
          result {
            let! txtD = 
                WsFile.readAllText  
                    wsFile.SorterSetConcatMap
                    (cfg |> SorterSetSelfAppendCfg.getSorterSetConcatFileName)
            return! txtD |> SorterSetConcatMapDto.fromJson
          }


    let makeSorterSetConcatMap
            (cfg:sorterSetSelfAppendCfg) 
        =
        result {
            let parentMap = 
                    SorterSetSelfAppendCfg.makeSorterSetConcatMap
                        cfg

            let! resSs = parentMap |> saveSorterSetConcatMap cfg
            return parentMap
        }


    let getConcatMap
            (cfg:sorterSetSelfAppendCfg) 
        =
        result {
            let loadRes  = 
                result {
                    let! mut = loadSorterSetConcatMap cfg
                    return mut
                }

            match loadRes with
            | Ok mut -> return mut
            | Error _ -> return! (makeSorterSetConcatMap cfg)
        }


    //********  SorterSet ****************

    let saveSorterSet
            (fileName:string)
            (sst:sorterSet) 
        =
        WsFile.writeToFile wsFile.SorterSet
            fileName
            (sst |> SorterSetDto.toJson)


    let loadSorterSet (fileName:string) =
          result {
            let! txtD = 
                WsFile.readAllText  
                    wsFile.SorterSet
                    fileName
            return! txtD |> SorterSetDto.fromJson
          }


    let getSorterSet
            (cfg:sorterSetCfg)
        =
        SorterSetCfg.getSorterSet
                saveSorterSet
                loadSorterSet
                getParentMap
                getConcatMap
                cfg


    //********  SorterSet_Eval  ****************


    let saveSorterSetEval
            (fileName:string)
            (sst:sorterSetEval) 
        =
        WsFile.writeToFile 
            wsFile.SorterSetEval 
            fileName 
            (sst |> SorterSetEvalDto.toJson )


    let loadSorterSet_Eval (fileName:string) 
        =
        result {
            let! txtD = 
                WsFile.readAllText  
                    wsFile.SorterSetEval
                    fileName
            return! txtD |> SorterSetEvalDto.fromJson
        }


    let getSorterSetEval
            (up:useParallel)
            (cfg:sorterSet_EvalCfg) 
        =
        SorterSet_EvalCfg.getSorterSetEval
            getSortableSet
            getSorterSet
            saveSorterSetEval
            loadSorterSet_Eval
            up
            cfg


    //********  SorterSetRnd_EvalAllBitsCfg  ****************

    let makeSorterSetRnd_EvalAllBitsCfg
            (cfg:sorterSetRnd_EvalAllBitsCfg) 
        =
        result {
            let fileName = cfg |> SorterSetRnd_EvalAllBitsCfg.getFileName 
            let! ssEval = 
                    SorterSetRnd_EvalAllBitsCfg.makeSorterSetEval
                        WsCfgs.useParall
                        cfg
                        getSortableSet
                        getSorterSet

            let! resSs = ssEval |> saveSorterSetEval fileName
            return ssEval
        }


    let getSorterSetRnd_EvalAllBits
            (cfg:sorterSetRnd_EvalAllBitsCfg) 
        =
        result {
            let loadRes  = 
                result {
                    let fileName = cfg |> SorterSetRnd_EvalAllBitsCfg.getFileName
                    let! ssEval = fileName |> loadSorterSet_Eval
                    return ssEval
                }

            match loadRes with
            | Ok ssEval -> return ssEval
            | Error _ -> return! (makeSorterSetRnd_EvalAllBitsCfg cfg)
        }


    //********  ssMfr_EvalAllBitsCfg  ****************

    let makeSsMfr_EvalAllBitsCfg
            (cfg:ssMfr_EvalAllBitsCfg) 
        =
        result {
            let fileName = cfg |> SsMfr_EvalAllBitsCfg.getFileName 
            let! ssEval = 
                    SsMfr_EvalAllBitsCfg.makeSorterSetEval
                        WsCfgs.useParall
                        cfg
                        getSortableSet
                        getSorterSet

            let! resSs = ssEval |> saveSorterSetEval fileName
            return ssEval
        }


    let getSsMfr_EvalAllBits
            (cfg:ssMfr_EvalAllBitsCfg) 
        =
        result {
            let loadRes  = 
                result {
                    let fileName = cfg |> SsMfr_EvalAllBitsCfg.getFileName
                    let! ssEval = fileName |> loadSorterSet_Eval
                    return ssEval
                }

            match loadRes with
            | Ok ssEval -> return ssEval
            | Error _ -> return! (makeSsMfr_EvalAllBitsCfg cfg)
        }



    //********  ssAfr_EvalAllBitsCfg  ****************

    let makeSsAfr_EvalAllBitsCfg
            (cfg:ssAfr_EvalAllBitsCfg) 
        =
        result {
            let fileName = cfg |> SsAfr_EvalAllBitsCfg.getFileName 
            let! ssEval = 
                    SsAfr_EvalAllBitsCfg.makeSorterSetEval
                        WsCfgs.useParall
                        cfg
                        getSortableSet
                        getSorterSet


            //WsCfgs.allSsAfr_EvalAllBitsCfg ()
            //|> Array.map(sorterSet_EvalCfg.RndAppended)
            //|> Array.map(getSorterSetEval ( true |> UseParallel.create))


            let! resSs = ssEval |> saveSorterSetEval fileName
            return ssEval
        }


    let getSsAfr_EvalAllBits
            (cfg:ssAfr_EvalAllBitsCfg) 
        =
        result {
            let loadRes  = 
                result {
                    let fileName = cfg |> SsAfr_EvalAllBitsCfg.getFileName
                    let! ssEval = fileName |> loadSorterSet_Eval
                    return ssEval
                }

            match loadRes with
            | Ok ssEval -> return ssEval
            | Error _ -> return! (makeSsAfr_EvalAllBitsCfg cfg)
        }




    //********  sorterSetRnd_EvalAllBits_ReportCfg  ****************

    let make_sorterSetRnd_Report 
            (cfg:sorterSetRnd_EvalAllBits_ReportCfg) 
        =
        let reportFileName = cfg |> SorterSetRnd_EvalAllBits_ReportCfg.getReportFileName

        WsFile.writeLinesIfNew
            wsFile.SorterEvalReport
            reportFileName 
            [SorterSetRnd_EvalAllBits_ReportCfg.getReportHeader()]
        |> Result.ExtractOrThrow |> ignore


        let repLines_set =
            SorterSetRnd_EvalAllBits_ReportCfg.makeSorterSetEvalReport
                    cfg
                    getSorterSetRnd_EvalAllBits

        repLines_set
            |> Seq.iter(
                fun res ->
                    match res with
                    | Ok repLines ->
                            WsFile.appendLines
                                    wsFile.SorterEvalReport
                                    reportFileName 
                                    repLines
                            |> Result.ExtractOrThrow |> ignore

                    | Error m -> Console.WriteLine(m)
                )


    //********  sorterSetRnd_EvalAllBits_ReportCfg  ****************

    let make_ssMfr_Report 
            (cfg:ssMfr_EvalAllBits_ReportCfg) 
        =
        let reportFileName = cfg |> SsMfr_EvalAllBits_ReportCfg.getReportFileName

        WsFile.writeLinesIfNew
            wsFile.SorterEvalReport
            reportFileName 
            [SsMfr_EvalAllBits_ReportCfg.getReportHeader()]
        |> Result.ExtractOrThrow |> ignore


        let repLines_set =
            SsMfr_EvalAllBits_ReportCfg.makeSorterSetEvalReport
                    cfg
                    getSsMfr_EvalAllBits

        repLines_set
            |> Seq.iter(
                fun res ->
                    match res with
                    | Ok repLines ->
                            WsFile.appendLines
                                    wsFile.SorterEvalReport
                                    reportFileName 
                                    repLines
                            |> Result.ExtractOrThrow |> ignore

                    | Error m -> Console.WriteLine(m)
                )


    //********  ssAfr_EvalAllBits_ReportCfg  ****************

    let make_ssAfr_Report 
            (cfg:ssAfr_EvalAllBits_ReportCfg) 
        =
        let reportFileName = cfg |> SsAfr_EvalAllBits_ReportCfg.getReportFileName

        WsFile.writeLinesIfNew
            wsFile.SorterEvalReport
            reportFileName 
            [SsAfr_EvalAllBits_ReportCfg.getReportHeader()]
        |> Result.ExtractOrThrow |> ignore



        
            //WsCfgs.allSsAfr_EvalAllBitsCfg ()
            //|> Array.map(sorterSet_EvalCfg.RndAppended)
            //|> Array.map(getSorterSetEval ( true |> UseParallel.create))



        let repLines_set =
            SsAfr_EvalAllBits_ReportCfg.makeSorterSetEvalReport
                    cfg
                    getSsAfr_EvalAllBits
                    getConcatMap

        repLines_set
            |> Seq.iter(
                fun res ->
                    match res with
                    | Ok repLines ->
                            WsFile.appendLines
                                    wsFile.SorterEvalReport
                                    reportFileName 
                                    repLines
                            |> Result.ExtractOrThrow |> ignore

                    | Error m -> Console.WriteLine(m)
                )


    //********  ssMfr_EvalAllBitsMerge_ReportCfg  ****************

    let make_ssMfrMerge_Report 
            (cfg:ssMfr_EvalAllBitsMerge_ReportCfg) 
        =
        let reportFileName = cfg |> SsMfr_EvalAllBitsMerge_ReportCfg.getReportFileName

        WsFile.writeLinesIfNew
            wsFile.SorterEvalMergeReport
            reportFileName 
            [SsMfr_EvalAllBitsMerge_ReportCfg.getReportHeader()]
        |> Result.ExtractOrThrow |> ignore


        let repLines_set =
            SsMfr_EvalAllBitsMerge_ReportCfg.makeSorterSetEvalReport
                    cfg
                    getSorterSetRnd_EvalAllBits
                    getSsMfr_EvalAllBits
                    getParentMap
 
        repLines_set
            |> Seq.iter(
                fun res ->
                    match res with
                    | Ok repLines ->
                            WsFile.appendLines
                                    wsFile.SorterEvalMergeReport
                                    reportFileName 
                                    repLines
                            |> Result.ExtractOrThrow |> ignore

                    | Error m -> Console.WriteLine(m)
                )




    let makeEm () =
        let res =
            //WsCfgs.allSortableSetCfgs ()
            //|> Array.map(getSortableSet)

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

            WsCfgs.allssAfrEvalReportCfgs ()
            |> Array.map(make_ssAfr_Report)

            //WsCfgs.allssmfrEvalMergeReportCfgs ()
            //|> Array.map(make_ssmrMerge_Report)

        res