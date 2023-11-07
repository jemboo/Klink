namespace global
open System
open System.Text.RegularExpressions

type noiseFraction = private NoiseFraction of float
module NoiseFraction =
    let value (NoiseFraction v) = v
    let create vl = NoiseFraction vl
    let toFloat (nf: noiseFraction option) =
        match nf with
        | Some v ->  v |> value
        | None -> 0.0


type sorterPhenotypeCount = private SorterPhenotypeCount of int
module SorterPhenotypeCount =
    let value (SorterPhenotypeCount v) = v
    let create vl = SorterPhenotypeCount vl



type sorterSetPrunerId = private SorterSetPrunerId of Guid
module SorterSetPrunerId =
    let value (SorterSetPrunerId v) = v
    let create vl = SorterSetPrunerId vl


type sorterSetPruner = 
    private 
        {
            id: sorterSetPrunerId;
            prunedCount:sorterCount;
            noiseFraction:noiseFraction option;
            stageWeight:stageWeight;
        }


type sorterSetPruneMethod = 
    | Whole
    | PhenotypeCap of sorterPhenotypeCount
    | Shc


module SorterSetPruneMethod =

    let toReport (sspm:sorterSetPruneMethod) 
        =
        match sspm with
        | Whole -> "Whole"
        | PhenotypeCap sspc -> $"PhenotypeCap({sspc |> SorterPhenotypeCount.value})"
        | Shc -> "Shc"


    let extractWordAndNumber (input: string) =
        let pattern = @"(\w+)\((\d+)\)"
        let matchResult = Regex.Match(input, pattern)
        if matchResult.Success then
            let word = matchResult.Groups.[1].Value
            let number = matchResult.Groups.[2].Value
            Some(word, int number)
        else
            None


    let fromReport (repVal:string)
        =
         match repVal with
         | "Whole" -> sorterSetPruneMethod.Whole |> Ok
         | "Shc" -> sorterSetPruneMethod.Shc |> Ok
         | _ -> 
            let er = extractWordAndNumber repVal
            match er with
            | None -> $"{repVal} not valid in SorterSetPruneMethod.fromReport" |> Error
            | Some (w, n) ->
                match w with
                | "PhenotypeCap" ->
                    n |> SorterPhenotypeCount.create
                      |> sorterSetPruneMethod.PhenotypeCap |> Ok
                | _ ->
                    $"{repVal} not valid in SorterSetPruneMethod.fromReport" |> Error



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
                (sorterSetPruneMethod:sorterSetPruneMethod)
                (sorterSetIdParent:sorterSetId) 
                (sorterSetIdChild:sorterSetId)
                (stageWeight:stageWeight)
                (noiseFraction:noiseFraction option)
                (rngGen:rngGen) 
         =
        [|
            sorterSetPrunerId |> SorterSetPrunerId.value :> obj
            sorterSetPruneMethod |> SorterSetPruneMethod.toReport :> obj
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
                (sorterSetPruneMethod:sorterSetPruneMethod)
                (sorterSetEvalIdParent:sorterSetEvalId) 
                (sorterSetEvalIdChild:sorterSetEvalId)
                (rngGen:rngGen) 
         =
        [|
            sorterSetPrunerId |> SorterSetPrunerId.value :> obj;
            sorterSetPruneMethod |> SorterSetPruneMethod.toReport :> obj
            sorterSetEvalIdParent |> SorterSetEvalId.value :> obj;
            sorterSetEvalIdChild |> SorterSetEvalId.value :> obj;
            rngGen :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetEvalId.create


    let getSigmaSelection 
            (sorterEvalsWithFitness:(sorterEval*sorterFitness)[])
            (sigmaRatio:float)
            (rngGen:rngGen)
        =
            let deviation = 
                sorterEvalsWithFitness 
                |> Array.map(snd >> SorterFitness.value)
                |> CollectionProps.stdDeviation

            let noiseLevel = deviation * sigmaRatio

            let randy = rngGen |> Rando.fromRngGen
            let noiseMaker = RandVars.gaussianDistribution 0.0 noiseLevel randy

            let yab =
                sorterEvalsWithFitness 
                |> Seq.zip noiseMaker 
                |> Seq.map(fun (n, (srtrEval, srtrFitness)) ->
                                ((srtrEval, srtrFitness), n + (srtrFitness |> SorterFitness.value)) )
                |> Seq.sortByDescending(snd)
                |> Seq.toArray
            yab |> Array.map(fun ((sEv, sFt), npFt) -> sEv)



    let runWholePrune
            (sorterSetPruner:sorterSetPruner)
            (rngGen:rngGen)
            (sorterEvalsToPrune:sorterEval[])
         =
            let stageWgt = getStageWeight sorterSetPruner
            let sorterEvalsWithFitness = 
                sorterEvalsToPrune 
                |> Array.filter(SorterEval.getSorterSpeed >> Option.isSome)
                |> Array.filter(SorterEval.failedForSure >> not)
                |> Array.map(fun sEv -> 
                     ( sEv,
                       sEv |> SorterEval.getSorterSpeed |> Option.get |> SorterFitness.fromSpeed stageWgt
                     )
                   )
            let rankedSorters =
                if (sorterEvalsWithFitness.Length = 0) then
                    [||]
                elif sorterSetPruner.noiseFraction |> Option.isSome then
                    getSigmaSelection 
                        sorterEvalsWithFitness 
                        (sorterSetPruner.noiseFraction |> NoiseFraction.toFloat) 
                        rngGen

                else
                    let yab = sorterEvalsWithFitness 
                              |> Array.map(fun (srtrEval, srtrFit) -> ((srtrEval, srtrFit), (srtrFit |> SorterFitness.value)))
                              |> Array.sortByDescending(snd)
                    yab |> Array.map(fun ((sEv, sFt), npFt) -> sEv)

            rankedSorters
            |> CollectionOps.takeUpto (sorterSetPruner.prunedCount |> SorterCount.value)
            |> Seq.toArray




    let runWholeCappedPrune
            (sorterSetPruner:sorterSetPruner)
            (rngGen:rngGen)
            (sorterPhenotypeCount:sorterPhenotypeCount)
            (sorterEvalsToPrune:sorterEval[])
         =
            let stageWgt = getStageWeight sorterSetPruner
            let sorterEvalsWithFitness = 
                sorterEvalsToPrune
                |> Array.filter(SorterEval.getSorterSpeed >> Option.isSome)
                |> Array.filter(SorterEval.failedForSure >> not)
                |> CollectionOps.getItemsUpToMaxTimes 
                            (SorterEval.getSortrPhenotypeId)
                            (sorterPhenotypeCount |> SorterPhenotypeCount.value)
                |> Seq.toArray
                |> Array.map(fun sEv -> 
                     ( sEv,
                       sEv |> SorterEval.getSorterSpeed |> Option.get |> SorterFitness.fromSpeed stageWgt
                     )
                   )
            let rankedSorters =
                if (sorterEvalsWithFitness.Length = 0) then
                    [||]
                elif sorterSetPruner.noiseFraction |> Option.isSome then
                    getSigmaSelection 
                        sorterEvalsWithFitness 
                        (sorterSetPruner.noiseFraction |> NoiseFraction.toFloat) 
                        rngGen

                else
                    let yab = sorterEvalsWithFitness 
                              |> Array.map(fun (srtrEval, srtrFit) -> ((srtrEval, srtrFit), (srtrFit |> SorterFitness.value)))
                              |> Array.sortByDescending(snd)
                    yab |> Array.map(fun ((sEv, sFt), npFt) -> sEv)

            rankedSorters
            |> CollectionOps.takeUpto (sorterSetPruner.prunedCount |> SorterCount.value)
            |> Seq.toArray




    let runShcPrune
            (sorterSetPruner:sorterSetPruner)
            (rngGen:rngGen)
            (sorterSetParentMap:sorterSetParentMap)
            (sorterEvalsToPrune:sorterEval[])
         =
            let extendedPm = sorterSetParentMap |> SorterSetParentMap.extendToParents
            let familySorterEvalGroups = 
                    sorterEvalsToPrune 
                            |> Array.map(fun sev -> (sev, extendedPm.[sev.sortrId]))
                            |> Array.groupBy(fun (sorterEv, sorterPid) -> sorterPid)
                            |> Array.map(snd)
                            |> Array.map(Array.map(fst))

            let familyCount = (familySorterEvalGroups |> Array.length)
            let familyPrunedCount = (sorterSetPruner.prunedCount |> SorterCount.value) 
                                        / familyCount
                                     |> SorterCount.create

            let familyRngGens = 
                rngGen |> Rando.toMoreRngGens
                |> Seq.take(familyCount)
                |> Seq.toArray

            let familyPruner = make familyPrunedCount sorterSetPruner.noiseFraction sorterSetPruner.stageWeight

            familyRngGens
                    |> Array.mapi(fun dex rg -> runWholePrune familyPruner rg familySorterEvalGroups.[dex])
                    |> Array.concat



    let runPrune
            (sorterSetPruneMethod:sorterSetPruneMethod)
            (rngGen:rngGen)
            (sorterSetParentMap:sorterSetParentMap)
            (sorterSetPruner:sorterSetPruner)
            (sorterEvalsToPrune:sorterEval[])
         =
         match sorterSetPruneMethod with
         | Whole -> runShcPrune sorterSetPruner rngGen sorterSetParentMap sorterEvalsToPrune
         | Shc -> runShcPrune sorterSetPruner rngGen sorterSetParentMap sorterEvalsToPrune
         | PhenotypeCap spc -> runWholeCappedPrune sorterSetPruner rngGen spc sorterEvalsToPrune