namespace global

module ShcWsParamKeys =
    let runId = "runId" |> WorkspaceParamsKey.create
    let generation_current = "generation_current" |> WorkspaceParamsKey.create
    let generation_filter_short = "generation_filter_short" |> WorkspaceParamsKey.create
    let generation_filter_long = "generation_filter_long" |> WorkspaceParamsKey.create
    let generation_max = "generation_max" |> WorkspaceParamsKey.create
    let rngGenCreate = "rngGenCreate" |> WorkspaceParamsKey.create
    let rngGenMutate = "rngGenMutate" |> WorkspaceParamsKey.create
    let rngGenPrune = "rngGenPrune" |> WorkspaceParamsKey.create
    let mutationRate = "mutationRate" |> WorkspaceParamsKey.create
    let noiseFraction = "noiseFraction" |> WorkspaceParamsKey.create
    let order = "order" |> WorkspaceParamsKey.create
    let sortableSetCfgType = "sortableSetCfgType" |> WorkspaceParamsKey.create
    let sorterCount = "sorterCount" |> WorkspaceParamsKey.create
    let sorterCountMutated = "sorterCountMutated" |> WorkspaceParamsKey.create
    let sorterEvalMode = "sorterEvalMode" |> WorkspaceParamsKey.create
    let stagesSkipped = "stagesSkipped" |> WorkspaceParamsKey.create
    let sorterLength = "sorterLength" |> WorkspaceParamsKey.create
    let stageWeight = "stageWeight" |> WorkspaceParamsKey.create
    let switchGenMode = "switchGenMode" |> WorkspaceParamsKey.create
    let sortableSetId = "sortableSetId" |> WorkspaceParamsKey.create
    let sorterSetPruneMethod = "sorterSetPruneMethod" |> WorkspaceParamsKey.create
    let useParallel = "useParallel" |> WorkspaceParamsKey.create
