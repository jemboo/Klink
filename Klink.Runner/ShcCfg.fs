namespace global
open System

type shcInitRunCfg =
    {
        newGenerations:generation
        mutationRate:mutationRate
        noiseFraction:noiseFraction
        order:order
        rngGen:rngGen
        sorterEvalMode:sorterEvalMode
        sorterCount:sorterCount
        sorterCountMutated:sorterCount
        sorterSetPruneMethod:sorterSetPruneMethod
        stageWeight:stageWeight
        orderToSwitchCount:orderToSwitchCount
        switchGenMode:switchGenMode
        reportFreqFunc:string
        reportFreqParams:string array
    }


type shcContinueRunCfg =
    {
        runId:runId
        newGenerations:generation
    }


type shcReportCfg =
    {
        runIds:runId array
        genMin:generation
        genMax:generation
        evalCompName:wsComponentName
        reportFreqFunc:string
        reportFreqParams:string array
    }


type shcRunCfg =
    | Run of shcInitRunCfg
    | Continue of shcContinueRunCfg
    | Report of shcReportCfg


type shcRunCfgSet = {setName:string; runs:shcRunCfg[]}



module ShcRunCfg =

    let getRunId (cfg:shcInitRunCfg) =
        [

            cfg.mutationRate |> MutationRate.value :> obj;
            cfg.noiseFraction |> NoiseFraction.value :> obj;
            cfg.order |> Order.value :> obj;
            cfg.rngGen :> obj;
            cfg.sorterEvalMode :> obj
            cfg.sorterCount |> SorterCount.value :> obj;
            cfg.sorterCountMutated |> SorterCount.value :> obj;
            cfg.sorterSetPruneMethod :> obj;
            cfg.stageWeight |> StageWeight.value :> obj;
            cfg.orderToSwitchCount :> obj;
            cfg.switchGenMode :> obj;

        ] |> GuidUtils.guidFromObjs |> RunId.create




type shcInitRunCfgDto =
    {
        newGenerations:int
        mutationRate:float
        noiseFraction:float
        order:int
        rngGen:rngGenDto
        sorterEvalMode:sorterEvalMode
        sorterCount:int
        sorterCountMutated:int
        sorterSetPruneMethod:sorterSetPruneMethod
        stageWeight:float
        orderToSwitchCount:orderToSwitchCount
        switchGenMode:switchGenMode
        reportFreqFunc:string
        reportFreqParams:string array
    }


