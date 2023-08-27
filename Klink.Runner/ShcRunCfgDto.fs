﻿namespace global
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
            stageWeight = cfg.stageWeight |> StageWeight.value
            switchCount = cfg.switchCount |> SwitchCount.value
            switchGenMode = cfg.switchGenMode
            reportGenFilter = cfg.reportFilter |> GenerationFilterDto.toDto
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
                    stageWeight = cfg.stageWeight |> StageWeight.create
                    switchCount = cfg.switchCount |> SwitchCount.create
                    switchGenMode = cfg.switchGenMode
                    reportFilter = reportFilter
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



type shcReportCfgDto =
    {
        runIds:string array
        genMin:int
        genMax:int
        evalCompName:string
        reportFreqFunc:string
        reportFreqParams:string array
    }


module ShcReportCfgDto =

    let toDto (cfg:shcReportCfg) =
        {
            shcReportCfgDto.runIds = cfg.runIds |> Array.map(RunId.value >> string)
            genMin = cfg.genMin |> Generation.value
            genMax = cfg.genMax |> Generation.value
            evalCompName = cfg.evalCompName |> WsComponentName.value
            reportFreqFunc = cfg.reportFreqFunc
            reportFreqParams = cfg.reportFreqParams
        }

    let toJson (cfg:shcReportCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcReportCfgDto) =
        {
            shcReportCfg.runIds = dto.runIds |> Array.map(fun sid -> Guid.Parse(sid) |> RunId.create)
            genMin = dto.genMin |> Generation.create
            genMax = dto.genMax |> Generation.create
            evalCompName = dto.evalCompName |> WsComponentName.create
            reportFreqFunc = dto.reportFreqFunc
            reportFreqParams = dto.reportFreqParams
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcReportCfgDto> cereal
            return fromDto dto
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
            {
                duType = "Report"
                cereal = cRpt |> ShcReportCfgDto.toJson
            }

    let toJson (cfg:shcRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcCfgDto) =
        match dto.duType with
        | "Run" ->
            dto.cereal |> ShcInitRunCfgDto.fromJson |> Result.map(shcRunCfg.InitRun)
        | "Continue" -> 
            dto.cereal |> ShcContinueRunCfgDto.fromJson |> Result.map(shcRunCfg.Continue)
        | "Report" -> 
            dto.cereal |> ShcReportCfgDto.fromJson |> Result.map(shcRunCfg.Report)
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