namespace global
open System
open System.IO
open ShcReportEvalsCfg


type reportCfgDto = {duType:string; cereal:string}

module ReportCfgDto =

    let toDto (cfg:reportCfg) =
        match cfg with
        | reportCfg.Shc shcRptCfg -> 
            match shcRptCfg with
            | Evals shcReportEvalsCfg ->
                {
                    duType = "shc_Evals"
                    cereal = shcReportEvalsCfg |> ShcReportEvalsCfgDto.toJson
                }
            | Bins shcReportBinsCfgDto ->
                {
                    duType = "shc_Bins"
                    cereal = shcReportBinsCfgDto |> ShcReportBinsCfgDto.toJson
                }

         | reportCfg.Ga gaRptCfg -> 
            match gaRptCfg with
            | gaReportCfg.Evals gaReportEvalsCfg ->
                {
                    duType = "ga_Evals"
                    cereal = gaReportEvalsCfg |> GaReportAllCfgDto.toJson
                }
            | gaReportCfg.Bins gaReportBinsCfg ->
                {
                    duType = "ga_Bins"
                    cereal = gaReportBinsCfg |> GaReportBinsCfgDto.toJson
                }

    let toJson (cfg:reportCfg) =
        cfg |> toDto |> Json.serialize




    let fromDto (dto:reportCfgDto) =
        match dto.duType with
        | "shc_Evals" ->
            dto.cereal |> ShcReportEvalsCfgDto.fromJson 
                       |> Result.map(shcReportCfg.Evals)
                       |> Result.map(reportCfg.Shc)
        | "shc_Bins" -> 
            dto.cereal |> ShcReportBinsCfgDto.fromJson 
                       |> Result.map(shcReportCfg.Bins)
                       |> Result.map(reportCfg.Shc)
        | "ga_Evals" ->
            dto.cereal |> GaReportAllCfgDto.fromJson 
                       |> Result.map(gaReportCfg.Evals)
                       |> Result.map(reportCfg.Ga)
        | "ga_Bins" -> 
            dto.cereal |> GaReportBinsCfgDto.fromJson 
                       |> Result.map(gaReportCfg.Bins)
                       |> Result.map(reportCfg.Ga)


        | _ -> $"{dto.duType} not handled in ReportCfgDto.fromDto" |> Error



    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<reportCfgDto> cereal
            return! fromDto dto
        }
