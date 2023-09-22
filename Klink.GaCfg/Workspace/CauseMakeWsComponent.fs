namespace global

open System


type causeMutateSorterSet
            (wsnSorterSetParent:wsComponentName,
             wsnSorterSetMutated:wsComponentName,
             wsnSorterSetMutator:wsComponentName,
             wsnSorterSetParentMap:wsComponentName,
             rngGen:rngGen) 
    = 
    member this.causeName = "causeMutateSorterSet"
    member this.sorterSetParentName = wsnSorterSetParent
    member this.sorterSetMutatedName = wsnSorterSetMutated
    member this.sorterSetMutatorName = wsnSorterSetMutator
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
                                    ( sorterSetParent 
                                      |> SorterSet.getSorters 
                                      |> Array.map(Sorter.getSorterId) )

                              
                let! mutantSorterSet = SorterSetMutator.createMutantSorterSetFromParentMap
                                        parentMap
                                        sorterSetMutator
                                        _rngGen
                                        sorterSetParent
                                        |> Result.map(workspaceComponent.SorterSet)

                return w |> Workspace.addComponents
                            newWorkspaceId 
                            this.causeName
                            [
                                (this.sorterSetParentMapName, parentMap |> workspaceComponent.SorterSetParentMap);
                                (this.sorterSetMutatedName, mutantSorterSet);
                            ]
            }
    member this.id =   
        [
            this.causeName :> obj
            this.sorterSetParentName |> WsComponentName.value  :> obj
            this.sorterSetMutatedName |> WsComponentName.value  :> obj
            this.sorterSetMutatorName |> WsComponentName.value  :> obj
            this.rngGen :> obj
            this.sorterSetParentMapName |> WsComponentName.value  :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = true



type causeMakeSorterSetEval
        (
            wsnSortableSet:wsComponentName,
            wsnSorterSet:wsComponentName,
            sorterEvalMode:sorterEvalMode,
            wsnSorterSetEval:wsComponentName,
            useParallel:useParallel
        ) 
    =
    member this.causeName = "causeMakeSorterSetEval"
    member this.wsParamsName = WsConstants.workSpaceComponentNameForParams
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
                let! sorterSet   = w |> Workspace.getComponent this.sorterSetName
                                     |> Result.bind(WorkspaceComponent.asSorterSet)
                let! wsParams    = w |> Workspace.getComponent this.wsParamsName
                                     |> Result.bind(WorkspaceComponent.asWorkspaceParams)

                let order = sorterSet |> SorterSet.getOrder
                let! stagesSkipped = wsParams |> WorkspaceParamsAttrs.getStageCount GaWsParamKeys.stagesSkipped

                let! sorterSetEval = 
                    SorterSetEval.make
                        this.sorterEvalMode
                        sorterSet
                        sortableSet
                        (fun sev -> sev |> SorterEval.modifyForPrefix order stagesSkipped)
                        this.useParallel

                let wsSorterSetEval = 
                        sorterSetEval |> workspaceComponent.SorterSetEval


                return w |> 
                        Workspace.addComponents 
                            newWorkspaceId
                            this.causeName
                            [
                                (this.sorterSetEvalName, wsSorterSetEval)
                            ]
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
        member this.ResetId = None
        member this.Name = $"causeMakeSorterSetEval { this.sorterSetEvalName |> WsComponentName.value }"
        member this.Updater = this.updater
        member this.UseInWorkspaceId = true



type causePruneSorterSetsWhole
            (
             wsnSorterSetParentName:wsComponentName,
             wsnSorterSetChildName:wsComponentName,
             wsnSorterSetEvalParentName:wsComponentName,
             wsnSorterSetEvalChildName:wsComponentName,
             wsnSorterSetPrunedName:wsComponentName,
             wsnSorterSetEvalPrunedName:wsComponentName,
             rngGen:rngGen,
             prunedCount:sorterCount,
             noiseFraction: noiseFraction option,
             stageWeight:stageWeight) 
    = 
    member this.causeName = "causePruneSorterSetsWhole"
    member this.sorterSetParentName = wsnSorterSetParentName
    member this.sorterSetChildName = wsnSorterSetChildName
    member this.sorterSetEvalParentName = wsnSorterSetEvalParentName
    member this.sorterSetEvalChildName = wsnSorterSetEvalChildName
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
                        (this.stageWeight)
                        (this.noiseFraction)
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
                            this.causeName
                            [
                                (this.sorterSetPrunedName, prunedSorterSet |> workspaceComponent.SorterSet)
                                (this.sorterSetEvalPrunedName, sorterSetEvalsPruned |> workspaceComponent.SorterSetEval )
                            ]
            }


    member this.id =
        [
            this.causeName :> obj
            this.sorterSetParentName |> WsComponentName.value  :> obj
            this.sorterSetChildName |> WsComponentName.value  :> obj
            this.sorterSetEvalParentName |> WsComponentName.value  :> obj
            this.sorterSetEvalChildName |> WsComponentName.value  :> obj
            this.sorterSetPrunedName |> WsComponentName.value  :> obj
            this.prunedCount |> SorterCount.value  :> obj
            this.noiseFraction :> obj
            this.stageWeight |> StageWeight.value  :> obj
            rngGen :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = true



type causePruneSorterSetsShc
            (
             wsnSorterSetParentName:wsComponentName,
             wsnSorterSetChildName:wsComponentName,
             wsnSorterSetEvalParentName:wsComponentName,
             wsnSorterSetEvalChildName:wsComponentName,
             sorterSetPrunedName:wsComponentName,
             wsnSorterSetParentMapName:wsComponentName,
             wsnSorterSetEvalPrunedName:wsComponentName,
             rngGen:rngGen,
             prunedCount:sorterCount,
             noiseFraction: noiseFraction option,
             stageWeight:stageWeight) 
    = 
    member this.causeName = "causePruneSorterSetsShc"
    member this.sorterSetParentName = wsnSorterSetParentName
    member this.sorterSetChildName = wsnSorterSetChildName
    member this.sorterSetEvalParentName = wsnSorterSetEvalParentName
    member this.sorterSetEvalChildName = wsnSorterSetEvalChildName
    member this.sorterSetPrunedName = sorterSetPrunedName
    member this.sorterSetParentMapName = wsnSorterSetParentMapName
    member this.sorterSetEvalPrunedName = wsnSorterSetEvalPrunedName
    member this.rngGen = rngGen
    member this.prunedCount = prunedCount
    member this.noiseFraction = noiseFraction
    member this.stageWeight = stageWeight

    member this.updater =

        fun (w: workspace) (newWorkspaceId: workspaceId) ->

            result {

                let! _sorterSetParent = 
                        w |> Workspace.getComponent this.sorterSetParentName
                          |> Result.bind(WorkspaceComponent.asSorterSet)
                let! _sorterSetChild = 
                        w |> Workspace.getComponent this.sorterSetChildName
                          |> Result.bind(WorkspaceComponent.asSorterSet)
                let! _sorterSetEvalParent = 
                        w |> Workspace.getComponent this.sorterSetEvalParentName
                          |> Result.bind(WorkspaceComponent.asSorterSetEval)
                let! _sorterSetEvalChild = 
                        w |> Workspace.getComponent this.sorterSetEvalChildName
                          |> Result.bind(WorkspaceComponent.asSorterSetEval)
                let! _sorterSetParentMap = 
                        w |> Workspace.getComponent this.sorterSetParentMapName
                          |> Result.bind(WorkspaceComponent.asSorterSetParentMap)


                let _rngGen = RngGenProvider.make 
                                this.rngGen |> RngGenProvider.nextRngGen

                let sorterSetEvalsAll = 
                        (_sorterSetEvalParent |> SorterSetEval.getSorterEvals)
                                |> Array.append
                                    (_sorterSetEvalChild |> SorterSetEval.getSorterEvals)

                let sorterSetPruner = 
                    SorterSetPruner.make 
                                this.prunedCount 
                                this.noiseFraction 
                                this.stageWeight

                let sorterEvalsPruned = 
                        sorterSetEvalsAll 
                        |> SorterSetPruner.runShcPrune 
                                sorterSetPruner 
                                _rngGen
                                _sorterSetParentMap

                let mergedSorterMap = 
                        (_sorterSetParent |> SorterSet.getSorters)
                        |> Array.append (_sorterSetChild |> SorterSet.getSorters)
                        |> Array.map(fun s -> ((s |> Sorter.getSorterId), s))
                        |> Map.ofArray

                let prunedSorters =
                        sorterEvalsPruned 
                        |> Array.map(fun sev -> sev |> SorterEval.getSorterId)
                        |> Array.map(fun sid -> mergedSorterMap.[sid])

                let prunedSorterSetId = 
                    SorterSetPruner.makePrunedSorterSetId
                        (sorterSetPruner |> SorterSetPruner.getId)
                        (_sorterSetParent |> SorterSet.getId)
                        (_sorterSetChild |> SorterSet.getId)
                        (this.stageWeight)
                        (this.noiseFraction)
                        _rngGen

                let prunedSorterSet = 
                     SorterSet.load 
                        prunedSorterSetId
                        (_sorterSetParent |> SorterSet.getOrder)
                        prunedSorters


                let prunedSorterSetEvalId = 
                    SorterSetPruner.makePrunedSorterSetEvalId
                        (sorterSetPruner |> SorterSetPruner.getId)
                        (_sorterSetEvalParent |> SorterSetEval.getSorterSetEvalId)
                        (_sorterSetEvalChild |> SorterSetEval.getSorterSetEvalId)
                        _rngGen


                let sorterSetEvalsPruned = 
                    SorterSetEval.load
                        prunedSorterSetEvalId
                        prunedSorterSetId
                        (_sorterSetEvalChild |> SorterSetEval.getSortableSetId)
                        sorterEvalsPruned


                return
                    w |> Workspace.addComponents
                            newWorkspaceId
                            this.causeName
                            [
                                (this.sorterSetPrunedName, prunedSorterSet |> workspaceComponent.SorterSet)
                                (this.sorterSetEvalPrunedName, sorterSetEvalsPruned |> workspaceComponent.SorterSetEval )
                            ]
            }


    member this.id =
        [
            this.causeName :> obj
            this.sorterSetParentName |> WsComponentName.value  :> obj
            this.sorterSetChildName |> WsComponentName.value  :> obj
            this.sorterSetEvalParentName |> WsComponentName.value  :> obj
            this.sorterSetEvalChildName |> WsComponentName.value  :> obj
            this.sorterSetPrunedName |> WsComponentName.value  :> obj
            this.prunedCount |> SorterCount.value  :> obj
            this.noiseFraction :> obj
            this.stageWeight |> StageWeight.value  :> obj
            rngGen :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = true



type causeSetupForNextGen
            (
             wnSortableSet:wsComponentName,
             wnSorterSetParent:wsComponentName,
             wnSorterSetPruned:wsComponentName,
             wnSorterSetEvalParent:wsComponentName,
             wnSorterSetEvalPruned:wsComponentName,
             wnParentMap:wsComponentName,
             wnSorterSpeedBinSet:wsComponentName,
             workspaceParams:workspaceParams) 
    = 
    member this.causeName = "causeSetupForNextGen"
    member this.wnSortableSet = wnSortableSet
    member this.wnSorterSetParent = wnSorterSetParent
    member this.wnSorterSetPruned = wnSorterSetPruned
    member this.wnSorterSetEvalParent = wnSorterSetEvalParent
    member this.wnSorterSetEvalPruned = wnSorterSetEvalPruned
    member this.wnSorterSpeedBinSet = wnSorterSpeedBinSet
    member this.wsnParams = WsConstants.workSpaceComponentNameForParams
    member this.workspaceParams = workspaceParams
    member this.wnParentMap = wnParentMap

    member this.updater =

        fun (w: workspace) (newWorkspaceId: workspaceId) ->

            result {
                let! wcSortableSet = 
                        w |> Workspace.getComponent this.wnSortableSet

                let! wcSorterSetNewParent = 
                        w |> Workspace.getComponent this.wnSorterSetPruned

                let! wcSorterSetEvalParentNew = 
                        w |> Workspace.getComponent this.wnSorterSetEvalPruned

                let! wcSorterSpeedBinSet = 
                        w |> Workspace.getComponent this.wnSorterSpeedBinSet

                let! wcSorterSetParentMap = 
                        w |> Workspace.getComponent this.wnParentMap

                let wcParams = 
                     this.workspaceParams
                            |> workspaceComponent.WorkspaceParams

                return Workspace.load 
                            newWorkspaceId
                            (w |> Workspace.getId |> Some)
                            (w |> Workspace.getParentId)
                            this.causeName
                            [
                                (this.wnSortableSet, wcSortableSet)
                                (this.wnSorterSetParent, wcSorterSetNewParent)
                                (this.wnSorterSetEvalParent, wcSorterSetEvalParentNew)
                                (this.wnParentMap, wcSorterSetParentMap)
                                (this.wnSorterSpeedBinSet, wcSorterSpeedBinSet)
                                (this.wsnParams, wcParams)
                            ]
            }

    member this.id =
        [
            this.causeName :> obj
            this.wnSorterSetParent |> WsComponentName.value  :> obj
            this.wnSorterSetPruned |> WsComponentName.value  :> obj
            this.wnSorterSetEvalParent |> WsComponentName.value  :> obj
            this.wnSorterSetEvalPruned |> WsComponentName.value  :> obj
            this.workspaceParams :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create


    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = true

