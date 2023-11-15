namespace global
open System

module O64_Stage_PhenoPrune =

    open CommonParams
    
    let baseDir = $"c:\Klink"
    let projectFolder  = $"o64\StagePhenoPrune" |> ProjectFolder.create


    let runCfgPlex (projectFolder:projectFolder)  =
        {
            name = "O64_Stage_PhenoPrune" |> CfgPlexName.create
            shcCfgPlex.orders = [| 64 |> Order.createNr |]
            sortableSetCfgs =  
                    [| 
                        (
                            sortableSetCfgType.MergeWithInts, 
                            0 |> StageCount.create,
                            sorterEvalMode.CheckSuccess
                        )
                    |]
            mutationRates = [|mr2;|];
            noiseFractions = [|nf2;nf3;nf4;|];
            rngGens = rndGens 0 10 ;
            tupSorterSetSizes = [|ssz5_6|];
            sorterSetPruneMethods = 
                [|
                    spc2;
                    spc4;
                    spc6;
                |];
            stageWeights = [|sw0;|];
            switchGenModes = [|switchGenMode.stageSymmetric|];
            projectFolder = projectFolder 
        } |> runCfgPlex.Shc


