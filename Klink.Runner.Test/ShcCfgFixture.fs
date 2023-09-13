namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type RunCfgDtos () =

    [<TestMethod>]
    member this.shcInitRunCfgDtos () =
        let newGens = 80000 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 80}
                            |> generationFilter.ModF
        let runSetName = "initRunSevenEight"
        let shcRunCfgSet = 
            Exp1Cfg.sevenEightShcInitRunCfgPlex 
               |> ShcRunCfgSet.initRunFromPlex newGens genFilter runSetName

        let cereal = shcRunCfgSet |> ShcRunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT2\scripts" |> Some) "toDo" runSetName cereal
        |> ignore


        let runSetName = "initRunSevenEight2"
        let shcRunCfgSet = 
            Exp1Cfg.sevenEightShcInitRunCfgPlex1
               |> ShcRunCfgSet.initRunFromPlex newGens genFilter runSetName

        let cereal = shcRunCfgSet |> ShcRunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT2\scripts" |> Some) "toDo" runSetName cereal
        |> ignore



        let yabba = cereal |> ShcRunCfgSetDto.fromJson
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
               |> ShcRunCfgSet.continueRunFromPlex newGens genFilter runSetName

        let cereal = shcRunCfgSet |> ShcRunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" runSetName cereal
        |> ignore

        let yabba = cereal |> ShcRunCfgSetDto.fromJson
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
               |> ShcRunCfgSet.reportAllFromPlexSeq
                            genMin 
                            genMax 
                            wsCompName 
                            genFilter 
                            runSetName 
                            reportFileName

        let cereal = shcRunCfgSet |> ShcRunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" scriptFileName cereal
        |> ignore

        let yabba = cereal |> ShcRunCfgSetDto.fromJson
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
               |> ShcRunCfgSet.reportBinsFromPlexSeq
                            genMin 
                            genMax 
                            runSetName 
                            reportFileName

        let cereal = shcRunCfgSet |> ShcRunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT2\scripts" |> Some) "toDo" scriptFileName cereal
        |> ignore

        let yabba = cereal |> ShcRunCfgSetDto.fromJson
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
