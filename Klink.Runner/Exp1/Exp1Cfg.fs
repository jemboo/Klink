namespace global
open System

module Exp1Cfg =

    open ShcCfgParams

    let testShcInitRunCfgPlex =
    {
        shcInitRunCfgPlex.mutationRates = [|mr0;mr2;mr4|];
        noiseFractions = [|nf0;nf3|];
        rngGens = rndGens 0 2 ;
        tupSorterSetSizes = [|ssz2_3|];
        sorterSetPruneMethods = [|sspm1; sspm2|];
        stageWeights = [|sw0|];
        switchGenModes = [|switchGenMode.Switch|];
    }