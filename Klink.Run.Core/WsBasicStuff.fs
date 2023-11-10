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


type runId = private RunId of Guid
module RunId =
    let value (RunId v) = v
    let create (v: Guid) = RunId v


type wsComponentName = private WsComponentName of string
module WsComponentName =
    let value (WsComponentName v) = v
    let create (v: string) = WsComponentName v


type workspaceComponentType =
    | WorkspaceDescription = 0
    | SortableSet = 10
    | SorterSet = 20
    | SorterSetAncestry = 21
    | SorterSetConcatMap = 22
    | SorterSetEval = 23
    | SorterSetMutator = 24
    | SorterSetParentMap = 25
    | SorterSpeedBinSet = 30
    | SorterSetPruner = 40
    | WorkspaceParams = 50



module WsConstants =

    let  workSpaceComponentNameForParams = "workspaceParams" |> WsComponentName.create

