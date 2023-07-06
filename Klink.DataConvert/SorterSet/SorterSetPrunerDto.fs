namespace global
open Microsoft.FSharp.Core
open System


type sorterSetPrunerWholeDto = { 
        id: Guid
        prunedCount: int;
        noiseFraction: float option; 
        stageWeight:float;}

module SorterSetPrunerWholeDto =

    let fromDto (dto:sorterSetPrunerWholeDto) =
        result {
            return SorterSetPrunerWhole.load
                        (dto.id |> SorterSetPrunerId.create )
                        (dto.prunedCount |> SorterCount.create)
                        (dto.noiseFraction)
                        (dto.stageWeight |> StageWeight.create)
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetPrunerWholeDto> jstr
            return! fromDto dto
        }

    let toDto (sorterSetPrunerWhole: sorterSetPrunerWhole) =
        {
            id = sorterSetPrunerWhole 
                 |> SorterSetPrunerWhole.getId 
                 |> SorterSetPrunerId.value
            prunedCount = sorterSetPrunerWhole
                |> SorterSetPrunerWhole.getPrunedCount
                |> SorterCount.value
            noiseFraction = sorterSetPrunerWhole
                |> SorterSetPrunerWhole.getNoiseFraction
            stageWeight = sorterSetPrunerWhole
                 |> SorterSetPrunerWhole.getStageWeight
                 |> StageWeight.value
        }

    let toJson (sorterSetPrunerWhole: sorterSetPrunerWhole) =
        sorterSetPrunerWhole |> toDto |> Json.serialize



type sorterSetPrunerShcDto = { 
        id: Guid
        prunedCount: int;
        noiseFraction: float option; 
        stageWeight:float;}


module SorterSetPrunerShcDto =

    let fromDto (dto:sorterSetPrunerShcDto) =
        result {
            return SorterSetPrunerShc.load
                        (dto.id |> SorterSetPrunerId.create )
                        (dto.prunedCount |> SorterCount.create)
                        (dto.noiseFraction)
                        (dto.stageWeight |> StageWeight.create)
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetPrunerShcDto> jstr
            return! fromDto dto
        }

    let toDto (sorterSetPrunerShc: sorterSetPrunerShc) =
        {
            id = sorterSetPrunerShc 
                 |> SorterSetPrunerShc.getId 
                 |> SorterSetPrunerId.value
            prunedCount = sorterSetPrunerShc
                |> SorterSetPrunerShc.getPrunedCount
                |> SorterCount.value
            noiseFraction = sorterSetPrunerShc
                |> SorterSetPrunerShc.getNoiseFraction
            stageWeight = sorterSetPrunerShc
                 |> SorterSetPrunerShc.getStageWeight
                 |> StageWeight.value
        }

    let toJson (sorterSetPrunerShc: sorterSetPrunerShc) =
        sorterSetPrunerShc |> toDto |> Json.serialize



type sorterSetPrunerDto = { 
        duType: string
        cereal: string; }

module SorterSetPrunerDto =

    let fromDto (dto:sorterSetPrunerDto) =
        result {
            match dto.duType with
            | "Whole" ->
                return! dto.cereal 
                        |> SorterSetPrunerWholeDto.fromJson
                        |> Result.map(sorterSetPruner.Whole)
                                
            | _ -> 
                return! "not handled" |> Error
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetPrunerDto> jstr
            return! fromDto dto
        }

    let toDto (sorterSetPruner: sorterSetPruner) =
        match sorterSetPruner with
        | Whole w ->
            {
                duType = "Whole"
                cereal = w |> SorterSetPrunerWholeDto.toJson
            }
        | Shc w ->
            {
                duType = "Lhc"
                cereal = w |> SorterSetPrunerShcDto.toJson
            }
        | Batch b -> failwith "not implemented"

    let toJson (sorterSetPruner: sorterSetPruner) =
        sorterSetPruner |> toDto |> Json.serialize