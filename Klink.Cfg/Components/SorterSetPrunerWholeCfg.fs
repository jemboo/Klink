namespace global

type sorterSetPrunerWholeCfg
            (name:wsComponentName,
             prunedCount:sorterCount,
             noiseFraction:float option,
             stageWeight:stageWeight
            ) =
    let id =         
        [|
          "sorterSetPrunerWholeCfg" :> obj;
           prunedCount :> obj;
           noiseFraction :> obj;
           stageWeight :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create

    member this.sorterSetPrunerId = id
    member this.name = name
    member this.prunedCount = prunedCount
    member this.noiseFraction = noiseFraction
    member this.stageWeight = stageWeight
    //interface IWorkspaceComponent with
    //    member this.Id = (id |> SorterSetPrunerId.value)
    //    member this.WsComponentName = name
    //    member this.WorkspaceComponentType =
    //            workspaceComponentType.SorterSetPruner


module SorterSetPrunerWholeCfg =

    let getSorterSetPruner
            (cfg:sorterSetPrunerWholeCfg) 
        =
        SorterSetPrunerWhole.load
            cfg.sorterSetPrunerId
            cfg.prunedCount
            cfg.noiseFraction
            cfg.stageWeight


type sorterSetPrunerShcCfg
            (name:wsComponentName,
             prunedCount:sorterCount,
             noiseFraction:float option,
             stageWeight:stageWeight
            ) =
    let id =         
        [|
          "sorterSetPrunerShcCfg" :> obj;
           prunedCount :> obj;
           stageWeight :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create

    member this.sorterSetPrunerId = id
    member this.name = name
    member this.prunedCount = prunedCount
    member this.noiseFraction = noiseFraction
    member this.stageWeight = stageWeight


module SorterSetPrunerShcCfg =

    let getSorterSetPruner
            (cfg:sorterSetPrunerShcCfg)
            (rngGenNoise:rngGen) 
        =
        SorterSetPrunerWhole.load
            cfg.sorterSetPrunerId
            cfg.prunedCount
            cfg.noiseFraction
            cfg.stageWeight


type sorterSetPrunerCfg = 
    | Whole of sorterSetPrunerWholeCfg
    | Shc of sorterSetPrunerShcCfg