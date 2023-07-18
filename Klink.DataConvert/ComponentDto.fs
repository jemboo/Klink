namespace global
open Microsoft.FSharp.Core
open System

type sparseIntArrayDto = 
    { emptyVal:int; 
      length:int; 
      indexes:int[];
      values:int[] }

module SparseIntArrayDto =

    let fromDto 
            (dto:sparseIntArrayDto) 
        =
        result {
            return 
                SparseArray.create
                    dto.length
                    dto.indexes
                    dto.values
                    dto.emptyVal
        }

    let toDto 
            (sia:sparseArray<int>) 
        =
        { 
            emptyVal = sia |> SparseArray.getEmptyVal; 
            length = sia |> SparseArray.getLength; 
            indexes = sia |> SparseArray.getIndexes;
            values =  sia |> SparseArray.getValues
        }
        
    let fromJson 
            (cereal:string)
        =
        result {
            let! dto = Json.deserialize<sparseIntArrayDto> cereal
            return! fromDto dto
        }

    let toJson 
            (sia:sparseArray<int>) 
        = 
        sia |> toDto |> Json.serialize

type rngGenProviderDto = { id: Guid; rngGenDto: string }

module RngGenProviderDto =

    let fromDto (dto: rngGenProviderDto) =
        result {
            let id = dto.id |> RngGenProviderId.create
            let! rngGen = dto.rngGenDto |> RngGenDto.fromJson
            return RngGenProvider.load id rngGen
        }

    let fromJson (jstr: string) =
        result {
            let! dto = Json.deserialize<rngGenProviderDto> jstr
            return! fromDto dto
        }

    let toDto (rngGen: rngGenProvider) =
        { 
            id = rngGen |> RngGenProvider.getId |> RngGenProviderId.value
            rngGenDto =  rngGen |> RngGenProvider.getFixedRngGen |> RngGenDto.toJson 
        }

    let toJson (rngGen: rngGenProvider) = rngGen |> toDto |> Json.serialize
