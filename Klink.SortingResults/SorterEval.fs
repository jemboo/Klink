﻿namespace global

open System

type sorterPhenotypeId = private SorterPhenotypeId of Guid

module SorterPhenotypeId =
    let value (SorterPhenotypeId v) = v

    let create (id: Guid) =
        id |> SorterPhenotypeId

    let createFromSwitches (switches: seq<switch>) =
        switches
        |> Seq.map (fun sw -> sw :> obj)
        |> GuidUtils.guidFromObjs
        |> SorterPhenotypeId

    
type switchesUsed = private SwitchesUsed of switch[]

module SwitchesUsed = 
    let make (switches: switch[]) =
        SwitchesUsed switches

    let get (sus: switchesUsed) =
        let (SwitchesUsed rv) = sus
        rv

type sorterSpeed = private { switchCt:switchCount; stageCt:stageCount }

module SorterSpeed =
    let create 
            (switchCt: switchCount) 
            (stageCt: stageCount) 
        =
        { switchCt = switchCt
          stageCt = stageCt }

    let getSwitchCount (sorterSpeedBn: sorterSpeed) = sorterSpeedBn.switchCt

    let getStageCount (sorterSpeedBn: sorterSpeed) = sorterSpeedBn.stageCt

    let toIndex (sorterSpeedBn: sorterSpeed) =
        let switchCtV = sorterSpeedBn.switchCt |> SwitchCount.value
        let stageCtV = sorterSpeedBn.stageCt |> StageCount.value
        ((switchCtV * (switchCtV + 1)) / 2) + stageCtV

    let fromIndex (index: int) =
        let indexFlt = (index |> float) + 1.0
        let p = (sqrt (1.0 + 8.0 * indexFlt) - 1.0) / 2.0
        let pfloor = Math.Floor(p)

        if (p = pfloor) then
            let stageCt = 1 |> (-) (int pfloor) |> StageCount.create
            let switchCt = 1 |> (-) (int pfloor) |> SwitchCount.create

            { sorterSpeed.switchCt = switchCt
              stageCt = stageCt }
        else
            let stageCt =
                (float index) - (pfloor * (pfloor + 1.0)) / 2.0 |> int |> StageCount.create

            let switchCt = (int pfloor) |> SwitchCount.create

            { sorterSpeed.switchCt = switchCt
              stageCt = stageCt }


    let fromSorterOpOutput (sorterOpOutpt: sorterOpOutput) =
        let sortr = sorterOpOutpt |> SorterOpOutput.getSorter
        try
            let switchUseCts = 
                   sorterOpOutpt
                   |> SorterOpOutput.getSorterOpTracker
                   |> SorterOpTracker.getSwitchUseCounts

            let switchesUsd =
                    switchUseCts
                    |> SwitchUseCounts.getUsedSwitchesFromSorter sortr
                
            let usedSwitchCt = switchesUsd.Length |> SwitchCount.create
            let usedStageCt = (switchesUsd |> StageCover.getStageCount)
            let sortrPhenotypId = switchesUsd |> SorterPhenotypeId.createFromSwitches
            (create usedSwitchCt usedStageCt, sortrPhenotypId, switchUseCts) |> Ok
        with ex ->
            (sprintf "error in SorterSpeed.fromSorterOpOutput: %s" ex.Message)
            |> Result.Error


    let modifyForPrefix
            (ordr:order)
            (tcAdded:stageCount)
            (ss:sorterSpeed) 
        =
        let wcNew = 
            tcAdded 
            |> StageCount.toSwitchCount ordr
            |> SwitchCount.add (getSwitchCount ss)

        let tcNew = 
            tcAdded
            |> StageCount.add (getStageCount ss)

        create wcNew tcNew


    let report (sorterSpd : sorterSpeed option) =
        match sorterSpd with
        | Some ss -> sprintf "%d\t%d"
                         (ss |> getStageCount |> StageCount.value)
                         (ss |> getSwitchCount |> SwitchCount.value)
        | None -> "-\t-"

    let getProps (sorterSpd : sorterSpeed option) =
        match sorterSpd with
        | Some ss -> ((ss |> getStageCount |> StageCount.value |> string),
                      (ss |> getSwitchCount |> SwitchCount.value |> string))
        | None -> ("", "")

    let getStageCount0 (sorterSpd : sorterSpeed option) =
        match sorterSpd with
        | Some ss -> (ss |> getStageCount |> StageCount.value)
        | None -> 0


    let getSwitchCount0 (sorterSpd : sorterSpeed option) =
        match sorterSpd with
        | Some ss -> (ss |> getSwitchCount |> SwitchCount.value)
        | None -> 0


