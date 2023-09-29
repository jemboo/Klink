namespace global


type causeAddSorterSpeedBinSet(
             wsnSorterSpeedBinSet:wsComponentName,
             workspaceParams:workspaceParams)
    = 
    member this.causeName = "causeAddSorterSpeedBinSet"
    member this.wsnSorterSpeedBinSet = wsnSorterSpeedBinSet
    member this.updater = 
            fun (w :workspace) (newWorkspaceId :workspaceId) ->
            result {
                let wsSorterSpeedBinSet = 
                      SorterSpeedBinSet.create 
                            Map.empty
                            (0 |> Generation.create)
                            (workspaceParams |> WorkspaceParams.getId |> WorkspaceParamsId.value)
                      |> workspaceComponent.SorterSpeedBinSet
                return w |> Workspace.addComponents 
                                newWorkspaceId
                                this.causeName
                                [
                                    (this.wsnSorterSpeedBinSet, wsSorterSpeedBinSet);
                                ]
            }
    member this.id =
        [
            this.wsnSorterSpeedBinSet :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = false




type causeUpdateSorterSpeedBinSet
            (
             wsnSorterSpeedBinSet:wsComponentName,
             wsnSorterSetEval:wsComponentName,
             order:order,
             sorterSpeedBinType:sorterSpeedBinType,
             generation:generation
            )
    = 
    member this.causeName = "causeUpdateSorterSpeedBinSet"
    member this.wsnSorterSpeedBinSet = wsnSorterSpeedBinSet
    member this.wsnSorterSetEval = wsnSorterSetEval
    member this.sorterSpeedBinType = sorterSpeedBinType
    member this.updater =
            fun (w :workspace) (newWorkspaceId :workspaceId) ->
            result {

                let! wcSorterSpeedBinSet 
                    = w |> Workspace.getComponent this.wsnSorterSpeedBinSet
                        |> Result.bind(WorkspaceComponent.asSorterSpeedBinSet)

                let! sorterSetEval 
                    = w |> Workspace.getComponent this.wsnSorterSetEval
                        |> Result.bind(WorkspaceComponent.asSorterSetEval)


                let ssBins =
                        sorterSetEval 
                        |> SorterSetEval.getSorterEvalsArray 
                        |> Array.map(SorterSpeedBin.fromSorterEval order sorterSpeedBinType)


                let wsSpeedBinsUpdated =
                        ssBins 
                        |> SorterSpeedBinSet.addBins wcSorterSpeedBinSet generation
                        |> workspaceComponent.SorterSpeedBinSet

                return w |> Workspace.addComponents 
                            newWorkspaceId
                            this.causeName
                            [
                                (this.wsnSorterSpeedBinSet, wsSpeedBinsUpdated)
                            ]
            }

    member this.id =
        [
            this.wsnSorterSpeedBinSet :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = false




type causeAddSorterSetAncestry
            (
             wsnSorterSetAncestry:wsComponentName,
             workspaceParams:workspaceParams,
             wsnSorterSetEvalParentName:wsComponentName)
    = 
    member this.causeName = "causeAddSorterSetAncestry"
    member this.wsnSorterSetEvalParentName = wsnSorterSetEvalParentName
    member this.wsnSorterSetAncestry = wsnSorterSetAncestry
    member this.updater = 
            fun (w :workspace) (newWorkspaceId :workspaceId) ->
            result {
                let! sorterSetEvalParent = 
                        w |> Workspace.getComponent this.wsnSorterSetEvalParentName
                          |> Result.bind(WorkspaceComponent.asSorterSetEval)
                let! stageWeight = 
                    workspaceParams 
                        |> WorkspaceParamsAttrs.getStageWeight ShcWsParamKeys.stageWeight
                let wsSorterSetAncestry = 
                      SorterSetAncestryP.create 
                            sorterSetEvalParent
                            stageWeight
                            (0 |> Generation.create)
                            (workspaceParams |> WorkspaceParams.getId |> WorkspaceParamsId.value)
                      |> workspaceComponent.SorterSetAncestry
                return w |> Workspace.addComponents 
                                newWorkspaceId
                                this.causeName
                                [
                                    (this.wsnSorterSetAncestry, wsSorterSetAncestry);
                                ]
            }
    member this.id =
        [
            this.wsnSorterSetAncestry :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = false




type causeUpdateSorterSetAncestry
            (
             wsnSorterSpeedBinSet:wsComponentName,
             wsnSorterSetEval:wsComponentName,
             order:order,
             sorterSpeedBinType:sorterSpeedBinType,
             generation:generation
            )
    = 
    member this.causeName = "causeUpdateSorterSpeedBinSet"
    member this.wsnSorterSpeedBinSet = wsnSorterSpeedBinSet
    member this.wsnSorterSetEval = wsnSorterSetEval
    member this.sorterSpeedBinType = sorterSpeedBinType
    member this.updater =
            fun (w :workspace) (newWorkspaceId :workspaceId) ->
            result {

                let! wcSorterSpeedBinSet 
                    = w |> Workspace.getComponent this.wsnSorterSpeedBinSet
                        |> Result.bind(WorkspaceComponent.asSorterSpeedBinSet)

                let! sorterSetEval 
                    = w |> Workspace.getComponent this.wsnSorterSetEval
                        |> Result.bind(WorkspaceComponent.asSorterSetEval)


                let ssBins =
                        sorterSetEval 
                        |> SorterSetEval.getSorterEvalsArray 
                        |> Array.map(SorterSpeedBin.fromSorterEval order sorterSpeedBinType)


                let wsSpeedBinsUpdated =
                        ssBins 
                        |> SorterSpeedBinSet.addBins wcSorterSpeedBinSet generation
                        |> workspaceComponent.SorterSpeedBinSet

                return w |> Workspace.addComponents 
                            newWorkspaceId
                            this.causeName
                            [
                                (this.wsnSorterSpeedBinSet, wsSpeedBinsUpdated)
                            ]
            }

    member this.id =
        [
            this.wsnSorterSpeedBinSet :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = false