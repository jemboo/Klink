namespace global
open Microsoft.FSharp.Core
open System


type sorterSetMutatorDto = { 
        id: Guid
        sorterCountFinal: int; 
        sorterMutatorDto:sorterMutatorDto;
      }


module SorterSetMutatorDto =

    let fromDto (dto:sorterSetMutatorDto) =
        result {
            let! sorterMutator = dto.sorterMutatorDto |> SorterMutatorDto.fromDto
            let sorterCountFinal =
                match dto.sorterCountFinal with
                | -1 -> None
                | v -> v |> SorterCount.create |> Some

            return SorterSetMutator.load
                        (dto.id |> SorterSetMutatorId.create )
                        sorterMutator
                        sorterCountFinal
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetMutatorDto> jstr
            return! fromDto dto
        }

    let toDto (sorterSetMutator: sorterSetMutator) =
        {
            id = sorterSetMutator 
                 |> SorterSetMutator.getId 
                 |> SorterSetMutatorId.value

            sorterCountFinal =
                match (sorterSetMutator |> SorterSetMutator.getSorterCountFinal) with
                | None -> -1
                | Some v -> v |> SorterCount.value

            sorterMutatorDto =
                sorterSetMutator 
                |> SorterSetMutator.getSorterMutator
                |> SorterMutatorDto.toDto
        }

    let toJson (sorterSetMutator: sorterSetMutator) =
        sorterSetMutator |> toDto |> Json.serialize

