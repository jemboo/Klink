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
                return w |> Workspace.addComponents newWorkspaceId [(wsCompName, ssComp)]
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
                return w |> Workspace.addComponents newWorkspaceId [(wsCompName, ssComp)]
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
            ssmCfg:sorterSetMutatorCfg) 
    = 
    member this.name = wsCompName
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let ssComp = ssmCfg
                            |> SorterSetMutatorCfg.getSorterSetMutator 
                            |> workspaceComponent.SorterSetMutator
                return w |> Workspace.addComponents newWorkspaceId [(wsCompName, ssComp)]
            }
    member this.id =   
        [
            wsCompName :> obj
            ssmCfg :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater

        
type causeCfgAddSorterSetPruneWhole
            (wsnName:wsComponentName,
             selectionFraction:selectionFraction,
             temp:temp,
             stageWeight:stageWeight) 
    = 
    member this.name = wsnName
    member this.selectionFraction = selectionFraction
    member this.temp = temp
    member this.stageWeight = stageWeight
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let ssph = SorterSetPrunerWhole.make 
                                this.selectionFraction
                                this.temp
                                this.stageWeight
                            |> sorterSetPruner.Whole
                            |> workspaceComponent.SorterSetPruner
                return w |> Workspace.addComponents newWorkspaceId [(this.name, ssph)]
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


        
type causeCfgAddSorterSetPruneLhc
            (wsnName:wsComponentName,
             selectionFraction:selectionFraction,
             temp:temp,
             stageWeight:stageWeight)
    = 
    member this.name = wsnName
    member this.selectionFraction = selectionFraction
    member this.temp = temp
    member this.stageWeight = stageWeight
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let ssph = SorterSetPrunerShc.make 
                                this.selectionFraction
                                this.temp
                                this.stageWeight
                            |> sorterSetPruner.Lhc
                            |> workspaceComponent.SorterSetPruner
                return w |> Workspace.addComponents newWorkspaceId [(this.name, ssph)]
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


