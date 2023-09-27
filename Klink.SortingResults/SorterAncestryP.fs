namespace global

open System

type genInfo =
        private {
        generation:generation
        sorterId: sorterId
        sorterPhenotypeId:sorterPhenotypeId
        sorterFitness:sorterFitness
        }

type sorterAncestryP =
        private {
        sorterId: sorterId
        ancestors: List<(genInfo)>
        }


module SorterAncestryP =

    let getSorterId (sa:sorterAncestryP) =
        sa.sorterId

    let getAncestors (sa:sorterAncestryP) =
        sa.ancestors

    let create 
            (sorterId:sorterId) 
            (generation:generation) 
            (sorterPhenotypeId:sorterPhenotypeId) 
            (sorterFitness:sorterFitness) 
        =
        {
            sorterAncestryP.sorterId = sorterId;
            ancestors = 
                [
                    {
                        genInfo.sorterId = sorterId;
                        generation = generation
                        sorterPhenotypeId = sorterPhenotypeId
                        sorterFitness = sorterFitness
                    }
                ]
        }

    let update 
            (parentSorterAncestry:sorterAncestryP) 
            (sorterId:sorterId) 
            (generation:generation) 
            (sorterPhenotypeId:sorterPhenotypeId) 
            (sorterFitness:sorterFitness) 
        =
        let _replace last newStuff history =
            if newStuff.sorterPhenotypeId = last.sorterPhenotypeId then
                newStuff::history
            else
                newStuff::last::history

        let newStuff = 
            {
                genInfo.sorterId = sorterId;
                generation = generation
                sorterPhenotypeId = sorterPhenotypeId
                sorterFitness = sorterFitness
            }

        let updatedAncestors =
            match parentSorterAncestry.ancestors with
            | [] -> [newStuff]
            | last::t -> _replace newStuff last t
        {
            sorterAncestryP.sorterId = sorterId;
            ancestors = updatedAncestors
        }


type sorterSetAncestryId = private SorterSetAncestryId of Guid
module SorterSetAncestryId =
    let value (SorterSetAncestryId v) = v
    let create (v: Guid) = SorterSetAncestryId v


type sorterSetAncestryP =
        private {
        id: sorterSetAncestryId;
        generation:generation;
        ancestorMap:Map<sorterId, sorterAncestryP>
        }


module SorterSetAncestryP =

    let getId (sa:sorterSetAncestryP) =
        sa.id

    let getAncestorMap (sa:sorterSetAncestryP) =
        sa.ancestorMap

    let create (id:sorterSetAncestryId) 
               (sorterSetEval:sorterSetEval)
               (stageWeight:stageWeight)
        =
        let _makeSorterAncestry 
                (sev:sorterEval)
                (gen:generation)
                (sw:stageWeight)
            =
            SorterAncestryP.create
                sev.sortrId
                gen
                (sev.sortrPhenotypeId |> Option.get)
                (sev.sorterSpeed |> Option.get |> SorterFitness.fromSpeed sw)


        let generation =  0 |> Generation.create
        {
            id = id;
            generation = generation
            ancestorMap = 
                sorterSetEval.sorterEvals
                |> Map.toArray
                |> Array.map(fun (srtrId, sev) -> (srtrId, _makeSorterAncestry sev generation stageWeight))
                |> Map.ofSeq
        }


    let update 
            (newId:sorterSetAncestryId)
            (sorterSetEval:sorterSetEval)
            (stageWeight:stageWeight)
            (parentMap:Map<sorterId, sorterParentId>)
            (sorterSetAncestry:sorterSetAncestryP) 
        =
        let nextGen = (sorterSetAncestry.generation |> Generation.next)

        let _updateSorterAncestry 
                (parentSorterAncestry:sorterAncestryP)
                (sev:sorterEval)
                (gen:generation)
                (stageWeight:stageWeight)
            =
            SorterAncestryP.update
                parentSorterAncestry
                sev.sortrId
                gen
                (sev.sortrPhenotypeId |> Option.get)
                (sev.sorterSpeed |> Option.get |> SorterFitness.fromSpeed stageWeight)

        let evalMap = sorterSetEval |> SorterSetEval.getSorterEvalsMap

        let _update  (sorterId, sorterParentId) =
            if (sorterId |> SorterId.value) = (sorterParentId |> SorterParentId.value) 
               then
                (sorterId, sorterSetAncestry.ancestorMap.[sorterId])
               else
                let parentSorterAncestry = 
                    sorterSetAncestry.ancestorMap.[sorterParentId |> SorterParentId.toSorterId]
                (
                    sorterId,
                    _updateSorterAncestry
                        parentSorterAncestry 
                        evalMap.[sorterId]
                        nextGen
                        stageWeight
                  )

        let newAncestorMap = 
            parentMap
            |> Map.toSeq 
            |> Seq.map(_update)
            |> Map.ofSeq

        {
            id = newId;
            generation = nextGen; 
            ancestorMap = newAncestorMap
        }