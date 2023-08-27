namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.shcCfgInitRunDto () =
        let order = 16 |> Order.createNr
        let newGens = 50 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 1}
                            |> generationFilter.ModF
        let runSetName = "initRun"
        let yab = 
            Exp1Cfg.testShcInitRunCfgPlex 
               |> ShcRunCfgSet.initRunFromPlex order newGens genFilter runSetName

        let cereal = yab |> ShcRunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" runSetName cereal
        |> ignore

        let yabba = cereal |> ShcRunCfgSetDto.fromJson
                           |> Result.ExtractOrThrow

        Assert.AreEqual (1, 1);



    [<TestMethod>]
    member this.shcCfgContinueRunDto () =
        let order = 16 |> Order.createNr
        let newGens = 50 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 1}
                            |> generationFilter.ModF
        let runSetName = "continueRun"
        let yab = 
            Exp1Cfg.testShcInitRunCfgPlex 
               |> ShcRunCfgSet.continueRunFromPlex order newGens genFilter runSetName

        let cereal = yab |> ShcRunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" runSetName cereal
        |> ignore

        let yabba = cereal |> ShcRunCfgSetDto.fromJson
                           |> Result.ExtractOrThrow

        Assert.AreEqual (1, 1);



    [<TestMethod>]
    member this.shcRunCfgSetDto () =

        let runSetName = "testRun"

        let cereal =
            TextIO.readAllText "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" runSetName
            |> Result.ExtractOrThrow

        let yabba = cereal |> ShcRunCfgSetDto.fromJson
                           |> Result.ExtractOrThrow


        Assert.AreEqual (1, 1);
