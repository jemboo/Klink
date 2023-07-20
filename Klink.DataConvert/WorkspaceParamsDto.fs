namespace global
open System
open Microsoft.FSharp.Core
        
type workspaceParamsDto = { 
        id: Guid
        data: Map<string,string>
     }

module WorkspaceParamsDto =

    let fromDto (dto:workspaceParamsDto) =
        WorkspaceParams.load
            (dto.id |> WorkspaceParamsId.create)
            (dto.data)

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<workspaceParamsDto> jstr
            return fromDto dto
        }

    let toDto (workspaceParams: workspaceParams) =
        {
            workspaceParamsDto.id = workspaceParams |> WorkspaceParams.getId |> WorkspaceParamsId.value
            data = workspaceParams |> WorkspaceParams.getData
        }

    let toJson (workspaceParams: workspaceParams) =
        workspaceParams |> toDto |> Json.serialize