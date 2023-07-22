namespace global
open System


module WsOps = 


    let runFolder = "testRnd"

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

        let runDir = IO.Path.Combine(rootDir, runFolder)
        let fs = new WorkspaceFileStore(runDir)
        let wsParams = gaWs.wsPs()

        result {
            let! wsCfg = WsOpsLib.genZero
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))

            let mutable curGen = 0
            let mutable curCfg = wsCfg
            let mutable curParams = wsParams

            while curGen < 4 do
                let! wsCfgN, wsPramsN = 
                    WsOpsLib.doGen
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


    let selectedSorterEvalProps =
        [
            sorterEvalProps.ErrorMsg;
            sorterEvalProps.Phenotype;
            sorterEvalProps.SortableSetId;
            sorterEvalProps.SortedSetSize;
            sorterEvalProps.SorterId;
            sorterEvalProps.StageCount;
            sorterEvalProps.Success;
            sorterEvalProps.SwitchCount;
        ]

    let sorterEvalHeaders () = 
        selectedSorterEvalProps
            |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""

    let sorterEvalValues (sev:sorterEval) =
        let yab = sev |> SorterEval.getSorterEvalProps
        selectedSorterEvalProps
            |> List.fold(fun st t -> sprintf "%s\t%s" st yab.[t]) ""

    let selectedParams =
        [
            "generation";
            "mutationRate";
            "noiseFraction";
            "order";
            "sorterCount";
            "sorterCountMutated";
            "stageWeight";
            "sorterLength";
            "switchGenMode";
        ]

    let selectedParamValues (wsps:workspaceParams) =
        let yab = wsps |> WorkspaceParams.getMap
        let woof = 
            selectedParams
            |> List.fold(fun st t -> 
                        if (st = "" ) then
                              yab.[t]
                        else sprintf "%s\t%s" st yab.[t]
                        ) ""
        woof

    let reportHeader () =
        sprintf "%s%s" 
            (selectedParams |> List.reduce(fun st t -> sprintf "%s\t%s" st t))
            (sorterEvalHeaders ())


    let reportLines
                   (wsComp:workspaceComponent) 
                   (wsps:workspaceParams)
                   (fileName:string)
                   (fs:WorkspaceFileStore)
        =
        let sorterSetEval = wsComp |> WorkspaceComponent.asSorterSetEval |> Result.ExtractOrThrow
        let lines = 
            sorterSetEval 
                |> SorterSetEval.getSorterEvals 
                |> Array.map(fun sev ->
                                    sprintf "%s%s" 
                                        (selectedParamValues wsps)
                                        (sorterEvalValues sev))
        fs.appendLines None fileName lines


    let reportEm (rootDir:string) 
        =
        let runDir = System.IO.Path.Combine(rootDir, runFolder)
        let reportFileName = "SorterEvalReport"
        let fs = new WorkspaceFileStore(runDir)

        fs.appendLines None reportFileName [reportHeader ()] 
                |> Result.ExtractOrThrow
                |> ignore

        let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
        let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
        let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
        result {
            let! compTupes = fs.getAllComponentsWithParamsByName wnSorterSetEvalMutated
            let yab = compTupes 
                        |> List.map (fun (wsComp, wsPram) -> 
                            reportLines wsComp wsPram reportFileName fs)
            return ()
        }
