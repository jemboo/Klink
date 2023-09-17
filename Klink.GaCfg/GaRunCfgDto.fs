namespace global
open System


type gaInitRunCfgDto =
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


module GaInitRunCfgDto =

    let toDto (cfg:gaInitRunCfg) =
        {
            newGenerations = cfg.newGenerations |> Option.get |> Generation.value
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
                    gaInitRunCfg.newGenerations = cfg.newGenerations |> Generation.create |> Some
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
    }

module GaContinueRunCfgDto =

    let toDto (cfg:gaContinueRunCfg) =
        {
            runId = cfg.runId |> RunId.value |> string
            newGenerations = cfg.newGenerations |> Generation.value

        }

    let toJson (cfg:gaContinueRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (cfg:gaContinueRunCfgDto) =
        {
            gaContinueRunCfg.runId = cfg.runId |> Guid.Parse |> RunId.create
            newGenerations = cfg.newGenerations |> Generation.create
        }


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<gaContinueRunCfgDto> cereal
            return fromDto dto
        }



type gaReportAllCfgDto =
    {
        runIds:string array
        genMin:int
        genMax:int
        evalCompName:string
        reportFileName:string
        reportGenFilter:generationFilterDto
    }


module GaReportAllCfgDto =

    let toDto (cfg:gaReportEvalsCfg) =
        {
            gaReportAllCfgDto.runIds = cfg.runIds |> Array.map(RunId.value >> string)
            genMin = cfg.genMin |> Generation.value
            genMax = cfg.genMax |> Generation.value
            evalCompName = cfg.evalCompName |> WsComponentName.value
            reportGenFilter = cfg.reportFilter |> GenerationFilterDto.toDto
            reportFileName = cfg.reportFileName
        }

    let toJson (cfg:gaReportEvalsCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:gaReportAllCfgDto) =
        result {
            let! reportFilter = dto.reportGenFilter |> GenerationFilterDto.fromDto
            return
                {
                    gaReportEvalsCfg.runIds = dto.runIds |> Array.map(fun sid -> Guid.Parse(sid) |> RunId.create)
                    genMin = dto.genMin |> Generation.create
                    genMax = dto.genMax |> Generation.create
                    evalCompName = dto.evalCompName |> WsComponentName.create
                    reportFilter = reportFilter
                    reportFileName = dto.reportFileName
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<gaReportAllCfgDto> cereal
            return! fromDto dto
        }



type gaReportBinsCfgDto =
    {
        runIds:string array
        genMin:int
        genMax:int
        reportFileName:string
    }


module GaReportBinsCfgDto =

    let toDto (cfg:gaReportBinsCfg) =
        {
            gaReportBinsCfgDto.runIds = cfg.runIds |> Array.map(RunId.value >> string)
            genMin = cfg.genMin |> Generation.value
            genMax = cfg.genMax |> Generation.value
            reportFileName = cfg.reportFileName
        }

    let toJson (cfg:gaReportBinsCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:gaReportBinsCfgDto) =
        result {
            return
                {
                    gaReportBinsCfg.runIds = dto.runIds |> Array.map(fun sid -> Guid.Parse(sid) |> RunId.create)
                    genMin = dto.genMin |> Generation.create
                    genMax = dto.genMax |> Generation.create
                    reportFileName = dto.reportFileName
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<gaReportBinsCfgDto> cereal
            return! fromDto dto
        }
