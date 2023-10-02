namespace global

open System

type genInfo =
        private {
        generation:generation
        sorterId: sorterId
        sorterPhenotypeId:sorterPhenotypeId
        sorterFitness:sorterFitness
        }


module GenInfo =

    let create 
        (gen:generation)
        (sorterId:sorterId)
        (sorterPhenotypeId:sorterPhenotypeId)
        (sorterFitness:sorterFitness)
        =
        {
            generation = gen
            sorterId = sorterId
            sorterPhenotypeId = sorterPhenotypeId
            sorterFitness = sorterFitness
        }

    let getGeneration (sa:genInfo) =
        sa.generation

    let getSorterId (sa:genInfo) =
        sa.sorterId

    let getSorterPhenotypeId (sa:genInfo) =
        sa.sorterPhenotypeId

    let getSorterFitness (sa:genInfo) =
        sa.sorterFitness


type sorterAncestryP =
        private {
        sorterId: sorterId
        ancestors: List<genInfo>
        }


module SorterAncestryP =

    let getSorterId (sa:sorterAncestryP) =
        sa.sorterId

    let getAncestors (sa:sorterAncestryP) =
        sa.ancestors

    let load (sorterId: sorterId)
             (ancestors: genInfo[])
        =
        {
            sorterId =sorterId
            ancestors = ancestors |> Array.toList
        }

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
            (sorterId:sorterId) 
            (generation:generation) 
            (sorterPhenotypeId:sorterPhenotypeId) 
            (sorterFitness:sorterFitness) 
            (parentSorterAncestry:sorterAncestryP) 
        =
        let _replace newInfo last history =
            if newInfo.sorterPhenotypeId = last.sorterPhenotypeId then
                match history with
                | [] -> [last]
                | _ ->  newInfo::history
            else
                newInfo::last::history

        let newStuff = 
            {
                genInfo.sorterId = sorterId;
                generation = generation
                sorterPhenotypeId = sorterPhenotypeId
                sorterFitness = sorterFitness
            }

        let updatedAncestors =
            match parentSorterAncestry.ancestors with
            | [] -> 
                [newStuff]
            | last::history -> 
                let yab = _replace newStuff last history
                yab

        {
            sorterAncestryP.sorterId = sorterId;
            ancestors = updatedAncestors
        }


type sorterSetAncestryP =
        private {
            id: sorterSetAncestryId;
            generation:generation;
            ancestorMap:Map<sorterId, sorterAncestryP>
            tag:Guid
        }


module SorterSetAncestryP =

    let getId (sa:sorterSetAncestryP) =
        sa.id

    let getGeneration (sa:sorterSetAncestryP) =
        sa.generation

    let getAncestorMap (sa:sorterSetAncestryP) =
        sa.ancestorMap

    let load (id:sorterSetAncestryId) 
             (generation:generation)
             (ancestors:sorterAncestryP[])
             (tag:Guid) 
        =
        let ancestorMap = 
                ancestors
                |> Array.map(fun am -> (am.sorterId, am))
                |> Map.ofArray
        {
            sorterSetAncestryP.id = id;
            generation = generation;
            ancestorMap = ancestorMap
            tag = tag
        }


    let create (sorterSetEval:sorterSetEval)
               (stageWeight:stageWeight)
               (generation:generation)
               (tag:Guid)
        =
        let sorterSetAncestryId = 
              SorterSetAncestryId.fromTag tag generation

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

        {
            id = sorterSetAncestryId;
            generation = generation
            ancestorMap = 
                sorterSetEval.sorterEvals
                |> Map.toArray
                |> Array.map(fun (srtrId, sev) -> (srtrId, _makeSorterAncestry sev generation stageWeight))
                |> Map.ofSeq
            tag = tag
        }


    let update
            (generation:generation)
            (stageWeight:stageWeight)
            (sorterSetEval:sorterSetEval)
            (parentMap:Map<sorterId, sorterParentId>)
            (sorterSetAncestry:sorterSetAncestryP)
        =
        let newId = SorterSetAncestryId.fromTag sorterSetAncestry.tag generation

        let _updateSorterAncestry 
                (parentSorterAncestry:sorterAncestryP)
                (sev:sorterEval)
                (gen:generation)
                (stageWeight:stageWeight)
            =
            parentSorterAncestry
                |> SorterAncestryP.update
                    sev.sortrId
                    gen
                    (sev.sortrPhenotypeId |> Option.get)
                    (sev.sorterSpeed |> Option.get |> SorterFitness.fromSpeed stageWeight)

        let evalMap = sorterSetEval |> SorterSetEval.getSorterEvalsMap

        let _update  (sorterId, sorterParentId) =
            let asSorterId = sorterParentId |> SorterParentId.toSorterId
            if (sorterId |> SorterId.value) = (sorterParentId |> SorterParentId.value) 
               then
                (sorterId, sorterSetAncestry.ancestorMap.[asSorterId])

               else
                let parentSorterAncestry = 
                    sorterSetAncestry.ancestorMap.[asSorterId]
                let updatedSorterAncestry = 
                     _updateSorterAncestry
                        parentSorterAncestry 
                        evalMap.[sorterId]
                        generation
                        stageWeight
                (
                    sorterId,
                    updatedSorterAncestry
                )

        // if the sorter is not in the parent map, then it is it's own parent
        let _lookupParentSorter (sorterId:sorterId) =
            if (parentMap.ContainsKey sorterId) then
                parentMap.[sorterId]
            else
                sorterId |> SorterParentId.toSorterParentId

        let newAncestorMap = 
            sorterSetEval
            |> SorterSetEval.getSorterEvalsMap
            |> Map.toArray
            |> Array.map(fst)
            |> Array.map(fun sorterId -> (sorterId, _lookupParentSorter sorterId))
            |> Seq.map(_update)
            |> Map.ofSeq

        {
            id = newId;
            generation = generation; 
            ancestorMap = newAncestorMap
            tag = sorterSetAncestry.tag
        }