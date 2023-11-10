namespace global
open System
open Microsoft.FSharp.Core
        
type workspaceParamsDto = { 
        id: Guid
        data: Map<string,string>
     }

module WorkspaceParamsDto =

    let fromDto (dto:workspaceParamsDto) =
        let rkm = dto.data 
                    |> Map.toSeq 
                    |> Seq.map(fun (k,v) -> (k |> WorkspaceParamsKey.create ,v))
                    |> Map.ofSeq

        WorkspaceParams.load
            (dto.id |> WorkspaceParamsId.create)
            rkm

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<workspaceParamsDto> jstr
            return fromDto dto
        }

    let toDto (workspaceParams: workspaceParams) =
        {
            workspaceParamsDto.id = workspaceParams |> WorkspaceParams.getId |> WorkspaceParamsId.value
            data = workspaceParams 
                        |> WorkspaceParams.getMap
                        |> Map.toSeq
                        |> Seq.map(fun (k,v) -> (k |> WorkspaceParamsKey.value ,v))
                        |> Map.ofSeq
        }

    let toJson (workspaceParams: workspaceParams) =
        workspaceParams |> toDto |> Json.serialize