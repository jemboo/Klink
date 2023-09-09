namespace global

open System


type sorterSpeedBinType = SorterSpeedBinType of string
module SorterSpeedBinType =
    let value (SorterSpeedBinType v) = v
    let create vl = SorterSpeedBinType vl


type sorterSpeedBin = 
    private
        {
            successful:bool option
            sorterSpeedBinType:sorterSpeedBinType
            sorterSpeed:sorterSpeed
            sorterPhenotypeId:sorterPhenotypeId
        }


module SorterSpeedBin =
    let make (successful: bool option)
             (sorterSpeedBinType: sorterSpeedBinType) 
             (sorterSpeed: sorterSpeed)
             (sorterPhenotypeId: sorterPhenotypeId)
        =
        { sorterSpeedBin.successful = successful
          sorterSpeedBinType = sorterSpeedBinType
          sorterSpeed = sorterSpeed
          sorterPhenotypeId = sorterPhenotypeId
        }

    let getSuccessful (sorterSpeedBin:sorterSpeedBin) =
        sorterSpeedBin.successful
    let getSorterSpeedBinType (sorterSpeedBin:sorterSpeedBin) =
        sorterSpeedBin.sorterSpeedBinType
    let getSorterSpeed (sorterSpeedBin:sorterSpeedBin) =
        sorterSpeedBin.sorterSpeed
    let getSorterPhenotypeId (sorterSpeedBin:sorterSpeedBin) =
        sorterSpeedBin.sorterPhenotypeId

    let fromSorterEval 
            (order:order)
            (binType:sorterSpeedBinType) 
            (sorterEval:sorterEval)
        =
        let speed = sorterEval |> SorterEval.getSorterSpeed
        let sorterPhenotypeId = sorterEval |> SorterEval.getSortrPhenotypeId
        let success = 
            sorterEval
            |> SorterEval.getSorterPerf
            |> Option.map(SorterPerf.isSuccessful order)

        match speed, sorterPhenotypeId with
        | _, None -> None
        | None, _ -> None
        | Some ss, Some p ->  
            make success binType ss p
            |> Some        
        

type sorterSpeedBinKey = 
    private
        {
            successful:bool option
            sorterSpeedBinType:sorterSpeedBinType
            sorterSpeed:sorterSpeed
        }


module SorterSpeedBinKey =
    let make (successful: bool option)
             (sorterSpeedBinType: sorterSpeedBinType) 
             (sorterSpeed: sorterSpeed)
        =
        { sorterSpeedBinKey.successful = successful
          sorterSpeedBinType = sorterSpeedBinType
          sorterSpeed = sorterSpeed
        }

    let getSuccessful (sorterSpeedBin:sorterSpeedBinKey) =
        sorterSpeedBin.successful
    let getSorterSpeedBinType (sorterSpeedBin:sorterSpeedBinKey) =
        sorterSpeedBin.sorterSpeedBinType
    let getSorterSpeed (sorterSpeedBin:sorterSpeedBinKey) =
        sorterSpeedBin.sorterSpeed
    let fromSorterSpeedBin (spBin:sorterSpeedBin) =
        make spBin.successful spBin.sorterSpeedBinType spBin.sorterSpeed


type sorterSpeedBinSetId = private SorterSpeedBinSetId of Guid

module SorterSpeedBinSetId =
    let value (SorterSpeedBinSetId v) = v
    let create (id: Guid) =
        id |> SorterSpeedBinSetId

type sorterSpeedBinSet = 
    private
        {
            id: sorterSpeedBinSetId;
            binMap : Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>
            tag:Guid
        }

