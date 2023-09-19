namespace global
open System

type runCfgSetDto = {   setName:string;
                        runs:runCfgDto[] }

module RunCfgSetDto =
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