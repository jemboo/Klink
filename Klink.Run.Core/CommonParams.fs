namespace global
open System

module CommonParams =

    let modulusFilter (modulus:int) = 
            {modGenerationFilter.modulus = modulus } |> generationFilter.ModF

    let expFilter (exper:float) =
            {expGenerationFilter.exp = exper } |> generationFilter.ExpF


    let rndGens (offset:int) (take:int) = 
        let rngGenSeed = 1234 |> RandomSeed.create |> RngGen.createLcg
        rngGenSeed |> Rando.toMoreRngGens
        |> Seq.skip offset
        |> Seq.take take
        |> Seq.toArray


    let sc0 = 1     |> SorterCount.create
    let sc1 = 2     |> SorterCount.create
    let sc2 = 4     |> SorterCount.create
    let sc3 = 8     |> SorterCount.create
    let sc4 = 16    |> SorterCount.create
    let sc5 = 32    |> SorterCount.create
    let sc6 = 64    |> SorterCount.create
    let sc7 = 128   |> SorterCount.create
    let sc8 = 256   |> SorterCount.create
    let sc9 = 512   |> SorterCount.create
    let sc10 = 1024 |> SorterCount.create
    let sc11 = 2048 |> SorterCount.create
    let sc12 = 4096 |> SorterCount.create

    
    let ssz0_0 =   (sc0, sc0)
    let ssz0_1 =   (sc0, sc1)
    let ssz0_2 =   (sc0, sc2)
    let ssz0_3 =   (sc0, sc3)
    let ssz2_3 =   (sc2, sc3)
    let ssz4_5 =   (sc4, sc5)
    let ssz5_6 =   (sc5, sc6)
    let ssz7_8 =   (sc7, sc8)
    let ssz8_9 =   (sc0, sc0)
    let ssz8_10 =  (sc8, sc10)
    let ssz9_10 =  (sc9, sc10)
    let ssz9_11 =  (sc9, sc11)
    let ssz10_11 = (sc10, sc11)
    let ssz10_12 = (sc10, sc12)
    let ssz11_12 = (sc11, sc12)


    let nf0 = 0.001 |> NoiseFraction.create
    let nf1 = 0.025 |> NoiseFraction.create
    let nf2 = 0.050 |> NoiseFraction.create
    let nf3 = 0.100 |> NoiseFraction.create
    let nf4 = 0.250 |> NoiseFraction.create
    let nf5 = 0.500 |> NoiseFraction.create
    let nf6 = 1.000 |> NoiseFraction.create
    let nf7 = 2.000 |> NoiseFraction.create

        
    let sw0 = 0.05 |> StageWeight.create
    let sw1 = 0.10 |> StageWeight.create
    let sw2 = 0.25 |> StageWeight.create
    let sw3 = 0.50 |> StageWeight.create
    let sw4 = 0.75 |> StageWeight.create
    let sw5 = 1.00 |> StageWeight.create
    let sw6 = 2.00 |> StageWeight.create
    let sw7 = 4.00 |> StageWeight.create

    
    let mr0 =  0.0075 |> MutationRate.create
    let mr1 =   0.0100 |> MutationRate.create
    let mr2 =    0.0125 |> MutationRate.create
    let mr3 =    0.0150 |> MutationRate.create
    let mr4 =    0.0200 |> MutationRate.create
    let mr5 =    0.0250 |> MutationRate.create
    let mr6 =    0.0300 |> MutationRate.create
    let mr7 =    0.0350 |> MutationRate.create
    let mr8 =    0.0400 |> MutationRate.create

    let sspm1 = sorterSetPruneMethod.Whole
    let sspm2 = sorterSetPruneMethod.Shc
