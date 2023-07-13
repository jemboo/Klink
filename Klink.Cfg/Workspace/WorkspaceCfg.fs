namespace global
open System

type workspaceCfg =
    { id:workspaceId;
      history:ICause list }
and ICause =
    abstract member Id:causeId
    abstract member Name:string
    abstract member Updater:(workspace->workspaceId->Result<workspace, string>)


module WorkspaceCfg = 

    let makeWorkspaceId 
            (curId:workspaceId) 
            (future:ICause list) 
        =
        let rec _makeWorkspaceId 
                (curId:workspaceId) 
                (future:ICause list) 
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


    let makeWorkspaceCfg (history:ICause list) =
        { 
          id = makeWorkspaceId (Workspace.empty |> Workspace.getId) history;
          history = history
        }


    let Empty = makeWorkspaceCfg []


    let addCauseCfg (ccfg:ICause) (wscfg:workspaceCfg) =
        makeWorkspaceCfg ([ccfg] |> List.append wscfg.history)

    let addCauseCfgs (ccfgs:ICause list) (wscfg:workspaceCfg) =
        makeWorkspaceCfg (ccfgs |> List.append wscfg.history)

    let removeLastCauseCfg (wscfg:workspaceCfg) =
        match wscfg.history with
        | [] ->   (makeWorkspaceCfg [], [])
        | h::t -> (
                   makeWorkspaceCfg 
                            (wscfg.history |> List.removeAt (wscfg.history.Length - 1)),
                   [wscfg.history.[wscfg.history.Length - 1]])


    let makeWorkspace 
            (causeHist:ICause list) 
            (logger: string->unit)
            (startingWs:workspace)
        =
        let rec _makeWorkspace 
                (wksR:Result<workspace,string>) 
                (history:ICause list)
            =
            match history with 
            | [] -> wksR
            | h::t ->
                match wksR with
                | Error _ -> wksR
                | Ok wsCur ->
                let nextId = makeWorkspaceId (wsCur |> Workspace.getId) [h]
                let nextWs = h.Updater wsCur nextId
                logger $"updatingQ: {h.Name}"
                _makeWorkspace nextWs t

        _makeWorkspace (startingWs |> Ok) causeHist


    let getLastSavedWorspaceIdAndFuture 
                (fileStore:IWorkspaceStore)
                (logger: string->unit)
                (workspaceCfg:workspaceCfg) 
        =
        let rec _lastWorkspaceCfg
                (wksCfgCurrent:workspaceCfg) 
                (future:ICause list)
            =
            let chkLatest = fileStore.WorkSpaceExists(wksCfgCurrent.id)
            match chkLatest with
            | Error m -> $"Error in getLastSavedWorspaceIdAndFuture (1) {m}" |> Error
            | Ok wasFound ->
                if wasFound then
                    logger $"found {wksCfgCurrent.id |> WorkspaceId.value }"
                    (wksCfgCurrent.id, future) |> Ok
                elif wksCfgCurrent.history.Length = 0 then
                    logger $"no prior workspace found"
                    (Empty.id, future) |> Ok
                else
                    let lastWksCfg, lastCause = removeLastCauseCfg wksCfgCurrent
                    _lastWorkspaceCfg lastWksCfg (lastCause@future)

        _lastWorkspaceCfg workspaceCfg []


    let updateWorkspace 
            (fileStore:IWorkspaceStore)
            (logger: string->unit)
            (workspaceCfg:workspaceCfg)
        =
        result {
            let! latestSavedId, future = 
                    workspaceCfg |>
                        getLastSavedWorspaceIdAndFuture fileStore logger

            let! restoredWs = 
                if (latestSavedId = Empty.id) then
                    Workspace.empty |> Ok
                else
                    fileStore.LoadWorkSpace(latestSavedId)
            return! restoredWs |> makeWorkspace future logger
        }