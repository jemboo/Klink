namespace global

open System

module WorkspaceComponentDto = 

    let toJsonT (comp:workspaceComponent) =
        match comp with
        | SortableSet sortableSet -> 
             sortableSet |> SortableSetDto.toJson
        | SorterSet sorterSet -> 
             sorterSet |> SorterSetDto.toJson
        | SorterSetMutator sorterSetMutator -> 
             sorterSetMutator |> SorterSetMutatorDto.toJson
        | SorterSetParentMap sorterSetParentMap -> 
             sorterSetParentMap |> SorterSetParentMapDto.toJson
        | SorterSetConcatMap sorterSetConcatMap -> 
             sorterSetConcatMap |> SorterSetConcatMapDto.toJson
        | SorterSetEval sorterSetEval -> 
             sorterSetEval |> SorterSetEvalDto.toJson
        | SorterSetAncestry sorterSetAncestry ->
             sorterSetAncestry |> SorterSetAncestryDto.toJson
        | SorterSpeedBinSet sorterSpeedBinSet ->
             sorterSpeedBinSet |> SorterSpeedBinSetDto.toJson
        | SorterSetPruner sorterSetPruner ->
             sorterSetPruner |> SorterSetPrunerWholeDto.toJson
        | WorkspaceDescription workspaceDescription ->
             workspaceDescription |> WorkspaceDescriptionDto.toJson
        | WorkspaceParams workspaceParams ->
            workspaceParams |> WorkspaceParamsDto.toJson



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
        | (c, workspaceComponentType.SorterSpeedBinSet) ->
            c |> SorterSpeedBinSetDto.fromJson |> Result.map(workspaceComponent.SorterSpeedBinSet)
        | (c, workspaceComponentType.SorterSetPruner) ->
            c |> SorterSetPrunerWholeDto.fromJson |> Result.map(workspaceComponent.SorterSetPruner)
        | (c, workspaceComponentType.WorkspaceParams) ->
            c |> WorkspaceParamsDto.fromJson |> Result.map(workspaceComponent.WorkspaceParams)
        | _ -> "unhandled workspaceComponentType" |> Error

