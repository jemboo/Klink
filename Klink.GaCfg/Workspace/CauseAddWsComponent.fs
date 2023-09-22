namespace global

type causeAddWorkspaceParams 
            (workspaceParams:workspaceParams) 
    = 
    member this.causeName = "causeAddWorkspaceParams"
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
                                this.causeName
                                [(this.wsnParams, wsParams)]
            }
    member this.id =
        [
            this.workspaceParams :> obj
            this.causeName:> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICause with
        member this.Id = this.id
        member this.ResetId = None
        member this.Name = this.wsnParams |> WsComponentName.value
        member this.Updater = this.updater
        member this.UseInWorkspaceId = true


type causeAddSortableSet
            (wsnSortableSet:wsComponentName,
             ssCfg:sortableSetCfg) 
    = 
    member this.causeName = "causeAddSortableSet"
    member this.wsnSortableSet = wsnSortableSet
    member this.updater = 
            fun (w:workspace) (newWorkspaceId:workspaceId) ->
            result {
                let! ssComp = ssCfg
                            |> SortableSetCfg.makeSortableSet 
                            |> Result.map(workspaceComponent.SortableSet)

                return w |> Workspace.addComponents 
                                newWorkspaceId
                                this.causeName
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
        member this.UseInWorkspaceId = true



type causeAddSorterSetRnd
            (wsnSorterSet:wsComponentName,
             ssCfg:sorterSetRndCfg,
             rngGen:rngGen) 
    = 
    member this.causeName = "causeAddSorterSetRnd"
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
                                this.causeName
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
        member this.Name = this.causeName
        member this.Updater = this.updater
        member this.UseInWorkspaceId = true


type causeAddSorterSetMutator 
            (wsnSorterSetMutator:wsComponentName,
             order:order,
             switchGenMode:switchGenMode,
             sorterCountMutated:sorterCount,
             mutationRate:mutationRate) 
    = 
    member this.causeName = "causeAddSorterSetMutator"
    member this.wsnSorterSetMutator = wsnSorterSetMutator
    member this.order = order
    member this.switchGenMode = switchGenMode
    member this.sorterCountMutated = sorterCountMutated
    member this.mutationRate = mutationRate

    member this._id =
                [|
                  this.causeName :> obj;
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
                                this.causeName
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
        member this.UseInWorkspaceId = true

