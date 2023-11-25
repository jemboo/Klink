namespace global
open System
open Microsoft.FSharp.Core
    
type cfgPlexItemDto =
        { 
            name: string
            rank: int
            cfgPlexItemValues: string[][]
        }
    
 module CfgPlexItemDto =
    let toDto (cfgPlexItem:cfgPlexItem) : cfgPlexItemDto =
        {
            name = cfgPlexItem |> CfgPlexItem.getName |> CfgPlexItemName.value
            rank = cfgPlexItem |> CfgPlexItem.getRank |> CfgPlexItemRank.value
            cfgPlexItemValues =
               cfgPlexItem
                    |> CfgPlexItem.getCfgPlexItemValues
                    |> Array.map(fun itm -> itm |> CfgPlexItemValue.toArrayOfStrings)
        }
    let toJson (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem |> toDto |> Json.serialize

    
    let fromDto (cfgPlexItemDto:cfgPlexItemDto) = 
        result {
            let! cfgPlexItemValueList =
                   cfgPlexItemDto.cfgPlexItemValues
                   |> Array.map(CfgPlexItemValue.fromArrayOfStrings)
                   |> Array.toList
                   |> Result.sequence
            
            return CfgPlexItem.create
                        (cfgPlexItemDto.name |> CfgPlexItemName.create)
                        (cfgPlexItemDto.rank |> CfgPlexItemRank.create)
                        (cfgPlexItemValueList |> List.toArray)
        }
       

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgPlexItemDto> cereal
            return! fromDto dto
        }
            
        
            
    
 type cfgPlexDto =
        { 
            name: string
            rngGenDto: rngGenDto
            cfgPlexItemDtos: cfgPlexItemDto[]
        }
    
 module CfgPlexDto =
    let toDto (cfgPlex:cfgPlex) : cfgPlexDto =
        {
            name = cfgPlex |> CfgPlex.getName |> CfgPlexName.value
            rngGenDto = cfgPlex |> CfgPlex.getRngGen |> RngGenDto.toDto
            cfgPlexItemDtos =
               cfgPlex
                    |> CfgPlex.getCfgPlexItems
                    |> Array.map(fun itm -> itm |> CfgPlexItemDto.toDto)
        }
        
    let toJson (cfgPlex:cfgPlex) =
        cfgPlex |> toDto |> Json.serialize

   
    let fromDto (cfgPlexDto:cfgPlexDto) = 
        result {
            let! cfgPlexItems =
                   cfgPlexDto.cfgPlexItemDtos
                   |> Array.map(CfgPlexItemDto.fromDto)
                   |> Array.toList
                   |> Result.sequence
            let! rngGen = cfgPlexDto.rngGenDto |> RngGenDto.fromDto
            return CfgPlex.create
                        (cfgPlexDto.name |> CfgPlexName.create)
                        rngGen
                        (cfgPlexItems |> List.toArray)
        }
   

    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<cfgPlexDto> cereal
            return! fromDto dto
        }
            
        
    
type shcCfgPlexDtoOld =
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


module ShcCfgPlexDtoOld =

    let toDto (plex:shcCfgPlexOld) =
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
            (cfg:shcCfgPlexOld) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:shcCfgPlexDtoOld) =
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
                    shcCfgPlexOld.name = dto.name |> CfgPlexName.create
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
            let! dto = Json.deserialize<shcCfgPlexDtoOld> cereal
            return! fromDto dto
        }


    
type gaCfgPlexDtoOld =
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


module GaCfgPlexDtoOld =

    let toDto (plex:gaCfgPlexOld) =
        let sortableSetCfgTypes = plex.sortableSetCfgs |> Array.map(fun (a,_,_) -> a)
        let stageCounts = plex.sortableSetCfgs |> Array.map(fun (_,b,_) -> b |> StageCount.value)
        let sorterEvalModes = plex.sortableSetCfgs |> Array.map(fun (_,_,c) -> c)

        let sorterCountParents = plex.parentAndChildCounts |> Array.map(fun (a,_) -> a |> SorterCount.value)
        let sorterCountMutants = plex.parentAndChildCounts |> Array.map(fun (_,b) -> b |> SorterCount.value)

        {
            gaCfgPlexDtoOld.name = plex.name |> CfgPlexName.value
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
            (cfg:gaCfgPlexOld) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:gaCfgPlexDtoOld) =
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
                    gaCfgPlexOld.name = dto.name |> CfgPlexName.create
                    orders =  dto.orders |> Array.map(Order.createNr)
                    sortableSetCfgs = sortableSetCfgs
                    mutationRates = dto.mutationRates |> Array.map(MutationRate.create)
                    noiseFractions = dto.noiseFractions |> Array.map(NoiseFraction.create)
                    rngGens = rngGens |> List.toArray
                    parentAndChildCounts = tupSorterSetSizes
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
            let! dto = Json.deserialize<gaCfgPlexDtoOld> cereal
            return! fromDto dto
        }




type runCfgPlexDtoOld = {duType:string; cereal:string}

module RunCfgPlexDtoOld =
    let toDto (cfg:runCfgPlex) =
        match cfg with
        | runCfgPlex.Ga gaCfgPlex -> 
            {
                duType = "Run_ga"
                cereal = gaCfgPlex |> GaCfgPlexDtoOld.toJson
            }

        | runCfgPlex.Shc shcCfgPlex ->
            {
                duType = "Run_shc"
                cereal = shcCfgPlex |> ShcCfgPlexDtoOld.toJson
            }


    let toJson (cfg:runCfgPlex) =
        cfg |> toDto |> Json.serialize


    let fromDto (dto:runCfgPlexDtoOld) =
        match dto.duType with
        | "Run_ga" ->
            result {
                let! gaCfg = dto.cereal |> GaCfgPlexDtoOld.fromJson
                return gaCfg |> runCfgPlex.Ga
            }
        | "Run_shc" -> 
            result {
                let! gaCfg = dto.cereal |> ShcCfgPlexDtoOld.fromJson
                return gaCfg |> runCfgPlex.Shc
            }

        | _ -> $"{dto.duType} not handled in RunCfgPlexDto.fromDto" |> Error


    let fromJson (cereal:string) =
        result {
            let! dto = Json.deserialize<runCfgPlexDtoOld> cereal
            return! fromDto dto
        }


