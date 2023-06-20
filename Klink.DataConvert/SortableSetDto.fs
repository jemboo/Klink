﻿namespace global
open System
open Microsoft.FSharp.Core

type sortableSetAllBitsDto =
    { sortableSetRId: Guid
      order: int
      fmt: string }

module SortableSetAllBitsDto =

    let fromDto (dto: sortableSetAllBitsDto) =
        result {
            let! order = dto.order |> Order.create
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            return! SortableSet.makeAllBits (dto.sortableSetRId |> SortableSetId.create) ssFormat order
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sortableSetAllBitsDto> jstr
            return! fromDto dto
        }

    let toDto (sortableSetId: sortableSetId) (rolloutFormat: rolloutFormat) (ord: order) =
        { sortableSetAllBitsDto.sortableSetRId = sortableSetId |> SortableSetId.value
          order = (ord |> Order.value)
          fmt = rolloutFormat |> RolloutFormat.toString }

    let toJson (sortableSetId: sortableSetId) (rolloutFormat: rolloutFormat) (ord: order) =
        ord |> toDto sortableSetId rolloutFormat |> Json.serialize



type sortableSetOrbitDto =
    { sortableSetRId: Guid
      maxCount: int
      permutation: int[]
      fmt: string }

module SortableSetOrbitDto =

    let fromDto (dto: sortableSetOrbitDto) =
        result {
            let maxCt =
                match dto.maxCount with
                | v when v > 0 -> v |> SortableCount.create |> Some
                | _ -> None

            let! perm = dto.permutation |> Permutation.create
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            return! SortableSet.makeOrbits
                        (dto.sortableSetRId |> SortableSetId.create)
                        maxCt
                        perm
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sortableSetOrbitDto> jstr
            return! fromDto dto
        }

    let toDto (sortableSetId: sortableSetId) (rolloutFormt: rolloutFormat) (maxCount: int) (perm: permutation) =
        { sortableSetOrbitDto.sortableSetRId = sortableSetId |> SortableSetId.value
          maxCount = maxCount
          sortableSetOrbitDto.permutation = perm |> Permutation.getArray
          fmt = rolloutFormt |> RolloutFormat.toString }

    let toJson (sortableSetId: sortableSetId) (rolloutFormt: rolloutFormat) (maxCount: int) (perm: permutation) =
        perm |> toDto sortableSetId rolloutFormt maxCount |> Json.serialize



type sortableSetSortedStacksDto =
    { sortableSetRId: Guid
      orderStack: int[]
      fmt: string }

module SortableSetSortedStacksDto =

    let fromDto (dto: sortableSetSortedStacksDto) =
        result {
            let! orderStack = dto.orderStack |> Array.map (Order.create) |> Array.toList |> Result.sequence
            let! ssFormat = dto.fmt |> RolloutFormat.fromString

            return!
                SortableSet.makeSortedStacks
                    (dto.sortableSetRId |> SortableSetId.create)
                    ssFormat
                    (orderStack |> List.toArray)
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<sortableSetSortedStacksDto> jstr
            return! fromDto dto
        }

    let toDto (sortableSetId: sortableSetId) (rolloutFormt: rolloutFormat) (orderStack: order[]) =
        { sortableSetSortedStacksDto.sortableSetRId = sortableSetId |> SortableSetId.value
          orderStack = orderStack |> Array.map (Order.value)
          fmt = rolloutFormt |> RolloutFormat.toString }

    let toJson (sortableSetId: sortableSetId) (rolloutFormt: rolloutFormat) (orderStack: order[]) =
        orderStack |> toDto sortableSetId rolloutFormt |> Json.serialize



type sortableSetRandomPermutationDto =
    { sortableSetRId: Guid
      order: int
      sortableCount: int
      rngGenId: int
      fmt: string }

module SortableSetRandomPermutationDto =

    let fromDto (dto: sortableSetRandomPermutationDto) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! rngGen = rnGenLookup dto.rngGenId
            let randy = Rando.fromRngGen rngGen
            let! order = dto.order |> Order.create
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            let sortableCt = dto.sortableCount |> SortableCount.create

            return!
                SortableSet.makeRandomPermutation
                    order
                    sortableCt
                    randy
                    (dto.sortableSetRId |> SortableSetId.create)
        }

    let fromJson (jstr: string) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! dto = Json.deserialize<sortableSetRandomPermutationDto> jstr
            return! fromDto dto rnGenLookup
        }

    let toDto
        (sortableSetId: sortableSetId)
        (rolloutFormt: rolloutFormat)
        (ord: order)
        (sortableCt: sortableCount)
        (rngId: int)
        =
        { sortableSetRandomPermutationDto.sortableSetRId = sortableSetId |> SortableSetId.value
          sortableSetRandomPermutationDto.order = ord |> Order.value
          sortableSetRandomPermutationDto.rngGenId = rngId
          sortableSetRandomPermutationDto.sortableCount = (sortableCt |> SortableCount.value)
          fmt = rolloutFormt |> RolloutFormat.toString }

    let toJson
        (sortableSetId: sortableSetId)
        (rolloutFormt: rolloutFormat)
        (ord: order)
        (sortableCt: sortableCount)
        (rngId: int)
        =
        rngId |> toDto sortableSetId rolloutFormt ord sortableCt |> Json.serialize



