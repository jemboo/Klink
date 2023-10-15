namespace global
open System


type shcInitRunCfgDto =
    {
        newGenerations:int
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


module ShcInitRunCfgDto =

    let toDto (cfg:shcInitRunCfg) =
        {
            newGenerations = cfg.newGenerations |> Generation.value
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
            (cfg:shcInitRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (cfg:shcInitRunCfgDto) =
        result {
            let! reportFilter = cfg.reportGenFilter |> GenerationFilterDto.fromDto
            return
                {
                    shcInitRunCfg.newGenerations = cfg.newGenerations |> Generation.create
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
            let! dto = Json.deserialize<shcInitRunCfgDto> cereal
            return! fromDto dto
        }


type shcContinueRunCfgDto =
    {
        runId:string
        newGenerations:int
    }

module ShcContinueRunCfgDto =

    let toDto (cfg:shcContinueRunCfg) =
        {
            runId = cfg.runId |> RunId.value |> string
            newGenerations = cfg.newGenerations |> Generation.value

        }

    let toJson (cfg:shcContinueRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (cfg:shcContinueRunCfgDto) =
        {
            shcContinueRunCfg.runId = cfg.runId |> Guid.Parse |> RunId.create
            newGenerations = cfg.newGenerations |> Generation.create
        }


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcContinueRunCfgDto> cereal
            return fromDto dto
        }
