namespace global
open System

type history =
    { id:workspaceId;
      causes:ICause list }
and ICause =
    abstract member Id:causeId
    abstract member ResetId:workspaceId option
    abstract member Name:string
    abstract member Updater:(workspace->workspaceId->Result<workspace, string>)
    abstract member UseInWorkspaceId:bool


module History = 

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
                match h.ResetId with
                | Some id -> 
                    _makeWorkspaceId id t
                | None ->
                    let nextId =
                        if h.UseInWorkspaceId then
                             [
                                curId |> WorkspaceId.value :> obj;
                                h.Id |> CauseId.value :> obj
                             ]
                             |> GuidUtils.guidFromObjs 
                             |> WorkspaceId.create
                        else
                            curId
                    _makeWorkspaceId nextId t

        _makeWorkspaceId curId future


    let makeWorkspaceCfg (causes:ICause list) =
        { 
          id = makeWorkspaceId (Workspace.empty |> Workspace.getId) causes;
          causes = causes
        }


    let Empty = makeWorkspaceCfg []


    let addCause 
            (ccfg:ICause) 
            (wscfg:history) =
        makeWorkspaceCfg ([ccfg] |> List.append wscfg.causes)

    let addCauses (ccfgs:ICause list) (wscfg:history) =
        makeWorkspaceCfg (ccfgs |> List.append wscfg.causes)

    let removeLastCause (wscfg:history) =
        match wscfg.causes with
        | [] ->   (makeWorkspaceCfg [], [])
        | h::t -> (
                   makeWorkspaceCfg 
                            (wscfg.causes |> List.removeAt (wscfg.causes.Length - 1)),
                   [wscfg.causes.[wscfg.causes.Length - 1]])


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
                | Error m -> 
                    logger $"error in History.makeWorkspace: {m}"
                    wksR
                | Ok wsCur ->
                    let nextId = makeWorkspaceId (wsCur |> Workspace.getId) [h]
                    let nextWs = h.Updater wsCur nextId
                    //logger $"updatingQ: {h.Name}"
                    _makeWorkspace nextWs t

        _makeWorkspace (startingWs |> Ok) causeHist


    let getLastSavedWorspaceIdAndFuture 
                (fileStore:IWorkspaceStore)
                (logger: string->unit)
                (history:history) 
        =
        let rec _lastHistory
                (wksCfgCurrent:history) 
                (future:ICause list)
            =
            let chkLatest = fileStore.WorkSpaceExists(wksCfgCurrent.id)
            match chkLatest with
            | Error m -> $"Error in getLastSavedWorspaceIdAndFuture (1) {m}" |> Error
            | Ok wasFound ->
                if wasFound then
                    logger $"found {wksCfgCurrent.id |> WorkspaceId.value }"
                    (wksCfgCurrent.id, future) |> Ok
                elif wksCfgCurrent.causes.Length = 0 then
                    logger $"no prior workspace found"
                    (Empty.id, future) |> Ok
                else
                    let lastWksCfg, lastCause = removeLastCause wksCfgCurrent
                    _lastHistory lastWksCfg (lastCause@future)

        _lastHistory history []


    let runWorkspaceCfg 
            (fileStore:IWorkspaceStore)
            (logger: string->unit)
            (history:history)
        =
        result {
            let! latestSavedId, future = 
                    history |>
                        getLastSavedWorspaceIdAndFuture fileStore logger

            let! restoredAncestorWs = 
                if (latestSavedId = Empty.id) then
                    Workspace.empty |> Ok
                else
                    fileStore.LoadWorkSpace(latestSavedId)
            return! restoredAncestorWs |> makeWorkspace future logger
        }


    let runWorkspaceCfgOnWorkspace
            (logger: string->unit)
            (causes:ICause list)
            (ws:workspace)
        =
        result {

            return! ws |> makeWorkspace causes logger
        }


