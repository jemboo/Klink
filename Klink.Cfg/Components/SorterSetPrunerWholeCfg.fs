namespace global

type sorterSetPrunerWholeCfg
            (name:wsComponentName,
             selectionFraction:selectionFraction,
             stageWeight:stageWeight,
             temp:temp
            ) =
    let id =         
        [|
          "sorterSetPrunerWholeCfg" :> obj;
           selectionFraction :> obj;
           stageWeight :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create

    member this.sorterSetPrunerId = id
    member this.name = name
    member this.selectionFraction = selectionFraction
    member this.temp = temp
    member this.stageWeight = stageWeight
    interface IWorkspaceComponentCfg with
        member this.Id = (id |> SorterSetPrunerId.value)
        member this.WsComponentName = name
        member this.WorkspaceComponentType =
                workspaceComponentType.SorterSetMutator


module SorterSetPrunerWholeCfg =

    let getSorterSetPruner
            (cfg:sorterSetPrunerWholeCfg) 
        =
        SorterSetPrunerWhole.load
            cfg.sorterSetPrunerId
            cfg.selectionFraction
            cfg.temp
            cfg.stageWeight


type sorterSetPrunerShcCfg
            (name:wsComponentName,
             selectionFraction:selectionFraction,
             temp:temp,
             stageWeight:stageWeight
            ) =
    let id =         
        [|
          "sorterSetPrunerShcCfg" :> obj;
           selectionFraction :> obj;
           stageWeight :> obj;
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create

    member this.sorterSetPrunerId = id
    member this.name = name
    member this.selectionFraction = selectionFraction
    member this.temp = temp
    member this.stageWeight = stageWeight
    interface IWorkspaceComponentCfg with
        member this.Id = (id |> SorterSetPrunerId.value)
        member this.WsComponentName = name
        member this.WorkspaceComponentType =
                workspaceComponentType.SorterSetMutator


module SorterSetPrunerShcCfg =

    let getSorterSetPruner
            (cfg:sorterSetPrunerShcCfg) 
        =
        SorterSetPrunerWhole.load
            cfg.sorterSetPrunerId
            cfg.selectionFraction
            cfg.temp
            cfg.stageWeight


type sorterSetPrunerCfg = 
    | Whole of sorterSetPrunerWholeCfg
    | Shc of sorterSetPrunerShcCfg