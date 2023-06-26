namespace global

open System

type workspaceId = private WorkspaceId of Guid
module WorkspaceId =
    let value (WorkspaceId v) = v
    let create (v: Guid) = WorkspaceId v

type causeId = private CauseId of Guid
module CauseId =
    let value (CauseId v) = v
    let create (v: Guid) = CauseId v
    let empty = 
        [("causeId" :> obj)]
            |> GuidUtils.guidFromObjs 
            |> create


type wsComponentName = private WsComponentName of string
module WsComponentName =
    let value (WsComponentName v) = v
    let create (v: string) = WsComponentName v


type workspaceComponentType =
    | Workspace = 1
    | SortableSet = 2
    | SorterSet = 3
    | SorterSetMutator = 4
    | SorterSetParentMap = 5
    | SorterSetConcatMap = 6
    | SorterSetEval = 7


type IWorkspaceComponent = 
    abstract member Id:Guid
    abstract member WsComponentName:wsComponentName
    abstract member WorkspaceComponentType:workspaceComponentType


type workspaceComponentDescr = 
        private 
            { compId:Guid; 
              compName:wsComponentName; 
              compType:workspaceComponentType }