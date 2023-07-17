namespace global
open System

type workspaceParamsId = private WorkspaceParametersId of Guid
module WorkspaceParamsId =
    let value (WorkspaceParametersId v) = v
    let create vl = WorkspaceParametersId vl

type workspaceParams =
    private 
        { id: workspaceParamsId; 
          data: Map<string,string> }

module WorkspaceParams =

    let load 
            (id:workspaceParamsId) 
            (data: Map<string,string>)
        =
        {
            id = id;
            data = data;
        }

    let make (data: Map<string,string>) =
        let nextId = 
            data |> Map.toArray |> Array.map(fun tup -> tup :> obj)
            |> GuidUtils.guidFromObjs 
            |> WorkspaceParamsId.create

        load nextId data

    let getId (workspaceParams:workspaceParams) =
        workspaceParams.id

    let getData (workspaceParams:workspaceParams) =
        workspaceParams.data

    let addItem (key:string) (cereal:string) (workspaceParams:workspaceParams) =
        let newMap = workspaceParams.data |> Map.add key cereal
        make newMap

    let addItems
            (kvps:(string*string) seq) 
            (jsonDataMap:workspaceParams) =
        Seq.fold (fun wp tup -> addItem (fst tup) (snd tup) wp) jsonDataMap kvps


    let getItem 
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        if workspaceParams.data.ContainsKey(key) then
           workspaceParams.data.[key] |> Ok
        else
            $"the key: {key} was not found" |> Error


    let getGeneration
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return Convert.ToInt32(cereal) |> Generation.create
        }


    let setGeneration
            (key:string) 
            (value:generation)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (value |> Generation.value |> string)


    let getMutationRate
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return Convert.ToDouble(cereal) |> MutationRate.create
        }


    let setMutationRate
            (key:string) 
            (value:mutationRate)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (value |> MutationRate.value |> string)


    let getNoiseFraction
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
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
        workspaceParams |> addItem key cereal


    let getOrder
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return Convert.ToInt32(cereal) |> Order.createNr
        }

    let setOrder
            (key:string) 
            (value:order)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (value |> Order.value |> string)


    let getRngGen
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return! cereal |> RngGenDto.fromJson
        }


    let setRngGen
            (key:string) 
            (value:rngGen)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (value |> RngGenDto.toJson)


    let getSorterCount
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return Convert.ToInt32(cereal) |> SorterCount.create
        }


    let setSorterCount
            (key:string) 
            (value:sorterCount)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (value |> SorterCount.value |> string)


    let getSorterEvalMode
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return! Json.deserialize<sorterEvalMode>(cereal)
        }

    let setSorterEvalMode
            (key:string) 
            (value:sorterEvalMode)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (Json.serialize(value))


    let getStageWeight
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return Convert.ToDouble(cereal) |> StageWeight.create
        }

    let setStageWeight
            (key:string) 
            (value:stageWeight)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (value |> StageWeight.value |> string)


    let getSwitchCount
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return Convert.ToInt32(cereal) |> SwitchCount.create
        }

    let setSwitchCount
            (key:string) 
            (value:switchCount)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (value |> SwitchCount.value |> string)


    let getSwitchGenMode
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return! Json.deserialize<switchGenMode>(cereal)
        }


    let setSwitchGenMode
            (key:string) 
            (value:switchGenMode)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (Json.serialize(value))


    let getUseParallel
            (key:string) 
            (workspaceParams:workspaceParams) 
        =
        result {
          let! cereal = getItem key workspaceParams
          return Convert.ToBoolean (cereal) |> UseParallel.create
        }

    let setUseParallel
            (key:string) 
            (value:useParallel)
            (workspaceParams:workspaceParams) 
        =
        workspaceParams |> addItem key (value |> UseParallel.value |> string)