namespace global
open System


module WsParamsGen = 

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
        
    let forGa 
            (runId:runId) 
            (stageWeight:stageWeight) 
            (noiseFraction:noiseFraction option) 
            (mutationRate:mutationRate) 
        =

        let randy = (runId |> RunId.value) 
                        |> RandomSeed.create |> RngGen.createLcg
                        |> Rando.fromRngGen

        let nextRngGen () =
            randy |> Rando.toRngGen

        let rngGenCreate = (nextRngGen ())
        let rngGenMutate = (nextRngGen ())
        let rngGenPrune = (nextRngGen ())


        let generation = 1 |> Generation.create
        let order = 16 |> Order.createNr
        let sorterCount = SorterCount.create 16
        let switchCount = SwitchCount.orderTo999SwitchCount order 
        let switchGenMode = switchGenMode.Stage
        let sorterCountMutated = SorterCount.create 32
        let mutationRate = mutationRate
        let noiseFraction = noiseFraction
        //let noiseFraction = None
        let sorterEvalMode = sorterEvalMode.DontCheckSuccess
        let stageWeight = stageWeight
        let useParallel = true |> UseParallel.create

        WorkspaceParams.make Map.empty
        |> WorkspaceParams.setRunId "runId" runId
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