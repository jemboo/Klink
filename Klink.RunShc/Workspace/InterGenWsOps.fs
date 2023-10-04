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
    let wnSorterSetAncestry = "sorterSetAncestry" |> WsComponentName.create
       


    let setupGenLoopStart
            (projectFolderPath:string)
            (wsParams: workspaceParams)
            (workplaceFileStoreF: string -> IWorkspaceStore)
        =
        result {

            let! runId = wsParams |> WorkspaceParamsAttrs.getRunId ShcWsParamKeys.runId
            let runDir = IO.Path.Combine(projectFolderPath, runId |> RunId.value |> string)
            let workplaceFileStore = workplaceFileStoreF runDir
            let! wsParamsGen1, curWorkspace = 
                    IntraGenWsOps.setupWorkspace
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetEvalParent
                            wnSorterSpeedBinSet
                            wnSorterSetAncestry
                            wsParams
                            workplaceFileStore
                            (fun s-> Console.WriteLine(s))
            let! maxGen = 
                    wsParams |> WorkspaceParamsAttrs.getGeneration ShcWsParamKeys.generation_max


            let! curgen = wsParamsGen1 |> snd |> WorkspaceParamsAttrs.getGeneration ShcWsParamKeys.generation_current
            let curParams = wsParamsGen1 |> snd

            return (curgen, maxGen, curParams, curWorkspace, workplaceFileStore)
        }



    let runLoops 
            (curGen:generation)
            (maxGen:generation)
            (curParams:workspaceParams)
            (curWorkspace:workspace)
            (workplaceFileStore:IWorkspaceStore)
        =
            let mutable _curGen = curGen |> Generation.value
            let mutable _maxGen = maxGen |> Generation.value
            let mutable _curParams = curParams
            let mutable _curWorkspace = curWorkspace
            let mutable _keepGoing = true
            let mutable _msg = ""

            while ((_curGen < _maxGen ) && _keepGoing) do
                let tup =
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
                        wnSorterSetAncestry
                        workplaceFileStore
                        (fun s-> Console.WriteLine(s))
                        _curParams
                        _curWorkspace
                _msg <-
                    match tup with
                    | Ok (wsN, wsPramsN) -> 
                            _keepGoing <- true
                            _curWorkspace <- wsN
                            _curParams <- wsPramsN
                            _curGen <- _curGen + 1
                            ""
                    | Error m -> 
                            _keepGoing <- false
                            m
            _msg



    let startGenLoops
            (projectFolderPath:string)
            (workplaceFileStoreF: string -> IWorkspaceStore)
            (wsParams: workspaceParams)
        = 
        let res = setupGenLoopStart projectFolderPath wsParams workplaceFileStoreF
        match res with
        | Ok (curgen, maxGen, curParams, curWorkspace, workplaceFileStore) ->
            let msg = runLoops curgen maxGen curParams curWorkspace workplaceFileStore
            Console.WriteLine($"error in runLoops: {msg}")
        | Error m -> Console.Write($"error in setupGenLoopStart: {m}")




    let setupGenLoopContinue
            (projectDir:string)
            (runId:runId)
            (newGenerations:generation)
            (workspaceFileStoreF: string -> IWorkspaceStore)
        =
        result {
            let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
            //Console.WriteLine(runDir)

            let workplaceFileStore = workspaceFileStoreF runDir

            let! workspaceId = workplaceFileStore.GetLastWorkspaceId()
                
            Console.WriteLine($" wsId: {workspaceId}")

            let! wsLoaded = workplaceFileStore.LoadWorkSpace workspaceId
            let! paramsLoaded = wsLoaded 
                                |> Workspace.getComponent ("workspaceParams" |> WsComponentName.create)
                                |> Result.bind(WorkspaceComponent.asWorkspaceParams)
            let! genLoaded = paramsLoaded |> WorkspaceParamsAttrs.getGeneration ShcWsParamKeys.generation_current
            let maxGen = genLoaded |> Generation.add (newGenerations |> Generation.value)

            return (genLoaded, maxGen, paramsLoaded, wsLoaded, workplaceFileStore)
        }



    let continueGenLoops
            (projectDir:string)
            (runId:runId)
            (newGenerations:generation)
            (workspaceFileStoreF: string -> IWorkspaceStore)
        =
        let res = setupGenLoopContinue projectDir runId newGenerations workspaceFileStoreF
        match res with
        | Ok (curgen, maxGen, curParams, curWorkspace, workplaceFileStore) ->
            let msg = runLoops curgen maxGen curParams curWorkspace workplaceFileStore
            Console.WriteLine($"error in runLoops: {msg}")
        | Error m -> Console.Write($"error in setupGenLoopContinue: {m}")
