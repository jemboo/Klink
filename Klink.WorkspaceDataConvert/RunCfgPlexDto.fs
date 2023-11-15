namespace global
open Microsoft.FSharp.Core
    
    
type shcCfgPlexDto =
    {
        name:string
        orders:int[]
        sortableSetCfgTypes:sortableSetCfgType[];
        stageCounts: int[];
        sorterEvalModes: sorterEvalMode[]
        mutationRates:float[]
        noiseFractions:float[]
        rngGens:string[]
        sorterCountParents:int[]
        sorterCountMutants:int[]
        sorterSetPruneMethods:string[]
        stageWeights:float[]
        switchGenModes:switchGenMode[]
        projectFolder:string
    }


module ShcCfgPlexDto =

    let toDto (plex:shcCfgPlex) =
        let sortableSetCfgTypes = plex.sortableSetCfgs |> Array.map(fun (a,_,_) -> a)
        let stageCounts = plex.sortableSetCfgs |> Array.map(fun (_,b,_) -> b |> StageCount.value)
        let sorterEvalModes = plex.sortableSetCfgs |> Array.map(fun (_,_,c) -> c)

        let sorterCountParents = plex.tupSorterSetSizes |> Array.map(fun (a,_) -> a |> SorterCount.value)
        let sorterCountMutants = plex.tupSorterSetSizes |> Array.map(fun (_,b) -> b |> SorterCount.value)

        {
            name = plex.name |> CfgPlexName.value
            orders = plex.orders |> Array.map(Order.value)
            sortableSetCfgTypes = sortableSetCfgTypes
            stageCounts = stageCounts
            sorterEvalModes = sorterEvalModes
            mutationRates = plex.mutationRates |> Array.map(MutationRate.value)
            noiseFractions = plex.noiseFractions |> Array.map(NoiseFraction.value)
            rngGens = plex.rngGens |> Array.map(RngGenDto.toJson)
            sorterCountParents = sorterCountParents
            sorterCountMutants = sorterCountMutants
            sorterSetPruneMethods = plex.sorterSetPruneMethods |> Array.map(SorterSetPruneMethodDto.toJson)
            stageWeights = plex.stageWeights |> Array.map(StageWeight.value)
            switchGenModes = plex.switchGenModes
            projectFolder = plex.projectFolder  |> ProjectFolder.value
        }


    let toJson 
            (cfg:shcCfgPlex) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcCfgPlexDto) =
        result {
            //let! fullReportFilter = cfg.fullReportGenFilter |> GenerationFilterDto.fromDto
            let! sorterSetPruneMethods = 
                    dto.sorterSetPruneMethods
                    |> Array.map(SorterSetPruneMethodDto.fromJson)
                    |> Array.toList
                    |> Result.sequence

            let sortableSetCfgs = 
                    Array.zip3
                        dto.sortableSetCfgTypes
                        (dto.stageCounts |> Array.map(StageCount.create))
                        dto.sorterEvalModes
                            
            let! rngGens = 
                    dto.rngGens
                    |> Array.map(RngGenDto.fromJson)
                    |> Array.toList
                    |> Result.sequence

            let tupSorterSetSizes = 
                    Array.zip
                        (dto.sorterCountParents |> Array.map(SorterCount.create))
                        (dto.sorterCountMutants |> Array.map(SorterCount.create))


            return
                {
                    shcCfgPlex.name = dto.name |> CfgPlexName.create
                    orders =  dto.orders |> Array.map(Order.createNr)
                    sortableSetCfgs = sortableSetCfgs
                    mutationRates = dto.mutationRates |> Array.map(MutationRate.create)
                    noiseFractions = dto.noiseFractions |> Array.map(NoiseFraction.create)
                    rngGens = rngGens |> List.toArray
                    tupSorterSetSizes = tupSorterSetSizes
                    sorterSetPruneMethods = sorterSetPruneMethods |> List.toArray
                    stageWeights = dto.stageWeights |> Array.map(StageWeight.create)
                    switchGenModes = dto.switchGenModes
                    projectFolder = dto.projectFolder |> ProjectFolder.create
                }
        }

    let fromJson 
            (cereal:string)
        =
        result {
            let! dto = Json.deserialize<shcCfgPlexDto> cereal
            return! fromDto dto
        }


    
type gaCfgPlexDto =
    {
        name:string
        orders:int[]
        sortableSetCfgTypes:sortableSetCfgType[];
        stageCounts: int[];
        sorterEvalModes: sorterEvalMode[]
        mutationRates:float[]
        noiseFractions:float[]
        rngGens:string[]
        sorterCountParents:int[]
        sorterCountMutants:int[]
        sorterSetPruneMethods:string[]
        stageWeights:float[]
        switchGenModes:switchGenMode[]
        projectFolder:string
    }


