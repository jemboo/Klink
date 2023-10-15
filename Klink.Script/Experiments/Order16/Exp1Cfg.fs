namespace global
open System

module Exp1Cfg =

    open CommonParams
    let yab = ()


































    ////let testShcInitRunCfgPlex =
    ////    {
    ////        shcCfgPlex.orders = [|16 |> Order.createNr |]
    ////        mutationRates = [|mr0;mr2;mr4|];
    ////        noiseFractions = [|nf0;nf3|];
    ////        rngGens = rndGens 0 3 ;
    ////        tupSorterSetSizes = [|ssz4_5|];
    ////        sorterSetPruneMethods = [|sspm1; sspm2|];
    ////        stageWeights = [|sw0; sw1|];
    ////        switchGenModes = [|switchGenMode.StageSymmetric|];
    ////    } |> runCfgPlex.Shc


    ////let testShcInitRunCfgPlex2 =
    ////    {
    ////        shcCfgPlex.orders = [|16 |> Order.createNr |]
    ////        mutationRates = [|mr0;mr2;mr4|];
    ////        noiseFractions = [|nf0;nf3|];
    ////        rngGens = rndGens 3 3 ;
    ////        tupSorterSetSizes = [|ssz4_5|];
    ////        sorterSetPruneMethods = [|sspm1; sspm2|];
    ////        stageWeights = [|sw2; sw3|];
    ////        switchGenModes = [|switchGenMode.StageSymmetric|];
    ////    } |> runCfgPlex.Shc



    ////let fiveSixShcInitRunCfgPlex =
    ////    {
    ////        shcCfgPlex.orders = [|16 |> Order.createNr |]
    ////        mutationRates = [|mr0;mr2;mr4|];
    ////        noiseFractions = [|nf3;nf4|];
    ////        rngGens = rndGens 0 3 ;
    ////        tupSorterSetSizes = [|ssz5_6|];
    ////        sorterSetPruneMethods = [|sspm1;|];
    ////        stageWeights = [|sw0; sw1|];
    ////        switchGenModes = [|switchGenMode.StageSymmetric|];
    ////    } |> runCfgPlex.Shc


    ////let fiveSixShcInitRunCfgPlex1 =
    ////    {
    ////        shcCfgPlex.orders = [|16 |> Order.createNr |]
    ////        mutationRates = [|mr0;mr2;mr4|];
    ////        noiseFractions = [|nf3;nf4|];
    ////        rngGens = rndGens 3 3 ;
    ////        tupSorterSetSizes = [|ssz5_6|];
    ////        sorterSetPruneMethods = [|sspm1;|];
    ////        stageWeights = [|sw2; sw3|];
    ////        switchGenModes = [|switchGenMode.StageSymmetric|];
    ////    } |> runCfgPlex.Shc


    ////let sevenEightShcInitRunCfgPlex =
    ////    {
    ////        shcCfgPlex.orders = [|16 |> Order.createNr |]
    ////        mutationRates = [|mr2;mr4|];
    ////        noiseFractions = [|nf3;nf4|];
    ////        rngGens = rndGens 1 4 ;
    ////        tupSorterSetSizes = [|ssz7_8|];
    ////        sorterSetPruneMethods = [|sspm1;|];
    ////        stageWeights = [|sw0;|];
    ////        switchGenModes = [|switchGenMode.StageSymmetric|];
    ////    } |> runCfgPlex.Shc

    ////let sevenEightShcInitRunCfgPlex1 =
    ////    {
    ////        shcCfgPlex.orders = [|16 |> Order.createNr |]
    ////        mutationRates = [|mr2;mr4|];
    ////        noiseFractions = [|nf3;nf4|];
    ////        rngGens = rndGens 3 1 ;
    ////        tupSorterSetSizes = [|ssz7_8|];
    ////        sorterSetPruneMethods = [|sspm1;|];
    ////        stageWeights = [|sw1|];
    ////        switchGenModes = [|switchGenMode.StageSymmetric|];
    ////    } |> runCfgPlex.Shc