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



    let fromJson (wct:workspaceComponentType) (cereal:string) =
        match wct with
        | workspaceComponentType.SortableSet ->
            cereal |> SortableSetDto.fromJson |> Result.map(workspaceComponent.SortableSet)
        | workspaceComponentType.SorterSet ->
            cereal |> SorterSetDto.fromJson |> Result.map(workspaceComponent.SorterSet)
        | workspaceComponentType.SorterSetAncestry ->
            cereal |> SorterSetAncestryDto.fromJson |> Result.map(workspaceComponent.SorterSetAncestry)
        | workspaceComponentType.SorterSetMutator ->
            cereal |> SorterSetMutatorDto.fromJson |> Result.map(workspaceComponent.SorterSetMutator)
        | workspaceComponentType.SorterSetConcatMap ->
            cereal |> SorterSetConcatMapDto.fromJson |> Result.map(workspaceComponent.SorterSetConcatMap)
        | workspaceComponentType.SorterSetParentMap ->
            cereal |> SorterSetParentMapDto.fromJson |> Result.map(workspaceComponent.SorterSetParentMap)
        | workspaceComponentType.SorterSetEval ->
            cereal |> SorterSetEvalDto.fromJson |> Result.map(workspaceComponent.SorterSetEval)
        | workspaceComponentType.SorterSpeedBinSet ->
            cereal |> SorterSpeedBinSetDto.fromJson |> Result.map(workspaceComponent.SorterSpeedBinSet)
        | workspaceComponentType.SorterSetPruner ->
            cereal |> SorterSetPrunerWholeDto.fromJson |> Result.map(workspaceComponent.SorterSetPruner)
        | workspaceComponentType.WorkspaceDescription ->
            cereal |> WorkspaceDescriptionDto.fromJson |> Result.map(workspaceComponent.WorkspaceDescription)
        | workspaceComponentType.WorkspaceParams ->
            cereal |> WorkspaceParamsDto.fromJson |> Result.map(workspaceComponent.WorkspaceParams)
        | _ -> "unhandled workspaceComponentType" |> Error

