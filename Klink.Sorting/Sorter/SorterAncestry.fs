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

    let getAncestorMap (sa:sorterSetAncestry) =
        sa.ancestorMap

    let create (tag:Guid) 
               (sorterIds:sorterId seq) 
               (generation:generation)
        =
        let id = SorterSetAncestryId.fromTag tag generation
        {
            id = id;
            generation = 0 |> Generation.create; 
            ancestorMap = 
                sorterIds
                |> Seq.map(fun sid -> (sid, SorterAncestry.create sid))
                |> Map.ofSeq
            tag = tag
        }

    let update 
            (parentMap:Map<sorterId, sorterParentId>)
            (sorterSetAncestry:sorterSetAncestry) 
        =
        let nextGen = (sorterSetAncestry.generation |> Generation.next)
        let newId = SorterSetAncestryId.fromTag sorterSetAncestry.tag nextGen
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
            tag = sorterSetAncestry.tag
        }

