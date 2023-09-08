namespace global
open System
open Microsoft.FSharp.Core


type sorterSpeedBinKeyDto = {
        sorterSpeedDto:sorterSpeedDto;
        sorterSpeedBinType : string
        successful: bool option
    }

module SorterSpeedBinKeyDto =

    let fromDto (dto:sorterSpeedBinKeyDto) =
        result {
            let! sorterSpeed = 
                    dto.sorterSpeedDto 
                    |> SorterSpeedDto.fromDto
            return SorterSpeedBinKey.make 
                        dto.successful 
                        (dto.sorterSpeedBinType |> SorterSpeedBinType.create)
                        sorterSpeed
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSpeedBinKeyDto> jstr
            return! fromDto dto
        }


    let toDto (ssBk:sorterSpeedBinKey) =
        {
            sorterSpeedBinKeyDto.sorterSpeedDto = 
                ssBk 
                |> SorterSpeedBinKey.getSorterSpeed
                |> SorterSpeedDto.toDto;
            sorterSpeedBinType =  
                ssBk 
                |> SorterSpeedBinKey.getSorterSpeedBinType |> SorterSpeedBinType.value
            successful = ssBk 
                |> SorterSpeedBinKey.getSuccessful
        }

    let toJson (sorterSpeedBin: sorterSpeedBinKey) =
        sorterSpeedBin |> toDto |> Json.serialize


type sorterSpeedBinSetDto =
    {
        id: Guid
        binMap:(sorterSpeedBinKeyDto*Map<Guid,int>) array;
        tag:Guid
    }

module SorterSpeedBinSetDto =

    let fromDto (dto:sorterSpeedBinSetDto) =
        let _fromKvp (bkDto, (m:Map<Guid,int>)) =
            result {
               let! bk = bkDto |> SorterSpeedBinKeyDto.fromDto
               let mp = m |> Map.toSeq
                          |> Seq.map(fun (gu,ctv)->(gu |> SorterPhenotypeId.create, ctv|>SorterCount.create))
                          |> Map.ofSeq
               return (bk, mp)
            }
            
        result {
            let! kvps = 
                dto.binMap
                     |> Seq.map(_fromKvp)
                     |> Seq.toList
                     |> Result.sequence

            return SorterSpeedBinSet.load 
                        (kvps |> Map.ofList)
                        (dto.id |> SorterSpeedBinSetId.create)
                        dto.tag
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sorterSpeedBinSetDto> jstr
            return! fromDto dto
        }


    let toDto (sorterSpeedBinSet:sorterSpeedBinSet) =
        let _fromKvp (ssbk, (m:Map<sorterPhenotypeId,sorterCount>)) =
            let bk = ssbk |> SorterSpeedBinKeyDto.toDto
            let mp = m |> Map.toSeq
                        |> Seq.map(fun (pid, ctv)->(pid |> SorterPhenotypeId.value, ctv |> SorterCount.value))
                        |> Map.ofSeq
            (bk, mp)

        let binMap = sorterSpeedBinSet 
                        |> SorterSpeedBinSet.getBinMap 
                        |> Map.toSeq 
                        |> Seq.map(_fromKvp)
                        |> Seq.toArray
        {
            binMap = binMap
            id = sorterSpeedBinSet 
                    |> SorterSpeedBinSet.getId
                    |> SorterSpeedBinSetId.value
            tag = sorterSpeedBinSet
                    |> SorterSpeedBinSet.getTag
        }

    let toJson (sorterSpeedBinSet: sorterSpeedBinSet) =
        sorterSpeedBinSet |> toDto |> Json.serialize




////type sorterSpeedBinDto = {
////        sorterSpeedDto:sorterSpeedDto;
////        sorterSpeedBinType : string
////        successful: bool option
////    }

////module SorterSpeedBinKeyDto =

////    let fromDto (dto:sorterSpeedBinKeyDto) =
////        result {
////            let! sorterSpeed = 
////                    dto.sorterSpeedDto 
////                    |> SorterSpeedDto.fromDto
////            return SorterSpeedBinKey.make 
////                        dto.successful 
////                        (dto.sorterSpeedBinType |> SorterSpeedBinType.create)
////                        sorterSpeed
////        }

