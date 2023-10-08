namespace global
open System




type scriptItemDto = 
    { itemType:string;
      cereal:string  }


module ScriptItemDto =
    let toDto (item:scriptItem) =
        match item with
        | Run runCfg -> 
            {
                scriptItemDto.itemType = "Run";
                cereal = runCfg |> RunCfgDto.toJson
            }
        | Report reportCfg -> 
            {
                scriptItemDto.itemType = "Report";
                cereal = reportCfg |> ReportCfgDto.toJson
            }


    let toJson (cfg:scriptItem) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:scriptItemDto) =
        match dto.itemType with
        | "Run" -> 
            result {
                let! yab = dto.cereal |> RunCfgDto.fromJson
                return yab |> scriptItem.Run
            }
        | "Report" -> 
            result {
                let! yab = dto.cereal |> ReportCfgDto.fromJson
                return yab |> scriptItem.Report
            }
        | yab -> $"{yab} is not handled in ScriptItemDto.fromDto" |> Error


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<scriptItemDto> cereal
            return! fromDto dto
        }




type klinkScriptDto = {   name:string;
                          items:scriptItemDto[] }

module KlinkScriptDto =
    let toDto (runCfgSet:runCfgSet) =
        {
            runCfgSetDto.setName = runCfgSet.setName;
            runs = runCfgSet.runCfgs |> Array.map(RunCfgDto.toDto)
        }

    let toJson (cfg:runCfgSet) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:runCfgSetDto) =
        result {
            let! runs = dto.runs 
                       |> Array.map(RunCfgDto.fromDto)
                       |> Array.toList 
                       |> Result.sequence
            return
                {
                    runCfgSet.setName = dto.setName;
                    runCfgs = runs |> List.toArray
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<runCfgSetDto> cereal
            return! fromDto dto
        }