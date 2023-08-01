namespace global
open System

type noiseFraction = private NoiseFraction of float
module NoiseFraction =
    let value (NoiseFraction v) = v
    let create vl = NoiseFraction vl
    let toFloat (nf: noiseFraction option) =
        match nf with
        | Some v ->  v |> value
        | None -> 0.0

type sorterSetPrunerId = private SorterSetPrunerId of Guid
module SorterSetPrunerId =
    let value (SorterSetPrunerId v) = v
    let create vl = SorterSetPrunerId vl


type sorterSetPruner = 
    private {
            id: sorterSetPrunerId;
            prunedCount:sorterCount;
            noiseFraction:noiseFraction option;
            stageWeight:stageWeight;
        }

type sorterSetPruneMethod = 
    | Whole = 1
    | Shc = 2


module SorterSetPruner =

    let getId
            (sorterSetPruner:sorterSetPruner) 
         =
         sorterSetPruner.id

    let getPrunedCount
             (sorterSetPruner:sorterSetPruner) 
         =
         sorterSetPruner.prunedCount

    let getNoiseFraction
             (sorterSetPruner:sorterSetPruner) 
         =
         sorterSetPruner.noiseFraction

    let getStageWeight
                (sorterSetPruner:sorterSetPruner) 
         =
         sorterSetPruner.stageWeight

    let load
            (id:sorterSetPrunerId)
            (prunedCount:sorterCount)
            (noiseFraction:noiseFraction option)
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
            (noiseFraction:noiseFraction option)
        =
        [|
            "sorterSetPrunerWhole" :> obj
            stageWeight |> StageWeight.value :> obj; 
            noiseFraction |> Option.map(NoiseFraction.value) :> obj; 
            prunedCount |> SorterCount.value :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create


    let make (prunedCount:sorterCount)
             (noiseFraction:noiseFraction option)
             (stageWeight:stageWeight)
        =
        {
            id = makeId prunedCount stageWeight noiseFraction;
            prunedCount = prunedCount;
            noiseFraction = noiseFraction;
            stageWeight = stageWeight; 
        }


    let makePrunedSorterSetId
                (sorterSetPrunerId:sorterSetPrunerId) 
                (sorterSetIdParent:sorterSetId) 
                (sorterSetIdChild:sorterSetId)
                (stageWeight:stageWeight)
                (noiseFraction:noiseFraction option)
                (rngGen:rngGen) 
         =
        [|
            sorterSetPrunerId |> SorterSetPrunerId.value :> obj
            sorterSetIdParent |> SorterSetId.value :> obj;
            sorterSetIdChild |> SorterSetId.value :> obj;
            stageWeight |> StageWeight.value :> obj
            noiseFraction :> obj
            rngGen :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetId.create


    let makePrunedSorterSetEvalId
                (sorterSetPrunerId:sorterSetPrunerId) 
                (sorterSetEvalIdParent:sorterSetEvalId) 
                (sorterSetEvalIdChild:sorterSetEvalId) 
                (rngGen:rngGen) 
         =
        [|
            sorterSetPrunerId |> SorterSetPrunerId.value :> obj
            sorterSetEvalIdParent |> SorterSetEvalId.value :> obj;
            sorterSetEvalIdChild |> SorterSetEvalId.value :> obj;
            rngGen :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetEvalId.create


    let getSigmaSelection 
            (sorterEvalsWithFitness:(sorterEval*float)[])
            (sigmaRatio:float)
            (rngGen:rngGen)
        =
            let deviation = 
                sorterEvalsWithFitness 
                |> Array.map(snd)
                |> CollectionProps.stdDeviation

            let noiseLevel = deviation * sigmaRatio

            let randy = rngGen |> Rando.fromRngGen
            let noiseMaker = RandVars.gaussianDistribution 0.0 noiseLevel randy

            let yab =
                sorterEvalsWithFitness 
                |> Seq.zip noiseMaker 
                |> Seq.map(fun (n, (srtrEval, f)) ->((srtrEval, f), n + f) )
                |> Seq.sortByDescending(snd)
                |> Seq.toArray
            yab

    let runWholePrune
            (sorterSetPruner:sorterSetPruner)
            (rngGen:rngGen)
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
                getSigmaSelection sorterEvalsWithFitness (sorterSetPruner.noiseFraction |> NoiseFraction.toFloat) rngGen
                |> CollectionOps.takeUpto (sorterSetPruner.prunedCount |> SorterCount.value)
                |> Seq.toArray
            else
                sorterEvalsWithFitness 
                |> Array.map(fun (sev, ft) -> ((sev, 0.0), ft))
                |> Array.sortByDescending(snd)
                |> CollectionOps.takeUptoArray (sorterSetPruner.prunedCount |> SorterCount.value)



    let runShcPrune
            (sorterSetPruner:sorterSetPruner)
            (rngGen:rngGen)
            (sorterSetParentMap:sorterSetParentMap)
            (sorterEvalsToPrune:sorterEval[])
         =
            let extendedPm = sorterSetParentMap |> SorterSetParentMap.extendToParents
            let familyGroups = sorterEvalsToPrune 
                                |> Array.map(fun sev -> (sev, extendedPm.[sev.sortrId]))
                                |> Array.groupBy(fun (sorterEv, sorterPid) -> sorterPid)
                                |> Array.map(snd)
                                |> Array.map(Array.map(fst))

            let familyTargetSize = (sorterSetPruner.prunedCount |> SorterCount.value) / (familyGroups |> Array.length)
                                   |> SorterCount.create

            let familyPruner = make familyTargetSize sorterSetPruner.noiseFraction sorterSetPruner.stageWeight

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
                getSigmaSelection sorterEvalsWithFitness (sorterSetPruner.noiseFraction |> NoiseFraction.toFloat) rngGen
                |> CollectionOps.takeUpto (sorterSetPruner.prunedCount |> SorterCount.value)
                |> Seq.toArray
            else
                sorterEvalsWithFitness 
                |> Array.map(fun (sev, ft) -> ((sev, 0.0), ft))
                |> Array.sortByDescending(snd)
                |> CollectionOps.takeUptoArray (sorterSetPruner.prunedCount |> SorterCount.value)




//type sorterSetPrunerBatch = 
//        private {
//            id: sorterSetPrunerId;
//            prunedCount:sorterCount;
//            noiseFraction:float option;
//            stageWeight:stageWeight;
//        }


//module SorterSetPrunerBatch =

//    let getId
//            (sorterSetPruner:sorterSetPrunerBatch) 
//         =
//         sorterSetPruner.id


//    let getPrunedCount
//             (sorterSetPruner:sorterSetPrunerWhole) 
//         =
//         sorterSetPruner.prunedCount

//    let getNoiseFraction
//             (sorterSetPruner:sorterSetPrunerBatch) 
//         =
//         sorterSetPruner.noiseFraction


//    let getStageWeight
//                (sorterSetPruner:sorterSetPrunerBatch) 
//         =
//         sorterSetPruner.stageWeight

//    let load
//            (id:sorterSetPrunerId)
//            (prunedCount:sorterCount)
//            (noiseFraction:float option)
//            (stageWeight:stageWeight)
//         =
//        {   
//            id=id
//            prunedCount=prunedCount
//            noiseFraction=noiseFraction
//            stageWeight=stageWeight
//        }

//    let makeId
//            (prunedCount:sorterCount)
//            (stageWeight:stageWeight)
//            (noiseFraction:float option)
//        =
//        [|
//            "sorterSetPrunerShc" :> obj
//            stageWeight |> StageWeight.value :> obj; 
//            noiseFraction :> obj; 
//            prunedCount |> SorterCount.value :> obj
//        |] 
//        |> GuidUtils.guidFromObjs
//        |> SorterSetPrunerId.create


//    let make (prunedCount:sorterCount)
//             (noiseFraction:float option)
//             (stageWeight:stageWeight)
//        =
//        {
//            id = makeId prunedCount stageWeight noiseFraction;
//            prunedCount = prunedCount;
//            noiseFraction=noiseFraction;
//            stageWeight =  stageWeight;
//        }

//    let makePrunedSorterSetId
//            (sorterSetPruner:sorterSetPrunerBatch)
//            (sorterSetToPrune:sorterSet)
//        =
//        [|
//            sorterSetPruner |> getId :> obj
//            sorterSetToPrune |> SorterSet.getId :> obj;
//        |] 
//        |> GuidUtils.guidFromObjs
//        |> SorterSetId.create


//    let run (sorterSetPruner:sorterSetPrunerBatch) 
//            (rngGen:rngGen)
//            (meta:gaMetaData)
//            (sorterEvalsToPrune:sorterEval[])
//         =
//         [||]



//type sorterSetPrunerShc = 
//        private {
//            id: sorterSetPrunerId;
//            prunedCount:sorterCount;
//            noiseFraction:float option;
//            stageWeight:stageWeight;
//        }


//module SorterSetPrunerShc =

//    let getId
//            (sorterSetPruner:sorterSetPrunerShc) 
//         =
//         sorterSetPruner.id


//    let getPrunedCount
//             (sorterSetPruner:sorterSetPrunerShc) 
//         =
//         sorterSetPruner.prunedCount

//    let getNoiseFraction
//             (sorterSetPruner:sorterSetPrunerShc) 
//         =
//         sorterSetPruner.noiseFraction


//    let getStageWeight
//                (sorterSetPruner:sorterSetPrunerShc) 
//         =
//         sorterSetPruner.stageWeight

//    let load
//            (id:sorterSetPrunerId)
//            (prunedCount:sorterCount)
//            (noiseFraction:float option)
//            (stageWeight:stageWeight)
//        =
//        {   
//            id=id
//            prunedCount=prunedCount
//            noiseFraction=noiseFraction
//            stageWeight=stageWeight
//        }

//    let makeId
//            (prunedCount:sorterCount)
//            (stageWeight:stageWeight)
//            (noiseFraction:float option)
//        =
//        [|
//            "sorterSetPrunerShc" :> obj
//            stageWeight |> StageWeight.value :> obj; 
//            noiseFraction :> obj;
//            prunedCount |> SorterCount.value :> obj
//        |] 
//        |> GuidUtils.guidFromObjs
//        |> SorterSetPrunerId.create


//    let make (prunedCount:sorterCount)
//             (noiseFraction:float option)
//             (stageWeight:stageWeight)
//        =
//        {
//            id = makeId prunedCount stageWeight noiseFraction;
//            prunedCount = prunedCount;
//            noiseFraction = noiseFraction;
//            stageWeight =  stageWeight;        
//        }

//    let makePrunedSorterSetId
//            (sorterSetPruner:sorterSetPrunerShc)
//            (sorterSetToPrune:sorterSet)
//        =
//        [|
//            sorterSetPruner |> getId :> obj
//            sorterSetToPrune |> SorterSet.getId :> obj;
//        |] 
//        |> GuidUtils.guidFromObjs
//        |> SorterSetId.create


//    let run (sorterSetPruner:sorterSetPrunerShc)              
//            (rngGen:rngGen)
//            (meta:gaMetaData)
//            (sorterEvalsToPrune:sorterEval[])
//         =
//         [||]



//type sorterSetPruner =
//    | Whole of sorterSetPrunerWhole
//    | Batch of sorterSetPrunerBatch
//    | Shc of sorterSetPrunerShc


//module SorterSetPruner =

//    let getId
//            (sorterSetPruner:sorterSetPruner) 
//         =
//         match sorterSetPruner with
//         | Whole ssphW ->  ssphW.id
//         | Batch ssphW ->  ssphW.id
//         | Shc ssphW ->  ssphW.id


//    let makePrunedSorterSetId
//                (sorterSetPrunerId:sorterSetPrunerId) 
//                (sorterSetIdParent:sorterSetId) 
//                (sorterSetIdChild:sorterSetId) 
//                (rngGen:rngGen) 
//         =
//        [|
//            sorterSetPrunerId |> SorterSetPrunerId.value :> obj
//            sorterSetIdParent |> SorterSetId.value :> obj;
//            sorterSetIdChild |> SorterSetId.value :> obj;
//            rngGen :> obj;
//        |] 
//        |> GuidUtils.guidFromObjs
//        |> SorterSetId.create


//    let getPrunedCount
//            (sorterSetPruner:sorterSetPruner) 
//         =
//         match sorterSetPruner with
//         | Whole ssphW ->  ssphW.prunedCount
//         | Batch ssphW ->  ssphW.prunedCount
//         | Shc ssphW ->  ssphW.prunedCount

//    let getNoiseFraction
//            (sorterSetPruner:sorterSetPruner) 
//         =
//         match sorterSetPruner with
//         | Whole ssphW ->  ssphW.noiseFraction
//         | Batch ssphW ->  ssphW.noiseFraction
//         | Shc ssphW ->  ssphW.noiseFraction

//    let getStageWeight
//            (sorterSetPruner:sorterSetPruner) 
//         =
//         match sorterSetPruner with
//         | Whole ssphW ->  ssphW.stageWeight
//         | Batch ssphW ->  ssphW.stageWeight
//         | Shc ssphW ->  ssphW.stageWeight

//    let run
//            (sorterSetPruner:sorterSetPruner)              
//            (rngGen:rngGen)
//            (meta:gaMetaData)
//            (sorterEvalsToPrune:sorterEval[])
//         =
//         match sorterSetPruner with
//         | Whole ssphW ->  sorterEvalsToPrune |> SorterSetPrunerWhole.run ssphW rngGen meta
//         | Batch ssphS ->  sorterEvalsToPrune |> SorterSetPrunerBatch.run ssphS rngGen meta
//         | Shc ssphS ->  sorterEvalsToPrune |> SorterSetPrunerShc.run ssphS rngGen meta