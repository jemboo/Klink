namespace global
open Microsoft.FSharp.Core
open System


type sorterSetPrunerWholeDto = { 
        id: Guid
        prunedCount: int;
        noiseFraction: float option; 
        stageWeight:float;
        }


module SorterSetPrunerWholeDto =

    let fromDto (dto:sorterSetPrunerWholeDto) =
        result {
            return SorterSetPruner.load
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


    let toDto (sorterSetPrunerWhole: sorterSetPruner) =
        {
            id = sorterSetPrunerWhole 
                 |> SorterSetPruner.getId
                 |> SorterSetPrunerId.value
            prunedCount = sorterSetPrunerWhole
                |> SorterSetPruner.getPrunedCount
                |> SorterCount.value
            noiseFraction = sorterSetPrunerWhole
                |> SorterSetPruner.getNoiseFraction
            stageWeight = sorterSetPrunerWhole
                 |> SorterSetPruner.getStageWeight
                 |> StageWeight.value
        }


    let toJson (sorterSetPrunerWhole: sorterSetPruner) 
        =
        sorterSetPrunerWhole |> toDto |> Json.serialize



//type sorterSetPrunerShcDto = { 
//        id: Guid
//        prunedCount: int;
//        noiseFraction: float option; 
//        stageWeight:float;
//        }


//module SorterSetPrunerShcDto =

//    let fromDto (dto:sorterSetPrunerShcDto) =
//        result {
//            return SorterSetPrunerShc.load
//                        (dto.id |> SorterSetPrunerId.create )
//                        (dto.prunedCount |> SorterCount.create)
//                        (dto.noiseFraction)
//                        (dto.stageWeight |> StageWeight.create)
//        }

//    let fromJson (jstr: string) =
//        result {
//            let! dto = Json.deserialize<sorterSetPrunerShcDto> jstr
//            return! fromDto dto
//        }

//    let toDto (sorterSetPrunerShc: sorterSetPrunerShc) =
//        {
//            id = sorterSetPrunerShc 
//                 |> SorterSetPrunerShc.getId 
//                 |> SorterSetPrunerId.value
//            prunedCount = sorterSetPrunerShc
//                |> SorterSetPrunerShc.getPrunedCount
//                |> SorterCount.value
//            noiseFraction = sorterSetPrunerShc
//                |> SorterSetPrunerShc.getNoiseFraction
//            stageWeight = sorterSetPrunerShc
//                 |> SorterSetPrunerShc.getStageWeight
//                 |> StageWeight.value

//        }

//    let toJson (sorterSetPrunerShc: sorterSetPrunerShc) =
//        sorterSetPrunerShc |> toDto |> Json.serialize



//type sorterSetPrunerDto = { 
//        duType: string
//        cereal: string; }

//module SorterSetPrunerDto =

//    let fromDto (dto:sorterSetPrunerDto) =
//        result {
//            match dto.duType with
//            | "Whole" ->
//                return! dto.cereal 
//                        |> SorterSetPrunerWholeDto.fromJson
//                        |> Result.map(sorterSetPruner.Whole)
                                
//            | _ -> 
//                return! "not handled (009)" |> Error
//        }

//    let fromJson (jstr: string) =
//        result {
//            let! dto = Json.deserialize<sorterSetPrunerDto> jstr
//            return! fromDto dto
//        }

//    let toDto (sorterSetPruner: sorterSetPruner) =
//        match sorterSetPruner with
//        | Whole w ->
//            {
//                duType = "Whole"
//                cereal = w |> SorterSetPrunerWholeDto.toJson
//            }
//        | Shc w ->
//            {
//                duType = "Lhc"
//                cereal = w |> SorterSetPrunerShcDto.toJson
//            }
//        | Batch b -> failwith "not implemented"

//    let toJson (sorterSetPruner: sorterSetPruner) =
//        sorterSetPruner |> toDto |> Json.serialize



type metaDataMapDto = { 
        id: Guid
        data: Map<string,string>
     }

module MetaDataMapDto =

    let fromDto (dto:metaDataMapDto) =
        MetaDataMap.load
            (dto.id |> MetaDataMapId.create)
            (dto.data)

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<metaDataMapDto> jstr
            return fromDto dto
        }

    let toDto (metaDataMap: metaDataMap) =
        {
            metaDataMapDto.id = metaDataMap |> MetaDataMap.getId |> MetaDataMapId.value
            data = metaDataMap |> MetaDataMap.getData
        }

    let toJson (metaDataMap: metaDataMap) =
        metaDataMap |> toDto |> Json.serialize