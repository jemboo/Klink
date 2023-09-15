namespace global
open System


type runCfgDto = {duType:string; cereal:string}
module RunCfgDto =

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


    let fromDto (dto:runCfgDto) =
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
            let! dto = Json.deserialize<runCfgDto> cereal
            return! fromDto dto
        }


type runCfgSetDto = {setName:string; runs:runCfgDto[]}

module RunCfgSetDto =
    let toDto (runSet:shcRunCfgSet) =
        {
            runCfgSetDto.setName = runSet.setName;
            runs = runSet.runCfgs |> Array.map(RunCfgDto.toDto)
        }

    let toJson (cfg:shcRunCfgSet) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:runCfgSetDto) =
        result {
            let! runs = dto.runs 
                       |> Array.map(RunCfgDto.fromDto)
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
            let! dto = Json.deserialize<runCfgSetDto> cereal
            return! fromDto dto
        }