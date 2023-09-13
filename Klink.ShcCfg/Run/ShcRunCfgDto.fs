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
            (cfg:shcInitRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (cfg:shcInitRunCfgDto) =
        result {
            let! reportFilter = cfg.reportGenFilter |> GenerationFilterDto.fromDto
            return
                {
                    shcInitRunCfg.newGenerations = cfg.newGenerations |> Generation.create |> Some
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

    let toDto (cfg:shcContinueRunCfgs) =
        {
            runId = cfg.runId |> RunId.value |> string
            newGenerations = cfg.newGenerations |> Generation.value

        }

    let toJson (cfg:shcContinueRunCfgs) =
        cfg |> toDto |> Json.serialize


    let fromDto (cfg:shcContinueRunCfgDto) =
        {
            shcContinueRunCfgs.runId = cfg.runId |> Guid.Parse |> RunId.create
            newGenerations = cfg.newGenerations |> Generation.create
        }


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcContinueRunCfgDto> cereal
            return fromDto dto
        }



type shcReportAllCfgDto =
    {
        runIds:string array
        genMin:int
        genMax:int
        evalCompName:string
        reportFileName:string
        reportGenFilter:generationFilterDto
    }


module ShcReportAllCfgDto =

    let toDto (cfg:shcReportEvalsCfg) =
        {
            shcReportAllCfgDto.runIds = cfg.runIds |> Array.map(RunId.value >> string)
            genMin = cfg.genMin |> Generation.value
            genMax = cfg.genMax |> Generation.value
            evalCompName = cfg.evalCompName |> WsComponentName.value
            reportGenFilter = cfg.reportFilter |> GenerationFilterDto.toDto
            reportFileName = cfg.reportFileName
        }

    let toJson (cfg:shcReportEvalsCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcReportAllCfgDto) =
        result {
            let! reportFilter = dto.reportGenFilter |> GenerationFilterDto.fromDto
            return
                {
                    shcReportEvalsCfg.runIds = dto.runIds |> Array.map(fun sid -> Guid.Parse(sid) |> RunId.create)
                    genMin = dto.genMin |> Generation.create
                    genMax = dto.genMax |> Generation.create
                    evalCompName = dto.evalCompName |> WsComponentName.create
                    reportFilter = reportFilter
                    reportFileName = dto.reportFileName
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcReportAllCfgDto> cereal
            return! fromDto dto
        }



type shcReportBinsCfgDto =
    {
        runIds:string array
        genMin:int
        genMax:int
        reportFileName:string
    }


module ShcReportBinsCfgDto =

    let toDto (cfg:shcReportBinsCfg) =
        {
            shcReportBinsCfgDto.runIds = cfg.runIds |> Array.map(RunId.value >> string)
            genMin = cfg.genMin |> Generation.value
            genMax = cfg.genMax |> Generation.value
            reportFileName = cfg.reportFileName
        }

    let toJson (cfg:shcReportBinsCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcReportBinsCfgDto) =
        result {
            return
                {
                    shcReportBinsCfg.runIds = dto.runIds |> Array.map(fun sid -> Guid.Parse(sid) |> RunId.create)
                    genMin = dto.genMin |> Generation.create
                    genMax = dto.genMax |> Generation.create
                    reportFileName = dto.reportFileName
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcReportBinsCfgDto> cereal
            return! fromDto dto
        }



type shcCfgDto = {duType:string; cereal:string}
module ShcRunCfgDto =

    let toDto (cfg:shcRunCfg) =
        match cfg with
        | InitRun rCfg -> 
            {
                duType = "Run"
                cereal = rCfg |> ShcInitRunCfgDto.toJson
            }

        | Continue cCfg ->
            {
                duType = "Continue"
                cereal = cCfg |> ShcContinueRunCfgDto.toJson
            }

        | Report cRpt ->
            match cRpt with
            | Evals arc ->
                {
                    duType = "Report_All"
                    cereal = arc |> ShcReportAllCfgDto.toJson
                }
            | Bins brc ->
                {
                    duType = "Report_Bins"
                    cereal = brc |> ShcReportBinsCfgDto.toJson
                }



    let toJson (cfg:shcRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcCfgDto) =
        match dto.duType with
        | "Run" ->
            dto.cereal |> ShcInitRunCfgDto.fromJson |> Result.map(shcRunCfg.InitRun)
        | "Continue" -> 
            dto.cereal |> ShcContinueRunCfgDto.fromJson |> Result.map(shcRunCfg.Continue)
        | "Report_All" -> 
            dto.cereal |> ShcReportAllCfgDto.fromJson |> Result.map(shcReportCfg.Evals >> shcRunCfg.Report)
        | "Report_Bins" -> 
            dto.cereal |> ShcReportBinsCfgDto.fromJson |> Result.map(shcReportCfg.Bins >> shcRunCfg.Report)

        | _ -> $"{dto.duType} not handled in ShcCfgDto.fromDto" |> Error


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcCfgDto> cereal
            return! fromDto dto
        }


type shcRunCfgSetDto = {setName:string; runs:shcCfgDto[]}

module ShcRunCfgSetDto =
    let toDto (runSet:shcRunCfgSet) =
        {
            shcRunCfgSetDto.setName = runSet.setName;
            runs = runSet.runCfgs |> Array.map(ShcRunCfgDto.toDto)
        }

    let toJson (cfg:shcRunCfgSet) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcRunCfgSetDto) =
        result {
            let! runs = dto.runs 
                       |> Array.map(ShcRunCfgDto.fromDto)
                       |> Array.toList 
                       |> Result.sequence
            return
                {
                    shcRunCfgSet.setName = dto.setName;
                    runCfgs = runs |> List.toArray
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcRunCfgSetDto> cereal
            return! fromDto dto
        }