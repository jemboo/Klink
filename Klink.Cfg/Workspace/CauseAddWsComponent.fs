﻿namespace global

open System

type causeAddRndGenProvider 
            (wsnRngGen:wsComponentName,
             rngGen:rngGen) 
    = 
    member this.wsnRngGen = wsnRngGen
    member this.rngGen = rngGen
    member this.updater = 
            fun (w:workspace) (newWorkspaceId:workspaceId) ->
            result {
                let _id =         
                    [|
                       rngGen :> obj;
                    |] 
                    |> GuidUtils.guidFromObjs

                let rngProviderId = _id |> RngGenProviderId.create
                let ssComp = 
                    (RngGenProvider.load rngProviderId this.rngGen)
                            |> workspaceComponent.RandomProvider
                return w |> Workspace.addComponents 
                                newWorkspaceId 
                                [(this.wsnRngGen, ssComp)]
            }
    member this.id =
        [
            this.wsnRngGen :> obj
            this.rngGen :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Updater = this.updater



type causeAddSortableSet 
            (wsnSortableSet:wsComponentName,
             ssCfg:sortableSetCfg) 
    = 
    member this.wsnSortableSet = wsnSortableSet
    member this.updater = 
            fun (w:workspace) (newWorkspaceId:workspaceId) ->
            result {
                let! ssComp = ssCfg
                            |> SortableSetCfg.makeSortableSet 
                            |> Result.map(workspaceComponent.SortableSet)
                return w |> Workspace.addComponents 
                                newWorkspaceId 
                                [(this.wsnSortableSet, ssComp)]
            }
    member this.id =
        [
            this.wsnSortableSet :> obj
            ssCfg :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Updater = this.updater



type causeAddSorterSet 
            (wsnSorterSet:wsComponentName,
             wsCompNameRando:wsComponentName,
             ssCfg:sorterSetCfg) 
    = 
    member this.wsnSorterSet = wsnSorterSet
    member this.wsCompNameRando = wsCompNameRando
    member this.updater = 
            fun (w :workspace) (newWorkspaceId :workspaceId) ->
            result {
                let! wsCompRando = 
                        w |> Workspace.getComponent this.wsCompNameRando
                          |> Result.bind(WorkspaceComponent.asRandomProvider)
                let! wsCompSorterSet = 
                      ssCfg |> SorterSetCfg.makeSorterSet wsCompRando
                            |> Result.map(workspaceComponent.SorterSet)
                return w |> Workspace.addComponents 
                                newWorkspaceId 
                                [(this.wsnSorterSet, wsCompSorterSet)]
            }
    member this.id =
        [
            this.wsnSorterSet :> obj
            ssCfg :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Updater = this.updater



type causeAddSorterSetMutator 
            (wsnSorterSetMutator:wsComponentName,
             order:order,
             switchGenMode:switchGenMode,
             sorterCountMutated:sorterCount,
             mutationRate:mutationRate) 
    = 
    member this.wsnSorterSetMutator = wsnSorterSetMutator
    member this.order = order
    member this.switchGenMode = switchGenMode
    member this.sorterCountMutated = sorterCountMutated
    member this.mutationRate = mutationRate

    member this._id =
                [|
                  "sorterSetMutatorCfg" :> obj;
                   order :> obj;
                   switchGenMode :> obj;
                   sorterCountMutated :> obj;
                |] 
                |> GuidUtils.guidFromObjs
                |> SorterSetMutatorId.create

    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {

              let _id =       
                [|
                  "sorterSetMutator" :> obj;
                   order :> obj;
                   switchGenMode :> obj;
                   sorterCountMutated :> obj;
                |] 
                |> GuidUtils.guidFromObjs
                |> SorterSetMutatorId.create


              let sorterUniformMutator = 
                    SorterUniformMutator.create
                            None
                            None
                            switchGenMode
                            mutationRate
                    |> sorterMutator.Uniform

              let ssComp =
                        SorterSetMutator.load
                            _id
                            sorterUniformMutator
                            (Some sorterCountMutated)
                            |> workspaceComponent.SorterSetMutator
              return w |> Workspace.addComponents 
                                newWorkspaceId 
                                [(this.wsnSorterSetMutator, ssComp)]
            }
    member this.id =   
        [
            this.wsnSorterSetMutator :> obj
            this._id |> SorterSetMutatorId.value :> obj
        ]
        |> GuidUtils.guidFromObjs
        |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Updater = this.updater

        
type causeAddSorterSetPruneWhole
            (wsnSorterSetPrune:wsComponentName,
             wsnRndGenName:wsComponentName,
             prunedCount:sorterCount,
             noiseFraction:float option,
             stageWeight:stageWeight) 
    = 
    member this.wsnSorterSetPrune = wsnSorterSetPrune
    member this.prunedCount = prunedCount
    member this.noiseFraction = noiseFraction
    member this.stageWeight = stageWeight
    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let ssph = SorterSetPrunerWhole.make 
                                this.prunedCount
                                this.noiseFraction
                                this.stageWeight
                            |> sorterSetPruner.Whole
                            |> workspaceComponent.SorterSetPruner
                return w |> Workspace.addComponents 
                                newWorkspaceId 
                                [(this.wsnSorterSetPrune, ssph)]
            }
    member this.id =
        [
            this.wsnSorterSetPrune |> WsComponentName.value :> obj
            this.prunedCount |> SorterCount.value :> obj
            this.noiseFraction :> obj
            this.stageWeight |> StageWeight.value :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Updater = this.updater


        
type causeAddSorterSetPruneShc
            (wsnSorterSetShc:wsComponentName,
             prunedCount:sorterCount,
             noiseFraction:float option,
             stageWeight:stageWeight
             )
    = 
    member this.wsnSorterSetShc = wsnSorterSetShc
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
                return w |> Workspace.addComponents 
                                newWorkspaceId 
                                [(this.wsnSorterSetShc, ssph)]
            }
    member this.id =
        [
            this.wsnSorterSetShc |> WsComponentName.value :> obj
            this.prunedCount |> SorterCount.value :> obj
            this.noiseFraction :> obj
            this.stageWeight |> StageWeight.value :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Updater = this.updater