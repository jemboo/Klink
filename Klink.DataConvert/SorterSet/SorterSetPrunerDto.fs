namespace global
open Microsoft.FSharp.Core
open System


type sorterSetPrunerWholeDto = { 
        id: Guid
        selectionFraction: float;
        temp: float; 
        stageWeight:float;}

module SorterSetPrunerWholeDto =

    let fromDto (dto:sorterSetPrunerWholeDto) =
        result {
            return SorterSetPrunerWhole.load
                        (dto.id |> SorterSetPrunerId.create )
                        (dto.selectionFraction |> SelectionFraction.create)
                        (dto.temp |> Temp.create)
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
            temp = sorterSetPrunerWhole
                |> SorterSetPrunerWhole.getTemp
                |> Temp.value
            stageWeight = sorterSetPrunerWhole
                 |> SorterSetPrunerWhole.getStageWeight
                 |> StageWeight.value
        }

    let toJson (sorterSetPrunerWhole: sorterSetPrunerWhole) =
        sorterSetPrunerWhole |> toDto |> Json.serialize



type sorterSetPrunerShcDto = { 
        id: Guid
        selectionFraction: float;
        temp: float; 
        stageWeight:float;}


module SorterSetPrunerShcDto =

    let fromDto (dto:sorterSetPrunerShcDto) =
        result {
            return SorterSetPrunerShc.load
                        (dto.id |> SorterSetPrunerId.create )
                        (dto.selectionFraction |> SelectionFraction.create)
                        (dto.temp |> Temp.create)
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
            selectionFraction = sorterSetPrunerShc
                |> SorterSetPrunerShc.getSelectionFraction
                |> SelectionFraction.value
            temp = sorterSetPrunerShc
                |> SorterSetPrunerShc.getTemp
                |> Temp.value
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
        | Lhc w ->
            {
                duType = "Lhc"
                cereal = w |> SorterSetPrunerShcDto.toJson
            }

    let toJson (sorterSetPruner: sorterSetPruner) =
        sorterSetPruner |> toDto |> Json.serialize