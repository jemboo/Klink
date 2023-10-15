namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
    open CommonParams

[<TestClass>]
type RunScriptFixture () =

    [<TestMethod>]
    member this.shcInitRunCfgDtos () =
        let initGens = 40 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 4}
                            |> generationFilter.ModF

        let maxRunsPerScript = 4

        let sevenEightShcInitRunCfgPlex =
            {
                shcCfgPlex.orders = [|16 |> Order.createNr |]
                mutationRates = [|mr2;mr4|];
                noiseFractions = [|nf3;nf4|];
                rngGens = rndGens 1 4 ;
                tupSorterSetSizes = [|ssz7_8|];
                sorterSetPruneMethods = [|sspm1;|];
                stageWeights = [|sw0;|];
                switchGenModes = [|switchGenMode.StageSymmetric|];
            } |> runCfgPlex.Shc

        let initRunsPrefix = "yabbs"
        let initScriptSet = 
                InitScriptSet.make
                    initRunsPrefix
                    initGens
                    genFilter
                    sevenEightShcInitRunCfgPlex


        let scriptFiles = 
            initScriptSet |> InitScriptSet.createScriptFiles maxRunsPerScript

        scriptFiles |> Array.map(fun (fn, cereal) -> 
                  TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT2t\scripts" |> Some) "toDo" fn cereal
                  
            ) |> ignore


        Assert.AreEqual (1, 1);





    [<TestMethod>]
    member this.shcContinueRunCfgDtos () =
        let newGens = 50 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 1}
                            |> generationFilter.ModF


        let sevenEightShcInitRunCfgPlex =
            {
                shcCfgPlex.orders = [|16 |> Order.createNr |]
                mutationRates = [|mr2;mr4|];
                noiseFractions = [|nf3;nf4|];
                rngGens = rndGens 1 4 ;
                tupSorterSetSizes = [|ssz7_8|];
                sorterSetPruneMethods = [|sspm1;|];
                stageWeights = [|sw0;|];
                switchGenModes = [|switchGenMode.StageSymmetric|];
            } |> runCfgPlex.Shc


        let runSetName = "continueRun"
        let shcRunCfgSet = 
            sevenEightShcInitRunCfgPlex 
               |> RunCfgSet.continueRunFromPlex newGens runSetName

        let cereal = shcRunCfgSet |> RunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" runSetName cereal
        |> ignore

        let yabba = cereal |> RunCfgSetDto.fromJson
                           |> Result.ExtractOrThrow

        Assert.AreEqual (1, 1);



    [<TestMethod>]
    member this.shcReportCfgDtos () =
        let genMin = 2 |> Generation.create
        let genMax = 40000 |> Generation.create
        let wsCompName = "sorterSetEvalParent" |> WsComponentName.create
        let genFilter = { modGenerationFilter.modulus = 1}
                            |> generationFilter.ModF
        let reportFileName = "reportAllReport"
        let scriptFileName = "reportAllScript"
        let runSetName = "reportAllRunset"

        //let shcRunCfgSet = 
        //    [Exp1Cfg.testShcInitRunCfgPlex; Exp1Cfg.testShcInitRunCfgPlex2]
        //       |> RunCfgSet.reportAllFromPlexSeq
        //                    genMin 
        //                    genMax 
        //                    wsCompName 
        //                    genFilter 
        //                    runSetName 
        //                    reportFileName

        //let cereal = shcRunCfgSet |> RunCfgSetDto.toJson

        //TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" scriptFileName cereal
        //|> ignore

        //let yabba = cereal |> RunCfgSetDto.fromJson
        //                   |> Result.ExtractOrThrow

        Assert.AreEqual (1, 1);



    [<TestMethod>]
    member this.shcReportBinCfgDtos () =
        let genMin = 1 |> Generation.create
        let genMax = 80000 |> Generation.create
        let reportFileName = "reportBinsReport"
        let scriptFileName = "reportBinsScript"
        let runSetName = "reportBinsRunset"

        //let shcRunCfgSet = 
        //    [Exp1Cfg.testShcInitRunCfgPlex; Exp1Cfg.testShcInitRunCfgPlex2]
        //       |> RunCfgSet.reportBinsFromPlexSeq
        //                    genMin 
        //                    genMax 
        //                    runSetName 
        //                    reportFileName

        //let cereal = shcRunCfgSet |> RunCfgSetDto.toJson

        //TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT2\scripts" |> Some) "toDo" scriptFileName cereal
        //|> ignore

        //let yabba = cereal |> RunCfgSetDto.fromJson
        //                   |> Result.ExtractOrThrow

        Assert.AreEqual (1, 1);

    //[<TestMethod>]
    //member this.shcRunCfgSetDto () =

    //    let runSetName = "testRun"

    //    let cereal =
    //        TextIO.readAllText "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" runSetName
    //        |> Result.ExtractOrThrow

    //    let yabba = cereal |> ShcRunCfgSetDto.fromJson
    //                       |> Result.ExtractOrThrow


    //    Assert.AreEqual (1, 1);
