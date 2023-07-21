namespace global

type causeAddWorkspaceParams 
            (workspaceParams:workspaceParams) 
    = 
    member this.wsnParams = WsConstants.workSpaceComponentNameForParams
    member this.workspaceParams = workspaceParams
    member this.updater = 
            fun (w:workspace) (newWorkspaceId:workspaceId) ->
            result {
                let wsParams = 
                     this.workspaceParams
                            |> workspaceComponent.WorkspaceParams
                return w |> Workspace.addComponents 
                                newWorkspaceId 
                                [(this.wsnParams, wsParams)]
            }
    member this.id =
        [
            this.wsnParams :> obj
            "causeAddWorkspaceParams" :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.wsnParams |> WsComponentName.value
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
        member this.ResetId = None
        member this.Name = wsnSortableSet |> WsComponentName.value
        member this.Updater = this.updater



type causeAddSorterSetRnd
            (wsnSorterSet:wsComponentName,
             ssCfg:sorterSetRndCfg,
             rngGen:rngGen) 
    = 
    member this.wsnSorterSet = wsnSorterSet
    member this.rngGen = rngGen
    member this.updater = 
            fun (w :workspace) (newWorkspaceId :workspaceId) ->
            result {

                let rngGenProvider = 
                        RngGenProvider.make this.rngGen
                let! wsCompSorterSet = 
                      ssCfg |> SorterSetRndCfg.makeSorterSet rngGenProvider
                            |> Result.map(workspaceComponent.SorterSet)
                return w |> Workspace.addComponents 
                                newWorkspaceId
                                [
                                    (this.wsnSorterSet, wsCompSorterSet);
                                ]
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
        member this.ResetId = None
        member this.Name = wsnSorterSet |> WsComponentName.value
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
        member this.ResetId = None
        member this.Name = wsnSorterSetMutator |> WsComponentName.value
        member this.Updater = this.updater

        
//type causeAddSorterSetPruneWhole
//            (wsnSorterSetPrune:wsComponentName,
//             wsnRndGenName:wsComponentName,
//             prunedCount:sorterCount,
//             noiseFraction:float option,
//             stageWeight:stageWeight) 
//    = 
//    member this.wsnSorterSetPrune = wsnSorterSetPrune
//    member this.prunedCount = prunedCount
//    member this.noiseFraction = noiseFraction
//    member this.stageWeight = stageWeight
//    member this.updater = 
//            fun (w: workspace) (newWorkspaceId: workspaceId) ->
//            result {
//                let ssph = SorterSetPrunerWhole.make 
//                                this.prunedCount
//                                this.noiseFraction
//                                this.stageWeight
//                            |> sorterSetPruner.Whole
//                            |> workspaceComponent.SorterSetPruner
//                return w |> Workspace.addComponents 
//                                newWorkspaceId 
//                                [(this.wsnSorterSetPrune, ssph)]
//            }
//    member this.id =
//        [
//            this.wsnSorterSetPrune |> WsComponentName.value :> obj
//            this.prunedCount |> SorterCount.value :> obj
//            this.noiseFraction :> obj
//            this.stageWeight |> StageWeight.value :> obj
//        ]
//             |> GuidUtils.guidFromObjs
//             |> CauseId.create
//    interface ICause with
//        member this.Id = this.id
//        member this.Name = wsnSorterSetPrune |> WsComponentName.value
//        member this.Updater = this.updater


        
//type causeAddSorterSetPruneShc
//            (wsnSorterSetShc:wsComponentName,
//             prunedCount:sorterCount,
//             noiseFraction:float option,
//             stageWeight:stageWeight
//             )
//    = 
//    member this.wsnSorterSetShc = wsnSorterSetShc
//    member this.prunedCount = prunedCount
//    member this.noiseFraction = noiseFraction
//    member this.stageWeight = stageWeight
//    member this.updater = 
//            fun (w: workspace) (newWorkspaceId: workspaceId) ->
//            result {
//                let ssph = SorterSetPrunerShc.make 
//                                this.prunedCount
//                                this.noiseFraction
//                                this.stageWeight
//                            |> sorterSetPruner.Shc
//                            |> workspaceComponent.SorterSetPruner
//                return w |> Workspace.addComponents 
//                                newWorkspaceId 
//                                [(this.wsnSorterSetShc, ssph)]
//            }
//    member this.id =
//        [
//            this.wsnSorterSetShc |> WsComponentName.value :> obj
//            this.prunedCount |> SorterCount.value :> obj
//            this.noiseFraction :> obj
//            this.stageWeight |> StageWeight.value :> obj
//        ]
//             |> GuidUtils.guidFromObjs
//             |> CauseId.create
//    interface ICause with
//        member this.Id = this.id
//        member this.Name = wsnSorterSetShc |> WsComponentName.value
//        member this.Updater = this.updater