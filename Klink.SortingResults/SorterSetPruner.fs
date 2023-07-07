namespace global
open System

type sorterSetPrunerGaData =
    | NoData
    | ParentMap of sorterSetParentMap


type sorterSetPrunerId = private SorterSetPrunerId of Guid
module SorterSetPrunerId =
    let value (SorterSetPrunerId v) = v
    let create vl = SorterSetPrunerId vl


type sorterSetPrunerWhole = 
    private {
            id: sorterSetPrunerId;
            prunedCount:sorterCount;
            noiseFraction:float option;
            stageWeight:stageWeight;
        }


module SorterSetPrunerWhole =

    let getId
            (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.id

    let getPrunedCount
             (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.prunedCount

    let getNoiseFraction
             (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.noiseFraction

    let getStageWeight
                (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.stageWeight

    let load
            (id:sorterSetPrunerId)
            (prunedCount:sorterCount)
            (noiseFraction:float option)
            (stageWeight:stageWeight)
        =
        {   
            id=id
            prunedCount=prunedCount
            noiseFraction=noiseFraction
            stageWeight=stageWeight
        }


    let makeId
            (prunedCount:sorterCount)
            (stageWeight:stageWeight) 
            (noiseFraction:float option)
        =
        [|
            "sorterSetPrunerWhole" :> obj
            stageWeight |> StageWeight.value :> obj; 
            noiseFraction :> obj; 
            prunedCount |> SorterCount.value :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create


    let make (prunedCount:sorterCount)
             (noiseFraction:float option)
             (stageWeight:stageWeight)
        =
        {
            id = makeId prunedCount stageWeight noiseFraction;
            prunedCount = prunedCount;
            noiseFraction = noiseFraction;
            stageWeight = stageWeight; 
        }

    let makePrunedSorterSetId
            (sorterSetPruner:sorterSetPrunerWhole)
            (sorterSetToPrune:sorterSet)
        =
        [|
            sorterSetPruner |> getId :> obj
            sorterSetToPrune |> SorterSet.getId :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetId.create


    let getSigmaSelection 
            (sorterEvalsWithFitness:(sorterEval*float)[])
            (sigmaRatio:float)

        =
            let deviation = 
                sorterEvalsWithFitness 
                |> Array.map(snd)
                |> CollectionProps.stdDeviation
            let noiseMaker = ()
                
            [||]


    let run (sorterSetPruner:sorterSetPrunerWhole)
            (rngGen:rngGen)
            (meta:sorterSetPrunerGaData)
            (sorterEvalsToPrune:sorterEval[])
         =
            let stageWgt = getStageWeight sorterSetPruner
            let sorterEvalsWithFitness = 
                sorterEvalsToPrune 
                |> Array.filter(SorterEval.getSorterSpeed >> Option.isSome)
                |> Array.map(fun sEv -> 
                     ( sEv, 
                       sEv |> SorterEval.getSorterSpeed |> Option.get |> SorterFitness.fromSpeed stageWgt
                     )
                   )

            if (sorterEvalsWithFitness.Length = 0) then
                [||]
            elif sorterSetPruner.noiseFraction |> Option.isSome then
                getSigmaSelection sorterEvalsWithFitness (sorterSetPruner.noiseFraction |> Option.get)
                |> CollectionOps.takeUpto (sorterSetPruner.prunedCount |> SorterCount.value)
                |> Seq.toArray
            else
                sorterEvalsWithFitness 
                |> Array.sortByDescending(snd)
                |> CollectionOps.takeUptoArray (sorterSetPruner.prunedCount |> SorterCount.value)



type sorterSetPrunerBatch = 
        private {
            id: sorterSetPrunerId;
            prunedCount:sorterCount;
            noiseFraction:float option;
            stageWeight:stageWeight;
        }


module SorterSetPrunerBatch =

    let getId
            (sorterSetPruner:sorterSetPrunerBatch) 
         =
         sorterSetPruner.id


    let getPrunedCount
             (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.prunedCount

    let getNoiseFraction
             (sorterSetPruner:sorterSetPrunerBatch) 
         =
         sorterSetPruner.noiseFraction


    let getStageWeight
                (sorterSetPruner:sorterSetPrunerBatch) 
         =
         sorterSetPruner.stageWeight

    let load
            (id:sorterSetPrunerId)
            (prunedCount:sorterCount)
            (noiseFraction:float option)
            (stageWeight:stageWeight)
         =
        {   
            id=id
            prunedCount=prunedCount
            noiseFraction=noiseFraction
            stageWeight=stageWeight
        }

    let makeId
            (prunedCount:sorterCount)
            (stageWeight:stageWeight)
            (noiseFraction:float option)
        =
        [|
            "sorterSetPrunerShc" :> obj
            stageWeight |> StageWeight.value :> obj; 
            noiseFraction :> obj; 
            prunedCount |> SorterCount.value :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create


    let make (prunedCount:sorterCount)
             (noiseFraction:float option)
             (stageWeight:stageWeight)
        =
        {
            id = makeId prunedCount stageWeight noiseFraction;
            prunedCount = prunedCount;
            noiseFraction=noiseFraction;
            stageWeight =  stageWeight;
        }

    let makePrunedSorterSetId
            (sorterSetPruner:sorterSetPrunerBatch)
            (sorterSetToPrune:sorterSet)
        =
        [|
            sorterSetPruner |> getId :> obj
            sorterSetToPrune |> SorterSet.getId :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetId.create


    let run (sorterSetPruner:sorterSetPrunerBatch) 
            (rngGen:rngGen)
            (meta:sorterSetPrunerGaData)
            (sorterEvalsToPrune:sorterEval[])
         =
         [||]



type sorterSetPrunerShc = 
        private {
            id: sorterSetPrunerId;
            prunedCount:sorterCount;
            noiseFraction:float option;
            stageWeight:stageWeight;
        }


module SorterSetPrunerShc =

    let getId
            (sorterSetPruner:sorterSetPrunerShc) 
         =
         sorterSetPruner.id


    let getPrunedCount
             (sorterSetPruner:sorterSetPrunerShc) 
         =
         sorterSetPruner.prunedCount

    let getNoiseFraction
             (sorterSetPruner:sorterSetPrunerShc) 
         =
         sorterSetPruner.noiseFraction


    let getStageWeight
                (sorterSetPruner:sorterSetPrunerShc) 
         =
         sorterSetPruner.stageWeight

    let load
            (id:sorterSetPrunerId)
            (prunedCount:sorterCount)
            (noiseFraction:float option)
            (stageWeight:stageWeight)
        =
        {   
            id=id
            prunedCount=prunedCount
            noiseFraction=noiseFraction
            stageWeight=stageWeight
        }

    let makeId
            (prunedCount:sorterCount)
            (stageWeight:stageWeight)
            (noiseFraction:float option)
        =
        [|
            "sorterSetPrunerShc" :> obj
            stageWeight |> StageWeight.value :> obj; 
            noiseFraction :> obj;
            prunedCount |> SorterCount.value :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create


    let make (prunedCount:sorterCount)
             (noiseFraction:float option)
             (stageWeight:stageWeight)
        =
        {
            id = makeId prunedCount stageWeight noiseFraction;
            prunedCount = prunedCount;
            noiseFraction = noiseFraction;
            stageWeight =  stageWeight;        
        }

    let makePrunedSorterSetId
            (sorterSetPruner:sorterSetPrunerShc)
            (sorterSetToPrune:sorterSet)
        =
        [|
            sorterSetPruner |> getId :> obj
            sorterSetToPrune |> SorterSet.getId :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetId.create


    let run (sorterSetPruner:sorterSetPrunerShc)              
            (rngGen:rngGen)
            (meta:sorterSetPrunerGaData)
            (sorterEvalsToPrune:sorterEval[])
         =
         [||]



type sorterSetPruner =
    | Whole of sorterSetPrunerWhole
    | Batch of sorterSetPrunerBatch
    | Shc of sorterSetPrunerShc


module SorterSetPruner =

    let getId
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.id
         | Batch ssphW ->  ssphW.id
         | Shc ssphW ->  ssphW.id


    let getPrunedCount
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.prunedCount
         | Batch ssphW ->  ssphW.prunedCount
         | Shc ssphW ->  ssphW.prunedCount

    let getNoiseFraction
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.noiseFraction
         | Batch ssphW ->  ssphW.noiseFraction
         | Shc ssphW ->  ssphW.noiseFraction

    let getStageWeight
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.stageWeight
         | Batch ssphW ->  ssphW.stageWeight
         | Shc ssphW ->  ssphW.stageWeight

    let run
            (sorterSetPruner:sorterSetPruner)              
            (rngGen:rngGen)
            (meta:sorterSetPrunerGaData)
            (sorterEvalsToPrune:sorterEval[])
         =
         match sorterSetPruner with
         | Whole ssphW ->  sorterEvalsToPrune |> SorterSetPrunerWhole.run ssphW rngGen meta
         | Batch ssphS ->  sorterEvalsToPrune |> SorterSetPrunerBatch.run ssphS rngGen meta
         | Shc ssphS ->  sorterEvalsToPrune |> SorterSetPrunerShc.run ssphS rngGen meta