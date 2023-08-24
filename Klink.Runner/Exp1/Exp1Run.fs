namespace global
open System


module Exp1Run =

    let scriptFolder = "scripts"
    let toDoFolder = "toDo"
    let runningFolder = "running"
    let completedFolder = "completed"

    let scriptToDoPath (root:string) =
        IO.Path.Combine(root, scriptFolder, toDoFolder)

    let scriptRunningPath (root:string) =
        IO.Path.Combine(root, scriptFolder, runningFolder)

    let scriptCompletedPath (root:string) =
        IO.Path.Combine(root, scriptFolder, completedFolder)

    let getNextScript (root:string) =
        try
            let nextScriptSourcePath = 
                IO.Directory.EnumerateFiles (scriptToDoPath root)
                    |> Seq.head
            let scriptName = IO.Path.GetFileName(nextScriptSourcePath)
            let runFolder = (scriptRunningPath root)
            IO.Directory.CreateDirectory(runFolder)
            let nextScriptWorkingPath = IO.Path.Combine(runFolder, scriptName)
            IO.File.Move(nextScriptSourcePath, nextScriptWorkingPath)
            nextScriptWorkingPath |> Ok
        with ex ->
            ("error in getNextScript: " + ex.Message) |> Result.Error


    let runNextScript (root:string) =
        result {
            let! script = 
                getNextScript root
            
            let! cfgSet = script |> ShcRunCfgSetDto.fromJson

            return cfgSet
        }


    let wnSortableSet = "sortableSet" |> WsComponentName.create
    let wnSorterSetParent = "sorterSetParent" |> WsComponentName.create
    let wnSorterSetMutator = "sorterSetMutator" |> WsComponentName.create
    let wnSorterSetMutated = "sorterSetMutated" |> WsComponentName.create
    let wnSorterSetPruned = "sorterSetPruned" |> WsComponentName.create
    let wnParentMap = "parentMap" |> WsComponentName.create
    let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
    let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
    let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create


    let doRun
            (projectDir:string)
            (wsParamsS: workspaceParams seq)
            (maxGen:generation)
        = 
        result {
            for wsParams in wsParamsS do
                let! runId = wsParams |> WorkspaceParams.getRunId "runId"

                let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
                let fs = new WorkspaceFileStore(runDir)

                let! wsCfg_params, _ = 
                        Exp1WsOps.setupWorkspace
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))

                let mutable curGen = 0
                let mutable curCfg = wsCfg_params |> fst
                let mutable curParams = wsCfg_params |> snd

                while curGen < (maxGen |> Generation.value) do
                    let! wsCfgN, wsPramsN = 
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
                            fs
                            (fun s-> Console.WriteLine(s))
                            curParams
                            curCfg

                    curCfg <- wsCfgN
                    curParams <- wsPramsN
                    curGen <- curGen + 1

            return "success"
        }


    let doRunRun
            (projectDir:string)
            (wsParamsS: workspaceParams seq)
            (maxGen:generation)
        = 
        result {
            for wsParams in wsParamsS do

                let! runId = wsParams |> WorkspaceParams.getRunId "runId"
                let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
                let fs = new WorkspaceFileStore(runDir)

                let! wsCfg_params, ws = 
                        Exp1WsOps.setupWorkspace
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))
                let! cg = wsParams |> WorkspaceParams.getGeneration "generation"
                                   |> Result.map(Generation.value)
                let mutable curGen = cg
                let mutable curParams = wsCfg_params |> snd
                let mutable curWorkspace = ws

                while curGen < (maxGen |> Generation.value) do
                    let! wsN, wsPramsN =
                        Exp1WsOps.doGenOnWorkspace
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetMutator
                            wnSorterSetMutated
                            wnSorterSetPruned
                            wnParentMap
                            wnSorterSetEvalParent
                            wnSorterSetEvalMutated
                            wnSorterSetEvalPruned
                            fs
                            (fun s-> Console.WriteLine(s))
                            curParams
                            curWorkspace

                    curWorkspace <- wsN
                    curParams <- wsPramsN
                    curGen <- curGen + 1

            return "success"
        }


    let continueUpdating
            (projectDir:string)
            (firstFolderIndex:int)
            (folderNum:int)
            (iterationCount:int)
        =
        let runDirs = IO.Directory.EnumerateDirectories(projectDir)
                        |> Seq.toArray
                        |> Array.sort
                        |> Array.skip firstFolderIndex
                        |> Array.take folderNum
        result {
            for runDir in runDirs do
                Console.WriteLine(runDir)

                let fs = new WorkspaceFileStore(runDir)

                let! workspaceId = fs.getLastWorkspaceId
                
                Console.WriteLine($" wsId: {workspaceId}")

                let! wsLoaded = fs.loadWorkSpace workspaceId
                let! paramsLoaded = wsLoaded 
                                    |> Workspace.getComponent ("workspaceParams" |> WsComponentName.create)
                                    |> Result.bind(WorkspaceComponent.asWorkspaceParams)
                let! genLoaded = paramsLoaded |> WorkspaceParams.getGeneration "generation"
                                 |> Result.map(Generation.value)

                let mutable curGen = genLoaded
                let mutable curParams = paramsLoaded
                let mutable curWorkspace = wsLoaded


                let maxGen = curGen + iterationCount
                while curGen < maxGen do
                    let! wsN, wsPramsN = 
                        Exp1WsOps.doGenOnWorkspace
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetMutator
                            wnSorterSetMutated
                            wnSorterSetPruned
                            wnParentMap
                            wnSorterSetEvalParent
                            wnSorterSetEvalMutated
                            wnSorterSetEvalPruned
                            fs
                            (fun s-> Console.WriteLine(s))
                            curParams
                            curWorkspace

                    curWorkspace <- wsN
                    curParams <- wsPramsN
                    curGen <- curGen + 1

            return "success"
        }
