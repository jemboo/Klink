namespace global
open System


module WorkspaceParamsAttrs =

    let getGeneration
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> Generation.create
        }
    let setGeneration
            (key:workspaceParamsKey) 
            (value:generation)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> Generation.value |> string)

    let incrGeneration
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          let nextGen = Convert.ToInt32(cereal) + 1
          return         
            workspaceParams |> WorkspaceParams.addItem key (nextGen |> string)
        }

    let generationIsGte
            (key:workspaceParamsKey) 
            (genVal:generation)
            (workspaceParams:workspaceParams) 
        =

        let _fg
                (key:workspaceParamsKey) 
                (fF: generation -> bool)
                (workspaceParams:workspaceParams) 
            =
            workspaceParams |> getGeneration key |> Result.filterF fF

        workspaceParams 
            |> _fg
                    key
                    (fun gen -> 
                            (gen |> Generation.value) >= (genVal |> Generation.value))


    let getGenerationFilter
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! cereal |> GenerationFilterDto.fromJson
        }
    let setGenerationFilter
            (key:workspaceParamsKey) 
            (gf:generationFilter)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (gf |> GenerationFilterDto.toJson)



    let getMutationRate
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToDouble(cereal) |> MutationRate.create
        }
    let setMutationRate
            (key:workspaceParamsKey) 
            (value:mutationRate)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> MutationRate.value |> string)


    let getNoiseFraction
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          let cv = Convert.ToDouble (cereal)
          if cv <> 0 then return cv |> NoiseFraction.create |> Some
          else return None
        }
    let setNoiseFraction
            (key:workspaceParamsKey) 
            (value:noiseFraction option) 
            (workspaceParams:workspaceParams) 
        =
        let cereal =
            match value with
            | Some nf -> nf |> NoiseFraction.value |> string
            | None -> 0.0 |> string
        workspaceParams |> WorkspaceParams.addItem key cereal


    let getOrder
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> Order.createNr
        }
    let setOrder
            (key:workspaceParamsKey) 
            (value:order)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> Order.value |> string)


    let getRngGen
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! cereal |> RngGenDto.fromJson
        }
    let setRngGen
            (key:workspaceParamsKey) 
            (value:rngGen)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> RngGenDto.toJson)

    let updateRngGen
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! curRngGen = workspaceParams |> getRngGen key
          let nextRngGen = curRngGen |> Rando.nextRngGen
          return setRngGen key nextRngGen workspaceParams
        }


    let getRunId
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return cereal |> Guid |> RunId.create
        }
    let setRunId
            (key:workspaceParamsKey) 
            (value:runId)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> RunId.value |> string)



    let getSortableSetCfgType
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! cereal |> SortableSetCfgType.fromString
        }
    let setSortableSetCfgType
            (key:workspaceParamsKey) 
            (sortableSetCfgType:sortableSetCfgType)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (sortableSetCfgType |> string)


    let getSortableSetId
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return cereal |> Guid |> SortableSetId.create
        }
    let setSortableSetId
            (key:workspaceParamsKey) 
            (value:sortableSetId)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> SortableSetId.value |> string)


    let getSorterCount
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> SorterCount.create
        }
    let setSorterCount
            (key:workspaceParamsKey) 
            (value:sorterCount)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> SorterCount.value |> string)


    let getSorterCountOption
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          if cereal = "None" then
             return None
          else return Convert.ToInt32(cereal) |> SorterCount.create |> Some
        }
    let setSorterCountOption
            (key:workspaceParamsKey) 
            (value:sorterCount option)
            (workspaceParams:workspaceParams) 
        =
        match value with
        | Some sc ->
            workspaceParams |> WorkspaceParams.addItem key (sc |> SorterCount.value |> string)
        | None -> 
            workspaceParams |> WorkspaceParams.addItem key "None"


    let getSorterEvalMode
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! Json.deserialize<sorterEvalMode>(cereal)
        }
    let setSorterEvalMode
            (key:workspaceParamsKey) 
            (value:sorterEvalMode)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (Json.serialize(value))


    let getSorterSetPruneMethod
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! cereal |> SorterSetPruneMethod.fromReport
        }
    let setSorterSetPruneMethod
            (key:workspaceParamsKey) 
            (value:sorterSetPruneMethod)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> SorterSetPruneMethod.toReport )


    let getStageCount
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> StageCount.create
        }
    let setStageCount
            (key:workspaceParamsKey) 
            (value:stageCount)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> StageCount.value |> string)


    let getStageWeight
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToDouble(cereal) |> StageWeight.create
        }
    let setStageWeight
            (key:workspaceParamsKey) 
            (value:stageWeight)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> StageWeight.value |> string)


    let getSwitchCount
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToInt32(cereal) |> SwitchCount.create
        }
    let setSwitchCount
            (key:workspaceParamsKey) 
            (value:switchCount)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> SwitchCount.value |> string)


    let getSwitchGenMode
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return! Json.deserialize<switchGenMode>(cereal)
        }
    let setSwitchGenMode
            (key:workspaceParamsKey) 
            (value:switchGenMode)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (Json.serialize(value))


    let getUseParallel
            (key:workspaceParamsKey) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = WorkspaceParams.getItem key workspaceParams
          return Convert.ToBoolean (cereal) |> UseParallel.create
        }
    let setUseParallel
            (key:workspaceParamsKey) 
            (value:useParallel)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> WorkspaceParams.addItem key (value |> UseParallel.value |> string)






