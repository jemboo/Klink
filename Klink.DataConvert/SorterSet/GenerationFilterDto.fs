namespace global
open System
open Microsoft.FSharp.Core


type generationFilterDto = {duType:string; cereal:string}
module GenerationFilterDto =

    let toDto (cfg:generationFilter) =
        match cfg with
        | ModF rCfg -> 
            {
                duType = "ModF"
                cereal = rCfg.modulus |> string
            }

        | ExpF cCfg ->
            {
                duType = "ExpF"
                cereal = cCfg.exp |> string
            }


    let toJson (cfg:generationFilter) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:generationFilterDto) =
        match dto.duType with
        | "ModF" ->
            { modGenerationFilter.modulus = dto.cereal |> Int32.Parse }
            |> generationFilter.ModF |> Ok
        | "ExpF" -> 
            { expGenerationFilter.exp = dto.cereal |> Double.Parse }
            |> generationFilter.ExpF |> Ok

        | _ -> $"{dto.duType} not handled in GenerationFilterDto.fromDto" |> Error


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<generationFilterDto> cereal
            return! fromDto dto
        }




