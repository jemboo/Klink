namespace global

open System


type workspaceId = private WorkspaceId of Guid
module WorkspaceId =
    let value (WorkspaceId v) = v
    let create (v: Guid) = WorkspaceId v

type wsComponentName = private WsComponentName of string
module WsComponentName =
    let value (WsComponentName v) = v
    let create (v: string) = WsComponentName v

type causeId = private CauseId of Guid
module CauseId =
    let value (CauseId v) = v
    let create (v: Guid) = CauseId v

type workspaceComponentType =
    | SortableSet
    | SorterSet
    | SorterSetMutator
    | SorterSetParentMap
    | SorterSetConcatMap
    | SorterSetEval

type IWorkspaceComponent = 
    abstract member fileName:string
    abstract member workspaceComponentType:workspaceComponentType

type Workspace =
    {
        id:workspaceId;
        items: Map<wsComponentName, obj>
    }

type IWorkspaceCfg = 
    abstract member Id:workspaceId
    abstract member History:ICauseCfg list
and ICauseCfg = 
    abstract member Id:causeId
    abstract member Updater:Workspace->Workspace


module WsDD = 
    ()

