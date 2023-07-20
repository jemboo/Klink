namespace global
open System
open System.IO

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

    member this.getAllFiles (wsCompType:workspaceComponentType) =
           let filePath = Path.Combine(this.wsRootDir, this.getFolderName wsCompType)
           Directory.GetFiles(filePath)


    member this.compStore (wsComp:workspaceComponent) 
        = 
        result {
            let cereal, wsCompType = wsComp |> WorkspaceComponentDto.toJsonT
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
                | workspaceComponentType.WorkspaceDescription ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(WorkspaceDescriptionDto.fromJson)
                    |> Result.map(workspaceComponent.WorkspaceDescription)
                | workspaceComponentType.WorkspaceParams ->
                    this.readAllText wsCompType fileName
                    |> Result.bind(WorkspaceParamsDto.fromJson)
                    |> Result.map(workspaceComponent.WorkspaceParams)
                | _ 
                    -> $"{wsCompType} not handled (001)" |> Error
        }

    member this.getAllComponents
                    (wsCompType:workspaceComponentType) 
        = 
        this.getAllFiles wsCompType 
            |> Array.map(Path.GetFileNameWithoutExtension >> Guid.Parse)
            |> Array.map(fun gu -> this.compRetreive gu wsCompType)
           

    member this.workSpaceExists (id:workspaceId) =
        result {
            let fileName = id |> WorkspaceId.value |> string
            return!                   
                this.fileExists workspaceComponentType.WorkspaceDescription fileName
        }

    member this.loadWorkSpace (id:workspaceId) =
        result {
            let fileName = id |> WorkspaceId.value |> string
            let! cereal = this.readAllText workspaceComponentType.WorkspaceDescription fileName
            let! wsd = cereal |> WorkspaceDescriptionDto.fromJson
            return! wsd |> Workspace.ofWorkspaceDescription this.compRetreive
        }

    member this.saveWorkSpace (workspace:workspace) =
        result {
            let fileName = workspace |> Workspace.getId |> WorkspaceId.value |> string
            let cereal = workspace |> Workspace.toWorkspaceDescription
                                   |> WorkspaceDescriptionDto.toJson
            let! res = this.writeToFile workspaceComponentType.WorkspaceDescription fileName cereal
            let _, comps = workspace |> Workspace.getWsComponents |> Map.toArray |> Array.unzip
            let! _ = comps |> Array.toList |> List.map(this.compStore) |> Result.sequence
            return fileName
        }

    interface IWorkspaceStore with
        member this.SaveWorkSpace(ws) = this.saveWorkSpace(ws)
        member this.LoadWorkSpace(wsId) = this.loadWorkSpace(wsId)
        member this.WorkSpaceExists(wsId) = this.workSpaceExists(wsId)
