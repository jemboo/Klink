namespace global
open System


type gaInitRunCfgDto =
    {
        newGenerations:int
        sortableSetCfgType:sortableSetCfgType
        mutationRate:float
        noiseFraction:float
        order:int
        rngGen:rngGenDto
        sorterEvalMode:sorterEvalMode
        sorterCount:int
        sorterCountMutated:int
        sorterSetPruneMethod:sorterSetPruneMethod
        stagesSkipped:int
        stageWeight:float
        switchCount:int
        switchGenMode:switchGenMode
        reportGenFilter:generationFilterDto
    }


module GaInitRunCfgDto =

    let toDto (cfg:gaInitRunCfg) =
        {
            newGenerations = cfg.newGenerations |> Generation.value
            sortableSetCfgType = cfg.sortableSetCfgType
            mutationRate = cfg.mutationRate |> MutationRate.value
            noiseFraction = cfg.noiseFraction |> NoiseFraction.value
            order = cfg.order |> Order.value
            rngGen = cfg.rngGen |> RngGenDto.toDto
            sorterEvalMode = cfg.sorterEvalMode
            sorterCount = cfg.sorterCount |> SorterCount.value
            sorterCountMutated = cfg.sorterCountMutated |> SorterCount.value
            sorterSetPruneMethod = cfg.sorterSetPruneMethod
            stagesSkipped = cfg.stagesSkipped |> StageCount.value
            stageWeight = cfg.stageWeight |> StageWeight.value
            switchCount = cfg.switchCount |> SwitchCount.value
            switchGenMode = cfg.switchGenMode
            reportGenFilter = cfg.reportFilter  |> Option.get |> GenerationFilterDto.toDto
        }


    let toJson 
            (cfg:gaInitRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (cfg:gaInitRunCfgDto) =
        result {
            let! reportFilter = cfg.reportGenFilter |> GenerationFilterDto.fromDto
            return
                {
                    gaInitRunCfg.newGenerations = cfg.newGenerations |> Generation.create
                    sortableSetCfgType = cfg.sortableSetCfgType
                    mutationRate = cfg.mutationRate |> MutationRate.create
                    noiseFraction = cfg.noiseFraction |> NoiseFraction.create
                    order = cfg.order |> Order.createNr
                    rngGen = cfg.rngGen |> RngGenDto.fromDto |> Result.ExtractOrThrow
                    sorterEvalMode = cfg.sorterEvalMode
                    sorterCount = cfg.sorterCount |> SorterCount.create
                    sorterCountMutated = cfg.sorterCountMutated |> SorterCount.create
                    sorterSetPruneMethod = cfg.sorterSetPruneMethod
                    stagesSkipped = cfg.stagesSkipped |> StageCount.create
                    stageWeight = cfg.stageWeight |> StageWeight.create
                    switchCount = cfg.switchCount |> SwitchCount.create
                    switchGenMode = cfg.switchGenMode
                    reportFilter = reportFilter |> Some
                }
        }

    let fromJson 
            (cereal:string)
        =
        result {
            let! dto = Json.deserialize<gaInitRunCfgDto> cereal
            return! fromDto dto
        }


type gaContinueRunCfgDto =
    {
        runId:string
        newGenerations:int
        reportGenFilter:generationFilterDto
    }

module GaContinueRunCfgDto =

    let toDto (cfg:gaContinueRunCfg) =
        {
            runId = cfg.runId |> RunId.value |> string
            newGenerations = cfg.newGenerations |> Generation.value
            reportGenFilter = cfg.reportGenFilter |> GenerationFilterDto.toDto
        }

    let toJson (cfg:gaContinueRunCfg) =
        cfg |> toDto |> Json.serialize

    let fromDto (cfg:gaContinueRunCfgDto) =
        result {
            let! reportFilter = cfg.reportGenFilter |> GenerationFilterDto.fromDto
            return
                {
                    gaContinueRunCfg.runId = cfg.runId |> Guid.Parse |> RunId.create
                    newGenerations = cfg.newGenerations |> Generation.create
                    reportGenFilter = reportFilter
                }
        }


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<gaContinueRunCfgDto> cereal
            return! fromDto dto
        }
