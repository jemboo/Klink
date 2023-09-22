namespace global
open System


type runCfgDto = {duType:string; cereal:string}
module RunCfgDto =

    let toDto (cfg:runCfg) =
        match cfg with
        | runCfg.Shc shcRc ->
            match shcRc with
            | shcRunCfg.InitRun rCfg -> 
                {
                    duType = "Run_shc"
                    cereal = rCfg |> ShcInitRunCfgDto.toJson
                }

            | shcRunCfg.Continue cCfg ->
                {
                    duType = "Continue_shc"
                    cereal = cCfg |> ShcContinueRunCfgDto.toJson
                }

            //| shcRunCfg.Report cRpt ->
            //    match cRpt with
            //    | shcReportCfg.Evals arc ->
            //        {
            //            duType = "Report_All_shc"
            //            cereal = arc |> ShcReportAllCfgDto.toJson
            //        }
            //    | shcReportCfg.Bins brc ->
            //        {
            //            duType = "Report_Bins_shc"
            //            cereal = brc |> ShcReportBinsCfgDto.toJson
            //        }
        | runCfg.Ga gaRc ->
            match gaRc with
            | gaRunCfg.InitRun rCfg -> 
                {
                    duType = "Run_ga"
                    cereal = rCfg |> GaInitRunCfgDto.toJson
                }

            | gaRunCfg.Continue cCfg ->
                {
                    duType = "Continue_ga"
                    cereal = cCfg |> GaContinueRunCfgDto.toJson
                }

            //| gaRunCfg.Report cRpt ->
            //    match cRpt with
            //    | gaReportCfg.Evals arc ->
            //        {
            //            duType = "Report_All_ga"
            //            cereal = arc |> GaReportAllCfgDto.toJson
            //        }
            //    | gaReportCfg.Bins brc ->
            //        {
            //            duType = "Report_Bins_ga"
            //            cereal = brc |> GaReportBinsCfgDto.toJson
            //        }


    let toJson (cfg:runCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:runCfgDto) =
        match dto.duType with
        | "Run_shc" ->
            dto.cereal |> ShcInitRunCfgDto.fromJson 
                       |> Result.map(shcRunCfg.InitRun)
                       |> Result.map(runCfg.Shc)
        | "Continue_shc" -> 
            dto.cereal |> ShcContinueRunCfgDto.fromJson 
                       |> Result.map(shcRunCfg.Continue)
                       |> Result.map(runCfg.Shc)
        //| "Report_All_shc" -> 
        //    dto.cereal |> ShcReportAllCfgDto.fromJson 
        //               |> Result.map(shcReportCfg.Evals >> shcRunCfg.Report)
        //               |> Result.map(runCfg.Shc)
        //| "Report_Bins_shc" -> 
        //    dto.cereal |> ShcReportBinsCfgDto.fromJson 
        //               |> Result.map(shcReportCfg.Bins >> shcRunCfg.Report)
        //               |> Result.map(runCfg.Shc)

        | "Run_ga" ->
            dto.cereal |> GaInitRunCfgDto.fromJson 
                       |> Result.map(gaRunCfg.InitRun)
                       |> Result.map(runCfg.Ga)
        | "Continue_ga" -> 
            dto.cereal |> GaContinueRunCfgDto.fromJson 
                       |> Result.map(gaRunCfg.Continue)                       
                       |> Result.map(runCfg.Ga)
        //| "Report_All_ga" -> 
        //    dto.cereal |> GaReportAllCfgDto.fromJson 
        //               |> Result.map(gaReportCfg.Evals >> gaRunCfg.Report)
        //               |> Result.map(runCfg.Ga)
        //| "Report_Bins_ga" -> 
        //    dto.cereal |> GaReportBinsCfgDto.fromJson 
        //               |> Result.map(gaReportCfg.Bins >> gaRunCfg.Report)
        //               |> Result.map(runCfg.Ga)

        | _ -> $"{dto.duType} not handled in ShcCfgDto.fromDto" |> Error


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<runCfgDto> cereal
            return! fromDto dto
        }