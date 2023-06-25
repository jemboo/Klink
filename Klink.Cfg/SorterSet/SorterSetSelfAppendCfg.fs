namespace global

type sorterSetSelfAppendCfg
            (
             name:wsComponentName,
             order: order,
             rngGen: rngGen,
             switchGenMode: switchGenMode,
             switchCount: switchCount,
             sorterCountFactor: sorterCount,
             sorterCount: sorterCount
            ) =

    let sorterStId =         
        [|
          "sorterSetSelfAppend" :> obj;
           order :> obj;
           rngGen :> obj;
           switchGenMode :> obj;
           switchCount :> obj;
           sorterCountFactor :> obj;
           sorterCount :> obj;
        |] |> GuidUtils.guidFromObjs
           |> SorterSetId.create

    member this.sorterSetId = sorterStId
    member this.name = name
    member this.order = order
    member this.rngGen = rngGen
    member this.switchGenMode = switchGenMode
    member this.switchCount = switchCount
    member this.sorterCountFactor = sorterCountFactor
    member this.sorterCount = sorterCount
    interface IWorkspaceComponent with
        member this.Id = this.sorterSetId |> SorterSetId.value
        member this.WsComponentName = name
        member this.WorkspaceComponentType =
                workspaceComponentType.SorterSet


module SorterSetSelfAppendCfg =

    let getProperties (rdsg: sorterSetSelfAppendCfg) = 
        [|
            ("order", rdsg.order :> obj);
            ("rngGen", rdsg.rngGen :> obj);
            ("switchGenMode", rdsg.switchGenMode :> obj);
            ("switchCount", rdsg.switchCount :> obj);
            ("sorterCountFactor", rdsg.sorterCountFactor :> obj);
            ("sorterCount", rdsg.sorterCount :> obj);
        |]


    let getSorterSetConcatId (cfg: sorterSetSelfAppendCfg) 
        = 
        [|
          "sorterSetSelfAppendCfg" :> obj;
           cfg :> obj;
        |] |> GuidUtils.guidFromObjs
           |> SorterSetId.create

    let getSorterSetConcatFileName (cfg:sorterSetSelfAppendCfg) 
        =
        cfg |> getSorterSetConcatId |> SorterSetId.value |> string


    let getConfigName 
            (rdsg:sorterSetSelfAppendCfg) 
        =
        sprintf "%d_%s"
            (rdsg.order |> Order.value)
            (rdsg.switchGenMode |> string)


    let getSorterSetConcatCount (rdsg: sorterSetSelfAppendCfg) 
        =
        rdsg.sorterCount


    let getSorterSetFactorCfg (cfg:sorterSetSelfAppendCfg)
        =
        let wsCompName = "sorterSetFactor" |> WsComponentName.create
        new sorterSetRndCfg(
            wsCompName,
            cfg.order,
            cfg.rngGen,
            cfg.switchGenMode,
            cfg.switchCount,
            cfg.sorterCountFactor)


    let getSorterSetFactorId (cfg: sorterSetSelfAppendCfg) 
        = 
        (cfg |> getSorterSetFactorCfg).sorterSetId


    let makeSorterSetConcatMap
            (cfg:sorterSetSelfAppendCfg)
        =
        SorterSetConcatMap.createForAppendSet
                (cfg |> getSorterSetFactorId)
                cfg.sorterCountFactor
                (cfg |> getSorterSetConcatId)


