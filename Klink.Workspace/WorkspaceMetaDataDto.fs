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


        
//type workspaceMetaDataDto = 
//        { 
//            id:Guid; 
//            parentId: Guid option; 
//            wsNames:string[]; 
//            workspaceComponentTypes:int[]; 
//            workspaceComponentIds:Guid[] 
//        }

//module WorkspaceMetaDataDto =

//    let loadWorkspaceFromDto 
//            (lookup:Guid -> workspaceComponentType -> Result<workspaceComponent, string>) 
//            (dto:workspaceMetaDataDto) =
//        result {
//            let compTypes = dto.workspaceComponentTypes |> Array.map(enum<workspaceComponentType>)
//            let tupeLst = compTypes |> Array.zip(dto.workspaceComponentIds) |> Array.toList
//            let! wsComps = tupeLst |> List.map(fun (gu, ct) -> lookup gu ct)
//                           |> Result.sequence
//            let compNames = dto.wsNames |> Array.map(WsComponentName.create)
//            return
//                Workspace.load
//                        (dto.id |> WorkspaceId.create)
//                        (dto.parentId |> Option.map(WorkspaceId.create))
//                        ((wsComps |> List.toArray) |> Array.zip compNames)
//            }

//    let loadWorkspaceMetaDataDtoFromJson 
//            (jstr: string) =
//        result {
//            return! Json.deserialize<workspaceMetaDataDto> jstr
//        }

//    let loadWorkspaceFromJson 
//            (lookup:Guid -> workspaceComponentType -> Result<workspaceComponent, string>) 
//            (jstr: string) =
//        result {
//            let! dto = Json.deserialize<workspaceMetaDataDto> jstr
//            return! loadWorkspaceFromDto lookup dto
//        }

//    let toDto (ws: workspace) =
//        let wsNames, comps = ws |> Workspace.getWsComponents |> Map.toArray |> Array.unzip
//        { 
//          workspaceMetaDataDto.id = ws |> Workspace.getId |> WorkspaceId.value
//          parentId = ws |> Workspace.getParentId |> Option.map(WorkspaceId.value)
//          wsNames = wsNames |> Array.map(WsComponentName.value)
//          workspaceComponentTypes = comps |> Array.map(WorkspaceComponent.getWorkspaceComponentType >> int)
//          workspaceComponentIds = comps |> Array.map(WorkspaceComponent.getId)
//        }

//    let toJson (ws: workspace) =
//        ws |> toDto |> Json.serialize


        
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