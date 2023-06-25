namespace global

open System

type sortableSetCertainCfgOld =
    | All_Bits of order
    | All_Bits_Reduced of order*array<switch>
    | Orbit of permutation


module SortableSetCertainCfgOld =

    let getOrder 
            (sscc:sortableSetCertainCfgOld) 
        = 
        match sscc with
        | All_Bits o -> o
        | All_Bits_Reduced (o, _) -> o
        | Orbit p -> p |> Permutation.getOrder


    let getId 
                (cfg:sortableSetCertainCfgOld) 
        = 
        [| "sortableSetCertainCfg" :> obj;
            cfg :> obj|] |> GuidUtils.guidFromObjs
        |> SortableSetId.create
    

    let getFileName
            (sscc:sortableSetCertainCfgOld) 
        =
        sscc |> getId |> SortableSetId.value |> string



    let getConfigName 
            (sscc:sortableSetCertainCfgOld) 
        =
        match sscc with
            | All_Bits o -> 
                sprintf "%s_%d"
                    "All"
                    (sscc |> getOrder |> Order.value)
            | All_Bits_Reduced (o, a) ->
                sprintf "%s_%d_%d"
                    "Reduced"
                    (sscc |> getOrder |> Order.value)
                    (a.Length)
            | Orbit perm ->
                sprintf "%s_%d_%d"
                    "Orbit"
                    (sscc |> getOrder |> Order.value)
                    (perm |> Permutation.powers None |> Seq.length)


    let switchReduceBits
            (ordr:order)
            (sortr:sorter)
        =
        result {
            let refinedSortableSetId = 
                (ordr, (sortr |> Sorter.getSwitches))
                        |> sortableSetCertainCfgOld.All_Bits_Reduced
                        |> getId
            let! baseSortableSet = 
                SortableSet.makeAllBits
                    (Guid.Empty |> SortableSetId.create)
                    rolloutFormat.RfBs64
                    ordr

            let! sorterOpOutput = 
                SortingRollout.makeSorterOpOutput
                    sorterOpTrackMode.SwitchUses
                    baseSortableSet
                    sortr

            let! refined = sorterOpOutput
                            |> SorterOpOutput.getRefinedSortableSet
                                                 refinedSortableSetId
            return refined
        }


    let makeSortableSet 
            (sscc:sortableSetCertainCfgOld) 
        = 
        match sscc with
        | All_Bits o ->
            SortableSet.makeAllBits
                (sscc |> getId)
                rolloutFormat.RfBs64
                o

        | All_Bits_Reduced (o, sa) -> 
            result {
                let sorterId = Guid.Empty |> SorterId.create
                let sorter = Sorter.fromSwitches 
                                sorterId 
                                o
                                sa
                let! refinedSortableSet =
                        switchReduceBits o sorter

                return refinedSortableSet
            }

        | Orbit perm -> 
                SortableSet.makeOrbits
                    (sscc |> getId)
                    None
                    perm


    let makeAllBitsReducedOneStage 
            (order:order) 
        =
        let sws = TwoCycle.evenMode order 
                    |> Switch.fromTwoCycle
                    |> Seq.toArray
        sortableSetCertainCfgOld.All_Bits_Reduced (order, sws)



type sortableSetCfgOld = 
     | Certain of sortableSetCertainCfgOld


module SortableSetCfgOld =

    let getId 
            (ssCfg: sortableSetCfgOld) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfgOld.getId


    let makeSortableSet
            (ssCfg: sortableSetCfgOld) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfgOld.makeSortableSet

    let getOrder
            (ssCfg: sortableSetCfgOld) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfgOld.getOrder


    let getCfgName
            (ssCfg: sortableSetCfgOld) 
        =
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfgOld.getConfigName


    let getFileName
            (ssCfg: sortableSetCfgOld) 
        =
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfgOld.getFileName


    let getSortableSet             
            (sortableSetSave: string -> sortableSet -> Result<bool, string>)
            (sortableSetLookup: string -> Result<sortableSet, string>)
            (cfg:sortableSetCfgOld) =
        result {
            let fileName = (cfg |> getFileName)
            let loadRes = sortableSetLookup fileName
            match loadRes with
            | Ok ss -> return ss
            | Error _ ->
                let! sortableSet = makeSortableSet cfg
                let! saveRes = sortableSetSave fileName sortableSet
                return sortableSet
        }
