namespace global

type sorterSetMutatorCfg
            (name:wsComponentName,
             order:order,
             switchGenMode:switchGenMode,
             sorterCountMutated:sorterCount,
             mutationRate:mutationRate
            ) =
    let id =         
        [|
          "sorterSetMutatorCfg" :> obj;
           order :> obj;
           switchGenMode :> obj;
           sorterCountMutated :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetMutatorId.create

    member this.sorterSetMutatorId = id
    member this.name = name
    member this.order = order
    member this.switchGenMode = switchGenMode
    member this.sorterCountMutated = sorterCountMutated
    member this.mutationRate = mutationRate
    interface IWorkspaceComponentCfg with
        member this.Id = (id |> SorterSetMutatorId.value)
        member this.WsComponentName = name
        member this.WorkspaceComponentType =
                workspaceComponentType.SorterSetMutator


module SorterSetMutatorCfg =

    let getSorterSetMutator 
            (cfg:sorterSetMutatorCfg) 
        =
        let sorterUniformMutator = 
            SorterUniformMutator.create
                    None
                    None
                    cfg.switchGenMode
                    cfg.mutationRate
            |> sorterMutator.Uniform

        SorterSetMutator.load
            cfg.sorterSetMutatorId
            sorterUniformMutator
            (Some cfg.sorterCountMutated)