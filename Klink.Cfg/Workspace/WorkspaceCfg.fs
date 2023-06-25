namespace global
open System

type WorkspaceCfg =
    { id:workspaceId;
      history:ICauseCfg list }
and ICauseCfg =
    abstract member Id:causeId
    abstract member Updater:(workspace->Result<workspace, string>)


module WorkspaceCfg = 
    let makeWorkspaceId (history:ICauseCfg list) =
        history 
            |> List.map(fun ccf -> ccf.Id |> CauseId.value :> obj)
            |> List.append [CauseId.empty |> CauseId.value :> obj]
            |> GuidUtils.guidFromObjs 
            |> WorkspaceId.create

    let makeWorkspaceCfg (history:ICauseCfg list) =
        { 
          id = makeWorkspaceId history;
          history = history
        }

    let addCauseCfg (ccfg:ICauseCfg) (wscfg:WorkspaceCfg) =
        makeWorkspaceCfg (ccfg :: wscfg.history)

    let removeCauseCfg (wscfg:WorkspaceCfg) =
        makeWorkspaceCfg (wscfg.history |> List.tail)