namespace global

open System

type workspace =
    {
        id:workspaceId;
        items: Map<wsComponentName, IWorkspaceComponent>
    }

module Workspace = 
    let empty = 
        {
            id = [CauseId.empty |> CauseId.value :> obj]            
                    |> GuidUtils.guidFromObjs 
                    |> WorkspaceId.create
            items = Map.empty
        }

    let addComponent 
            (workspace:workspace)
            (newWorkspaceId:workspaceId) 
            (comp:IWorkspaceComponent)
        =
        if (workspace.items.ContainsKey(comp.WsComponentName)) then
            $"{comp.WsComponentName |> WsComponentName.value} already present" 
            |> Error
        else
        {
            id = newWorkspaceId
            items = workspace.items.Add (comp.WsComponentName, comp)
        } |> Ok


    let removeComponent 
            (workspace:workspace)
            (newWorkspaceId:workspaceId) 
            (comp:IWorkspaceComponent)
        =
        if (workspace.items.ContainsKey(comp.WsComponentName)) then
            $"{comp.WsComponentName |> WsComponentName.value} not present" 
            |> Error
        else
        {
            id = newWorkspaceId
            items = workspace.items.Remove comp.WsComponentName
        } |> Ok