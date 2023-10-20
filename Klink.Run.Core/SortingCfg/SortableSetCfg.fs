﻿namespace global

open System

type sortableSetCertainCfg =
    | All_Bits of order
    | All_Bits_Reduced of order*array<switch>
    | Orbit of permutation


module SortableSetCertainCfg =

    let getOrder 
            (sscc:sortableSetCertainCfg) 
        = 
        match sscc with
        | All_Bits o -> o
        | All_Bits_Reduced (o, _) -> o
        | Orbit p -> p |> Permutation.getOrder


    let getId (cfg:sortableSetCertainCfg) 
        = 
        [| "sortableSetCertainCfg" :> obj;
            cfg :> obj|] |> GuidUtils.guidFromObjs
        |> SortableSetId.create
    

    let getFileName
            (sscc:sortableSetCertainCfg) 
        =
        sscc |> getId |> SortableSetId.value |> string


    let getConfigName 
            (sscc:sortableSetCertainCfg) 
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
                        |> sortableSetCertainCfg.All_Bits_Reduced
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
            (sscc:sortableSetCertainCfg) 
        = 
        match sscc with
        | All_Bits o ->
            SortableSet.makeAllBits
                (sscc |> getId)
                rolloutFormat.RfBs64
                o

        | All_Bits_Reduced (o, switchArray) -> 
            result {
                let sorterId = Guid.Empty |> SorterId.create
                let sorter = Sorter.fromSwitches 
                                sorterId 
                                o
                                switchArray
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
            (stagesReduced:stageCount)
            (order:order) 
        =
        if (stagesReduced |> StageCount.value) > 1 then
            failwith "StageReduction gt 1"
        let switchArray = 
            TwoCycle.evenMode order 
                    |> Switch.fromTwoCycle
                    |> Seq.toArray
        sortableSetCertainCfg.All_Bits_Reduced (order, switchArray)



type sortableSetCfg = 
     | Certain of sortableSetCertainCfg


module SortableSetCfg =

    let getId 
            (ssCfg: sortableSetCfg) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.getId


    let makeSortableSet
            (ssCfg: sortableSetCfg) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.makeSortableSet

    let getOrder
            (ssCfg: sortableSetCfg) 
        = 
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.getOrder


    let getCfgName
            (ssCfg: sortableSetCfg) 
        =
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.getConfigName


    let getFileName
            (ssCfg: sortableSetCfg) 
        =
        match ssCfg with
        | Certain cCfg -> 
            cCfg |> SortableSetCertainCfg.getFileName


    let getSortableSet             
            (sortableSetSave: string -> sortableSet -> Result<bool, string>)
            (sortableSetLookup: string -> Result<sortableSet, string>)
            (cfg:sortableSetCfg) =
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
