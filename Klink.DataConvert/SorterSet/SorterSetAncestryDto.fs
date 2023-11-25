namespace global
open System
open Microsoft.FSharp.Core

type genInfoDto = {
    generation:int;
    sorterId:Guid;
    sorterPhenotypeId:Guid;
    sorterFitness:float
    }

module GenInfoDto =

    let fromDto (dto:genInfoDto) =
        result {
            let generation = dto.generation |> Generation.create
            let sorterId = dto.sorterId |> SorterId.create
            let sorterPhenotypeId = dto.sorterPhenotypeId |> SorterPhenotypeId.create
            let sorterFitness = dto.sorterFitness |> SorterFitness.create
            return GenInfo.create generation sorterId sorterPhenotypeId sorterFitness
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<genInfoDto> jstr
            return! fromDto dto
        }

    let toDto (genInfo:genInfo) =
        {
            genInfoDto.generation = 
                        genInfo 
                        |> GenInfo.getGeneration 
                        |> Generation.value;
            sorterId =  genInfo 
                        |> GenInfo.getSorterId 
                        |> SorterId.value;
            sorterPhenotypeId =  
                        genInfo 
                        |> GenInfo.getSorterPhenotypeId 
                        |> SorterPhenotypeId.value;

            sorterFitness = 
                        genInfo 
                        |> GenInfo.getSorterFitness 
                        |> SorterFitness.value;

        }

    let toJson (genInfo: genInfo) =
        genInfo |> toDto |> Json.serialize



type sorterAncestryDto = {
    sorterId:Guid;
    ancestors:genInfoDto[];
    }

module SorterAncestryDto =

    let fromDto (dto:sorterAncestryDto) =
        result {
           let sorterId = dto.sorterId |> SorterId.create
           let! ancestors = 
                    dto.ancestors 
                    |> Array.map(GenInfoDto.fromDto)
                    |> Array.toList
                    |> Result.sequence

           return SorterAncestry.load sorterId (ancestors |> List.toArray)
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterAncestryDto> jstr
            return! fromDto dto
        }

    let toDto (sorterAncestry:sorterAncestry) =
        {
            sorterAncestryDto.sorterId = 
                sorterAncestry 
                |> SorterAncestry.getSorterId
                |> SorterId.value ;
            ancestors =
                sorterAncestry 
                |> SorterAncestry.getAncestors
                |> List.map(GenInfoDto.toDto)
                |> List.toArray;
        }

    let toJson (sorterAncestry: sorterAncestry) =
        sorterAncestry |> toDto |> Json.serialize




type sorterSetAncestryDto = { 
        id: Guid;
        generation:int;
        ancestors:sorterAncestryDto[];
        tag: Guid
     }

module SorterSetAncestryDto =

    let fromDto (dto:sorterSetAncestryDto) =
        result {
           let id = dto.id |> SorterSetAncestryId.create
           let generation = dto.generation |> Generation.create
           let! ancestors = 
                    dto.ancestors
                    |> Array.map(SorterAncestryDto.fromDto)
                    |> Array.toList
                    |> Result.sequence
 
           return SorterSetAncestry.load 
                    id 
                    generation
                    (ancestors |> List.toArray)
                    dto.tag
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSetAncestryDto> jstr
            return! fromDto dto
        }

    let toDto (sorterSetAncestry:sorterSetAncestry) =
        {
            sorterSetAncestryDto.id = 
                sorterSetAncestry 
                |> SorterSetAncestry.getId
                |> SorterSetAncestryId.value;

            generation = sorterSetAncestry 
                |> SorterSetAncestry.getGeneration
                |> Generation.value

            ancestors =
                sorterSetAncestry 
                |> SorterSetAncestry.getAncestorMap
                |> Map.toArray
                |> Array.map(snd)
                |> Array.map(SorterAncestryDto.toDto)

            tag = sorterSetAncestry |> SorterSetAncestry.getTag
        }

    let toJson (sorterAncestry: sorterSetAncestry) =
        sorterAncestry |> toDto |> Json.serialize