module SorterSpeedBinSet 
    = 
    let load (binMap : Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>) 
             (id:sorterSpeedBinSetId)
             (tag:Guid) 
        =
        {
            id = id
            binMap = binMap
            tag = tag
        }

    let create (binMap : Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
               (generation:generation)
               (tag:Guid)    =
        let sorterSpeedBinSetId  = 
                [|
                  tag:> obj;
                  generation |> Generation.value :> obj;
                  "sorterSpeedBinSet" :> obj;
                |] |> GuidUtils.guidFromObjs  
                   |> SorterSpeedBinSetId.create

        load binMap sorterSpeedBinSetId tag


    let getBinMap (ssbss:sorterSpeedBinSet) =
        ssbss.binMap

    let getId (ssbss:sorterSpeedBinSet) =
        ssbss.id

    let getTag (ssbss:sorterSpeedBinSet) =
        ssbss.tag


    let private _addBin
            (bin:sorterSpeedBin) 
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        let key = bin |> SorterSpeedBinKey.fromSorterSpeedBin
        let phId = bin |> SorterSpeedBin.getSorterPhenotypeId
        let oneSc = 1 |> SorterCount.create
        if  binMap.ContainsKey(key) then
            let pMap = binMap[key]
            let nuMap = 
                if pMap.ContainsKey(phId) then
                   let curCt = pMap[phId] |> SorterCount.add oneSc
                   pMap.Add (phId, curCt)
                else
                   pMap.Add (phId, oneSc)
            binMap.Add (key, nuMap)
        else
            binMap.Add (key, [| (phId, oneSc) |] |> Map.ofArray)


    let addBins
            (sorterSpeedBinSet:sorterSpeedBinSet)
            (generation:generation)
            (bins:sorterSpeedBin option seq)
        =
        let _folder 
            bMap
            (bin:sorterSpeedBin option)
            =
            match bin with
            | Some b -> (_addBin b bMap)
            | None -> bMap


        let updatedBinMap =
            bins |> Seq.fold _folder sorterSpeedBinSet.binMap

        create updatedBinMap generation sorterSpeedBinSet.tag







//type sorterSpeedBinKey2 = 
//    private
//        {
//            successful:bool option
//            sorterSpeed:sorterSpeed
//        }


//module SorterSpeedBinKey2 =
//    let make (successful: bool option)
//             (sorterSpeed: sorterSpeed)
//        =
//        { 
//          sorterSpeedBinKey2.successful = successful
//          sorterSpeed = sorterSpeed
//        }

//    let getSuccessful (sorterSpeedBin:sorterSpeedBinKey2) =
//        sorterSpeedBin.successful
//    let getSorterSpeed (sorterSpeedBin:sorterSpeedBinKey2) =
//        sorterSpeedBin.sorterSpeed
//    let fromSorterSpeedBin (spBin:sorterSpeedBin) =
//        make spBin.successful spBin.sorterSpeed




//type sorterSpeedBinGroup = 
//    private
//        {
//            sorterSpeedBinType:sorterSpeedBinType
//            binMap : Map<sorterSpeedBinKey2, int[]>
//        }

//module SorterSpeedBinGroup 
//    = 
//      let getSorterSpeedBinType (binGroup:sorterSpeedBinGroup) =
//          binGroup.sorterSpeedBinType

//      let getBinMap (binGroup:sorterSpeedBinGroup) =
//          binGroup.binMap




//type sorterSpeedBinSet2 = 
//    private
//        {
//            id: sorterSpeedBinSetId;
//            sorterSpeedBinType:sorterSpeedBinType
//            binMap : Map<sorterSpeedBinKey2, int[]>
//        }

//module SorterSpeedBinSet2
//    = 

//      let getId (ssbss:sorterSpeedBinSet) =
//          ssbss.id

//      let getBinMap (binGroup:sorterSpeedBinGroup) =
//          binGroup.binMap


////    let load (binMap : Map<sorterSpeedBinKey2, Map<sorterPhenotypeId,sorterCount>>)
////             (sorterSpeedBinType: sorterSpeedBinType) 
////        =
////        {
////            sorterSpeedBinType = sorterSpeedBinType
////            binMap = binMap
////        }

////    let create (binMap : Map<sorterSpeedBinKey2, Map<sorterPhenotypeId,sorterCount>>)
////               (tag:Guid)    =
////        let sorterSpeedBinSetId  = 
////                [|
////                  tag:> obj;
////                  generation |> Generation.value :> obj;
////                  "sorterSpeedBinSet" :> obj;
////                |] |> GuidUtils.guidFromObjs  
////                   |> SorterSpeedBinSetId.create

////        load binMap sorterSpeedBinSetId tag


////    let getBinMap (ssbss:sorterSpeedBinGroup) =
////        ssbss.binMap

////    let getSorterSpeedBinType (sorterSpeedBin:sorterSpeedBinGroup) =
////        sorterSpeedBin.sorterSpeedBinType



////    let addBin
////            (bin:sorterSpeedBin) 
////            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
////        =
////        let key = bin |> SorterSpeedBinKey.fromSorterSpeedBin
////        let phId = bin |> SorterSpeedBin.getSorterPhenotypeId
////        let oneSc = 1 |> SorterCount.create
////        if  binMap.ContainsKey(key) then
////            let pMap = binMap[key]
////            let nuMap = 
////                if pMap.ContainsKey(phId) then
////                   let curCt = pMap[phId] |> SorterCount.add oneSc
////                   pMap.Add (phId, curCt)
////                else
////                   pMap.Add (phId, oneSc)
////            binMap.Add (key, nuMap)
////        else
////            binMap.Add (key, [| (phId, oneSc) |] |> Map.ofArray)


////    let addBins
////            (sorterSpeedBinSet:sorterSpeedBinSet)
////            (generation:generation)
////            (bins:sorterSpeedBin option seq)
////        =
////        let _folder 
////            bMap
////            (bin:sorterSpeedBin option)
////            =
////            match bin with
////            | Some b -> (addBin b bMap)
////            | None -> bMap


////        let updatedBinMap =
////            bins |> Seq.fold _folder sorterSpeedBinSet.binMap

////        create updatedBinMap generation sorterSpeedBinSet.tag






