namespace global
open System
open Microsoft.FSharp.Core


type workspaceDto = 
        { 
            id:Guid; 
            parentId: Guid option; 
            wsNames:string[]; 
            workspaceComponentTypes:int[]; 
            workspaceComponentIds:Guid[] 
        }

module WorkspaceDto =

    let fromDto 
            (lookup:Guid -> workspaceComponentType -> Result<workspaceComponent, string>) 
            (dto:workspaceDto) =
        result {
            let compTypes = dto.workspaceComponentTypes |> Array.map(enum<workspaceComponentType>)
            let tupeLst = compTypes |> Array.zip(dto.workspaceComponentIds) |> Array.toList
            let! wsComps = tupeLst |> List.map(fun (gu, ct) -> lookup gu ct)
                           |> Result.sequence
            let compNames = dto.wsNames |> Array.map(WsComponentName.create)
            let compMap = (wsComps |> List.toArray) |> Array.zip compNames
                          |> Map.ofArray
            let ws = 
                {
                    workspace.id = dto.id |> WorkspaceId.create; 
                    parentId = dto.parentId |> Option.map(WorkspaceId.create)
                    wsComponents = compMap
                }
            return ws
        }

    let fromJson 
            (lookup:Guid -> workspaceComponentType -> Result<workspaceComponent, string>) 
            (jstr: string) =
        result {
            let! dto = Json.deserialize<workspaceDto> jstr
            return! fromDto lookup dto
        }

    let toDto (ws: workspace) =
        let wsNames, comps = ws.wsComponents |> Map.toArray |> Array.unzip
        { 
          workspaceDto.id = ws.id |> WorkspaceId.value
          parentId = ws.parentId |> Option.map(WorkspaceId.value)
          wsNames = wsNames |> Array.map(WsComponentName.value)
          workspaceComponentTypes = comps |> Array.map(WorkspaceComponent.getWorkspaceComponentType >> int)
          workspaceComponentIds = comps |> Array.map(WorkspaceComponent.getId)
        }

    let toJson (ws: workspace) =
        ws |> toDto |> Json.serialize
