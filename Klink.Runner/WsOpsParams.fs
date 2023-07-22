namespace global
open System


module gaWs = 

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

    let rngGen = 123 |> RandomSeed.create |> RngGen.createLcg
    let randy = rngGen |> Rando.fromRngGen
    let nextRngGen () =
        randy |> Rando.toRngGen

    let rngGenCreate = (nextRngGen ())
    let rngGenMutate = (nextRngGen ())
    let rngGenPrune = (nextRngGen ())
        
    let wsPs () =

        let generation = 1 |> Generation.create
        let order = 16 |> Order.createNr
        let sorterCount = SorterCount.create 4
        let switchCount = SwitchCount.orderTo999SwitchCount order 
        let switchGenMode = switchGenMode.Stage
        let sorterCountMutated = SorterCount.create 8
        let mutationRate = 0.05 |> MutationRate.create
        let noiseFraction = 0.05 |> NoiseFraction.create |> Some
        //let noiseFraction = None
        let sorterEvalMode = sorterEvalMode.DontCheckSuccess
        let stageWeight = 0.5 |> StageWeight.create
        let useParallel = true |> UseParallel.create

        WorkspaceParams.make Map.empty
        |> WorkspaceParams.setRngGen "rngGenCreate" rngGenCreate
        |> WorkspaceParams.setRngGen "rngGenMutate" rngGenMutate
        |> WorkspaceParams.setRngGen "rngGenPrune" rngGenPrune
        |> WorkspaceParams.setGeneration "generation" generation
        |> WorkspaceParams.setMutationRate "mutationRate" mutationRate
        |> WorkspaceParams.setNoiseFraction "noiseFraction" noiseFraction
        |> WorkspaceParams.setOrder "order" order
        |> WorkspaceParams.setSorterCount "sorterCount" sorterCount
        |> WorkspaceParams.setSorterCount "sorterCountMutated" sorterCountMutated
        |> WorkspaceParams.setSorterEvalMode "sorterEvalMode" sorterEvalMode
        |> WorkspaceParams.setStageWeight "stageWeight" stageWeight
        |> WorkspaceParams.setSwitchCount "sorterLength" switchCount
        |> WorkspaceParams.setSwitchGenMode "switchGenMode" switchGenMode
        |> WorkspaceParams.setUseParallel "useParallel" useParallel