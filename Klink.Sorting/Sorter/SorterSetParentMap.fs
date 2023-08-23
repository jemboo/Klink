namespace global

open System


type sorterSetParentMap = 
        private {
        id: sorterSetParentMapId;
        childSetId:sorterSetId;
        parentSetId:sorterSetId;
        parentMap:Map<sorterId, sorterParentId> }

module SorterSetParentMap =

    let getId
            (sorterParentMap:sorterSetParentMap) 
         =
         sorterParentMap.id


    let getParentMap 
             (sorterParentMap:sorterSetParentMap) 
         =
         sorterParentMap.parentMap


    let getChildSorterSetId
                (sorterParentMap:sorterSetParentMap) 
         =
         sorterParentMap.childSetId


    let getParentSorterSetId
                (sorterParentMap:sorterSetParentMap) 
         =
         sorterParentMap.parentSetId

    let load
            (id:sorterSetParentMapId)
            (childSetId:sorterSetId)
            (parentSetId:sorterSetId)
            (parentMap:Map<sorterId, sorterParentId>)
        =
        {   
            id=id
            parentMap=parentMap
            childSetId=childSetId
            parentSetId=parentSetId
        }

    let makeId
            (parentSetId:sorterSetId)
            (childSetId:sorterSetId)
        =
        [|
            "sorterSetParentMap" :> obj
            parentSetId |> SorterSetId.value :> obj; 
            childSetId |> SorterSetId.value :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetParentMapId.create


    let create
            (childSetId:sorterSetId)
            (parentSetId:sorterSetId)
            (childSetCount:sorterCount)
            (parentSorterIds:sorterId[])
        =
        let parentMap =
            parentSorterIds
            |> Seq.map(SorterParentId.toSorterParentId)
            |> CollectionOps.infinteLoop
            |> Seq.zip (childSetId |> SorterSet.generateSorterIds)
            |> Seq.take (childSetCount |> SorterCount.value)
            |> Map.ofSeq

        let sorterParentMapId = 
                makeId
                   parentSetId
                   childSetId
        load
            sorterParentMapId
            childSetId
            parentSetId
            parentMap


    // adds self-mapping of the parent sorterId's
    let extendToParents
            (sspm:sorterSetParentMap)
        =
            sspm 
            |> getParentMap
            |> Map.values
            |> Seq.distinct
            |> Seq.map(fun v -> (v |> SorterParentId.toSorterId, v))
            |> Seq.append
                (sspm |> getParentMap |> Map.toSeq)
            |> Map.ofSeq