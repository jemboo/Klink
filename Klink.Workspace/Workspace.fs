namespace global

open System

type workspaceComponent =
    | RandomProvider of rngGenProvider
    | SortableSet of sortableSet
    | SorterSet of sorterSet
    | SorterSetMutator of sorterSetMutator
    | SorterSetParentMap of sorterSetParentMap
    | SorterSetConcatMap of sorterSetConcatMap
    | SorterSetEval of sorterSetEval
    | SorterSetPruner of sorterSetPruner
    | JsonDataMap of jsonDataMap


module WorkspaceComponent =

    let getId (comp:workspaceComponent) =
        match comp with
        | RandomProvider rngGenProvider -> 
            rngGenProvider 
                |> RngGenProvider.getId |> RngGenProviderId.value
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
        | JsonDataMap jsonDataMap ->
            jsonDataMap 
                |> JsonDataMap.getId |> JsonDataMapId.value


    let getWorkspaceComponentType (comp:workspaceComponent) =
        match comp with
        | RandomProvider randomProvider -> 
             workspaceComponentType.RandomProvider
        | SortableSet sortableSet -> 
             workspaceComponentType.SortableSet
        | SorterSet sorterSet -> 
             workspaceComponentType.SorterSet
        | SorterSetMutator sorterSetMutator -> 
             workspaceComponentType.SorterSetMutator
        | SorterSetParentMap sorterSetParentMap -> 
             workspaceComponentType.SorterSetParentMap
        | SorterSetConcatMap sorterSetConcatMap -> 
             workspaceComponentType.SorterSetConcatMap
        | SorterSetEval sorterSetEval -> 
             workspaceComponentType.SorterSetEval
        | SorterSetPruner sorterSetPruner ->
            workspaceComponentType.SorterSetPruner


    let asRandomProvider (comp:workspaceComponent) =
        match comp with
        | RandomProvider rngGenProvider -> 
             rngGenProvider |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not rngGenProvider" |> Error



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


    let asGaMetaData (comp:workspaceComponent) =
        match comp with
        | JsonDataMap gaMetaData -> 
             gaMetaData |> Ok
        | _  -> 
             $"Workspace component type is {comp}, not GaMetaData" |> Error


type workspace = private {
        id:workspaceId;
        parentId:workspaceId option;
        wsComponents: Map<wsComponentName, workspaceComponent>
    }

module Workspace = 

    let getId (ws:workspace) = ws.id

    let getParentId (ws:workspace) = ws.parentId

    let getWsComponents (ws:workspace) = ws.wsComponents

    let empty = 
        {
            id = [CauseId.empty |> CauseId.value :> obj]            
                    |> GuidUtils.guidFromObjs 
                    |> WorkspaceId.create
            parentId = None
            wsComponents = Map.empty
        }

    let load 
            (id:workspaceId) 
            (parentId:workspaceId option) 
            (tupes:seq<wsComponentName*workspaceComponent>)
        =
        {
            workspace.id = id;
            parentId = parentId;
            wsComponents = tupes |> Map.ofSeq
        }

    let addComponents
            (newWorkspaceId:workspaceId) 
            (tupes:seq<wsComponentName*workspaceComponent>)
            (workspace:workspace)  =
        let newMap =
            tupes |> Seq.fold (fun a t -> a |> Map.add (fst t) (snd t)) 
                              (workspace |> getWsComponents)
        {
            id = newWorkspaceId
            parentId = Some workspace.id
            wsComponents = newMap
        }


    let getComponent
            (compName:wsComponentName)
            (workspace:workspace)
        =
        if (workspace.wsComponents.ContainsKey(compName) |> not) then
            $"{compName |> WsComponentName.value} not present (10)" 
            |> Error
        else
           workspace.wsComponents.[compName] |> Ok



    let removeComponent
            (workspace:workspace)
            (compName:wsComponentName)
        =
        if (workspace.wsComponents.ContainsKey(compName)) then
            load 
                workspace.id
                workspace.parentId
                (workspace.wsComponents.Remove compName |> Map.toSeq)
            |> Ok
        else
            $"{compName |> WsComponentName.value} not present (11)" 
            |> Error
