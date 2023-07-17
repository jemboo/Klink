namespace global

open System

module WorkspaceComponentDto = 

    let toJsonT (comp:workspaceComponent) =
        match comp with
        | RandomProvider rngGenProvider -> 
            (rngGenProvider |> RngGenProviderDto.toJson, 
             workspaceComponentType.RandomProvider)
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
        | SorterSetPruner sorterSetPruner ->
            (sorterSetPruner |> SorterSetPrunerWholeDto.toJson, 
             workspaceComponentType.SorterSetPruner)
        | WorkspaceParams workspaceParams ->
            (workspaceParams |> WorkspaceParamsDto.toJson, 
             workspaceComponentType.WorkspaceParams)


    let fromJsonT (cerealT:string*workspaceComponentType) =
        match cerealT with
        | (c, workspaceComponentType.RandomProvider) ->
            c |> RngGenProviderDto.fromJson |> Result.map(workspaceComponent.RandomProvider)
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
        | (c, workspaceComponentType.SorterSetPruner) ->
            c |> SorterSetPrunerWholeDto.fromJson |> Result.map(workspaceComponent.SorterSetPruner)
        | (c, workspaceComponentType.WorkspaceParams) ->
            c |> WorkspaceParamsDto.fromJson |> Result.map(workspaceComponent.WorkspaceParams)
        | _ -> "unhandled workspaceComponentType" |> Error

