namespace global
open System

module O128_Stage_PhenoPrune =

    open CommonParams

    let baseDir = $"c:\Klink"
    let projectFolder  = $"o128\StagePhenoPrune" |> ProjectFolder.create


    let runCfgPlex =
        {
            shcCfgPlex.name = "O128_Stage_PhenoPrune" |> CfgPlexName.create
            orders = [| 128 |> Order.createNr |]
            sortableSetCfgs =  
                    [| 
                        (
                            sortableSetCfgType.MergeWithInts, 
                            0 |> StageCount.create,
                            sorterEvalMode.CheckSuccess
                        )
                    |]
            mutationRates = [|mr2;|];
            noiseFractions = [|nf4;|];
            rngGens = rndGens 0 1 ;
            tupSorterSetSizes = [|ssz5_6|];
            sorterSetPruneMethods = 
                [|
                    spc3;
                    spc6;
                |];
            stageWeights = [|sw0;|];
            switchGenModes = [|switchGenMode.stageSymmetric|];
            projectFolder = projectFolder
        } |> runCfgPlex.Shc
