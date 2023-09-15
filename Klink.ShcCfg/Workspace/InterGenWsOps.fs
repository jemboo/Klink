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
        = 
        result {

            let! runId = wsParams |> WorkspaceParamsAttrs.getRunId "runId"
            let runDir = IO.Path.Combine(projectFolderPath, runId |> RunId.value |> string)
            let fs = new WorkspaceFileStore(runDir)

            let! wsParamsGen1, ws = 
                    Exp1WsOps.setupWorkspace
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetEvalParent
                            wnSorterSpeedBinSet
                            wsParams
                            fs
                            (fun s-> Console.WriteLine(s))
            let! maxGen = 
                    wsParams |> WorkspaceParamsAttrs.getGeneration "generation_max"
                                |> Result.map(Generation.value)


            let! cg = wsParamsGen1 |> snd |> WorkspaceParamsAttrs.getGeneration "generation_current"
                    
            let mutable curGen = cg |> Generation.value
            let mutable curParams = wsParamsGen1 |> snd
            let mutable curWorkspace = ws

            while curGen < maxGen do
                let! wsN, wsPramsN =
                    Exp1WsOps.doGen
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
                        fs
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
        =
        let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
        result {
            Console.WriteLine(runDir)

            let fs = new WorkspaceFileStore(runDir)

            let! workspaceId = fs.getLastWorkspaceId
                
            Console.WriteLine($" wsId: {workspaceId}")

            let! wsLoaded = fs.loadWorkSpace workspaceId
            let! paramsLoaded = wsLoaded 
                                |> Workspace.getComponent ("workspaceParams" |> WsComponentName.create)
                                |> Result.bind(WorkspaceComponent.asWorkspaceParams)
            let! genLoaded = paramsLoaded |> WorkspaceParamsAttrs.getGeneration "generation_current"
                                |> Result.map(Generation.value)

            let mutable curGen = genLoaded
            let mutable curParams = paramsLoaded
            let mutable curWorkspace = wsLoaded


            let maxGen = curGen + (newGenerations |> Generation.value)
            while curGen < maxGen do
                let! wsN, wsPramsN = 
                    Exp1WsOps.doGen
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
                        fs
                        (fun s-> Console.WriteLine(s))
                        curParams
                        curWorkspace

                curWorkspace <- wsN
                curParams <- wsPramsN
                curGen <- curGen + 1

            return ()
        }


