namespace global

type sorterSetRndCfg
            (
             name:wsComponentName,
             order: order,
             rngGen: rngGen,
             switchGenMode: switchGenMode,
             switchCount: switchCount,
             sorterCount: sorterCount
            ) =

    let sorterStId =         
        [|
          "sorterSetRndCfg" :> obj;
           order :> obj;
           rngGen :> obj;
           switchGenMode :> obj;
           switchCount :> obj;
           sorterCount :> obj;
        |] |> GuidUtils.guidFromObjs
           |> SorterSetId.create

    member this.sorterSetId = sorterStId
    member this.name = name
    member this.order = order
    member this.rngGen = rngGen
    member this.switchCount = switchCount
    member this.switchGenMode = switchGenMode
    member this.sorterCount = sorterCount
    interface IWorkspaceComponentCfg with
        member this.Id = this.sorterSetId |> SorterSetId.value
        member this.WsComponentName = name
        member this.WorkspaceComponentType =
                workspaceComponentType.SorterSetMutator


module SorterSetRndCfg =

    let getProperties (rdsg: sorterSetRndCfg) = 
        [|
            ("order", rdsg.order :> obj);
            ("rngGen", rdsg.rngGen :> obj);
            ("switchGenMode", rdsg.switchGenMode :> obj);
            ("switchCount", rdsg.switchCount :> obj);
            ("sorterCount", rdsg.sorterCount :> obj);
        |]

    let getFileName
            (cfg:sorterSetRndCfg) 
        =
        cfg.sorterSetId |> SorterSetId.value |> string

    let getConfigName 
            (rdsg:sorterSetRndCfg) 
        =
        sprintf "%d_%s"
            (rdsg.order |> Order.value)
            (rdsg.switchGenMode |> string)

    let makeSorterSet
            (save: string -> sorterSet -> Result<bool, string>)
            (rdsg: sorterSetRndCfg) 
        =
        let randy = rdsg.rngGen |> Rando.fromRngGen
        let nextRng () =
            randy |> Rando.nextRngGen
        result {
            let ssRet =
                match rdsg.switchGenMode with
                | Switch -> 
                    SorterSet.createRandomSwitches
                        rdsg.sorterSetId
                        rdsg.sorterCount
                        rdsg.order
                        []
                        rdsg.switchCount
                        nextRng

                | Stage -> 
                    SorterSet.createRandomStages2
                        rdsg.sorterSetId
                        rdsg.sorterCount
                        rdsg.order
                        []
                        rdsg.switchCount
                        nextRng

                | StageSymmetric -> 
                    SorterSet.createRandomSymmetric
                        rdsg.sorterSetId
                        rdsg.sorterCount
                        rdsg.order
                        []
                        rdsg.switchCount
                        nextRng
            let! wasSaved = save (rdsg |> getFileName) ssRet
            return ssRet
        }


    let getSorterSet
            (lookup: string -> Result<sorterSet, string>)
            (save: string -> sorterSet -> Result<bool, string>)
            (rdsg: sorterSetRndCfg)
        =
        result {
            let loadRes  = 
                result {
                    let! mut = lookup (rdsg |> getFileName)
                    return mut
                }

            match loadRes with
            | Ok mut -> return mut
            | Error _ -> return! (makeSorterSet save rdsg)
        }