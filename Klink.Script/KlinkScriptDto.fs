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
    let toDto (klinkScript:klinkScript) =
        {
            klinkScriptDto.name = klinkScript.name |> ScriptName.value;
            items = klinkScript.items |> Array.map(ScriptItemDto.toDto)
        }

    let toJson (klinkScript:klinkScript) =
        klinkScript |> toDto |> Json.serialize


    let fromDto (dto:klinkScriptDto) =
        result {
            let! items = dto.items 
                       |> Array.map(ScriptItemDto.fromDto)
                       |> Array.toList 
                       |> Result.sequence
            return
                {
                    klinkScript.name = dto.name |> ScriptName.create;
                    items = items |> List.toArray
                }
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<klinkScriptDto> cereal
            return! fromDto dto
        }