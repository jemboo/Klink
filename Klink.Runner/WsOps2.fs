namespace global
open System


module WsOps2 = 


    let makeEm (rootDir:string) 
        =
        
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

        let emptyWsCfg = WorkspaceCfg.Empty
        let runDir = System.IO.Path.Combine(rootDir, "noise_0.5")
        let fs = new WorkspaceFileStore(runDir)
        let wsParams = gaWs.wsPs()

        result {
            let! wsCfg = WsOpsLibB.genZero
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))

            let mutable curGen = 0
            let mutable curCfg = wsCfg
            let mutable curParams = wsParams

            while curGen < 10 do
                let! wsCfgN, wsPramsN = 
                    WsOpsLibB.doGen
                                    wnSortableSet
                                    wnSorterSetParent
                                    wnSorterSetMutator
                                    wnSorterSetMutated
                                    wnSorterSetPruned
                                    wnParentMap
                                    wnSorterSetEvalParent
                                    wnSorterSetEvalMutated
                                    wnSorterSetEvalPruned
                                    wnSorterSetPruner
                                    fs
                                    (fun s-> Console.WriteLine(s))
                                    curParams
                                    curCfg

                curCfg <- wsCfgN
                curParams <- wsPramsN
                curGen <- curGen + 1

            return ()
        }
