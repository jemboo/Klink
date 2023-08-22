namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.shcCfgDto () =

        let yab = Exp1Cfg.defaultInitRunCfg16
                    |> shcRunCfg.Run

        let cereal = yab |> ShcRunCfgDto.toJson

        let yabba = cereal |> ShcRunCfgDto.fromJson
                           |> Result.ExtractOrThrow


        Assert.AreEqual (yab, yabba);