type sorterPerf = 
    | IsSuccessful of bool 
    | SortedSetSize of sortableCount

module SorterPerf =
    let isSuccessful (ordr:order) (sorterPrf:sorterPerf) =
        match sorterPrf with
        | IsSuccessful bv -> bv
        | SortedSetSize ssz -> (SortableCount.value ssz) < (Order.value ordr) + 1

    let report (sp: sorterPerf option)
        =
        match sp with
        | Some perf ->
            match perf with
            | IsSuccessful bv ->
                sprintf "%b\t-" bv
            | SortedSetSize sz ->
                sprintf "-\t%d" (SortableCount.value sz)
        | None -> "-\t-"

    let getProps (sp : sorterPerf option) =
        match sp with
        | Some perf ->
            match perf with
            | IsSuccessful bv ->
                (bv |> string, "")
            | SortedSetSize sz ->
                ("", sz |> SortableCount.value |> string)
        | None -> ("", "")



type sorterEvalProps =
     | ErrorMsg 
     | Phenotype
     | SortableSetId
     | SortedSetSize
     | SorterId
     | StageCount
     | Success
     | SwitchCount


type sorterEvalMode = 
    | DontCheckSuccess
    | CheckSuccess 
    | GetSortedSetCount

module SorterEvalMode =
    let fromString (sv: string) =
        match sv.ToLowerInvariant() with
        | "DontCheckSuccess" -> Ok DontCheckSuccess
        | "CheckSuccess" -> Ok CheckSuccess
        | "GetSortedSetCount" -> Ok GetSortedSetCount
        | _ -> Error "Invalid sorterEvalMode string"

    let toString (mode: sorterEvalMode) =
        match mode with
        | DontCheckSuccess -> "DontCheckSuccess"
        | CheckSuccess -> "CheckSuccess"
        | GetSortedSetCount -> "GetSortedSetCount"


type sorterEval =
    private
        { errorMessage: string option
          switchUseCts: switchUseCounts option
          sorterSpeed: sorterSpeed option
          sorterPrf: sorterPerf option
          sortrPhenotypeId: sorterPhenotypeId option
          sortableSetId: sortableSetId
          sortrId: sorterId }


