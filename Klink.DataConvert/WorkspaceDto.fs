namespace global
open System
open Microsoft.FSharp.Core

type workspaceDto = { id:Guid; ts:int }

module WorkspaceDto =

    let fromDto (dto:workspaceDto) =
        result {
            let myEnumValue =  enum<workspaceComponentType>(dto.ts)
            let ws = 
                {
                    workspace.id = dto.id |> WorkspaceId.create; 
                    items = Map.empty
                }
            return ws
        }


    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<workspaceDto> jstr
            return! fromDto dto
        }


    let toDto (ws: workspace) =
        { 
          workspaceDto.id = ws.id |> WorkspaceId.value
          ts = workspaceComponentType.SortableSet |> int
        }


    let toJson (ws: workspace) =
        ws |> toDto |> Json.serialize
