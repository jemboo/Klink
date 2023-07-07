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
             wsnRndGenName:wsComponentName,
             prunedCount:sorterCount,
             noiseFraction:float option,
             stageWeight:stageWeight) 
    = 
    member this.name = wsnName
    member this.prunedCount = prunedCount
    member this.noiseFraction = noiseFraction
    member this.stageWeight = stageWeight
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let! rngGen = w |> Workspace.getComponent wsnRndGenName
                                |> Result.map(WorkspaceComponent.asSorterSet)
                let ssph = SorterSetPrunerWhole.make 
                                this.prunedCount
                                this.noiseFraction
                                this.stageWeight
                            |> sorterSetPruner.Whole
                            |> workspaceComponent.SorterSetPruner
                return w |> Workspace.addComponents newWorkspaceId [(this.name, ssph)]
            }
    member this.id =
        [
            this.name |> WsComponentName.value :> obj
            this.prunedCount |> SorterCount.value :> obj
            this.noiseFraction :> obj
            this.stageWeight |> StageWeight.value :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater


        
type causeCfgAddSorterSetPruneShc
            (wsnName:wsComponentName,
             prunedCount:sorterCount,
             noiseFraction:float option,
             stageWeight:stageWeight
             )
    = 
    member this.name = wsnName
    member this.prunedCount = prunedCount
    member this.noiseFraction = noiseFraction
    member this.stageWeight = stageWeight
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let ssph = SorterSetPrunerShc.make 
                                this.prunedCount
                                this.noiseFraction
                                this.stageWeight
                            |> sorterSetPruner.Shc
                            |> workspaceComponent.SorterSetPruner
                return w |> Workspace.addComponents newWorkspaceId [(this.name, ssph)]
            }
    member this.id =
        [
            this.name |> WsComponentName.value :> obj
            this.prunedCount |> SorterCount.value :> obj
            this.noiseFraction :> obj
            this.stageWeight |> StageWeight.value :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater


