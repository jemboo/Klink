namespace global
open System

module O128_StageCfg =

    open CommonParams
    
    let baseDir = $"c:\Klink"
    let projectFolder  = $"o128\Stage" |> ProjectFolder.create

    let runCfgPlex =
        {
            shcCfgPlexOld.name = "O128_StageCfg" |> CfgPlexName.create
            orders = [| 128 |> Order.createNr |]
            sortableSetCfgs =  
                    [| 
                        (
                            sortableSetCfgType.MergeWithInts, 
                            0 |> StageCount.create,
                            sorterEvalMode.CheckSuccess
                        )
                    |]
            mutationRates = [|mr1; mr2; mr3;|]
            noiseFractions = [|nf1; nf2; nf3|];
            rngGens = rndGens 0 4 ;
            tupSorterSetSizes = [|ssz4_5|];
            sorterSetPruneMethods = [|sspm1;|];
            stageWeights = [|sw0; sw1|];
            switchGenModes = [|switchGenMode.stage|];
            projectFolder = projectFolder
        } |> runCfgPlex.Shc
