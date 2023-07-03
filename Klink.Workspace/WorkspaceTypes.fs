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
    | WorkspaceDto = 0
    | SortableSet = 1
    | SorterSet = 2
    | SorterSetMutator = 3
    | SorterSetParentMap = 4
    | SorterSetConcatMap = 5
    | SorterSetEval = 6
    | SorterSetPruner = 7


type IWorkspaceComponentCfg = 
    abstract member Id:Guid
    abstract member WsComponentName:wsComponentName
    abstract member WorkspaceComponentType:workspaceComponentType


type workspaceComponentDescr = 
        private 
            { compId:Guid; 
              compName:wsComponentName; 
              compType:workspaceComponentType }