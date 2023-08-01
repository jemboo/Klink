namespace global

open System
type sorterSetMutatorId = private SorterSetMutatorId of Guid
module SorterSetMutatorId =
    let value (SorterSetMutatorId v) = v
    let create (v: Guid) = SorterSetMutatorId v

type sorterSetMutator = 
    private { 
            id: sorterSetMutatorId
            sorterMutator: sorterMutator
            sorterCountFinal: sorterCount Option
        }

module SorterSetMutator =

    let load
            (id:sorterSetMutatorId)
            (sorterMutator:sorterMutator) 
            (sorterCountFinal:sorterCount option) 
        =
        { 
          id = id
          sorterMutator = sorterMutator
          sorterCountFinal = sorterCountFinal
        }

    let getId (sum: sorterSetMutator) = sum.id

    let getSorterMutator (sum: sorterSetMutator) = sum.sorterMutator

    let getSorterCountFinal (sum: sorterSetMutator) = 
         sum.sorterCountFinal

    let getMutantSorterSetId
            (sorterSetMutator:sorterSetMutator)
            (rngGen:rngGen) 
            (parentSetId:sorterSetId)
        =
        [|  
            parentSetId :> obj;
            rngGen :> obj;
            (sorterSetMutator
                    |> getSorterMutator
                    |> SorterMutator.getMutatorId):> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetId.create


    let createMutantSorterSetFromParentMap
            (sorterSetParentMap:sorterSetParentMap)
            (ssm:sorterSetMutator)
            (rngGen:rngGen) 
            (sorterSetToMutate:sorterSet)
        =
        result {
            let! mutants = 
                SorterMutator.makeMutants
                    (ssm |> getSorterMutator)
                    (rngGen |> Rando.fromRngGen)
                    (sorterSetParentMap |> SorterSetParentMap.getParentMap)
                    (sorterSetToMutate |> SorterSet.getSorters)

            return
                SorterSet.load
                    (sorterSetParentMap |> SorterSetParentMap.getChildSorterSetId)
                    (sorterSetToMutate |> SorterSet.getOrder)
                    mutants
        }

