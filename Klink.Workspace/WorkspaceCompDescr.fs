namespace global

open System


type workspaceComponentDescr = 
        private 
            { compId:Guid; 
              compType:workspaceComponentType }


module WorkspaceComponentDescr 
        =
    let create 
            (id:Guid) 
            (compType:workspaceComponentType)
        =
        {
            compId = id;
            compType = compType;
        }

    let getId (descr:workspaceComponentDescr)
        =
            descr.compId

    let getCompType (descr:workspaceComponentDescr)
        =
            descr.compType

    let toTuple (descr:workspaceComponentDescr)
        =
        (descr.compId, descr.compType |> int)


    let fromTuple (tup:Guid*int)
        =
        create
            (tup |> fst) 
            (tup |> snd |> enum<workspaceComponentType>)



type workspaceComponent =
    | SortableSet of sortableSet
    | SorterSet of sorterSet
    | SorterSetMutator of sorterSetMutator
    | SorterSetParentMap of sorterSetParentMap
    | SorterSetConcatMap of sorterSetConcatMap
    | SorterSetEval of sorterSetEval
    | SorterSetPruner of sorterSetPruner
    | WorkspaceDescription of workspaceDescription
    | WorkspaceParams of workspaceParams
and workspaceDescription = 
     private
        { 
            id:workspaceId; 
            parentId: workspaceId option;
            grandParentId: workspaceId option;
            lastCauseName: string;
            components: Map<wsComponentName, workspaceComponentDescr>
        }

module WorkspaceDescription =

    let create 
            (id:workspaceId)
            (parentId:workspaceId option)
            (grandParentId:workspaceId option)
            (lastCauseName:string)
            (components: Map<wsComponentName, workspaceComponentDescr>)
        =
        {
            workspaceDescription.id = id
            parentId = parentId
            grandParentId = grandParentId
            lastCauseName = lastCauseName
            components = components
        }

    let getId (descr:workspaceDescription) =
        descr.id

    let getParentId (descr:workspaceDescription) =
        descr.parentId

    let getGrandParentId (descr:workspaceDescription) =
        descr.grandParentId
        
    let getLastCauseName (descr:workspaceDescription) =
        descr.lastCauseName

    let getComponents (descr:workspaceDescription) =
        descr.components


module WorkspaceComponent =

    let getId (comp:workspaceComponent) =
        match comp with
        | SortableSet sortableSet -> 
            sortableSet 
                |> SortableSet.getSortableSetId |> SortableSetId.value
        | SorterSet sorterSet -> 
            sorterSet 
                |> SorterSet.getId |> SorterSetId.value
        | SorterSetMutator sorterSetMutator -> 
            sorterSetMutator 
                |> SorterSetMutator.getId |> SorterSetMutatorId.value
        | SorterSetParentMap sorterSetParentMap -> 
            sorterSetParentMap 
                |> SorterSetParentMap.getId |> SorterSetParentMapId.value
        | SorterSetConcatMap sorterSetConcatMap -> 
            sorterSetConcatMap 
                |> SorterSetConcatMap.getId |> SorterSetConcatMapId.value
        | SorterSetEval sorterSetEval -> 
            sorterSetEval 
                |> SorterSetEval.getSorterSetEvalId |> SorterSetEvalId.value
        | SorterSetPruner sorterSetPruner ->
            sorterSetPruner 
                |> SorterSetPruner.getId |> SorterSetPrunerId.value
        | WorkspaceDescription workspaceDescription ->
            workspaceDescription
                |> WorkspaceDescription.getId |> WorkspaceId.value
        | WorkspaceParams workspaceParams ->
            workspaceParams 
                |> WorkspaceParams.getId |> WorkspaceParamsId.value


    let getWorkspaceComponentType (comp:workspaceComponent) =
        match comp with
        | SortableSet _ -> 
             workspaceComponentType.SortableSet
        | SorterSet _ -> 
             workspaceComponentType.SorterSet
        | SorterSetMutator _ -> 
             workspaceComponentType.SorterSetMutator
        | SorterSetParentMap _ -> 
             workspaceComponentType.SorterSetParentMap
        | SorterSetConcatMap _ -> 
             workspaceComponentType.SorterSetConcatMap
        | SorterSetEval _ -> 
             workspaceComponentType.SorterSetEval
        | SorterSetPruner _ ->
            workspaceComponentType.SorterSetPruner
        | WorkspaceDescription _ ->
            workspaceComponentType.WorkspaceDescription
        | WorkspaceParams _ ->
            workspaceComponentType.WorkspaceParams


    let getWorkspaceComponentDescr (comp:workspaceComponent) =
        match comp with
        | SortableSet _ -> 
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.SortableSet
        | SorterSet _ -> 
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.SorterSet
        | SorterSetMutator _ -> 
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.SorterSetMutator
        | SorterSetParentMap _ -> 
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.SorterSetParentMap
        | SorterSetConcatMap _ -> 
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.SorterSetConcatMap
        | SorterSetEval _ -> 
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.SorterSetEval
        | SorterSetPruner _ ->
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.SorterSetPruner
        | WorkspaceDescription _ ->
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.WorkspaceDescription
        | WorkspaceParams _ ->
            WorkspaceComponentDescr.create
                 (comp |> getId)
                 workspaceComponentType.WorkspaceParams


    let asSortableSet (comp:workspaceComponent) =
        match comp with
        | SortableSet sortableSet -> 
             sortableSet |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SortableSet" |> Error


    let asSorterSet (comp:workspaceComponent) =
        match comp with
        | SorterSet sorterSet -> 
             sorterSet |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSet" |> Error


    let asSorterSetMutator (comp:workspaceComponent) =
        match comp with
        | SorterSetMutator sorterSetMutator -> 
             sorterSetMutator |> Ok
        | _ -> 
             $"Workspace component type is {comp}, not SorterSetMutator" |> Error


    let asSorterSetParentMap (comp:workspaceComponent) =
        match comp with
        | SorterSetParentMap sorterSetParentMap -> 
             sorterSetParentMap |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetParentMap" |> Error


    let asSorterSetConcatMap (comp:workspaceComponent) =
        match comp with
        | SorterSetConcatMap sorterSetConcatMap -> 
             sorterSetConcatMap |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetConcatMap" |> Error


    let asSorterSetEval (comp:workspaceComponent) =
        match comp with
        | SorterSetEval sorterSetEval -> 
             sorterSetEval |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetEval" |> Error


    let asSorterSetPruner (comp:workspaceComponent) =
        match comp with
        | SorterSetPruner sorterSetPruner -> 
             sorterSetPruner |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not SorterSetPruner" |> Error


    let asWorkspaceDescription (comp:workspaceComponent) =
        match comp with
        | WorkspaceDescription workspaceDescription -> 
             workspaceDescription |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not WorkspaceParams" |> Error


    let asWorkspaceParams (comp:workspaceComponent) =
        match comp with
        | WorkspaceParams workspaceParams -> 
             workspaceParams |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not WorkspaceParams" |> Error
