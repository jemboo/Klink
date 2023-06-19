namespace global

type ss_ShcCfg = 
    private
        { 
          order: order
          rngGenCreate: rngGen
          switchGenMode: switchGenMode
          switchCount: switchCount
          sorterCountCreate: sorterCount
          rngGenMutate: rngGen
          sorterCountMutate: sorterCount
          mutationRate:mutationRate
          sorterEvalMode: sorterEvalMode
          stagePrefixCount: stageCount
          stageWeight:stageWeight
          temp:temp
          generation:generation
        }


module Ss_ShcCfg 
    =
    let create (order:order)
               (rngGenCreate:rngGen)
               (switchGenMode:switchGenMode)
               (switchCount:switchCount)
               (sorterCountCreate:sorterCount)
               (rngGenMutate:rngGen)
               (sorterCountMutate:sorterCount)
               (mutationRate:mutationRate)
               (sorterEvalMode: sorterEvalMode)
               (stagePrefixCount: stageCount)
               (stageWeight: stageWeight)
               (temp: temp)
               (generation: generation)
        =
        {
            order=order;
            rngGenCreate=rngGenCreate;
            switchGenMode=switchGenMode;
            switchCount=switchCount;
            sorterCountCreate=sorterCountCreate;
            rngGenMutate=rngGenMutate;
            sorterCountMutate=sorterCountMutate;
            mutationRate=mutationRate
            sorterEvalMode=sorterEvalMode
            stagePrefixCount=stagePrefixCount;
            stageWeight=stageWeight
            temp=temp
            generation=generation;
        }

    let getMutationRate (cfg: ss_ShcCfg) = 
            cfg.mutationRate

    let getOrder (cfg: ss_ShcCfg) = 
            cfg.order

    let getRngGenCreate (cfg: ss_ShcCfg) = 
            cfg.rngGenCreate

    let getSorterEvalMode  (cfg: ss_ShcCfg) = 
            cfg.sorterEvalMode

    let getStagePrefixCount  (cfg: ss_ShcCfg) = 
            cfg.stagePrefixCount

    let getSwitchGenMode (cfg: ss_ShcCfg) = 
            cfg.switchGenMode

    let getSwitchCount (cfg: ss_ShcCfg) = 
            cfg.switchCount

    let getSortableSetCfg
            (cfg:ss_ShcCfg)
        =
        cfg.order |> SortableSetCertainCfg.makeAllBitsReducedOneStage


    let getlId
            (cfg: ss_ShcCfg) 
        = 
        [|
          "ss_ShcCfg" :> obj;
           cfg :> obj;
        |] |> GuidUtils.guidFromObjs
           |> SorterSetEvalId.create


    let getFileName
            (cfg:ss_ShcCfg) 
        =
        cfg |> getlId 
            |> SorterSetEvalId.value 
            |> string


    let getSs_GenStepCfg
            (cfg:ss_ShcCfg)
        =
        Ss_GenStepCfg.create 
            cfg.order
            cfg.switchGenMode
            cfg.rngGenMutate
            cfg.sorterCountMutate
            cfg.mutationRate
            cfg.stagePrefixCount
            cfg.stageWeight
            cfg.temp


    let getSorterSetOriginalCfg 
            (cfg:ss_ShcCfg)
        =
        SorterSetRndCfg.create 
            cfg.order
            cfg.rngGenCreate
            cfg.switchGenMode
            cfg.switchCount
            cfg.sorterCountCreate
   