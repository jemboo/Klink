namespace global
open System

type wsComponentName = private WsComponentName of string
module WsComponentName =
    let value (WsComponentName v) = v
    let create (v: string) = WsComponentName v

type workspaceComponentType =
    | Workspace
    | SortableSet
    | SorterSet
    | SorterSetMutator
    | SorterSetParentMap
    | SorterSetConcatMap
    | SorterSetEval


type IWorkspaceComponent = 
    abstract member Id:Guid
    abstract member WsComponentName:wsComponentName
    abstract member WorkspaceComponentType:workspaceComponentType
