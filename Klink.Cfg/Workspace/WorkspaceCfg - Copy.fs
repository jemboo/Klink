namespace global
open System

type workspaceCfg =
    { id:workspaceId;
      history:ICause list }
and ICause =
    abstract member Id:causeId
    abstract member ResetId:workspaceId option
    abstract member Name:string
    abstract member Updater:(workspace->workspaceId->Result<workspace, string>)
    abstract member UseInWorkspaceId:bool


module WorkspaceCfg = 

    let makeWorkspaceId 
            (curId:workspaceId) 
            (future:ICause list) 
        =
        let rec _makeWorkspaceId 
                (curId:workspaceId) 
                (future:ICause list) 
            =
            match future with
            | [] -> curId
            | h::t ->
                match h.ResetId with
                | Some id -> 
                    _makeWorkspaceId id t
                | None ->
                    let nextId =
                        if h.UseInWorkspaceId then
                             [
                                curId |> WorkspaceId.value :> obj;
                                h.Id |> CauseId.value :> obj
                             ]
                             |> GuidUtils.guidFromObjs 
                             |> WorkspaceId.create
                        else
                            curId
                    _makeWorkspaceId nextId t

        _makeWorkspaceId curId future


    let makeWorkspaceCfg (history:ICause list) =
        { 
          id = makeWorkspaceId (Workspace.empty |> Workspace.getId) history;
          history = history
        }


    let Empty = makeWorkspaceCfg []


    let addCause 
            (ccfg:ICause) 
            (wscfg:workspaceCfg) =
        makeWorkspaceCfg ([ccfg] |> List.append wscfg.history)

    let addCauses (ccfgs:ICause list) (wscfg:workspaceCfg) =
        makeWorkspaceCfg (ccfgs |> List.append wscfg.history)

    let removeLastCause (wscfg:workspaceCfg) =
        match wscfg.history with
        | [] ->   (makeWorkspaceCfg [], [])
        | h::t -> (
                   makeWorkspaceCfg 
                            (wscfg.history |> List.removeAt (wscfg.history.Length - 1)),
                   [wscfg.history.[wscfg.history.Length - 1]])


    let makeWorkspace 
            (causeHist:ICause list) 
            (logger: string->unit)
            (startingWs:workspace)
        =
        let rec _makeWorkspace 
                (wksR:Result<workspace,string>) 
                (history:ICause list)
            =
            match history with 
            | [] -> wksR
            | h::t ->
                match wksR with
                | Error _ -> wksR
                | Ok wsCur ->
                let nextId = makeWorkspaceId (wsCur |> Workspace.getId) [h]
                let nextWs = h.Updater wsCur nextId
                //logger $"updatingQ: {h.Name}"
                _makeWorkspace nextWs t

        _makeWorkspace (startingWs |> Ok) causeHist


    let getLastSavedWorspaceIdAndFuture 
                (fileStore:IWorkspaceStore)
                (logger: string->unit)
                (workspaceCfg:workspaceCfg) 
        =
        let rec _lastWorkspaceCfg
                (wksCfgCurrent:workspaceCfg) 
                (future:ICause list)
            =
            let chkLatest = fileStore.WorkSpaceExists(wksCfgCurrent.id)
            match chkLatest with
            | Error m -> $"Error in getLastSavedWorspaceIdAndFuture (1) {m}" |> Error
            | Ok wasFound ->
                if wasFound then
                    logger $"found {wksCfgCurrent.id |> WorkspaceId.value }"
                    (wksCfgCurrent.id, future) |> Ok
                elif wksCfgCurrent.history.Length = 0 then
                    logger $"no prior workspace found"
                    (Empty.id, future) |> Ok
                else
                    let lastWksCfg, lastCause = removeLastCause wksCfgCurrent
                    _lastWorkspaceCfg lastWksCfg (lastCause@future)

        _lastWorkspaceCfg workspaceCfg []


    let runWorkspaceCfg 
            (fileStore:IWorkspaceStore)
            (logger: string->unit)
            (workspaceCfg:workspaceCfg)
        =
        result {
            let! latestSavedId, future = 
                    workspaceCfg |>
                        getLastSavedWorspaceIdAndFuture fileStore logger

            let! restoredAncestorWs = 
                if (latestSavedId = Empty.id) then
                    Workspace.empty |> Ok
                else
                    fileStore.LoadWorkSpace(latestSavedId)
            return! restoredAncestorWs |> makeWorkspace future logger
        }


    let runWorkspaceCfgOnWorkspace
            (logger: string->unit)
            (causes:ICause list)
            (ws:workspace)
        =
        result {

            return! ws |> makeWorkspace causes logger
        }




