namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
    open CommonParams

[<TestClass>]
type KlinkScriptFixture () =

    [<TestMethod>]
    member this.shcInitRunCfgDtos () =
        let maxRunsPerScript = 30

        let klinkScripts = O64_Stage_PhenoPrune.writeInitScripts maxRunsPerScript

        Assert.AreEqual (1, 1);





    [<TestMethod>]
    member this.shcContinueRunCfgDtos () =
        let maxRunsPerScript = 1

        let klinkScript2 = O64_Stage_PhenoPrune.writeContinueScripts maxRunsPerScript

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcReportEvalCfgDtos () =

        let klinkScript2 = O64_Stage_PhenoPrune.writeReportEvalsScript (Some (0,60))

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcReportBinCfgDtos () =

        let klinkScript2 = O64_Stage_PhenoPrune.writeReportBinsScript (Some (0, 12))

        Assert.AreEqual (1, 1);