module SorterEval =
    let make (errorMsg:string option)
             (switchUseCts: switchUseCounts option) 
             (sorterSpeed: sorterSpeed option) 
             (sorterPrf: sorterPerf option) 
             (sortrPhenotypeId: sorterPhenotypeId option)
             (sortableStId: sortableSetId)
             (sortrId: sorterId) 
        =
        { errorMessage = errorMsg
          switchUseCts = switchUseCts
          sorterSpeed = sorterSpeed
          sorterPrf = sorterPrf
          sortrPhenotypeId = sortrPhenotypeId
          sortableSetId = sortableStId
          sortrId = sortrId }

    let getErrorMessage (sorterEvl:sorterEval) =
        sorterEvl.errorMessage
    let getSwitchUseCounts (sorterEvl:sorterEval) =
        sorterEvl.switchUseCts
    let getSorterSpeed (sorterEvl:sorterEval) =
        sorterEvl.sorterSpeed
    let getSorterPerf (sorterEvl:sorterEval) =
        sorterEvl.sorterPrf
    let getSortrPhenotypeId (sorterEvl:sorterEval) =
        sorterEvl.sortrPhenotypeId
    let getSortableSetId (sorterEvl:sorterEval) =
        sorterEvl.sortableSetId
    let getSorterId (sorterEvl:sorterEval) =
        sorterEvl.sortrId

    let shouldRetest 
        (sorterEvalMod:sorterEvalMode) 
        (sorterEvl:sorterEval)
        : bool =
        if (sorterEvl.errorMessage |> Option.isSome)
            then false else
        match sorterEvalMod with
        | CheckSuccess -> (sorterEvl.sorterPrf |> Option.isNone)
        | DontCheckSuccess -> (sorterEvl.sorterSpeed |> Option.isNone)
        | GetSortedSetCount -> 
                match sorterEvl.sorterPrf with  
                | None -> true
                | Some perf -> 
                    match perf with
                    | IsSuccessful _ -> true
                    | sorterPerf.SortedSetSize _ -> false


    let failedForSure
        (sorterEvl:sorterEval)
        : bool =
        if (sorterEvl.errorMessage |> Option.isSome)
            then true
        else
            match sorterEvl.sorterPrf with  
            | None -> false
            | Some perf -> 
                match perf with
                | IsSuccessful sOrf -> not sOrf
                | sorterPerf.SortedSetSize _ -> false


    let evalSorterWithSortableSet 
            (sorterPerfEvalMod: sorterEvalMode) 
            (sortableSt: sortableSet) 
            (sortr: sorter) 
        : sorterEval =

        let _addSorterToErrorResultCase sortr resA =
            match resA with
            | Ok rv -> rv |> Ok
            | Error es -> (sortr, es) |> Error

        
        let sortrId = (sortr |> Sorter.getSorterId)
        let sortableStId = (sortableSt |> SortableSet.getSortableSetId)
        let sorterOpOutput =
            SortingRollout.makeSorterOpOutput sorterOpTrackMode.SwitchUses sortableSt sortr

        match sorterOpOutput with
        | Ok output ->
            let res = output |> SorterSpeed.fromSorterOpOutput
            match res with
            | Ok (sorterSpeed, sorterPhenotypId, switchUseCts) ->
                match sorterPerfEvalMod with
                | DontCheckSuccess -> 
                    make None (Some switchUseCts) (Some sorterSpeed) None (Some sorterPhenotypId) sortableStId sortrId
                | CheckSuccess -> 
                    let isSuccessfl = 
                        output 
                        |> SorterOpOutput.isSorted
                        |> sorterPerf.IsSuccessful
                        |> Option.Some
                    make None (Some switchUseCts) (Some sorterSpeed) isSuccessfl (Some sorterPhenotypId) sortableStId sortrId
                | GetSortedSetCount ->
                    let sortedSetCt = output |> SorterOpOutput.getRefinedSortableCount
                    match sortedSetCt with
                    | Ok ct ->
                        let sct = ct |> sorterPerf.SortedSetSize |> Some
                        make None (Some switchUseCts) (Some sorterSpeed) sct (Some sorterPhenotypId) sortableStId sortrId
                    | Error msg ->
                        make (Some msg) (Some switchUseCts) (Some sorterSpeed) None (Some sorterPhenotypId) sortableStId sortrId

            | Error msg -> make (Some msg) None None None None sortableStId sortrId

        | Error msg -> make  (Some msg) None None None None sortableStId sortrId


    let modifyForPrefix
            (ordr:order)
            (tc:stageCount)
            (sev:sorterEval) 
        =
        {sev with 
           sorterSpeed = 
           sev.sorterSpeed
           |> Option.map(SorterSpeed.modifyForPrefix ordr tc)}


    let getSorterEvalProps 
            (sev:sorterEval) =
        let stageCt, switchCt = (sev.sorterSpeed |> SorterSpeed.getProps)
        let successful, sortableCt = (sev.sorterPrf |> SorterPerf.getProps)
        [|
            (sorterEvalProps.ErrorMsg, sev.errorMessage |> Option.defaultValue "");
            (sorterEvalProps.Phenotype, sev.sortrPhenotypeId |> Option.map(SorterPhenotypeId.value >> string) |> Option.defaultValue "");
            (sorterEvalProps.SortableSetId, sev.sortableSetId |> SortableSetId.value |> string);
            (sorterEvalProps.SorterId, sev.sortrId |> SorterId.value |> string);
            (sorterEvalProps.StageCount, stageCt);
            (sorterEvalProps.Success, successful);
            (sorterEvalProps.SwitchCount, switchCt);
            (sorterEvalProps.SortedSetSize, sortableCt);
        |] |> Map.ofArray
