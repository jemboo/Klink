namespace global
open System

type workspaceParamsId = private WorkspaceParametersId of Guid
module WorkspaceParamsId =
    let value (WorkspaceParametersId v) = v
    let create vl = WorkspaceParametersId vl

type workspaceParams =
    private 
        { id: workspaceParamsId; 
          data: Map<string,string> }


module WorkspaceParams =

    let load 
            (id:workspaceParamsId) 
            (data: Map<string,string>)
        =
        {
            workspaceParams.id = id;
            data = data;
        }

    let make (data: Map<string,string>) =
        let nextId = 
            data |> Map.toArray |> Array.map(fun tup -> tup :> obj)
            |> GuidUtils.guidFromObjs 
            |> WorkspaceParamsId.create
        load nextId data

    let getHeaders (workspaceParams:workspaceParams) =
        workspaceParams.data.Keys |> Seq.fold (fun cur nv -> $"{cur}\t{nv}") ""

    let getValues (workspaceParams:workspaceParams) =
        workspaceParams.data.Values |> Seq.fold (fun cur nv -> $"{cur}\t{nv}") ""

    let getId (workspaceParams:workspaceParams) =
        workspaceParams.id

    let getMap (workspaceParams:workspaceParams) =
        workspaceParams.data

    let addItem (key:string) (cereal:string) (workspaceParams:workspaceParams) =
        let newMap = workspaceParams.data |> Map.add key cereal
        make newMap

    let addItems
            (kvps:(string*string) seq) 
            (jsonDataMap:workspaceParams) =
        Seq.fold (fun wp tup -> addItem (fst tup) (snd tup) wp) jsonDataMap kvps


    let getItem 
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        if workspaceParams.data.ContainsKey(key) then
           workspaceParams.data.[key] |> Ok
        else
            $"the key: {key} was not found (405)" |> Error