module WorkspaceParamsAttrs =

    let getGeneration
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> Generation.create
        }
    let setGeneration
            (key:string) 
            (value:generation)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> Generation.value |> string)

    let incrGeneration
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          let nextGen = Convert.ToInt32(cereal) + 1
          return         
            workspaceParams |> WorkspaceParams.addItem key (nextGen |> string)
        }

    let generationIsGte
            (genVal:generation)
            (workspaceParams:workspaceParams) 
        =

        let _fg
                (key:string) 
                (fF: generation -> bool)
                (workspaceParams:workspaceParams) 
            =
            workspaceParams |> getGeneration key |> Result.filterF fF

        workspaceParams 
            |> _fg
                    "generation_current"
                    (fun gen -> 
                            (gen |> Generation.value) >= (genVal |> Generation.value))


    //let getGenerationFilter
    //        (key:string) 
    //        (workspaceParams:workspaceParams) 
    //    =
    //    result {
    //      let! cereal = getItem key workspaceParams
    //      return! cereal |> GenerationFilterDto.fromDto
    //    }
    //let setGenerationFilter
    //        (key:string) 
    //        (gf:generationFilter)
    //        (workspaceParams:workspaceParams) 
    //    =
    //    workspaceParams |> addItem key (gf |> GenerationFilterDto.toJson)



    let getMutationRate
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToDouble(cereal) |> MutationRate.create
        }
    let setMutationRate
            (key:string) 
            (value:mutationRate)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> MutationRate.value |> string)


    let getNoiseFraction
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          let cv = Convert.ToDouble (cereal)
          if cv <> 0 then return cv |> NoiseFraction.create |> Some
          else return None
        }
    let setNoiseFraction
            (key:string) 
            (value:noiseFraction option) 
            (workspaceParams:workspaceParams) 
        =
        let cereal =
            match value with
            | Some nf -> nf |> NoiseFraction.value |> string
            | None -> 0.0 |> string
        workspaceParams |> WorkspaceParams.addItem key cereal


    let getOrder
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> Order.createNr
        }
    let setOrder
            (key:string) 
            (value:order)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> Order.value |> string)


    let getRngGen
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! cereal |> RngGenDto.fromJson
        }
    let setRngGen
            (key:string) 
            (value:rngGen)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> RngGenDto.toJson)

    let updateRngGen
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! curRngGen = workspaceParams |> getRngGen key
          let nextRngGen = curRngGen |> Rando.nextRngGen
          return setRngGen key nextRngGen workspaceParams
        }


    let getRunId
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return cereal |> Guid |> RunId.create
        }
    let setRunId
            (key:string) 
            (value:runId)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> RunId.value |> string)


    let getSortableSetId
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return cereal |> Guid |> SortableSetId.create
        }
    let setSortableSetId
            (key:string) 
            (value:sortableSetId)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> SortableSetId.value |> string)


    let getSorterCount
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> SorterCount.create
        }
    let setSorterCount
            (key:string) 
            (value:sorterCount)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> SorterCount.value |> string)


    let getSorterEvalMode
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! Json.deserialize<sorterEvalMode>(cereal)
        }
    let setSorterEvalMode
            (key:string) 
            (value:sorterEvalMode)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (Json.serialize(value))


    let getSorterSetPruneMethod
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! Json.deserialize<sorterSetPruneMethod>(cereal)
        }
    let setSorterSetPruneMethod
            (key:string) 
            (value:sorterSetPruneMethod)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (Json.serialize(value))


    let getStageCount
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> StageCount.create
        }
    let setStageCount
            (key:string) 
            (value:stageCount)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> StageCount.value |> string)


    let getStageWeight
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToDouble(cereal) |> StageWeight.create
        }
    let setStageWeight
            (key:string) 
            (value:stageWeight)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> StageWeight.value |> string)


    let getSwitchCount
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> SwitchCount.create
        }
    let setSwitchCount
            (key:string) 
            (value:switchCount)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> SwitchCount.value |> string)


    let getSwitchGenMode
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! Json.deserialize<switchGenMode>(cereal)
        }
    let setSwitchGenMode
            (key:string) 
            (value:switchGenMode)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (Json.serialize(value))


    let getUseParallel
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToBoolean (cereal) |> UseParallel.create
        }
    let setUseParallel
            (key:string) 
            (value:useParallel)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UseParallel.value |> string)




