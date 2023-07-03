namespace global

open System

type causeCfgAddSortableSet 
            (wsCompName:wsComponentName,
            ssCfg:sortableSetCfg) 
    = 
    member this.name = wsCompName
    member this.updater = 
            fun (w :workspace) (newWorkspaceId :workspaceId) ->
            result {
                let! ssComp = ssCfg
                            |> SortableSetCfg.makeSortableSet 
                            |> Result.map(workspaceComponent.SortableSet)
                return! w |> Workspace.addComponent newWorkspaceId wsCompName ssComp
            }
    member this.id =   
        [
            wsCompName :> obj
            ssCfg :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater


type causeCfgAddSorterSet 
            (wsCompName:wsComponentName,
            ssCfg:sorterSetCfg) 
    = 
    member this.name = wsCompName
    member this.updater = 
            fun (w :workspace) (newWorkspaceId :workspaceId) ->
            result {
                let! ssComp = ssCfg
                            |> SorterSetCfg.makeSorterSet 
                            |> Result.map(workspaceComponent.SorterSet)
                return! w |> Workspace.addComponent newWorkspaceId wsCompName ssComp
            }
    member this.id =   
        [
            wsCompName :> obj
            ssCfg :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater


type causeCfgAddSorterSetMutator 
            (wsCompName:wsComponentName,
            ssCfg:sorterSetMutatorCfg) 
    = 
    member this.name = wsCompName
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let ssComp = ssCfg
                            |> SorterSetMutatorCfg.getSorterSetMutator 
                            |> workspaceComponent.SorterSetMutator
                return! w |> Workspace.addComponent newWorkspaceId wsCompName ssComp
            }
    member this.id =   
        [
            wsCompName :> obj
            ssCfg :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater


type causeCfgMutateSorterSet
            (wsnSorterSetParent:wsComponentName,
             wsnSorterSetMutated:wsComponentName,
             wsnSorterSetMutator:wsComponentName,
             wsnSorterSetParentMap:wsComponentName) 
    = 
    member this.sorterSetParentName = wsnSorterSetParent
    member this.sorterSetMutatedName = wsnSorterSetMutated
    member this.sorterSetMutatorName = wsnSorterSetMutator
    member this.sorterSetParentMapName = wsnSorterSetParentMap

    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {

                return w 
            }
    member this.id =   
        [
            this.sorterSetParentName |> WsComponentName.value :> obj
            this.sorterSetMutatedName |> WsComponentName.value :> obj
            this.sorterSetMutatorName |> WsComponentName.value :> obj
            this.sorterSetParentMapName  |> WsComponentName.value :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater


type causeCfgCreateSorterSetEval
            (wsnSortableSet:wsComponentName,
             wsnSorterSet:wsComponentName,
             sorterEvalMode:sorterEvalMode,
             wsnSorterSetEval:wsComponentName,
             up:useParallel) 
    = 
    member this.sortableSetName = wsnSortableSet
    member this.sorterSetName = wsnSorterSet
    member this.sorterEvalMode = sorterEvalMode
    member this.sorterSetEvalName = wsnSorterSetEval
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let! sortableSet = w |> Workspace.getComponent this.sortableSetName
                let! sorterSet = w |> Workspace.getComponent this.sorterSetName

                return w
            }
    member this.id =
        [
            this.sortableSetName |> WsComponentName.value :> obj
            this.sorterSetName |> WsComponentName.value :> obj
            this.sorterEvalMode :> obj
            this.sorterSetEvalName |> WsComponentName.value :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater

        

type causeCfgAddSorterSetPruneWhole
            (wsnName:wsComponentName,
             selectionFraction:selectionFraction,
             stageWeight:stageWeight) 
    = 
    member this.name = wsnName
    member this.selectionFraction = selectionFraction
    member this.stageWeight = stageWeight
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let ssph = SorterSetPrunerWhole.make 
                                this.selectionFraction 
                                this.stageWeight
                            |> sorterSetPruner.Whole
                            |> workspaceComponent.SorterSetPruner
                return! w |> Workspace.addComponent newWorkspaceId this.name ssph
            }
    member this.id =
        [
            this.name |> WsComponentName.value :> obj
            this.selectionFraction |> SelectionFraction.value :> obj
            this.stageWeight |> StageWeight.value :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater


module CauseCfg = 

    ()