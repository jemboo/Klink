﻿namespace global

type sorterSetMutatorCfg
            (name:wsComponentName,
             order:order,
             switchGenMode:switchGenMode,
             rngGenMutate:rngGen,
             sorterCountMutated:sorterCount,
             mutationRate:mutationRate
            ) =
    let id =         
        [|
          "sorterSetMutatorCfg" :> obj;
           order :> obj;
           switchGenMode :> obj;
           rngGenMutate :> obj;
           sorterCountMutated :> obj;
        |] |> GuidUtils.guidFromObjs

    member this.sorterSetMutatorId = id
    member this.name = name
    member this.order = order
    member this.switchGenMode = switchGenMode
    member this.rngGenMutate = rngGenMutate
    member this.sorterCountMutated = sorterCountMutated
    member this.mutationRate = mutationRate
    interface IWorkspaceComponent with
        member this.Id = id
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
            sorterUniformMutator
            (Some cfg.sorterCountMutated)
            cfg.rngGenMutate