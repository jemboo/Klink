namespace global

open System

type workspaceComponent =
    | SortableSet of sortableSet
    | SorterSet of sorterSet
    | SorterSetMutator of sorterSetMutator
    | SorterSetParentMap of sorterSetParentMap
    | SorterSetConcatMap of sorterSetConcatMap
    | SorterSetEval of sorterSetEval


module WorkspaceComponent = 

    let toJsonT (comp:workspaceComponent) =
        match comp with
        | SortableSet sortableSet -> 
            (sortableSet |> SortableSetDto.toJson, 
             workspaceComponentType.SortableSet)
        | SorterSet sorterSet -> 
            (sorterSet |> SorterSetDto.toJson, 
             workspaceComponentType.SorterSet)
        | SorterSetMutator sorterSetMutator -> 
            (sorterSetMutator |> SorterSetMutatorDto.toJson, 
             workspaceComponentType.SorterSetMutator)
        | SorterSetParentMap sorterSetParentMap -> 
            (sorterSetParentMap |> SorterSetParentMapDto.toJson, 
             workspaceComponentType.SorterSetParentMap)
        | SorterSetConcatMap sorterSetConcatMap -> 
            (sorterSetConcatMap |> SorterSetConcatMapDto.toJson, 
             workspaceComponentType.SorterSetConcatMap)
        | SorterSetEval sorterSetEval -> 
            (sorterSetEval |> SorterSetEvalDto.toJson, 
             workspaceComponentType.SorterSetEval)


    let fromJsonT (cerealT:string*workspaceComponentType) =
        match cerealT with
        | (c, workspaceComponentType.SortableSet) ->
            c |> SortableSetDto.fromJson |> Result.map(workspaceComponent.SortableSet)
        | (c, workspaceComponentType.SorterSet) ->
            c |> SorterSetDto.fromJson |> Result.map(workspaceComponent.SorterSet)
        | (c, workspaceComponentType.SorterSetMutator) ->
            c |> SorterSetMutatorDto.fromJson |> Result.map(workspaceComponent.SorterSetMutator)
        | (c, workspaceComponentType.SorterSetParentMap) ->
            c |> SorterSetParentMapDto.fromJson |> Result.map(workspaceComponent.SorterSetParentMap)
        | (c, workspaceComponentType.SorterSetConcatMap) ->
            c |> SorterSetConcatMapDto.fromJson |> Result.map(workspaceComponent.SorterSetConcatMap)
        | (c, workspaceComponentType.SorterSetEval) ->
            c |> SorterSetEvalDto.fromJson |> Result.map(workspaceComponent.SorterSetEval)
        | _ -> "unhandled workspaceComponentType" |> Error


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


    let getWorkspaceComponentType (comp:workspaceComponent) =
        match comp with
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




type workspace = private {
        id:workspaceId;
        items: Map<wsComponentName, workspaceComponent>
    }

module Workspace = 

    let getId (ws:workspace) = ws.id

    let getItems (ws:workspace) = ws.items

    let empty = 
        {
            id = [CauseId.empty |> CauseId.value :> obj]            
                    |> GuidUtils.guidFromObjs 
                    |> WorkspaceId.create
            items = Map.empty
        }

    let addComponent 
            (newWorkspaceId:workspaceId) 
            (compName:wsComponentName)
            (comp:workspaceComponent)
            (workspace:workspace)
        =
        if (workspace.items.ContainsKey(compName)) then
            $"{compName |> WsComponentName.value} already present" 
            |> Error
        else
        {
            id = newWorkspaceId
            items = workspace.items.Add (compName, comp)
        } |> Ok


    let removeComponent 
            (newWorkspaceId:workspaceId) 
            (compName:wsComponentName)
            (workspace:workspace)
        =
        if (workspace.items.ContainsKey(compName)) then
            $"{compName |> WsComponentName.value} not present" 
            |> Error
        else
        {
            id = newWorkspaceId
            items = workspace.items.Remove compName
        } |> Ok