////    let fromJson (jstr: string) =
////        result {
////            let! dto = Json.deserialize<sorterSpeedBinKeyDto> jstr
////            return! fromDto dto
////        }


////    let toDto (ssBk:sorterSpeedBinKey) =
////        {
////            sorterSpeedBinKeyDto.sorterSpeedDto = 
////                ssBk 
////                |> SorterSpeedBinKey.getSorterSpeed
////                |> SorterSpeedDto.toDto;
////            sorterSpeedBinType =  
////                ssBk 
////                |> SorterSpeedBinKey.getSorterSpeedBinType |> SorterSpeedBinType.value
////            successful = ssBk 
////                |> SorterSpeedBinKey.getSuccessful
////        }

////    let toJson (sorterSpeedBin: sorterSpeedBinKey) =
////        sorterSpeedBin |> toDto |> Json.serialize






//type sorterSpeedBinGroupDto =
//    {
//        sorterSpeedBinType : string
//        speedBins:int array array;
//    }

//module SorterSpeedBinGroupDto =

//    let fromJson (jstr: string) =
//        result {
//            let! dto = Json.deserialize<sorterSpeedBinGroupDto> jstr
//            return dto
//        }


//    let toDto (binType:sorterSpeedBinType) (yabs: sorterSpeedBinKey*Map<sorterPhenotypeId, sorterCount> array) =
//        let _doink (yabs: sorterSpeedBinKey*Map<sorterPhenotypeId, sorterCount>) =
//            let sb = 
//                [|
//                yabs |> fst |> SorterSpeedBinKey.getSorterSpeed |> SorterSpeed.getStageCount |> StageCount.value
//                yabs |> fst |> SorterSpeedBinKey.getSorterSpeed |> SorterSpeed.getSwitchCount |> SwitchCount.value
//                |]
//            let wak = yabs |> snd |> Map.toSeq |> Seq.map(snd >> SorterCount.value) |> Seq.toArray
//            ()

//        {
//            sorterSpeedBinType = binType |> SorterSpeedBinType.value
//            speedBins = [||]
//        }

//    let toJson (binType:sorterSpeedBinType) =
//        binType |> toDto |> Json.serialize





//type sorterSpeedBinSetDto2 =
//    {
//        id:Guid
//        sorterSpeedBinGroupDtos :sorterSpeedBinGroupDto array array;
//    }

//module SorterSpeedBinSetDto2 =

//    let fromJson (jstr: string) =
//        result {
//            let! dto = Json.deserialize<sorterSpeedBinGroupDto> jstr
//            return dto
//        }


//    let toDto (sorterSpeedBinSet:sorterSpeedBinSet) =
//        let yab = sorterSpeedBinSet 
//                    |> SorterSpeedBinSet.getBinMap
//                    |> Map.toArray
//                    |> Array.groupBy(fun (k,v) -> k |> SorterSpeedBinKey.getSorterSpeedBinType)

//        {
//            id = sorterSpeedBinSet 
//                    |> SorterSpeedBinSet.getId
//                    |> SorterSpeedBinSetId.value

//            sorterSpeedBinGroupDtos = [||]
//        }

//        //let _fromKvp (ssbk, (m:Map<sorterPhenotypeId,sorterCount>)) =
//        //    let bk = ssbk |> SorterSpeedBinKeyDto.toDto
//        //    let mp = m |> Map.toSeq
//        //                |> Seq.map(fun (pid, ctv)->(pid |> SorterPhenotypeId.value, ctv |> SorterCount.value))
//        //                |> Map.ofSeq
//        //    (bk, mp)

//        //let binMap = sorterSpeedBinSet 
//        //                |> SorterSpeedBinSet.getBinMap 
//        //                |> Map.toSeq 
//        //                |> Seq.map(_fromKvp)
//        //                |> Seq.toArray
//        //{
//        //    binMap = binMap
//        //    id = sorterSpeedBinSet 
//        //            |> SorterSpeedBinSet.getId
//        //            |> SorterSpeedBinSetId.value
//        //    tag = sorterSpeedBinSet
//        //            |> SorterSpeedBinSet.getTag
//        //}

//    let toJson (sorterSpeedBinSet: sorterSpeedBinSet) =
//        sorterSpeedBinSet |> toDto |> Json.serialize

