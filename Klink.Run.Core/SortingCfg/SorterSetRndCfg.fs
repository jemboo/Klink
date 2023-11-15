namespace global

type sorterSetRndCfg
        (
            name:wsComponentName,
            order: order,
            switchGenMode: switchGenMode,
            switchCount: switchCount,
            sorterCount: sorterCount
        ) =

    let sorterSetRndCfgId =         
        [|
          "sorterSetRndCfg" :> obj;
           order :> obj;
           switchGenMode :> obj;
           switchCount :> obj;
           sorterCount :> obj;
        |] |> GuidUtils.guidFromObjs
           |> SorterSetId.create

    member this.sorterSetId = sorterSetRndCfgId
    member this.name = name
    member this.order = order
    member this.switchCount = switchCount
    member this.switchGenMode = switchGenMode
    member this.sorterCount = sorterCount


module SorterSetRndCfg =

    let makeSorterSetId
            (rdsg:sorterSetRndCfg)
            (rngGen:rngGen)
        =
        [|
          "sorterSetRndCfg" :> obj;
           rdsg.order :> obj;
           rngGen :> obj;
           rdsg.switchGenMode :> obj;
           rdsg.switchCount :> obj;
           rdsg.sorterCount :> obj;
        |] |> GuidUtils.guidFromObjs
           |> SorterSetId.create

    let getConfigName 
            (rdsg:sorterSetRndCfg) 
        =
        sprintf "%d_%s"
            (rdsg.order |> Order.value)
            (rdsg.switchGenMode |> string)

    let makeSorterSet
            (rngGenProvider: rngGenProvider)
            (rdsg: sorterSetRndCfg) 
        =
        let rndGenF () = 
            rngGenProvider |> RngGenProvider.nextRngGen

        result {
            let ssRet =
                match rdsg.switchGenMode with
                | switchGenMode.switch -> 
                    SorterSet.createRandomSwitches
                        rdsg.sorterSetId
                        rdsg.sorterCount
                        rdsg.order
                        []
                        rdsg.switchCount
                        rndGenF
                        |> Ok
                | switchGenMode.stage -> 
                    SorterSet.createRandomStages2
                        rdsg.sorterSetId
                        rdsg.sorterCount
                        rdsg.order
                        []
                        rdsg.switchCount
                        rndGenF
                        |> Ok

                | switchGenMode.stageSymmetric -> 
                    SorterSet.createRandomSymmetric
                        rdsg.sorterSetId
                        rdsg.sorterCount
                        rdsg.order
                        []
                        rdsg.switchCount
                        rndGenF
                        |> Ok

                | _ -> $"switchGenMode:{rdsg.switchGenMode} not handled" |> Error
            return! ssRet
        }