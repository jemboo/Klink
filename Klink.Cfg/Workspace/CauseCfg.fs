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
                let! sorterSetMutator = 
                        w |> Workspace.getComponent this.sorterSetMutatorName
                          |> Result.bind(WorkspaceComponent.asSorterSetMutator)
                let! sorterSetParent = 
                        w |> Workspace.getComponent this.sorterSetParentName
                          |> Result.bind(WorkspaceComponent.asSorterSet)

                let parentSorterSetId = sorterSetParent |> SorterSet.getId
                let parentSorterSetCount = sorterSetParent |> SorterSet.getSorterCount
                let mutantSorterSetCount = 
                        sorterSetMutator 
                        |> SorterSetMutator.getSorterCountFinal
                        |> Option.defaultValue parentSorterSetCount
                let mutantSorterSetId = parentSorterSetId |> SorterSetMutator.getMutantSorterSetId sorterSetMutator
                let parentMap = SorterSetParentMap.create 
                                    mutantSorterSetId
                                    parentSorterSetId
                                    mutantSorterSetCount
                                    parentSorterSetCount
                              
                let! mutantSorterSet = SorterSetMutator.createMutantSorterSetFromParentMap
                                        parentMap
                                        sorterSetMutator
                                        sorterSetParent
                                        |> Result.map(workspaceComponent.SorterSet)

                return w |> Workspace.addComponents newWorkspaceId 
                            [
                                (this.sorterSetParentMapName, parentMap |> workspaceComponent.SorterSetParentMap);
                                (this.sorterSetMutatorName, mutantSorterSet);
                            ]
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


type causeCfgMakeSorterSetEval
            (wsnSortableSet:wsComponentName,
             wsnSorterSet:wsComponentName,
             sorterEvalMode:sorterEvalMode,
             wsnSorterSetEval:wsComponentName,
             useParallel:useParallel) 
    = 
    member this.sortableSetName = wsnSortableSet
    member this.sorterSetName = wsnSorterSet
    member this.sorterEvalMode = sorterEvalMode
    member this.sorterSetEvalName = wsnSorterSetEval
    member this.useParallel = useParallel
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let! sortableSet = w |> Workspace.getComponent this.sortableSetName
                                     |> Result.bind(WorkspaceComponent.asSortableSet)
                let! sorterSet = w |> Workspace.getComponent this.sorterSetName
                                   |> Result.bind(WorkspaceComponent.asSorterSet)

                let! wsSorterSetEval = SorterSetEval.make
                                        this.sorterEvalMode
                                        sorterSet
                                        sortableSet
                                        this.useParallel
                                     |> Result.map(workspaceComponent.SorterSetEval)
                return w |> Workspace.addComponents newWorkspaceId [(wsnSorterSetEval, wsSorterSetEval)]
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


module CauseCfg = 

    ()