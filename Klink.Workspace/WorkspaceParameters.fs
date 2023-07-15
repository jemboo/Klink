namespace global
open System

type workspaceParamsId = private WorkspaceParametersId of Guid
module WorkspaceParamsId =
    let value (WorkspaceParametersId v) = v
    let create vl = WorkspaceParametersId vl

type workspaceParameters =
    private 
        { id: workspaceParamsId; 
          data: Map<string,string> }

module WorkspaceParameters =

    let load 
            (id:workspaceParamsId) 
            (data: Map<string,string>)
        =
        {
            id = id;
            data = data;
        }

    let make (data: Map<string,string>) =
        let nextId = 
            data |> Map.toArray |> Array.map(fun tup -> tup :> obj)
            |> GuidUtils.guidFromObjs 
            |> WorkspaceParamsId.create

        load nextId data

    let getId (jsonDataMap:workspaceParameters) =
        jsonDataMap.id

    let getData (jsonDataMap:workspaceParameters) =
        jsonDataMap.data

    let addItem (key:string) (cereal:string) (jsonDataMap:workspaceParameters) =
        let newMap = jsonDataMap.data |> Map.add key cereal
        make newMap

    let addItems 
            (kvps:(string*string) seq) 
            (jsonDataMap:workspaceParameters) =
        Seq.fold (fun wp tup -> addItem (fst tup) (snd tup) wp) jsonDataMap kvps

