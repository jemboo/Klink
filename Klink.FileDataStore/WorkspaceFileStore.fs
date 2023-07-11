namespace global
open System

type WorkspaceFileStore (wsRootDir:string) =

    member this.wsRootDir = wsRootDir
    member this.fileExt = "txt"


    member this.getFolderName (wsCompType:workspaceComponentType) =
        wsCompType |> string

    member this.writeToFile (wsCompType:workspaceComponentType) (fileName:string) (data: string) =
        TextIO.writeToFile this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName data

    member this.writeLinesIfNew (wsCompType:workspaceComponentType) (fileName:string) (data: string seq) =
        TextIO.writeLinesIfNew this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName data

    member this.appendLines (wsCompType:workspaceComponentType) (fileName:string) (data: string seq) =
        TextIO.appendLines this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName data

    member this.fileExists (wsCompType:workspaceComponentType) (fileName:string) =
        TextIO.fileExists this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName

    member this.readAllText (wsCompType:workspaceComponentType) (fileName:string) =
        TextIO.readAllText this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName

    member this.readAllLines (wsCompType:workspaceComponentType) (fileName:string) =
        TextIO.readAllLines this.fileExt (Some this.wsRootDir) (this.getFolderName wsCompType) fileName


    member this.compStore (wsComp:workspaceComponent) 
        = 
        result {
            let cereal, wsCompType = wsComp |> WorkspaceComponent.toJsonT
            let fileName = wsComp |> WorkspaceComponent.getId |> string
            let! res = this.writeToFile wsCompType fileName cereal
            return fileName
        }


    member this.compRetreive
                (compId:Guid) 
                (wsCompType:workspaceComponentType) 
        = 
        result {
            let fileName = compId |> string
            return!
                match wsCompType with
                | workspaceComponentType.RandomProvider ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(RngGenProviderDto.fromJson)
                    |> Result.map(workspaceComponent.RandomProvider)
                | workspaceComponentType.SortableSet ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(SortableSetDto.fromJson)
                    |> Result.map(workspaceComponent.SortableSet)
                | workspaceComponentType.SorterSet ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(SorterSetDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSet)
                | workspaceComponentType.SorterSetMutator ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(SorterSetMutatorDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetMutator)
                | workspaceComponentType.SorterSetParentMap ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(SorterSetParentMapDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetParentMap)
                | workspaceComponentType.SorterSetConcatMap ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(SorterSetConcatMapDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetConcatMap)
                | workspaceComponentType.SorterSetEval ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(SorterSetEvalDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetEval)
                | workspaceComponentType.SorterSetPruner ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(SorterSetPrunerWholeDto.fromJson)
                    |> Result.map(workspaceComponent.SorterSetPruner)
                | _ 
                    -> $"{wsCompType} not handled (001)" |> Error
        }

    member this.workSpaceExists (id:workspaceId) =
        result {
            let fileName = id |> WorkspaceId.value |> string
            return!                   
                this.fileExists workspaceComponentType.WorkspaceDto fileName
        }

    member this.loadWorkSpace (id:workspaceId) =
        result {
            let fileName = id |> WorkspaceId.value |> string
            return!                   
                this.readAllText workspaceComponentType.WorkspaceDto fileName
                |> Result.bind(WorkspaceDto.fromJson this.compRetreive)
        }

    member this.saveWorkSpace (workspace:workspace) =
        result {
            let fileName = workspace |> Workspace.getId |> WorkspaceId.value |> string
            let wsCereal = workspace |> WorkspaceDto.toJson
            let! res = this.writeToFile workspaceComponentType.WorkspaceDto fileName wsCereal
            let _, comps = workspace |> Workspace.getItems |> Map.toArray |> Array.unzip
            let! _ = comps |> Array.toList |> List.map(this.compStore) |> Result.sequence
            return fileName
        }

    interface IWorkspaceStore with
        member this.SaveWorkSpace(ws) = this.saveWorkSpace(ws)
        member this.LoadWorkSpace(wsId) = this.loadWorkSpace(wsId)
        member this.WorkSpaceExists(wsId) = this.workSpaceExists(wsId)