type sortableSetRandomBitsDto =
    { sortableSetRId: Guid
      order: int
      pctOnes: float
      sortableCount: int
      rngGenId: int
      fmt: string }

module SortableSetRandomBitsDto =

    let fromDto (dto: sortableSetRandomBitsDto) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! rngGen = rnGenLookup dto.rngGenId
            let randy = Rando.fromRngGen rngGen
            let! order = dto.order |> Order.create
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            let sortableCt = dto.sortableCount |> SortableCount.create

            return!
                SortableSet.makeRandomBits
                    ssFormat
                    order
                    dto.pctOnes
                    sortableCt
                    randy
                    (dto.sortableSetRId |> SortableSetId.create)
        }

    let fromJson (jstr: string) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! dto = Json.deserialize<sortableSetRandomBitsDto> jstr
            return! fromDto dto rnGenLookup
        }

    let toDto
        (sortableSetId: sortableSetId)
        (rolloutFormt: rolloutFormat)
        (ord: order)
        (pctOnes: float)
        (sortableCt: sortableCount)
        (rngGenId: int)
        =
        { sortableSetRandomBitsDto.sortableSetRId = sortableSetId |> SortableSetId.value
          order = ord |> Order.value
          pctOnes = pctOnes
          sortableCount = sortableCt |> SortableCount.value
          rngGenId = rngGenId
          fmt = rolloutFormt |> RolloutFormat.toString }

    let toJson
        (sortableSetId: sortableSetId)
        (rolloutFormt: rolloutFormat)
        (ord: order)
        (pctOnes: float)
        (sortableCt: sortableCount)
        (rngGenId: int)
        =
        rngGenId
        |> toDto sortableSetId rolloutFormt ord pctOnes sortableCt
        |> Json.serialize



type sortableSetRandomSymbolsDto =
    { sortableSetRId: Guid
      order: int
      symbolSetSize: int
      sortableCount: int
      rngGenId: int
      fmt: string }

module SortableSetRandomSymbolsDto =

    let fromDto (dto: sortableSetRandomSymbolsDto) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! rngGen = rnGenLookup dto.rngGenId
            let randy = Rando.fromRngGen rngGen
            let! order = dto.order |> Order.create
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            let! symbolSetSize = dto.symbolSetSize |> uint64 |> SymbolSetSize.create
            let sortableCt = dto.sortableCount |> SortableCount.create

            return!
                SortableSet.makeRandomSymbols
                    order
                    symbolSetSize
                    sortableCt
                    randy
                    (dto.sortableSetRId |> SortableSetId.create)
        }

    let fromJson (jstr: string) (rnGenLookup: int -> Result<rngGen, string>) =
        result {
            let! dto = Json.deserialize<sortableSetRandomSymbolsDto> jstr
            return! fromDto dto rnGenLookup
        }

    let toDto
        (sortableSetId: sortableSetId)
        (rolloutFormt: rolloutFormat)
        (order: order)
        (symbolSetSz: symbolSetSize)
        (sortableCt: sortableCount)
        (rngGenId: int)  =
        { sortableSetRandomSymbolsDto.sortableSetRId = sortableSetId |> SortableSetId.value
          order = order |> Order.value
          symbolSetSize = symbolSetSz |> SymbolSetSize.value |> int
          sortableCount = sortableCt |> SortableCount.value
          rngGenId = rngGenId
          fmt = rolloutFormt |> RolloutFormat.toString }

    let toJson
        (sortableSetId: sortableSetId)
        (rolloutFormt: rolloutFormat)
        (order: order)
        (symbolSetSz: symbolSetSize)
        (sortableCt: sortableCount)
        (rngGenId: int)  =
        rngGenId
        |> toDto sortableSetId rolloutFormt order symbolSetSz sortableCt
        |> Json.serialize



type sortableSetExplicitTableDto =
    { sortableSetId: Guid
      order: int
      symbolSetSize: int
      bitPackRId: int
      fmt: string }

