namespace global

open System
type sorterSetMutatorId = private SorterSetMutatorId of Guid
module SorterSetMutatorId =
    let value (SorterSetMutatorId v) = v
    let create (v: Guid) = SorterSetMutatorId v

type sorterSetMutator = 
    private
        { 
          id: sorterSetMutatorId
          sorterMutator: sorterMutator
          sorterCountFinal: sorterCount Option
          rngGen: rngGen
        }

module SorterSetMutator =

    let load
            (id:sorterSetMutatorId)
            (sorterMutator:sorterMutator) 
            (sorterCountFinal:sorterCount option) 
            (rngGen:rngGen) 
        =
        { 
          id = id
          sorterMutator = sorterMutator
          sorterCountFinal = sorterCountFinal
          rngGen = rngGen
        }
        
    let getId (sum: sorterSetMutator) = sum.id

    let getSorterMutator (sum: sorterSetMutator) = sum.sorterMutator

    let getRngGen (sum: sorterSetMutator) = sum.rngGen

    let getSorterCountFinal (sum: sorterSetMutator) = 
         sum.sorterCountFinal

    let getMutantSorterSetId
            (sorterSetMutator:sorterSetMutator)
            (parentSetId:sorterSetId)
        =
        [|  
            parentSetId :> obj;
            (sorterSetMutator |> getRngGen) :> obj;
            (sorterSetMutator
                    |> getSorterMutator
                    |> SorterMutator.getMutatorId):> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetId.create


    let createMutantSorterSetAndParentMap
            (sorterSetMutator:sorterSetMutator)
            (parentSet:sorterSet)
        =
        result {
            let parentSetId = parentSet |> SorterSet.getId
            let mutantSetId = parentSetId |> getMutantSorterSetId sorterSetMutator

            let randy = sorterSetMutator |> getRngGen |> Rando.fromRngGen
            let childSorterCount = 
                match (sorterSetMutator |> getSorterCountFinal) with
                | Some sc -> sc
                | None -> parentSet |> SorterSet.getSorterCount

            let! tupes = 
                SorterMutator.makeMutants
                    (sorterSetMutator |> getSorterMutator)
                    randy
                    childSorterCount
                    (parentSet |> SorterSet.getSorters)

            let mutantSet = 
                    SorterSet.load
                        mutantSetId
                        (parentSet |> SorterSet.getOrder)
                        (tupes |> Seq.map(snd))

            let sorterParentMapId = 
                    SorterSetParentMap.makeId
                        parentSetId
                        mutantSetId

            let parentMap =
                tupes 
                    |> Seq.map(fun (parentId, srtr) -> 
                          srtr |> Sorter.getSorterId, 
                          parentId |> SorterParentId.toSorterParentId )
                |> Map.ofSeq

            let sorterParentMap = 
                SorterSetParentMap.load
                    sorterParentMapId
                    mutantSetId
                    parentSetId
                    parentMap

            return  sorterParentMap, mutantSet
        }


    let createMutantSorterSetFromParentMap
            (sorterSetParentMap:sorterSetParentMap)
            (sorterSetMutator:sorterSetMutator)
            (sorterSetToMutate:sorterSet)
        =
        result {
            let! mutants = 
                SorterMutator.makeMutants2
                    (sorterSetMutator |> getSorterMutator)
                    (sorterSetMutator |> getRngGen |> Rando.fromRngGen)
                    (sorterSetParentMap |> SorterSetParentMap.getParentMap)
                    (sorterSetToMutate |> SorterSet.getSorters)


            return
                SorterSet.load
                    (sorterSetParentMap |> SorterSetParentMap.getChildSorterSetId)
                    (sorterSetToMutate |> SorterSet.getOrder)
                    mutants
        }