module GaCfgPlexDto =

    let toDto (plex:gaCfgPlex) =
        let sortableSetCfgTypes = plex.sortableSetCfgs |> Array.map(fun (a,_,_) -> a)
        let stageCounts = plex.sortableSetCfgs |> Array.map(fun (_,b,_) -> b |> StageCount.value)
        let sorterEvalModes = plex.sortableSetCfgs |> Array.map(fun (_,_,c) -> c)

        let sorterCountParents = plex.tupSorterSetSizes |> Array.map(fun (a,_) -> a |> SorterCount.value)
        let sorterCountMutants = plex.tupSorterSetSizes |> Array.map(fun (_,b) -> b |> SorterCount.value)

        {
            gaCfgPlexDto.name = plex.name |> CfgPlexName.value
            orders = plex.orders |> Array.map(Order.value)
            sortableSetCfgTypes = sortableSetCfgTypes
            stageCounts = stageCounts
            sorterEvalModes = sorterEvalModes
            mutationRates = plex.mutationRates |> Array.map(MutationRate.value)
            noiseFractions = plex.noiseFractions |> Array.map(NoiseFraction.value)
            rngGens = plex.rngGens |> Array.map(RngGenDto.toJson)
            sorterCountParents = sorterCountParents
            sorterCountMutants = sorterCountMutants
            sorterSetPruneMethods = plex.sorterSetPruneMethods |> Array.map(SorterSetPruneMethodDto.toJson)
            stageWeights = plex.stageWeights |> Array.map(StageWeight.value)
            switchGenModes = plex.switchGenModes
            projectFolder = plex.projectFolder  |> ProjectFolder.value
        }


    let toJson 
            (cfg:gaCfgPlex) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:gaCfgPlexDto) =
        result {
            //let! fullReportFilter = cfg.fullReportGenFilter |> GenerationFilterDto.fromDto
            let! sorterSetPruneMethods = 
                    dto.sorterSetPruneMethods
                    |> Array.map(SorterSetPruneMethodDto.fromJson)
                    |> Array.toList
                    |> Result.sequence

            let sortableSetCfgs = 
                    Array.zip3
                        dto.sortableSetCfgTypes
                        (dto.stageCounts |> Array.map(StageCount.create))
                        dto.sorterEvalModes
                            
            let! rngGens = 
                    dto.rngGens
                    |> Array.map(RngGenDto.fromJson)
                    |> Array.toList
                    |> Result.sequence

            let tupSorterSetSizes = 
                    Array.zip
                        (dto.sorterCountParents |> Array.map(SorterCount.create))
                        (dto.sorterCountMutants |> Array.map(SorterCount.create))

            return
                {
                    gaCfgPlex.name = dto.name |> CfgPlexName.create
                    orders =  dto.orders |> Array.map(Order.createNr)
                    sortableSetCfgs = sortableSetCfgs
                    mutationRates = dto.mutationRates |> Array.map(MutationRate.create)
                    noiseFractions = dto.noiseFractions |> Array.map(NoiseFraction.create)
                    rngGens = rngGens |> List.toArray
                    tupSorterSetSizes = tupSorterSetSizes
                    sorterSetPruneMethods = sorterSetPruneMethods |> List.toArray
                    stageWeights = dto.stageWeights |> Array.map(StageWeight.create)
                    switchGenModes = dto.switchGenModes
                    projectFolder = dto.projectFolder |> ProjectFolder.create
                }
        }

    let fromJson 
            (cereal:string)
        =
        result {
            let! dto = Json.deserialize<gaCfgPlexDto> cereal
            return! fromDto dto
        }




type runCfgPlexDto = {duType:string; cereal:string}

module RunCfgPlexDto =
    let toDto (cfg:runCfgPlex) =
        match cfg with
        | runCfgPlex.Ga gaCfgPlex -> 
            {
                duType = "Run_ga"
                cereal = gaCfgPlex |> GaCfgPlexDto.toJson
            }

        | runCfgPlex.Shc shcCfgPlex ->
            {
                duType = "Run_shc"
                cereal = shcCfgPlex |> ShcCfgPlexDto.toJson
            }


    let toJson (cfg:runCfgPlex) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:runCfgPlexDto) =
        match dto.duType with
        | "Run_ga" ->
            result {
                let! gaCfg = dto.cereal |> GaCfgPlexDto.fromJson
                return gaCfg |> runCfgPlex.Ga
            }
        | "Run_shc" -> 
            result {
                let! gaCfg = dto.cereal |> ShcCfgPlexDto.fromJson
                return gaCfg |> runCfgPlex.Shc
            }

        | _ -> $"{dto.duType} not handled in RunCfgPlexDto.fromDto" |> Error


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<runCfgPlexDto> cereal
            return! fromDto dto
        }


