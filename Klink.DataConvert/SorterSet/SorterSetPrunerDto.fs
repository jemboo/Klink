namespace global
open Microsoft.FSharp.Core
open System


type sorterSetPrunerWholeDto = { 
        id: Guid
        selectionFraction: float; 
        stageWeight:float;}

module SorterSetPrunerWholeDto =

    let fromDto (dto:sorterSetPrunerWholeDto) =
        result {
            return SorterSetPrunerWhole.load
                        (dto.id |> SorterSetPrunerId.create )
                        (dto.selectionFraction |> SelectionFraction.create)
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
            selectionFraction = sorterSetPrunerWhole
                |> SorterSetPrunerWhole.getSelectionFraction
                |> SelectionFraction.value
            stageWeight = sorterSetPrunerWhole
                 |> SorterSetPrunerWhole.getStageWeight
                 |> StageWeight.value
        }

    let toJson (sorterSetMutator: sorterSetPrunerWhole) =
        sorterSetMutator |> toDto |> Json.serialize



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

    let toJson (sorterSetPruner: sorterSetPruner) =
        sorterSetPruner |> toDto |> Json.serialize