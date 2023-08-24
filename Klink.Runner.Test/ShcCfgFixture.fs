namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.shcCfgDto () =
        let order = 16 |> Order.createNr
        let newGens = 50 |> Generation.create
        let genFilter = { modGenerationFilter.modulus = 1}
                            |> generationFilter.ModF
        let runSetName = "testRun"
        let yab = 
            Exp1Cfg.testShcInitRunCfgPlex 
               |> ShcRunCfgSet.fromPlex order newGens genFilter runSetName

        let cereal = yab |> ShcRunCfgSetDto.toJson

        TextIO.writeToFileOverwrite "txt" ($"c:\Klink\ShcT\scripts" |> Some) "toDo" runSetName cereal


        //let cereal = yab |> ShcRunCfgDto.toJson

        let yabba = cereal |> ShcRunCfgSetDto.fromJson
                           |> Result.ExtractOrThrow


        Assert.AreEqual (1, 1);
