namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
    open CommonParams

[<TestClass>]
type KlinkScriptFixture () =

    [<TestMethod>]
    member this.shcInitRunCfgDtos () =
        let maxRunsPerScript = 3

        let klinkScripts = O128_Stage_PhenoPrune.writeInitScripts maxRunsPerScript

        Assert.AreEqual (1, 1);





    [<TestMethod>]
    member this.shcContinueRunCfgDtos () =

        //let klinkScript2 = O16_StageRflCfg.writeReportAllScript (Some (0,24))

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcReportEvalCfgDtos () =

        let klinkScript2 = O128_Stage_PhenoPrune.writeReportEvalsScript (Some (0,32))

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcReportBinCfgDtos () =

        let klinkScript2 = O128_Stage_PhenoPrune.writeReportBinsScript (Some (0,2))

        Assert.AreEqual (1, 1);
