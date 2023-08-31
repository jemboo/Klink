namespace global

// 1.0 is neutral, higher numbers emphasize stageCount
type stageWeight = private StageWeight of float
module StageWeight =
    let value (StageWeight v) = v
    let create vl = StageWeight vl


// larger fitness numbers are better
type sorterFitness = private SorterFitness of float
module SorterFitness =
    let value (SorterFitness v) = v
    let create (spbCt: float) =
        spbCt |> SorterFitness

    let fromSpeed 
            (stageWght:stageWeight) 
            (sorterSpd:sorterSpeed) 
        = 
        (stageWght |> StageWeight.value) /
        (sorterSpd |> SorterSpeed.getStageCount |> StageCount.value |> float)
        +
        1.0 /
        (sorterSpd |> SorterSpeed.getSwitchCount |> SwitchCount.value |> float)


type selectionFraction = private SelectionFraction of float
module SelectionFraction =
    let value (SelectionFraction v) = v
    let create vl = SelectionFraction vl




type sorterSpeedBinType =
     | Parents
     | Mutants


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


type sorterSpeedBinSet = 
    private
        {
            binMap : Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>
        }

module SorterSpeedBinSet 
    = 
    let create (binMap : Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>) =
        {
            binMap = binMap
        }

    let getBinMap (ssbss:sorterSpeedBinSet) =
        ssbss.binMap

    let addBin 
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
            (binSet:sorterSpeedBinSet)
            (bins:sorterSpeedBin option seq)
        =
        let _folder 
            bMap
            (bin:sorterSpeedBin option)
            =
            match bin with
            | Some b -> (addBin b bMap)
            | None -> bMap


        let updatedMap =
            bins |> Seq.fold _folder binSet.binMap

        {binMap = updatedMap}