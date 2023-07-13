namespace global

open System


type causeMutateSorterSet
            (wsnSorterSetParent:wsComponentName,
             wsnSorterSetMutated:wsComponentName,
             wsnSorterSetMutator:wsComponentName,
             wsnRandomProvider:wsComponentName,
             wsnSorterSetParentMap:wsComponentName,
             rngGen:rngGen) 
    = 
    member this.sorterSetParentName = wsnSorterSetParent
    member this.sorterSetMutatedName = wsnSorterSetMutated
    member this.sorterSetMutatorName = wsnSorterSetMutator
    member this.randomProviderName = wsnRandomProvider
    member this.sorterSetParentMapName = wsnSorterSetParentMap
    member this.rngGen = rngGen

    member this.updater = 
            fun (w: workspace) (newWorkspaceId: workspaceId) ->
            result {
                let! sorterSetMutator = 
                        w |> Workspace.getComponent this.sorterSetMutatorName
                          |> Result.bind(WorkspaceComponent.asSorterSetMutator)
                let! sorterSetParent = 
                        w |> Workspace.getComponent this.sorterSetParentName
                          |> Result.bind(WorkspaceComponent.asSorterSet)

                let rngGenProvider = 
                        RngGenProvider.make this.rngGen
                let _rngGen = rngGenProvider |> RngGenProvider.nextRngGen

                let parentSorterSetId = sorterSetParent |> SorterSet.getId
                let parentSorterSetCount = sorterSetParent |> SorterSet.getSorterCount
                let mutantSorterSetCount = 
                        sorterSetMutator 
                        |> SorterSetMutator.getSorterCountFinal
                        |> Option.defaultValue parentSorterSetCount

                let mutantSorterSetId = 
                        parentSorterSetId 
                        |> SorterSetMutator.getMutantSorterSetId 
                                    sorterSetMutator 
                                    _rngGen

                let parentMap = SorterSetParentMap.create 
                                    mutantSorterSetId
                                    parentSorterSetId
                                    mutantSorterSetCount
                                    parentSorterSetCount
                              
                let! mutantSorterSet = SorterSetMutator.createMutantSorterSetFromParentMap
                                        parentMap
                                        sorterSetMutator
                                        _rngGen
                                        sorterSetParent
                                        |> Result.map(workspaceComponent.SorterSet)

                return w |> Workspace.addComponents newWorkspaceId 
                            [
                                (this.sorterSetParentMapName, parentMap |> workspaceComponent.SorterSetParentMap);
                                (this.sorterSetMutatedName, mutantSorterSet);
                                (this.randomProviderName, rngGenProvider |> workspaceComponent.RandomProvider);
                            ]
            }
    member this.id =   
        [
            "causeMutateSorterSet" :> obj
            this.sorterSetParentName |> WsComponentName.value  :> obj
            this.sorterSetMutatedName |> WsComponentName.value  :> obj
            this.sorterSetMutatorName |> WsComponentName.value  :> obj
            this.randomProviderName |> WsComponentName.value  :> obj
            this.sorterSetParentMapName |> WsComponentName.value  :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Name = $"causeMutateSorterSet { wsnSorterSetMutated |> WsComponentName.value }"
        member this.Updater = this.updater



type causeMakeSorterSetEval
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
                return w |> Workspace.addComponents newWorkspaceId [(this.sorterSetEvalName, wsSorterSetEval)]
            }
    member this.id =
        [
            "causeMakeSorterSetEval" :> obj
            this.sortableSetName |> WsComponentName.value  :> obj
            this.sorterSetName |> WsComponentName.value  :> obj
            this.sorterEvalMode  :> obj
            this.sorterSetEvalName |> WsComponentName.value  :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Name = $"causeMakeSorterSetEval { this.sorterSetEvalName |> WsComponentName.value }"
        member this.Updater = this.updater



type causePruneSorterSetsWhole
            (
             wsnRandomProvider:wsComponentName,
             wsnSorterSetParentName:wsComponentName,
             wsnSorterSetChildName:wsComponentName,
             wsnSorterSetEvalParentName:wsComponentName,
             wsnSorterSetEvalChildName:wsComponentName,
             wsnSorterSetPrunerName:wsComponentName,
             wsnSorterSetPrunedName:wsComponentName,
             wsnSorterSetEvalPrunedName:wsComponentName,
             rngGen:rngGen,
             prunedCount:sorterCount,
             noiseFraction: float option,
             stageWeight:stageWeight) 
    = 
    member this.randomProviderName = wsnRandomProvider
    member this.sorterSetParentName = wsnSorterSetParentName
    member this.sorterSetChildName = wsnSorterSetChildName
    member this.sorterSetEvalParentName = wsnSorterSetEvalParentName
    member this.sorterSetEvalChildName = wsnSorterSetEvalChildName
    member this.sorterSetPrunerName = wsnSorterSetPrunerName
    member this.sorterSetPrunedName = wsnSorterSetPrunedName
    member this.sorterSetEvalPrunedName = wsnSorterSetEvalPrunedName
    member this.rngGen = rngGen
    member this.prunedCount = prunedCount
    member this.noiseFraction = noiseFraction
    member this.stageWeight = stageWeight

    member this.updater =

        fun (w: workspace) (newWorkspaceId: workspaceId) ->

            result {

                let! sorterSetParent = 
                        w |> Workspace.getComponent this.sorterSetParentName
                          |> Result.bind(WorkspaceComponent.asSorterSet)
                let! sorterSetChild = 
                        w |> Workspace.getComponent this.sorterSetChildName
                          |> Result.bind(WorkspaceComponent.asSorterSet)
                let! sorterSetEvalParent = 
                        w |> Workspace.getComponent this.sorterSetEvalParentName
                          |> Result.bind(WorkspaceComponent.asSorterSetEval)
                let! sorterSetEvalChild = 
                        w |> Workspace.getComponent this.sorterSetEvalChildName
                          |> Result.bind(WorkspaceComponent.asSorterSetEval)

                let rngGenProvider = 
                        RngGenProvider.make this.rngGen
                let _rngGen = rngGenProvider |> RngGenProvider.nextRngGen

                let sorterSetEvalsAll = 
                        (sorterSetEvalParent |> SorterSetEval.getSorterEvals)
                                |> Array.append
                                    (sorterSetEvalChild |> SorterSetEval.getSorterEvals)

                let sorterSetPruner = 
                    SorterSetPruner.make 
                                this.prunedCount 
                                this.noiseFraction 
                                this.stageWeight

                let sorterEvalsPruned = 
                        sorterSetEvalsAll 
                        |> SorterSetPruner.runWholePrune 
                                sorterSetPruner 
                                _rngGen
                        |> Array.map(fun ((s,_), _) -> s)

                let mergedSorterMap = 
                        (sorterSetParent |> SorterSet.getSorters)
                            |> Array.append
                                (sorterSetChild |> SorterSet.getSorters)
                        |> Array.map(fun s -> ((s |> Sorter.getSorterId), s))
                        |> Map.ofArray

                let prunedSorters =
                        sorterEvalsPruned 
                        |> Array.map(fun sev -> sev |> SorterEval.getSorterId)
                        |> Array.map(fun sid -> mergedSorterMap.[sid])

                let prunedSorterSetId = 
                    SorterSetPruner.makePrunedSorterSetId
                        (sorterSetPruner |> SorterSetPruner.getId)
                        (sorterSetParent |> SorterSet.getId)
                        (sorterSetChild |> SorterSet.getId)
                        _rngGen

                let prunedSorterSet = 
                     SorterSet.load 
                            prunedSorterSetId
                            (sorterSetParent |> SorterSet.getOrder)
                            prunedSorters


                let prunedSorterSetEvalId = 
                    SorterSetPruner.makePrunedSorterSetEvalId
                        (sorterSetPruner |> SorterSetPruner.getId)
                        (sorterSetEvalParent |> SorterSetEval.getSorterSetEvalId)
                        (sorterSetEvalChild |> SorterSetEval.getSorterSetEvalId)
                        _rngGen


                let sorterSetEvalsPruned = 
                    SorterSetEval.load
                        prunedSorterSetEvalId
                        prunedSorterSetId
                        (sorterSetEvalChild |> SorterSetEval.getSortableSetId)
                        sorterEvalsPruned


                return
                    w |> Workspace.addComponents
                            newWorkspaceId 
                            [
                                (this.sorterSetPrunerName , sorterSetPruner |> workspaceComponent.SorterSetPruner)
                                (this.sorterSetPrunedName, prunedSorterSet |> workspaceComponent.SorterSet)
                                (this.sorterSetEvalPrunedName, sorterSetEvalsPruned |> workspaceComponent.SorterSetEval )
                                (this.randomProviderName, rngGenProvider |> workspaceComponent.RandomProvider);
                            ]
            }


    member this.id =
        [
            "causePruneSorterSetsWhole" :> obj
            this.sorterSetParentName |> WsComponentName.value  :> obj
            this.sorterSetChildName |> WsComponentName.value  :> obj
            this.sorterSetEvalParentName |> WsComponentName.value  :> obj
            this.sorterSetEvalChildName |> WsComponentName.value  :> obj
            this.sorterSetPrunerName |> WsComponentName.value  :> obj
            this.prunedCount |> SorterCount.value  :> obj
            this.noiseFraction :> obj
            this.stageWeight |> StageWeight.value  :> obj
            rngGen :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.Name = $"causePruneSorterSetsWhole"
        member this.Updater = this.updater




type causePruneSorterSetsNextGen
            (
             wnSorterSetParent:wsComponentName,
             wnSorterSetPruned:wsComponentName,
             wnSorterSetEvalParent:wsComponentName,
             wnSorterSetEvalPruned:wsComponentName) 
    = 
    member this.wnSorterSetParent = wnSorterSetParent
    member this.wnSorterSetPruned = wnSorterSetPruned
    member this.wnSorterSetEvalParent = wnSorterSetEvalParent
    member this.wnSorterSetEvalPruned = wnSorterSetEvalPruned

    member this.updater =

        fun (w: workspace) (newWorkspaceId: workspaceId) ->

            result {

                let! sorterSetNewParent = 
                        w |> Workspace.getComponent this.wnSorterSetPruned
                          |> Result.bind(WorkspaceComponent.asSorterSet)

                let! sorterSetEvalParentNew = 
                        w |> Workspace.getComponent this.wnSorterSetEvalPruned
                          |> Result.bind(WorkspaceComponent.asSorterSetEval)

                return Workspace.load 
                            newWorkspaceId
                            [
                                (this.wnSorterSetParent , sorterSetNewParent |> workspaceComponent.SorterSet)
                                (this.wnSorterSetEvalParent, sorterSetEvalParentNew |> workspaceComponent.SorterSetEval)
                            ]
            }

    member this.id =
        [
            "causePruneSorterSetsNextGen" :> obj
            this.wnSorterSetParent |> WsComponentName.value  :> obj
            this.wnSorterSetPruned |> WsComponentName.value  :> obj
            this.wnSorterSetEvalParent |> WsComponentName.value  :> obj
            this.wnSorterSetEvalPruned |> WsComponentName.value  :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create

    interface ICause with
        member this.Id = this.id
        member this.Name = $"causePruneSorterSetsNextGen"
        member this.Updater = this.updater
