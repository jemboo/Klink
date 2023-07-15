namespace global
open System


module WsOps = 


    let makeEm (rootDir:string) =
        let order = 16 |> Order.createNr
        let rngGen = 123 |> RandomSeed.create |> RngGen.createLcg
        let randy = rngGen |> Rando.fromRngGen
        let nextRngGen () =
            randy |> Rando.nextRngGen

        let sorterCount = SorterCount.create 32
        let switchGenMode = switchGenMode.Stage
        let sorterCountMutated = SorterCount.create 64
        let mutationRate = 0.1 |> MutationRate.create
        let noiseFraction = Some 0.5
        //let noiseFraction = None
        let stageWeight = 1.0 |> StageWeight.create


                
        let wnRandoCreate = "randoCreate" |> WsComponentName.create
        let wnRandoMutate = "randoMutate" |> WsComponentName.create
        let wnRandoPrune = "randoPrune" |> WsComponentName.create
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




        result {

            let! gen1Cfg = 
                WsOpsLibA.initParentMapAndEval
                    wnRandoCreate
                    wnSortableSet
                    wnSorterSetParent
                    wnSorterSetEvalParent
                    order
                    (nextRngGen ())
                    sorterCount
                    switchGenMode
                    emptyWsCfg

            let! wsGen1 = 
                    gen1Cfg
                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

            let! res = fs.saveWorkSpace wsGen1
            
            Console.WriteLine($"Saved Gen1 to {wsGen1 |> Workspace.getId |> WorkspaceId.value}")

            let! gen1PruneCfg = 
                 WsOpsLibA.makeMutantsAndPrune
                    wnRandoMutate
                    wnRandoPrune
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
                    order
                    (nextRngGen ())
                    sorterCount
                    sorterCountMutated
                    mutationRate
                    noiseFraction
                    switchGenMode
                    stageWeight
                    gen1Cfg

            let! wsGen1Prune = 
                    gen1PruneCfg
                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

            let! res = fs.saveWorkSpace wsGen1Prune
            Console.WriteLine($"Saved Gen1Prune to {wsGen1Prune |> Workspace.getId |> WorkspaceId.value}")

            let! gen2Cfg = 
                 WsOpsLibA.assignToNextGen
                    wnSorterSetParent
                    wnSorterSetPruned
                    wnSorterSetEvalParent
                    wnSorterSetEvalPruned
                    gen1PruneCfg

            let! wsGen2 = 
                    gen2Cfg
                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

            let! res = fs.saveWorkSpace wsGen2
            Console.WriteLine($"Saved Gen2 to {wsGen2 |> Workspace.getId |> WorkspaceId.value}")

            let! gen2PruneCfg = 
                 WsOpsLibA.makeMutantsAndPrune
                    wnRandoMutate
                    wnRandoPrune
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
                    order
                    (nextRngGen ())
                    sorterCount
                    sorterCountMutated
                    mutationRate
                    noiseFraction
                    switchGenMode
                    stageWeight
                    gen1Cfg

            let! wsGen2 = 
                    gen2PruneCfg
                        |> WorkspaceCfg.loadWorkspace fs (fun s-> Console.WriteLine(s))

            let! res = fs.saveWorkSpace wsGen2
            Console.WriteLine($"Saved Gen2Prune to { wsGen2 |> Workspace.getId |> WorkspaceId.value}")


            return ()
        }
