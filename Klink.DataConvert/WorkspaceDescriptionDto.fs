namespace global
open System
open Microsoft.FSharp.Core

type workspaceDescriptionDto = 
        { 
            id:Guid; 
            parentId: Guid option;
            wsTypes: Map<string, int>
            wsIds: Map<string, Guid>
        }


module WorkspaceDescriptionDto =

    let fromDto (dto:workspaceDescriptionDto) 
        =
        let id = dto.id |> WorkspaceId.create
        let pId = dto.parentId |> Option.map(WorkspaceId.create)
        let comps = dto.wsIds 
                    |> Map.toSeq
                    |> Seq.map(fun (str, id) ->
                        (
                            str |>  WsComponentName.create,
                            (id, dto.wsTypes.[str]) |> WorkspaceComponentDescr.fromTuple
                        ) )  |> Map.ofSeq

        WorkspaceDescription.create id pId comps


    let fromJson (jstr: string) 
        =
        result {
            let! dto = Json.deserialize<workspaceDescriptionDto> jstr
            return dto |> fromDto
        }

    let toDto (ws: workspaceDescription) =

        { 
            workspaceDescriptionDto.id = ws |> WorkspaceDescription.getId |> WorkspaceId.value
            parentId = ws |> WorkspaceDescription.getParentId |> Option.map(WorkspaceId.value)
            wsTypes =
                ws |> WorkspaceDescription.getComponents
                   |> Map.toSeq
                   |> Seq.map(
                        fun (name, descr) -> 
                            (  name |> WsComponentName.value, 
                               descr |> WorkspaceComponentDescr.getCompType |> int)
                            )
                   |> Map.ofSeq
            wsIds =
                ws |> WorkspaceDescription.getComponents
                   |> Map.toSeq
                   |> Seq.map(
                        fun (name, descr) -> 
                            (  name |> WsComponentName.value, 
                               descr |> WorkspaceComponentDescr.getId)
                            )
                   |> Map.ofSeq
        }

    let toJson (ws: workspaceDescription) =
        ws |> toDto |> Json.serialize
