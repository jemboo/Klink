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


type sorterAncestry =
        private {
        sorterId: sorterId
        ancestors: List<genInfo>
        }


module SorterAncestry =

    let getSorterId (sa:sorterAncestry) =
        sa.sorterId

    let getAncestors (sa:sorterAncestry) =
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
            sorterAncestry.sorterId = sorterId;
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
            (parentSorterAncestry:sorterAncestry) 
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
            sorterAncestry.sorterId = sorterId;
            ancestors = updatedAncestors
        }



type sorterSetAncestryId = private SorterSetAncestryId of Guid
module SorterSetAncestryId =
    let value (SorterSetAncestryId v) = v
    let create (v: Guid) = SorterSetAncestryId v
    let fromTag 
            (tag:Guid) 
            (generation:generation)
        =
        [|
            tag:> obj;
            generation |> Generation.value :> obj;
            "sorterSetAncestry" :> obj;
        |] |> GuidUtils.guidFromObjs  
            |> create


type sorterSetAncestry =
        private {
            id: sorterSetAncestryId;
            generation:generation;
            ancestorMap:Map<sorterId, sorterAncestry>
            tag:Guid
        }


module SorterSetAncestry =

    let getId (sa:sorterSetAncestry) =
        sa.id

    let getGeneration (sa:sorterSetAncestry) =
        sa.generation

    let getAncestorMap (sa:sorterSetAncestry) =
        sa.ancestorMap

    let getTag (sa:sorterSetAncestry) =
        sa.tag

    let load (id:sorterSetAncestryId) 
             (generation:generation)
             (ancestors:sorterAncestry[])
             (tag:Guid) 
        =
        let ancestorMap = 
                ancestors
                |> Array.map(fun am -> (am.sorterId, am))
                |> Map.ofArray
        {
            sorterSetAncestry.id = id;
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
            SorterAncestry.create
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
            (sorterSetAncestry:sorterSetAncestry)
        =
        let newId = SorterSetAncestryId.fromTag sorterSetAncestry.tag generation

        let _updateSorterAncestry 
                (parentSorterAncestry:sorterAncestry)
                (sev:sorterEval)
                (gen:generation)
                (stageWeight:stageWeight)
            =
            parentSorterAncestry
                |> SorterAncestry.update
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