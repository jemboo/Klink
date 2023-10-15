namespace global
open System


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
