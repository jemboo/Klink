namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open CommonParams

[<TestClass>]
type ShcCfgFixture () =


    [<TestMethod>]
    member this.shcContinueRunCfgDtos () =
        let newGens = 50 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 1}
                            |> generationFilter.ModF
        let runSetName = "continueRun"

        let testShcInitRunCfgPlex =
            {
                shcCfgPlex.orders = [|16 |> Order.createNr |]
                mutationRates = [|mr0;mr2;mr4|];
                noiseFractions = [|nf0;nf3|];
                rngGens = rndGens 0 3 ;
                tupSorterSetSizes = [|ssz4_5|];
                sorterSetPruneMethods = [|sspm1; sspm2|];
                stageWeights = [|sw0; sw1|];
                switchGenModes = [|switchGenMode.stageSymmetric|];
            } |> runCfgPlex.Shc

        //let seqSplicer = None

        //let shcRunCfgSet = 
        //    testShcInitRunCfgPlex 
        //       |> RunCfgSet.continueRunFromPlex newGens runSetName seqSplicer

        //let cereal = shcRunCfgSet |> RunCfgSetDto.toJson

        //TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" runSetName cereal
        //|> ignore

        //let yabba = cereal |> RunCfgSetDto.fromJson
        //                   |> Result.ExtractOrThrow

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
