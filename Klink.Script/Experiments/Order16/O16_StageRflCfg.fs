namespace global
open System

module O16_StageRflCfg =

    open CommonParams
    
    let baseDir = $"c:\Klink"
    let projectFolder  = $"o128\StageRfl" |> ProjectFolder.create


    let runCfgPlex =
        {
            shcCfgPlex.name = "O16_StageRflCfg" |> CfgPlexName.create
            orders = [| 128 |> Order.createNr |]
            sortableSetCfgs =  
                [| 
                    (   sortableSetCfgType.MergeWithInts, 
                        0 |> StageCount.create, 
                        sorterEvalMode.DontCheckSuccess
                    )
                |]
            mutationRates = [|mr0;mr1;mr2|];
            noiseFractions = [|nf0;nf1;nf2;|];
            rngGens = rndGens 0 2 ;
            tupSorterSetSizes = [|ssz5_6|];
            sorterSetPruneMethods = [|sspm1;|];
            stageWeights = [|sw0; sw1|];
            switchGenModes = [|switchGenMode.stageSymmetric|];
            projectFolder = projectFolder
        } |> runCfgPlex.Shc