namespace global
open System

type gaCfg =
    {
        curGen:generation
        maxGen:generation
        mutationRate:mutationRate
        noiseFraction:noiseFraction
        order:order
        rngGen:rngGen
        sorterEvalMode:sorterEvalMode
        sorterCount:sorterCount
        sorterCountMutated:sorterCount
        sorterSetPruneMethod:sorterSetPruneMethod
        stageWeight:stageWeight
        switchCountCalcMethod:switchCountCalcMethod
        switchGenMode:switchGenMode
        useParallel:useParallel
    }


module GaCfg =

    let defltCfg = 
        {
            gaCfg.curGen = 0 |> Generation.create
            gaCfg.maxGen = 10 |> Generation.create
            gaCfg.mutationRate = 0.01 |> MutationRate.create
            gaCfg.noiseFraction = 0.01 |> NoiseFraction.create
            gaCfg.order = 16 |> Order.createNr
            gaCfg.rngGen = 1234 |> RandomSeed.create |> RngGen.createLcg
            gaCfg.sorterEvalMode = sorterEvalMode.DontCheckSuccess
            gaCfg.sorterCount = 32 |> SorterCount.create
            gaCfg.sorterCountMutated = 64 |> SorterCount.create
            gaCfg.sorterSetPruneMethod = sorterSetPruneMethod.Whole
            gaCfg.stageWeight = 0.1 |> StageWeight.create
            gaCfg.switchCountCalcMethod = switchCountCalcMethod.For999
            gaCfg.switchGenMode = switchGenMode.Stage
            gaCfg.useParallel = true |> UseParallel.create
        }


    let getRunId (cfg:gaCfg) =
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
            cfg.switchCountCalcMethod :> obj;
            cfg.switchGenMode :> obj;
            cfg.useParallel :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create


    let getWorkspaceParams (gaCfg:gaCfg)
        =
        let _nextRngGen () =
            gaCfg.rngGen
            |> Rando.fromRngGen
            |> Rando.toRngGen

        let rngGenCreate = (_nextRngGen ())
        let rngGenMutate = (_nextRngGen ())
        let rngGenPrune = (_nextRngGen ())
        let switchCount = gaCfg.order |> SwitchCount.fromCalcMethod (gaCfg.switchCountCalcMethod)

        WorkspaceParams.make Map.empty
        |> WorkspaceParams.setRunId "runId" (gaCfg |> getRunId)
        |> WorkspaceParams.setRngGen "rngGenCreate" rngGenCreate
        |> WorkspaceParams.setRngGen "rngGenMutate" rngGenMutate
        |> WorkspaceParams.setRngGen "rngGenPrune" rngGenPrune
        |> WorkspaceParams.setGeneration "generation" gaCfg.curGen
        |> WorkspaceParams.setMutationRate "mutationRate" gaCfg.mutationRate
        |> WorkspaceParams.setNoiseFraction "noiseFraction" (Some gaCfg.noiseFraction)
        |> WorkspaceParams.setOrder "order" gaCfg.order
        |> WorkspaceParams.setSorterCount "sorterCount" gaCfg.sorterCount
        |> WorkspaceParams.setSorterCount "sorterCountMutated" gaCfg.sorterCountMutated
        |> WorkspaceParams.setSorterEvalMode "sorterEvalMode" gaCfg.sorterEvalMode
        |> WorkspaceParams.setStageWeight "stageWeight" gaCfg.stageWeight
        |> WorkspaceParams.setSwitchCount "sorterLength" switchCount
        |> WorkspaceParams.setSwitchGenMode "switchGenMode" gaCfg.switchGenMode
        |> WorkspaceParams.setUseParallel "useParallel" gaCfg.useParallel
        |> WorkspaceParams.setSorterSetPruneMethod "sorterSetPruneMethod" gaCfg.sorterSetPruneMethod


    let enumerate
            (rngGens:rngGen seq)
            (stageWeights:stageWeight seq) 
            (noiseFractions:noiseFraction seq) 
            (mutationRates:mutationRate seq)
            (sorterSetPruneMethods:sorterSetPruneMethod seq)
            (maxGen:generation)
        =
            seq {
            
              for rngGen in rngGens do
                for stageWeight in stageWeights do
                    for noiseFraction in noiseFractions do
                        for mutationRate in mutationRates do
                            for sorterSetPruneMethod in sorterSetPruneMethods do
                                yield
                                    { defltCfg with 
                                        maxGen = maxGen;
                                        rngGen = rngGen;
                                        stageWeight = stageWeight;
                                        noiseFraction = noiseFraction;
                                        mutationRate = mutationRate;
                                        sorterSetPruneMethod = sorterSetPruneMethod
                                    }
            }