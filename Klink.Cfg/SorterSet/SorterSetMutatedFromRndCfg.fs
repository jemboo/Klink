namespace global

type sorterSetMutatedFromRndCfg
            (
             name:wsComponentName,
             order: order,
             rngGenCreate: rngGen,
             switchGenMode: switchGenMode,
             switchCount: switchCount,
             sorterCountOriginal: sorterCount,
             rngGenMutate: rngGen,
             sorterCount: sorterCount,
             mutationRate:mutationRate
            ) =

    let sorterStId =         
        [|
          "sorterSetMutatedFromRndCfg" :> obj;
           order :> obj;
           rngGenCreate :> obj;
           switchGenMode :> obj;
           switchCount :> obj;
           sorterCountOriginal :> obj;
           rngGenMutate :> obj;
           sorterCount :> obj;
           mutationRate :> obj;
        |] |> GuidUtils.guidFromObjs
           |> SorterSetId.create

    member this.sorterSetId = sorterStId
    member this.name = name
    member this.order = order
    member this.rngGenCreate = rngGenCreate
    member this.switchGenMode = switchGenMode
    member this.switchCount = switchCount
    member this.sorterCountOriginal = sorterCountOriginal
    member this.rngGenMutate = rngGenMutate
    member this.sorterCount = sorterCount
    member this.mutationRate = mutationRate
    interface IWorkspaceComponentCfg with
        member this.Id = this.sorterSetId |> SorterSetId.value
        member this.WsComponentName = name
        member this.WorkspaceComponentType =
                workspaceComponentType.SorterSet


module SorterSetMutatedFromRndCfg =

    let getProperties (cfg: sorterSetMutatedFromRndCfg) = 
        [|
            ("order", cfg.order :> obj);
            ("rngGenCreate", cfg.rngGenCreate :> obj);
            ("switchGenMode", cfg.switchGenMode :> obj);
            ("switchCount", cfg.switchCount :> obj);
            ("sorterCountOriginal", cfg.sorterCountOriginal :> obj);
            ("rngGenMutate", cfg.rngGenMutate :> obj);
            ("sorterCount", cfg.sorterCount :> obj);
            ("mutationRate", cfg.mutationRate :> obj);
        |]

    let getSorterSetParentCfg (cfg:sorterSetMutatedFromRndCfg)
        =
        new sorterSetRndCfg(
            "sorterSetParent" |> WsComponentName.create,
            cfg.order,
            cfg.rngGenCreate,
            cfg.switchGenMode,
            cfg.switchCount,
            cfg.sorterCountOriginal)


    let getSorterSetMutatorCfg (cfg:sorterSetMutatedFromRndCfg)
        =
        new sorterSetMutatorCfg(
            "sorterSetMutator" |> WsComponentName.create,
            cfg.order,
            cfg.switchGenMode,
            cfg.sorterCount,
            cfg.mutationRate)


    let getFileName
            (cfg: sorterSetMutatedFromRndCfg) 
        =
        cfg.sorterSetId |> SorterSetId.value |> string


    let getConfigName 
            (rdsg:sorterSetMutatedFromRndCfg) 
        =
        sprintf "%d_%s_%f"
            (rdsg.order |> Order.value)
            (rdsg.switchGenMode |> string)
            (rdsg.mutationRate |> MutationRate.value )


    let getSorterSetOriginalId (cfg: sorterSetMutatedFromRndCfg) 
        = 
        (cfg |> getSorterSetParentCfg).sorterSetId


    let getSorterSetParentMapCfg (cfg:sorterSetMutatedFromRndCfg)
        =
        new sorterSetParentMapCfg
            (
                "sorterSetParentMap" |> WsComponentName.create,
                (cfg |> getSorterSetOriginalId),
                (cfg.sorterCountOriginal),
                (cfg.sorterSetId),
                (cfg.sorterCount)
            )

    let getSorterSetMutator (cfg:sorterSetMutatedFromRndCfg) 
        =
        cfg |> getSorterSetMutatorCfg 
            |> SorterSetMutatorCfg.getSorterSetMutator 