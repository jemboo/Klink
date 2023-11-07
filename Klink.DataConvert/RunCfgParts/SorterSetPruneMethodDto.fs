namespace global
open System
open Microsoft.FSharp.Core


//type sorterSetPruneMethod = 
//    | Whole
//    | PhenotypeCap of sorterPhenotypeCount
//    | Shc

type sorterSetPruneMethodDto = {duType:string; cereal:string}

module SorterSetPruneMethodDto =

    let toDto (sspm:sorterSetPruneMethod) =
        match sspm with
        | Whole -> 
            {
                duType = "Whole"
                cereal = ""
            }

        | PhenotypeCap cCfg ->
            {
                duType = "PhenotypeCap"
                cereal = cCfg |> SorterPhenotypeCount.value |> string
            }
        | Shc  ->
            {
                duType = "Shc"
                cereal = ""
            }


    let toJson (sspm:sorterSetPruneMethod) =
        sspm |> toDto |> Json.serialize


    let fromDto (dto:sorterSetPruneMethodDto) =
        match dto.duType with
        | "Whole" ->
             sorterSetPruneMethod.Whole |> Ok
        | "PhenotypeCap" -> 
            let phenoCap = 
                dto.cereal 
                    |> Int32.Parse
                    |> SorterPhenotypeCount.create
            phenoCap |> sorterSetPruneMethod.PhenotypeCap |> Ok
        | "Shc" -> 
             sorterSetPruneMethod.Shc |> Ok

        | _ -> $"{dto.duType} not handled in SorterSetPruneTypeDto.fromDto" |> Error


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<sorterSetPruneMethodDto> cereal
            return! fromDto dto
        }




