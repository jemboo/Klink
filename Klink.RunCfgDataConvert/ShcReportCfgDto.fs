namespace global
open System
open ShcReportEvalsCfg


type shcReportEvalsCfgDto =
    {
        runIds:string array
        genMin:int
        genMax:int
        evalCompName:string
        reportFileName:string
        reportGenFilter:generationFilterDto
    }


module ShcReportEvalsCfgDto =

    let toDto (cfg:shcReportEvalsCfg) =
        {
            shcReportEvalsCfgDto.runIds = cfg.runIds |> Array.map(RunId.value >> string)
            genMin = cfg.genMin |> Generation.value
            genMax = cfg.genMax |> Generation.value
            evalCompName = cfg.evalCompName |> WsComponentName.value
            reportGenFilter = cfg.reportFilter |> GenerationFilterDto.toDto
            reportFileName = cfg.reportFileName  |> ReportName.value
        }

    let toJson (cfg:shcReportEvalsCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcReportEvalsCfgDto) =
        result {
            let! reportFilter = dto.reportGenFilter |> GenerationFilterDto.fromDto
            return
                {
                    shcReportEvalsCfg.runIds = dto.runIds |> Array.map(fun sid -> Guid.Parse(sid) |> RunId.create)
                    genMin = dto.genMin |> Generation.create
                    genMax = dto.genMax |> Generation.create
                    evalCompName = dto.evalCompName |> WsComponentName.create
                    reportFilter = reportFilter
                    reportFileName = dto.reportFileName |> ReportName.create
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcReportEvalsCfgDto> cereal
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
            reportFileName = cfg.reportFileName |> ReportName.value
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
                    reportFileName = dto.reportFileName |> ReportName.create
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcReportBinsCfgDto> cereal
            return! fromDto dto
        }
