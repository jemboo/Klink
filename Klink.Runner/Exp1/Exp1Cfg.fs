namespace global
open System

type exp1Cfg0 =
    {
        mutationRate:mutationRate
        noiseFraction:noiseFraction
        order:order
        rngGen:rngGen
        sorterEvalMode:sorterEvalMode
        sorterCount:sorterCount
        sorterCountMutated:sorterCount
        sorterSetPruneMethod:sorterSetPruneMethod
        stageWeight:stageWeight
        orderToSwitchCount:orderToSwitchCount
        switchGenMode:switchGenMode
    }


module Exp1CfgOld =

    let rndGens = 
        let rngGenSeed = 1234 |> RandomSeed.create |> RngGen.createLcg
        rngGenSeed |> Rando.toMoreRngGens


    let sliceOfRndGens (modulo:int) =
        rndGens |> Seq.mapi(fun dex rndg -> (dex, rndg))
                |> Seq.filter(fun tup -> ((fst tup) % 16) = modulo)


    let defltCfg =
        {
            exp1Cfg0.mutationRate = 0.01 |> MutationRate.create
            exp1Cfg0.noiseFraction = 0.01 |> NoiseFraction.create
            exp1Cfg0.order = 16 |> Order.createNr
            exp1Cfg0.rngGen = 1234 |> RandomSeed.create |> RngGen.createLcg
            exp1Cfg0.sorterEvalMode = sorterEvalMode.DontCheckSuccess
            exp1Cfg0.sorterCount = 1 |> SorterCount.create
            exp1Cfg0.sorterCountMutated = 2 |> SorterCount.create
            exp1Cfg0.sorterSetPruneMethod = sorterSetPruneMethod.Whole
            exp1Cfg0.stageWeight = 0.1 |> StageWeight.create
            exp1Cfg0.orderToSwitchCount = orderToSwitchCount.For999
            exp1Cfg0.switchGenMode = switchGenMode.Stage
        }


    let getRunId (cfg:exp1Cfg0) =
        [

            cfg.mutationRate |> MutationRate.value :> obj;
            cfg.noiseFraction |> NoiseFraction.value :> obj;
            cfg.order |> Order.value :> obj;
            cfg.rngGen :> obj;
            cfg.sorterEvalMode :> obj
            cfg.sorterCount |> SorterCount.value :> obj;
            cfg.sorterCountMutated |> SorterCount.value :> obj;
            cfg.sorterSetPruneMethod :> obj;
            cfg.stageWeight |> StageWeight.value :> obj;
            cfg.orderToSwitchCount :> obj;
            cfg.switchGenMode :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create


    let toWorkspaceParams 
            (useParallel:useParallel)
            (gaCfg:exp1Cfg0)
        =
        let _nextRngGen rng =
            rng
            |> Rando.fromRngGen
            |> Rando.toRngGen

        let rngGenCreate = (_nextRngGen gaCfg.rngGen)
        let rngGenMutate = (_nextRngGen rngGenCreate)
        let rngGenPrune = (_nextRngGen rngGenMutate)
        let switchCount = gaCfg.order |> SwitchCount.fromOrder (gaCfg.orderToSwitchCount)

        WorkspaceParams.make Map.empty
        |> WorkspaceParams.setRunId "runId" (gaCfg |> getRunId)
        |> WorkspaceParams.setRngGen "rngGenCreate" rngGenCreate
        |> WorkspaceParams.setRngGen "rngGenMutate" rngGenMutate
        |> WorkspaceParams.setRngGen "rngGenPrune" rngGenPrune
        |> WorkspaceParams.setMutationRate "mutationRate" gaCfg.mutationRate
        |> WorkspaceParams.setNoiseFraction "noiseFraction" (Some gaCfg.noiseFraction)
        |> WorkspaceParams.setOrder "order" gaCfg.order
        |> WorkspaceParams.setSorterCount "sorterCount" gaCfg.sorterCount
        |> WorkspaceParams.setSorterCount "sorterCountMutated" gaCfg.sorterCountMutated
        |> WorkspaceParams.setSorterEvalMode "sorterEvalMode" gaCfg.sorterEvalMode
        |> WorkspaceParams.setStageWeight "stageWeight" gaCfg.stageWeight
        |> WorkspaceParams.setSwitchCount "sorterLength" switchCount
        |> WorkspaceParams.setSwitchGenMode "switchGenMode" gaCfg.switchGenMode
        |> WorkspaceParams.setSorterSetPruneMethod "sorterSetPruneMethod" gaCfg.sorterSetPruneMethod
        |> WorkspaceParams.setUseParallel "useParallel" useParallel


    let enumerate
            (rngGens:rngGen seq)
            (sorterSetSizes: (sorterCount*sorterCount) seq)
            (switchGenModes:switchGenMode seq)
            (stageWeights:stageWeight seq) 
            (noiseFractions:noiseFraction seq) 
            (mutationRates:mutationRate seq)
            (sorterSetPruneMethods:sorterSetPruneMethod seq)
            (maxGen:generation)
        =
            seq {
            
              for rngGen in rngGens do
                  for sorterSetSize in sorterSetSizes do
                      for switchGenMode in switchGenModes do
                        for stageWeight in stageWeights do
                            for noiseFraction in noiseFractions do
                                for mutationRate in mutationRates do
                                    for sorterSetPruneMethod in sorterSetPruneMethods do
                                        yield
                                            { defltCfg with
                                                rngGen = rngGen;
                                                sorterCount = fst sorterSetSize
                                                sorterCountMutated = snd sorterSetSize
                                                stageWeight = stageWeight;
                                                noiseFraction = noiseFraction;
                                                mutationRate = mutationRate;
                                                sorterSetPruneMethod = sorterSetPruneMethod
                                                switchGenMode = switchGenMode
                                            }
            }



//type exp1CfgDto = 
//    { 
//        curGen:generation
//        maxGen:generation
//        mutationRate:mutationRate
//        noiseFraction:noiseFraction
//        order:order
//        rngGenDto:rngGenDto

//    }


//type sparseIntArrayDto = 
//    { 
//      emptyVal:int; 
//      length:int; 
//      indexes:int[];
//      values:int[]
//    }

//module SparseIntArrayDto =

//    let fromDto 
//            (dto:sparseIntArrayDto) 
//        =
//        result {
//            return 
//                SparseArray.create
//                    dto.length
//                    dto.indexes
//                    dto.values
//                    dto.emptyVal
//        }

//    let toDto 
//            (sia:sparseArray<int>) 
//        =
//        { 
//            emptyVal = sia |> SparseArray.getEmptyVal; 
//            length = sia |> SparseArray.getLength; 
//            indexes = sia |> SparseArray.getIndexes;
//            values =  sia |> SparseArray.getValues
//        }
        
//    let fromJson 
//            (cereal:string)
//        =
//        result {
//            let! dto = Json.deserialize<sparseIntArrayDto> cereal
//            return! fromDto dto
//        }

//    let toJson 
//            (sia:sparseArray<int>) 
//        = 
//        sia |> toDto |> Json.serialize