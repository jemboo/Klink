namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
    open CommonParams

[<TestClass>]
type KlinkScriptFixture () =

    [<TestMethod>]
    member this.shcInitRunCfgDtos () =
        //let maxRunsPerScript = 24

        //let initScriptSet = StageSymmetricCfg.initScriptSet_nf
        //let scriptFiles = 
        //        InitScriptSet.createScriptFiles 
        //            maxRunsPerScript
        //            initScriptSet

        //scriptFiles |> Array.iter(
        //    fun (fn, fc) ->
        //        TextIO.writeToFileOverwrite 
        //                "txt" 
        //                ($"c:\Klink\Test\scripts" |> Some) 
        //                "toDo" 
        //                fn 
        //                fc
        //        |> ignore
        //     )

        Assert.AreEqual (1, 1);





    [<TestMethod>]
    member this.shcContinueRunCfgDtos () =
        let baseDir = $"c:\Klink"
        let maxRunsPerScript = 24

        let klinkScripts = O16_StageRflCfg.writeInitScripts maxRunsPerScript

        Assert.AreEqual (1, 1);
