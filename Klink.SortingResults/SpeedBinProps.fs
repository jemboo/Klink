namespace global

open System


type sorterSpeedBinProps =
     | ErrorMsg 
     | BestSwitchCt
     | BestStageCt
     | BestFitness
     | BestEntropy
     | AveSwitchCt
     | AveStageCt
     | AveFitness
     | DevSwitchCt
     | DevStageCt
     | DevFitness
     | PhenotypeEntropy
     | BestSwitchCtM
     | BestStageCtM
     | BestFitnessM
     | BestEntropyM
     | AveSwitchCtM
     | AveStageCtM
     | AveFitnessM
     | DevSwitchCtM
     | DevStageCtM
     | DevFitnessM
     | PhenotypeEntropyM





module SpeedBinProps 
       =
    let getMapForSpeedBinType 
            (ssbs:sorterSpeedBinSet) 
            (ssbt:sorterSpeedBinType) = 
        ssbs |> SorterSpeedBinSet.getBinMap
             |> Map.toSeq
             |> Seq.filter(fun (k, v) -> k.sorterSpeedBinType = ssbt)
             |> Map.ofSeq

    let getKvpWithMinStages
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>) =
        binMap |> Map.toSeq |> Seq.minBy(fun (k,v) -> k.sorterSpeed.stageCt |> StageCount.value) 


    let getKvpWithMinSwitches
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>) =
        binMap |> Map.toSeq |> Seq.minBy(fun (k,v) -> k.sorterSpeed.switchCt |> SwitchCount.value)


    let orderByFitnessDesc
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>) 
        =
        binMap |> Map.toArray
               |> Array.map(
               fun (k,v) -> ((k,v), k.sorterSpeed |> SorterFitness.fromSpeed stageWeight))
               |> Array.sortByDescending(snd >> SorterFitness.value)

    let getBestSorterSpeed
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        orderByFitnessDesc stageWeight binMap  
        |> Seq.head |> fst |> fst |> SorterSpeedBinKey.getSorterSpeed


    let getBestFitness
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
        if yab.Length = 0 then
            0.0
        else
            yab |> Seq.head |> snd |> SorterFitness.value


    let getBestStageCount
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
        if yab.Length = 0 then
            0
        else
            yab |> Seq.head |> fst |> fst |> SorterSpeedBinKey.getSorterSpeed 
                |> SorterSpeed.getStageCount |> StageCount.value


    let getBestSwitchCount
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
        if yab.Length = 0 then
            0
        else
            yab |> Seq.head |> fst |> fst |> SorterSpeedBinKey.getSorterSpeed 
                |> SorterSpeed.getSwitchCount |> SwitchCount.value



    let getBestSpeedBinEntropy
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
        if yab.Length = 0 then
            0.0
        else
            yab |> Seq.head |> fst |> snd |> Map.toArray |> Array.map(snd >> SorterCount.value)
                |> Combinatorics.entropyOfInts



    let getAllSorterSpeedBinKeysWithCounts
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        binMap |> Map.toArray
               |> Array.map(
               fun (k,v) -> ((k, v |> Map.toSeq |> Seq.sumBy(snd >> SorterCount.value))))


    let getAveFitness
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterFitness.fromSpeed stageWeight 
                 |> SorterFitness.value,
                ct))
            |> CollectionProps.weightedAverage


    let getAveSwitchCount
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterSpeed.getSwitchCount 
                 |> SwitchCount.value, ct))
        |> CollectionProps.weightedAverage


    let getAveStageCount
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterSpeed.getStageCount 
                 |> StageCount.value, ct))
        |> CollectionProps.weightedAverage


    let getStDevSwitchCount
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterSpeed.getSwitchCount 
                 |> SwitchCount.value, ct))
        |> CollectionProps.weightedStdDeviationS


    let getStDevStageCount
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterSpeed.getStageCount 
                 |> StageCount.value, ct))
        |> CollectionProps.weightedStdDeviationS



    let getStDevFitness
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        getAllSorterSpeedBinKeysWithCounts binMap 
        |> Array.map(fun (sbk, ct) -> 
            (sbk |> SorterSpeedBinKey.getSorterSpeed 
                 |> SorterFitness.fromSpeed stageWeight 
                 |> SorterFitness.value,
                ct))
            |> CollectionProps.weightedStdDeviationS


    let getPhenotypeEntropy
            (stageWeight:stageWeight)
            (binMap:Map<sorterSpeedBinKey, Map<sorterPhenotypeId,sorterCount>>)
        =
        let yab = orderByFitnessDesc stageWeight binMap
                    |> Seq.map(fst >> snd >> Map.toArray >> Array.map(snd >> SorterCount.value))
                    |> Seq.concat |> Seq.toArray
        yab |> Combinatorics.entropyOfInts






    
    let getAllProps =
        [
            sorterSpeedBinProps.AveFitness;
            sorterSpeedBinProps.AveStageCt;
            sorterSpeedBinProps.AveSwitchCt;
            sorterSpeedBinProps.BestEntropy;
            sorterSpeedBinProps.BestFitness;
            sorterSpeedBinProps.BestStageCt;
            sorterSpeedBinProps.BestSwitchCt;
            sorterSpeedBinProps.DevFitness;
            sorterSpeedBinProps.DevStageCt;
            sorterSpeedBinProps.DevSwitchCt;
            sorterSpeedBinProps.PhenotypeEntropy;
            sorterSpeedBinProps.AveFitnessM;
            sorterSpeedBinProps.AveStageCtM;
            sorterSpeedBinProps.AveSwitchCtM;
            sorterSpeedBinProps.BestEntropyM;
            sorterSpeedBinProps.BestFitnessM;
            sorterSpeedBinProps.BestStageCtM;
            sorterSpeedBinProps.BestSwitchCtM;
            sorterSpeedBinProps.DevFitnessM;
            sorterSpeedBinProps.DevStageCtM;
            sorterSpeedBinProps.DevSwitchCtM;
            sorterSpeedBinProps.PhenotypeEntropyM;
        ]
        
    let getHeader () =
        getAllProps 
            |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""



    let getAllProperties
            (stageWeight:stageWeight)
            (ssbs:sorterSpeedBinSet) 
        =
        let parentType = "sorterSetParent" |> SorterSpeedBinType.create
        let mutantType = "sorterSetMutated" |> SorterSpeedBinType.create
        let parentBins = getMapForSpeedBinType ssbs parentType
        let mutantBins = getMapForSpeedBinType ssbs mutantType

        let aveFitness = getAveFitness stageWeight parentBins
        let aveStageCt = getAveStageCount parentBins
        let aveSwitchCt = getAveSwitchCount parentBins
        let bestEntropy = getBestSpeedBinEntropy stageWeight parentBins
        let bestFitness = getBestFitness stageWeight parentBins
        let bestStageCt = getBestStageCount stageWeight parentBins
        let bestSwitchCt = getBestSwitchCount stageWeight parentBins
        let devFitness = getStDevFitness stageWeight parentBins
        let devStageCt = getStDevStageCount parentBins
        let devSwitchCt = getStDevSwitchCount parentBins
        let phenotypeEntropy = getPhenotypeEntropy stageWeight parentBins
        let aveFitnessM = getAveFitness stageWeight mutantBins
        let aveStageCtM = getAveStageCount mutantBins
        let aveSwitchCtM = getAveSwitchCount mutantBins
        let bestEntropyM = getBestSpeedBinEntropy stageWeight mutantBins
        let bestFitnessM = getBestFitness stageWeight mutantBins
        let bestStageCtM = getBestStageCount stageWeight mutantBins
        let bestSwitchCtM = getBestSwitchCount stageWeight mutantBins
        let devFitnessM = getStDevFitness stageWeight mutantBins
        let devStageCtM = getStDevStageCount mutantBins
        let devSwitchCtM = getStDevSwitchCount mutantBins
        let phenotypeEntropyM = getPhenotypeEntropy stageWeight mutantBins

        [
         aveFitness;
         aveStageCt;
         aveSwitchCt;
         bestEntropy;
         bestFitness;
         bestStageCt;
         bestSwitchCt;
         devFitness;
         devStageCt;
         devSwitchCt;
         phenotypeEntropy;
         aveFitnessM;
         aveStageCtM;
         aveSwitchCtM;
         bestEntropyM;
         bestFitnessM;
         bestStageCtM;
         bestSwitchCtM;
         devFitnessM;
         devStageCtM;
         devSwitchCtM;
         phenotypeEntropyM;
        ] |> List.fold(fun st t -> sprintf "%s\t%A" st t) ""
