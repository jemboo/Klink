namespace global

open System


type generation = private Generation of int

module Generation =
    let value (Generation v) = v
    let create dex = Generation dex
    let next (Generation v) = Generation (v + 1)
    let add ct (Generation v) = Generation (v + ct)
    let binnedValue (binSz:int) (gen:generation) =
        let gv = gen |> value
        gv - 1 - ((gv - 1) % binSz)



type modGenerationFilter = {modulus:int}

type expGenerationFilter = {exp:float}

type generationFilter =
    | ModF of modGenerationFilter
    | ExpF of expGenerationFilter

module GenerationFilter =
    let passing (filt:generationFilter) (gen:generation) =
        match filt with
        | ModF mgf -> ((gen |> Generation.value) % mgf.modulus) = 0
        | ExpF egf -> IntSeries.expoB egf.exp (gen |> Generation.value)