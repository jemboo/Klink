namespace global
open System


module WsOps = 


    let makeEm (rootDir:string) 
        =
        let rngGen = 123 |> RandomSeed.create |> RngGen.createLcg
        let randy = rngGen |> Rando.fromRngGen
        let nextRngGen () =
            randy |> Rando.nextRngGen

        let rngGenCreate = (nextRngGen ())
        let rngGenMutate = (nextRngGen ())
        let rngGenPrune = (nextRngGen ())
        
        let generation = 1 |> Generation.create
        let order = 16 |> Order.createNr
        let sorterCount = SorterCount.create 2
        let switchCount = SwitchCount.orderTo999SwitchCount order 
        let switchGenMode = switchGenMode.Stage
        let sorterCountMutated = SorterCount.create 4
        let mutationRate = 0.1 |> MutationRate.create
        let noiseFraction = 0.5 |> NoiseFraction.create |> Some
        //let noiseFraction = None
        let sorterEvalMode = sorterEvalMode.DontCheckSuccess
        let stageWeight = 1.0 |> StageWeight.create
        let useParallel = true |> UseParallel.create

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


        let wsParams = WorkspaceParams.make Map.empty
        let wsParams = wsParams |> WorkspaceParams.setRngGen "rngGenCreate" rngGenCreate
        let wsParams = wsParams |> WorkspaceParams.setRngGen "rngGenMutate" rngGenMutate
        let wsParams = wsParams |> WorkspaceParams.setRngGen "rngGenPrune" rngGenPrune
        let wsParams = wsParams |> WorkspaceParams.setGeneration "generation" generation
        let wsParams = wsParams |> WorkspaceParams.setMutationRate "mutationRate" mutationRate
        let wsParams = wsParams |> WorkspaceParams.setNoiseFraction "noiseFraction" noiseFraction
        let wsParams = wsParams |> WorkspaceParams.setOrder "order" order
        let wsParams = wsParams |> WorkspaceParams.setSorterCount "sorterCount" sorterCount
        let wsParams = wsParams |> WorkspaceParams.setSorterCount "sorterCountMutated" sorterCountMutated
        let wsParams = wsParams |> WorkspaceParams.setSorterEvalMode "sorterEvalMode" sorterEvalMode
        let wsParams = wsParams |> WorkspaceParams.setStageWeight "stageWeight" stageWeight
        let wsParams = wsParams |> WorkspaceParams.setSwitchCount "switchCount" switchCount
        let wsParams = wsParams |> WorkspaceParams.setSwitchGenMode "switchGenMode" switchGenMode
        let wsParams = wsParams |> WorkspaceParams.setUseParallel "useParallel" useParallel


        result {

            let! gen1Cfg = 
                WsOpsLibA.initParentMapAndEval
                    wnSortableSet
                    wnSorterSetParent
                    wnSorterSetEvalParent
                    wsParams
                    emptyWsCfg

            let! wsGen1 = 
                    gen1Cfg
                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

            let! res = fs.saveWorkSpace wsGen1
            
            Console.WriteLine($"Saved Gen1 to {wsGen1 |> Workspace.getId |> WorkspaceId.value}")

            let! gen1PruneCfg = 
                 WsOpsLibA.makeMutantsAndPrune
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
                    wsParams
                    gen1Cfg

            let! wsGen1Prune = 
                    gen1PruneCfg
                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

            let! res = fs.saveWorkSpace wsGen1Prune
            Console.WriteLine($"Saved Gen1Prune to {wsGen1Prune |> Workspace.getId |> WorkspaceId.value}")

            let! wsParams2 = wsParams |> WorkspaceParams.incrGeneration "generation"

            let! gen2Cfg = 
                 WsOpsLibA.assignToNextGen
                    wnSorterSetParent
                    wnSorterSetPruned
                    wnSorterSetEvalParent
                    wnSorterSetEvalPruned
                    wsParams2
                    gen1PruneCfg

            let! wsGen2 = 
                    gen2Cfg
                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

            let! res = fs.saveWorkSpace wsGen2
            Console.WriteLine($"Saved Gen2 to {wsGen2 |> Workspace.getId |> WorkspaceId.value}")

            let! gen2PruneCfg = 
                 WsOpsLibA.makeMutantsAndPrune
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
                    wsParams2
                    gen2Cfg

            let! wsGen2Prune = 
                    gen2PruneCfg
                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

            let! res = fs.saveWorkSpace wsGen2Prune
            Console.WriteLine($"Saved Gen2Prune to { wsGen2Prune |> Workspace.getId |> WorkspaceId.value}")


            return ()
        }