module SortableSetExplicitTableDto =

    let fromDto (dto: sortableSetExplicitTableDto) (bitPackLookup: int -> Result<bitPack, string>) =
        result {
            let! bitPack = bitPackLookup dto.bitPackRId
            let! order = dto.order |> Order.create
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            let! symbolSetSize = dto.symbolSetSize |> uint64 |> SymbolSetSize.create
            let sortableSetId = dto.sortableSetId |> SortableSetId.create
            return! SortableSet.fromBitPack sortableSetId ssFormat order symbolSetSize bitPack
        }

    let fromJson (jstr: string) (bitPackLookup: int -> Result<bitPack, string>) =
        result {
            let! dto = Json.deserialize<sortableSetExplicitTableDto> jstr
            return! fromDto dto bitPackLookup
        }

    let toDto
        (sortableSetId: sortableSetId)
        (rolloutFormt: rolloutFormat)
        (order: order)
        (symbolSetSz: symbolSetSize)
        (sortableCt: sortableCount)
        (bitPackRId: int)  =
        { sortableSetExplicitTableDto.sortableSetId = sortableSetId |> SortableSetId.value
          order = order |> Order.value
          symbolSetSize = symbolSetSz |> SymbolSetSize.value |> int
          bitPackRId = bitPackRId
          fmt = rolloutFormt |> RolloutFormat.toString }

    let toJson
        (sortableSetId: sortableSetId)
        (rolloutFormt: rolloutFormat)
        (order: order)
        (symbolSetSz: symbolSetSize)
        (sortableCt: sortableCount)
        (rngGenId: int) =
        rngGenId
        |> toDto sortableSetId rolloutFormt order symbolSetSz sortableCt
        |> Json.serialize



type sortableSetSwitchReducedDto =
    { sortableSetRId: Guid
      sorterId: int
      sortableSetSourceId: int
      fmt: string }

module SortableSetSwitchReducedDto =

    let fromDto
        (dto: sortableSetSwitchReducedDto)
        (sortableSetLookup: int -> Result<sortableSet, string>)
        (sorterLookup: Guid -> Result<sorter, string>) =
        result {
            let! sortableSet = sortableSetLookup dto.sortableSetSourceId
            let! sorter = sorterLookup  (Guid.NewGuid()) //dto.sorterId
            let! ssFormat = dto.fmt |> RolloutFormat.fromString
            return! SortableSet.switchReduce 
                        (dto.sortableSetRId |> SortableSetId.create) 
                        sortableSet 
                        sorter
        }

    let fromJson
        (jstr: string)
        (sortableSetLookup: int -> Result<sortableSet, string>)
        (sorterLookup: Guid -> Result<sorter, string>) =
        result {
            let! dto = Json.deserialize<sortableSetSwitchReducedDto> jstr
            return! fromDto dto sortableSetLookup sorterLookup
        }

    let toDto 
            (sortableSetId: sortableSetId) 
            (rolloutFormt: rolloutFormat) 
            (sorterId: int) 
            (sortableSetSourceId: int) 
            =
        { sortableSetSwitchReducedDto.sorterId = sorterId
          fmt = rolloutFormt |> RolloutFormat.toString
          sortableSetRId = sortableSetId |> SortableSetId.value
          sortableSetSourceId = sortableSetSourceId }

    let toJson 
            (sortableSetId: sortableSetId) (rolloutFormt: rolloutFormat) 
            (sorterId: int) (sortableSetSourceId: int) =
        toDto sortableSetId rolloutFormt sorterId sortableSetSourceId |> Json.serialize



type sortableSetDto =
    { sortableSetId: Guid
      rolloutDto: string
      symbolSetSize: int }

module SortableSetDto 
        =
    let fromDto 
            (dto: sortableSetDto)
        =
        result {
            let! rollout = dto.rolloutDto |> RolloutDto.fromJson
            let! symbolSetSize = dto.symbolSetSize |> uint64 |> SymbolSetSize.create
            let sortableSetId = dto.sortableSetId |> SortableSetId.create
            return SortableSet.make sortableSetId symbolSetSize rollout
        }

    let fromJson 
            (jstr: string) 
        =
        result {
            let! dto = Json.deserialize<sortableSetDto> jstr
            return! fromDto dto
        }

    let toDto
            (ss: sortableSet)
        =
        { 
          sortableSetDto.sortableSetId = ss |> SortableSet.getSortableSetId |> SortableSetId.value
          rolloutDto = ss |> SortableSet.getRollout |> RolloutDto.toJson
          symbolSetSize = ss |> SortableSet.getSymbolSetSize |> SymbolSetSize.value |> int
        }

    let toJson
            (ss: sortableSet)
        =
        ss |> toDto |> Json.serialize

