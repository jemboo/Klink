namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
    open CommonParams

[<TestClass>]
type KlinkScriptFixture () =

    [<TestMethod>]
    member this.shcInitRunCfgDtos () =
        let maxRunsPerScript = 24

        let klinkScripts = O16_StageRflCfg.writeInitScripts maxRunsPerScript

        Assert.AreEqual (1, 1);





    [<TestMethod>]
    member this.shcContinueRunCfgDtos () =

        //let klinkScript2 = O16_StageRflCfg.writeReportAllScript (Some (0,24))

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcReportEvalCfgDtos () =

        let klinkScript2 = O16_StageRflCfg.writeReportEvalsScript (Some (0,24))

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcReportBinCfgDtos () =

        let klinkScript2 = O16_StageRflCfg.writeReportBinsScript (Some (0,24))

        Assert.AreEqual (1, 1);
