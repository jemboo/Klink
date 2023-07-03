namespace global
open System

type workspaceCfg =
    { id:workspaceId;
      history:ICauseCfg list }
and ICauseCfg =
    abstract member Id:causeId
    abstract member Updater:(workspace->workspaceId->Result<workspace, string>)


module WorkspaceCfg = 

    let makeWorkspaceId 
            (curId:workspaceId) 
            (future:ICauseCfg list) 
        =
        let rec _makeWorkspaceId 
                (curId:workspaceId) 
                (future:ICauseCfg list) 
            =
            match future with
            | [] -> curId
            | h::t ->
                let nextId = 
                     [
                        curId |> WorkspaceId.value :> obj;
                        h.Id |> CauseId.value :> obj
                     ]
                     |> GuidUtils.guidFromObjs 
                     |> WorkspaceId.create
                _makeWorkspaceId nextId t

        _makeWorkspaceId curId (future |> List.rev)


    let makeWorkspaceCfg (history:ICauseCfg list) =
        { 
          id = makeWorkspaceId (Workspace.empty |> Workspace.getId) history;
          history = history
        }

    let Empty = makeWorkspaceCfg []

    let addCauseCfg (ccfg:ICauseCfg) (wscfg:workspaceCfg) =
        makeWorkspaceCfg (ccfg :: wscfg.history)

    let removeLastCauseCfg (wscfg:workspaceCfg) =
        makeWorkspaceCfg (wscfg.history |> List.tail)


    let makeWorkspace (workspaceCfg:workspaceCfg) 
        =
        let rec _makeWorkspace 
                (wksR:Result<workspace,string>) 
                (history:ICauseCfg list)
            =
            match history with 
            | [] -> wksR
            | h::t ->
                match wksR with
                | Error _ -> wksR
                | Ok wsCur ->
                let nextId = makeWorkspaceId (wsCur |> Workspace.getId) [h]
                let nextWs = h.Updater wsCur nextId
                _makeWorkspace nextWs t

        _makeWorkspace (Workspace.empty |> Ok) (workspaceCfg.history |> List.rev)