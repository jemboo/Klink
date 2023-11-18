namespace global
open System

module O128_StageRflCfg =

    open CommonParams
    let baseDir = $"c:\Klink"
    let projectFolder  = $"o128\StageRfl" |> ProjectFolder.create


    let runCfgPlex =
        {
            shcCfgPlexOld.name = "O128_StageRflCfg" |> CfgPlexName.create
            shcCfgPlexOld.orders = [| 128 |> Order.createNr |]
            sortableSetCfgs =  
                    [| 
                        (
                            sortableSetCfgType.MergeWithInts, 
                            0 |> StageCount.create,
                            sorterEvalMode.CheckSuccess
                        )
                    |]
            mutationRates = [|mr1;|];
            noiseFractions = [|nf0;nf1;nf2;nf3|];
            rngGens = rndGens 0 4 ;
            tupSorterSetSizes = [|ssz5_6|];
            sorterSetPruneMethods = [|sspm1;|];
            stageWeights = [|sw0; sw1|];
            switchGenModes = [|switchGenMode.stageSymmetric|];
            projectFolder = projectFolder
        } |> runCfgPlex.Shc
