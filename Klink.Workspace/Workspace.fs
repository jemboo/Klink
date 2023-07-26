namespace global

open System

type workspace = private {
        id:workspaceId;
        parentId:workspaceId option;
        grandParentId:workspaceId option;
        lastCauseName:string;
        wsComponents: Map<wsComponentName, workspaceComponent>
    }

module Workspace = 

    let getId (ws:workspace) = ws.id

    let getParentId (ws:workspace) = ws.parentId

    let getWsComponents (ws:workspace) = ws.wsComponents

    let empty = 
        {
            id = [CauseId.empty |> CauseId.value :> obj]            
                    |> GuidUtils.guidFromObjs 
                    |> WorkspaceId.create
            parentId = None
            grandParentId = None
            lastCauseName = "None"
            wsComponents = Map.empty
        }

    let load 
            (id:workspaceId) 
            (parentId:workspaceId option) 
            (grandParentId:workspaceId option)
            (lastCauseName:string)
            (tupes:seq<wsComponentName*workspaceComponent>)
        =
        {
            workspace.id = id;
            parentId = parentId;
            grandParentId = grandParentId;
            lastCauseName = lastCauseName
            wsComponents = tupes |> Map.ofSeq
        }

    let addComponents
            (newWorkspaceId:workspaceId)
            (lastCauseName:string)
            (tupes:seq<wsComponentName*workspaceComponent>)
            (workspace:workspace)  =
        let newMap =
            tupes |> Seq.fold (fun a t -> a |> Map.add (fst t) (snd t)) 
                              (workspace |> getWsComponents)
        {
            id = newWorkspaceId
            parentId = Some workspace.id
            grandParentId = workspace.parentId
            lastCauseName = lastCauseName
            wsComponents = newMap
        }


    let getComponent
            (compName:wsComponentName)
            (workspace:workspace)
        =
        if (workspace.wsComponents.ContainsKey(compName) |> not) then
            $"{compName |> WsComponentName.value} not present (10)" 
            |> Error
        else
           workspace.wsComponents.[compName] |> Ok


    let ofWorkspaceDescription 
            (lookup:Guid -> workspaceComponentType -> Result<workspaceComponent, string>) 
            (wsd: workspaceDescription) 
        =
        result {
             let names, wsCompDescrs = 
                       wsd |> WorkspaceDescription.getComponents
                           |> Map.toArray
                           |> Array.unzip
             let! components =
                    wsCompDescrs
                       |> Array.map(
                            fun wcd -> lookup 
                                        (wcd |> WorkspaceComponentDescr.getId) 
                                        (wcd |> WorkspaceComponentDescr.getCompType))
                       |> Array.toList
                       |> Result.sequence

            return
                load
                    (wsd |> WorkspaceDescription.getId)
                    (wsd |> WorkspaceDescription.getParentId)
                    (wsd |> WorkspaceDescription.getGrandParentId)
                    (wsd |> WorkspaceDescription.getLastCauseName)
                    (components |> List.toArray |> Array.zip names)
            }



    let toWorkspaceDescription (workspace:workspace)
        =
        let yab = workspace 
                    |> getWsComponents
                    |> Map.toSeq
                    |> Seq.map(fun (compN, comp) -> (compN, comp |> WorkspaceComponent.getWorkspaceComponentDescr))
                    |> Map.ofSeq
        WorkspaceDescription.create
            (workspace.id)
            (workspace.parentId)
            (workspace.grandParentId)
            (workspace.lastCauseName)
            yab