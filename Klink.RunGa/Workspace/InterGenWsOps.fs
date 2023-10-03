namespace global
open System


module InterGenWsOps = 

    let wnSortableSet = "sortableSet" |> WsComponentName.create
    let wnSorterSetParent = "sorterSetParent" |> WsComponentName.create
    let wnSorterSetMutator = "sorterSetMutator" |> WsComponentName.create
    let wnSorterSetMutated = "sorterSetMutated" |> WsComponentName.create
    let wnSorterSetPruned = "sorterSetPruned" |> WsComponentName.create
    let wnParentMap = "parentMap" |> WsComponentName.create
    let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
    let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
    let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create
    let wnSorterSpeedBinSet = "sorterSpeedBinSet" |> WsComponentName.create
       

    let doGenLoop
            (projectFolderPath:string)
            (wsParams: workspaceParams)
            (workplaceFileStore:IWorkspaceStore)
        = 
        result {

            let! runId = wsParams |> WorkspaceParamsAttrs.getRunId GaWsParamKeys.runId
            //let runDir = IO.Path.Combine(projectFolderPath, runId |> RunId.value |> string)
            //let fs = new WorkspaceFileStore(runDir)

            let! wsParamsGen1, ws = 
                    IntraGenWsOps.setupWorkspace
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetEvalParent
                            wnSorterSpeedBinSet
                            wsParams
                            workplaceFileStore
                            (fun s-> Console.WriteLine(s))
            let! maxGen = 
                    wsParams |> WorkspaceParamsAttrs.getGeneration GaWsParamKeys.generation_max
                                |> Result.map(Generation.value)


            let! cg = wsParamsGen1 |> snd |> WorkspaceParamsAttrs.getGeneration GaWsParamKeys.generation_current
                    
            let mutable curGen = cg |> Generation.value
            let mutable curParams = wsParamsGen1 |> snd
            let mutable curWorkspace = ws

            while curGen < maxGen do
                let! wsN, wsPramsN =
                    IntraGenWsOps.doGen
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetMutator
                        wnSorterSetMutated
                        wnSorterSetPruned
                        wnParentMap
                        wnSorterSetEvalParent
                        wnSorterSetEvalMutated
                        wnSorterSetEvalPruned
                        wnSorterSpeedBinSet
                        workplaceFileStore
                        (fun s-> Console.WriteLine(s))
                        curParams
                        curWorkspace

                curWorkspace <- wsN
                curParams <- wsPramsN
                curGen <- curGen + 1

            return ()
        }


    let continueUpdating
            (projectDir:string)
            (runId:runId)
            (newGenerations:generation)
            (workplaceFileStore:IWorkspaceStore)
        =
        let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
        result {
            //Console.WriteLine(runDir)

            //let fs = new WorkspaceFileStore(runDir)

            let! workspaceId = workplaceFileStore.GetLastWorkspaceId()
                
            Console.WriteLine($" wsId: {workspaceId}")

            let! wsLoaded = workplaceFileStore.LoadWorkSpace workspaceId
            let! paramsLoaded = wsLoaded 
                                |> Workspace.getComponent ("workspaceParams" |> WsComponentName.create)
                                |> Result.bind(WorkspaceComponent.asWorkspaceParams)
            let! genLoaded = paramsLoaded |> WorkspaceParamsAttrs.getGeneration GaWsParamKeys.generation_current
                                |> Result.map(Generation.value)

            let mutable curGen = genLoaded
            let mutable curParams = paramsLoaded
            let mutable curWorkspace = wsLoaded


            let maxGen = curGen + (newGenerations |> Generation.value)
            while curGen < maxGen do
                let! wsN, wsPramsN = 
                    IntraGenWsOps.doGen
                        wnSortableSet
                        wnSorterSetParent
                        wnSorterSetMutator
                        wnSorterSetMutated
                        wnSorterSetPruned
                        wnParentMap
                        wnSorterSetEvalParent
                        wnSorterSetEvalMutated
                        wnSorterSetEvalPruned
                        wnSorterSpeedBinSet
                        workplaceFileStore
                        (fun s-> Console.WriteLine(s))
                        curParams
                        curWorkspace

                curWorkspace <- wsN
                curParams <- wsPramsN
                curGen <- curGen + 1

            return ()
        }


