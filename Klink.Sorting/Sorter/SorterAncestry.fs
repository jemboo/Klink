namespace global

open System

type sorterAncestry =
        private {
        sorterId: sorterId
        ancestors: List<(sorterId*generation)>
        }


module SorterAncestry =

    let getSorterId (sa:sorterAncestry) =
        sa.sorterId

    let getAncestors (sa:sorterAncestry) =
        sa.ancestors

    let create (sorterId:sorterId) =
        {
            sorterAncestry.sorterId = sorterId;
            ancestors = [(sorterId, Generation.create 0)]
        }

    let update 
            (sa:sorterAncestry) 
            (sorterId:sorterId) 
            (gen:generation) =
        {
            sorterAncestry.sorterId = sorterId;
            ancestors = (sorterId, gen) :: sa.ancestors
        }


type sorterSetAncestryId = private SorterSetAncestryId of Guid
module SorterSetAncestryId =
    let value (SorterSetAncestryId v) = v
    let create (v: Guid) = SorterSetAncestryId v


type sorterSetAncestry =
        private {
        id: sorterSetAncestryId;
        generation:generation;
        ancestorMap:Map<sorterId, sorterAncestry>
        }


module SorterSetAncestry =

    let getId (sa:sorterSetAncestry) =
        sa.id

    let getAncestorMap (sa:sorterSetAncestry) =
        sa.ancestorMap

    let create (id:sorterSetAncestryId) 
               (sorterIds:sorterId seq) 
        =
        {
            id = id;
            generation = 0 |> Generation.create; 
            ancestorMap = 
                sorterIds
                |> Seq.map(fun sid -> (sid, SorterAncestry.create sid))
                |> Map.ofSeq
        }

    let update 
            (newId:sorterSetAncestryId)
            (parentMap:Map<sorterId, sorterParentId>)
            (sorterSetAncestry:sorterSetAncestry) 
        =
        let nextGen = (sorterSetAncestry.generation |> Generation.next)

        let _update (sorterId, sorterParentId) =
            if (sorterId |> SorterId.value) = (sorterParentId |> SorterParentId.value) 
               then
                (sorterId, sorterSetAncestry.ancestorMap.[sorterId])
               else
                let parentAncestry = 
                    sorterSetAncestry.ancestorMap.[sorterParentId |> SorterParentId.toSorterId]
                (
                    sorterId,
                    SorterAncestry.update 
                        parentAncestry 
                        sorterId
                        nextGen
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

