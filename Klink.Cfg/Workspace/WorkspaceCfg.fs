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

        _makeWorkspaceId curId future


    let makeWorkspaceCfg (history:ICauseCfg list) =
        { 
          id = makeWorkspaceId (Workspace.empty |> Workspace.getId) history;
          history = history
        }


    let Empty = makeWorkspaceCfg []


    let addCauseCfg (ccfg:ICauseCfg) (wscfg:workspaceCfg) =
        makeWorkspaceCfg ([ccfg] |> List.append wscfg.history)

    let addCauseCfgs (ccfgs:ICauseCfg list) (wscfg:workspaceCfg) =
        makeWorkspaceCfg (ccfgs |> List.append wscfg.history)

    let removeLastCauseCfg (wscfg:workspaceCfg) =
        match wscfg.history with
        | [] ->   (makeWorkspaceCfg [], [])
        | h::t -> (
                   makeWorkspaceCfg 
                            (wscfg.history |> List.removeAt (wscfg.history.Length - 1)),
                   [wscfg.history.[wscfg.history.Length - 1]])


    let makeWorkspace (causeHist:ICauseCfg list) (startingWs:workspace) 
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

        _makeWorkspace (startingWs |> Ok) causeHist


    let getLastSavedWorspaceIdAndFuture (workspaceCfg:workspaceCfg) (fileStore:IWorkspaceStore)
        =
        let rec _lastWorkspaceCfg
                (wksCfgCurrent:workspaceCfg) 
                (future:ICauseCfg list)
            =
            let chkLatest = fileStore.WorkSpaceExists(wksCfgCurrent.id)
            match chkLatest with
            | Error m -> $"Error in getLastSavedWorspaceIdAndFuture (1) {m}" |> Error
            | Ok wasFound ->
                if wasFound then
                    (wksCfgCurrent.id, future) |> Ok
                elif wksCfgCurrent.history.Length = 0 then
                    (Empty.id, []) |> Ok
                else
                    let lastWksCfg, lastCause = removeLastCauseCfg wksCfgCurrent
                    _lastWorkspaceCfg lastWksCfg (lastCause@future)

        _lastWorkspaceCfg workspaceCfg []


    let updateWorkspace (workspaceCfg:workspaceCfg) (fileStore:IWorkspaceStore)
        =
        result {
            let! latestSavedId, future = getLastSavedWorspaceIdAndFuture workspaceCfg fileStore
            let! restoredWs = 
                if (latestSavedId = Empty.id) then
                    Workspace.empty |> Ok
                else
                    fileStore.LoadWorkSpace(latestSavedId)
            return! restoredWs |> makeWorkspace future
        }