module ShcInitRunCfgDto =

    let toDto (cfg:shcInitRunCfg) =
        {
            newGenerations = cfg.newGenerations |> Generation.value
            mutationRate = cfg.mutationRate |> MutationRate.value
            noiseFraction = cfg.noiseFraction |> NoiseFraction.value
            order = cfg.order |> Order.value
            rngGen = cfg.rngGen |> RngGenDto.toDto
            sorterEvalMode = cfg.sorterEvalMode
            sorterCount = cfg.sorterCount |> SorterCount.value
            sorterCountMutated = cfg.sorterCountMutated |> SorterCount.value
            sorterSetPruneMethod = cfg.sorterSetPruneMethod
            stageWeight = cfg.stageWeight |> StageWeight.value
            orderToSwitchCount = cfg.orderToSwitchCount
            switchGenMode = cfg.switchGenMode
            reportFreqFunc = cfg.reportFreqFunc
            reportFreqParams = cfg.reportFreqParams
        }


    let toJson 
            (cfg:shcInitRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (cfg:shcInitRunCfgDto) =
        {
            shcInitRunCfg.newGenerations = cfg.newGenerations |> Generation.create
            mutationRate = cfg.mutationRate |> MutationRate.create
            noiseFraction = cfg.noiseFraction |> NoiseFraction.create
            order = cfg.order |> Order.createNr
            rngGen = cfg.rngGen |> RngGenDto.fromDto |> Result.ExtractOrThrow
            sorterEvalMode = cfg.sorterEvalMode
            sorterCount = cfg.sorterCount |> SorterCount.create
            sorterCountMutated = cfg.sorterCountMutated |> SorterCount.create
            sorterSetPruneMethod = cfg.sorterSetPruneMethod
            stageWeight = cfg.stageWeight |> StageWeight.create
            orderToSwitchCount = cfg.orderToSwitchCount
            switchGenMode = cfg.switchGenMode
            reportFreqFunc = cfg.reportFreqFunc
            reportFreqParams = cfg.reportFreqParams
        }


    let fromJson 
            (cereal:string)
        =
        result {
            let! dto = Json.deserialize<shcInitRunCfgDto> cereal
            return fromDto dto
        }


type shcContinueRunCfgDto =
    {
        runId:string
        newGenerations:int
    }

module ShcContinueRunCfgDto =

    let toDto (cfg:shcContinueRunCfg) =
        {
            runId = cfg.runId |> RunId.value |> string
            newGenerations = cfg.newGenerations |> Generation.value

        }

    let toJson (cfg:shcContinueRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (cfg:shcContinueRunCfgDto) =
        {
            shcContinueRunCfg.runId = cfg.runId |> Guid.Parse |> RunId.create
            newGenerations = cfg.newGenerations |> Generation.create
        }


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcContinueRunCfgDto> cereal
            return fromDto dto
        }



type shcReportCfgDto =
    {
        runIds:string array
        genMin:int
        genMax:int
        evalCompName:string
        reportFreqFunc:string
        reportFreqParams:string array
    }


module ShcReportCfgDto =

    let toDto (cfg:shcReportCfg) =
        {
            shcReportCfgDto.runIds = cfg.runIds |> Array.map(RunId.value >> string)
            genMin = cfg.genMin |> Generation.value
            genMax = cfg.genMax |> Generation.value
            evalCompName = cfg.evalCompName |> WsComponentName.value
            reportFreqFunc = cfg.reportFreqFunc
            reportFreqParams = cfg.reportFreqParams
        }

    let toJson (cfg:shcReportCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcReportCfgDto) =
        {
            shcReportCfg.runIds = dto.runIds |> Array.map(fun sid -> Guid.Parse(sid) |> RunId.create)
            genMin = dto.genMin |> Generation.create
            genMax = dto.genMax |> Generation.create
            evalCompName = dto.evalCompName |> WsComponentName.create
            reportFreqFunc = dto.reportFreqFunc
            reportFreqParams = dto.reportFreqParams
        }

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcReportCfgDto> cereal
            return fromDto dto
        }


type shcCfgDto = {duType:string; cereal:string}
module ShcRunCfgDto =

    let toDto (cfg:shcRunCfg) =
        match cfg with
        | Run rCfg -> 
            {
                duType = "Run"
                cereal = rCfg |> ShcInitRunCfgDto.toJson
            }

        | Continue cCfg ->
            {
                duType = "Continue"
                cereal = cCfg |> ShcContinueRunCfgDto.toJson
            }

        | Report cRpt ->
            {
                duType = "Report"
                cereal = cRpt |> ShcReportCfgDto.toJson
            }

    let toJson (cfg:shcRunCfg) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcCfgDto) =
        match dto.duType with
        | "Run" ->
            dto.cereal |> ShcInitRunCfgDto.fromJson |> Result.map(shcRunCfg.Run)
        | "Continue" -> 
            dto.cereal |> ShcContinueRunCfgDto.fromJson |> Result.map(shcRunCfg.Continue)
        | "Report" -> 
            dto.cereal |> ShcReportCfgDto.fromJson |> Result.map(shcRunCfg.Report)
        | _ -> $"{dto.duType} not handled in ShcCfgDto.fromDto" |> Error


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<shcCfgDto> cereal
            return! fromDto dto
        }


type shcRunCfgSetDto = {setName:string; runs:string[]}

module ShcRunCfgSetDto =
    let toDto (runSet:shcRunCfgSet) =
        {
            shcRunCfgSetDto.setName = runSet.setName;
            runs = runSet.runs |> Array.map(ShcRunCfgDto.toJson)
        }

    let fromDto (dto:shcRunCfgSetDto) =
        result {
            let! runs = dto.runs 
                       |> Array.map(ShcRunCfgDto.fromJson)
                       |> Array.toList 
                       |> Result.sequence
            return
                {
                    shcRunCfgSet.setName = dto.setName;
                    runs = runs |> List.toArray
                }
        }