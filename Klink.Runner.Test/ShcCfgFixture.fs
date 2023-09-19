namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type RunCfgDtos () =

    [<TestMethod>]
    member this.shcInitRunCfgDtos () =
        let newGens = 40 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 4}
                            |> generationFilter.ModF
        let runSetName = "yabbs"
        let shcRunCfgSet = 
            Exp1Cfg.sevenEightShcInitRunCfgPlex 
               |> RunCfgSet.initRunFromPlex newGens genFilter runSetName

        let cereal = shcRunCfgSet |> RunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT2\scripts" |> Some) "toDo" runSetName cereal
        |> ignore


        let runSetName = "yabba"
        let shcRunCfgSet = 
            Exp1Cfg.sevenEightShcInitRunCfgPlex1
               |> RunCfgSet.initRunFromPlex newGens genFilter runSetName

        let cereal = shcRunCfgSet |> RunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT2\scripts" |> Some) "toDo" runSetName cereal
        |> ignore



        let yabba = cereal |> RunCfgSetDto.fromJson
                           |> Result.ExtractOrThrow

        Assert.AreEqual (1, 1);





    [<TestMethod>]
    member this.shcContinueRunCfgDtos () =
        let newGens = 50 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 1}
                            |> generationFilter.ModF
        let runSetName = "continueRun"
        let shcRunCfgSet = 
            Exp1Cfg.testShcInitRunCfgPlex 
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

        let shcRunCfgSet = 
            [Exp1Cfg.testShcInitRunCfgPlex; Exp1Cfg.testShcInitRunCfgPlex2]
               |> RunCfgSet.reportAllFromPlexSeq
                            genMin 
                            genMax 
                            wsCompName 
                            genFilter 
                            runSetName 
                            reportFileName

        let cereal = shcRunCfgSet |> RunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" scriptFileName cereal
        |> ignore

        let yabba = cereal |> RunCfgSetDto.fromJson
                           |> Result.ExtractOrThrow

        Assert.AreEqual (1, 1);



    [<TestMethod>]
    member this.shcReportBinCfgDtos () =
        let genMin = 1 |> Generation.create
        let genMax = 80000 |> Generation.create
        let reportFileName = "reportBinsReport"
        let scriptFileName = "reportBinsScript"
        let runSetName = "reportBinsRunset"

        let shcRunCfgSet = 
            [Exp1Cfg.testShcInitRunCfgPlex; Exp1Cfg.testShcInitRunCfgPlex2]
               |> RunCfgSet.reportBinsFromPlexSeq
                            genMin 
                            genMax 
                            runSetName 
                            reportFileName

        let cereal = shcRunCfgSet |> RunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT2\scripts" |> Some) "toDo" scriptFileName cereal
        |> ignore

        let yabba = cereal |> RunCfgSetDto.fromJson
                           |> Result.ExtractOrThrow

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
