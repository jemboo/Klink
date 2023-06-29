namespace global
open System

module FileStore =

    let wsRootDir = "c:\\KlinkFiles"
    let fileExt = "txt"


    let getFolderName (wsCompType:workspaceComponentType) =
        nameof wsCompType

    let writeToFile (wsCompType:workspaceComponentType) (fileName:string) (data: string) =
        TextIO.writeToFile fileExt (Some wsRootDir) (getFolderName wsCompType) fileName data

    let writeLinesIfNew (wsCompType:workspaceComponentType) (fileName:string) (data: string seq) =
        TextIO.writeLinesIfNew fileExt (Some wsRootDir) (getFolderName wsCompType) fileName data

    let appendLines (wsCompType:workspaceComponentType) (fileName:string) (data: string seq) =
        TextIO.appendLines fileExt (Some wsRootDir) (getFolderName wsCompType) fileName data

    let readAllText (wsCompType:workspaceComponentType) (fileName:string) =
        TextIO.readAllText fileExt (Some wsRootDir) (getFolderName wsCompType) fileName

    let readAllLines (wsCompType:workspaceComponentType) (fileName:string) =
        TextIO.readAllLines fileExt (Some wsRootDir) (getFolderName wsCompType) fileName


    let compStore
            (wsComp:workspaceComponent) 
        = 
        result {
            let cereal, wsCompType = wsComp |> WorkspaceComponent.toJsonT
            let fileName = wsComp |> WorkspaceComponent.getId |> string
            let! res = writeToFile wsCompType fileName cereal
            return fileName
        }



    let compRetreive
                (compId:Guid) 
                (wsCompType:workspaceComponentType) 
        = 
        result {
            let fileName = compId |> string
            return!
                match wsCompType with
                | workspaceComponentType.SortableSet ->
                    readAllText wsCompType fileName
                    |> Result.bind(SortableSetDto.fromJson)
                    |> Result.map(workspaceComponent.SortableSet)
                | workspaceComponentType.SorterSet ->
                    readAllText wsCompType fileName
                    |> Result.bind(SorterSetDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSet)
                | workspaceComponentType.SorterSetMutator ->
                    readAllText wsCompType fileName
                    |> Result.bind(SorterSetMutatorDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetMutator)
                | workspaceComponentType.SorterSetParentMap ->
                    readAllText wsCompType fileName
                    |> Result.bind(SorterSetParentMapDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetParentMap)
                | workspaceComponentType.SorterSetConcatMap ->
                    readAllText wsCompType fileName
                    |> Result.bind(SorterSetConcatMapDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetConcatMap)
                | workspaceComponentType.SorterSetEval ->
                    readAllText wsCompType fileName
                    |> Result.bind(SorterSetEvalDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetEval)
                | _ 
                    -> $"{wsCompType} not handled" |> Error
        }


    let loadWorkSpaceFromId (id:workspaceId) =
        result {
            let fileName = id |> WorkspaceId.value |> string
            return!                   
                readAllText workspaceComponentType.WorkspaceDto fileName
                |> Result.bind(WorkspaceDto.fromJson compRetreive)
        }


    let saveWorkSpace (workspace:workspace) =
        result {
            let fileName = workspace |> Workspace.getId |> WorkspaceId.value |> string
            let wsCereal = workspace |> WorkspaceDto.toJson
            let _, comps = workspace |> Workspace.getItems |> Map.toArray |> Array.unzip
            let! _ = comps |> Array.toList |> List.map(compStore) |> Result.sequence
            return fileName
        }