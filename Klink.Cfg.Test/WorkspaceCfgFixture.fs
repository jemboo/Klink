namespace Klink.Cfg.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type WorkspaceCfgFixture () =

    [<TestMethod>]
    member this.AddAndRemoveWorkspaceCfg () =
        let order = 16 |> Order.createNr
        let wsCompName1 = "test1" |> WsComponentName.create
        let wsCompName2 = "test2" |> WsComponentName.create
        let ssCfg = sortableSetCertainCfg.All_Bits order
                    |> sortableSetCfg.Certain
        let cause1 =  new causeAddSortableSet(wsCompName1, ssCfg)
        let cause2 =  new causeAddSortableSet(wsCompName2, ssCfg)

        let emptyWsCfg = WorkspaceCfg.Empty
        let firstWsCfg = emptyWsCfg |> WorkspaceCfg.addCauseCfg cause1
        let secondWsCfg = firstWsCfg |> WorkspaceCfg.addCauseCfg cause2
        let reFirstWsCfg, future = secondWsCfg |> WorkspaceCfg.removeLastCauseCfg
        let emptied, futureNo = firstWsCfg |> WorkspaceCfg.removeLastCauseCfg

        Assert.AreEqual(firstWsCfg.id, reFirstWsCfg.id);
        Assert.AreEqual(emptyWsCfg.id, emptied.id);



    [<TestMethod>]
    member this.makeWorkspace () =
        let order = 16 |> Order.createNr
        let wsCompName1 = "test1" |> WsComponentName.create
        let wsCompName2 = "test2" |> WsComponentName.create
        let ssCfg = sortableSetCertainCfg.All_Bits order
                    |> sortableSetCfg.Certain
        let cause1 =  new causeAddSortableSet(wsCompName1, ssCfg)
        let cause2 =  new causeAddSortableSet(wsCompName2, ssCfg)

        let emptyWsCfg = WorkspaceCfg.Empty
        let firstWsCfg = emptyWsCfg |> WorkspaceCfg.addCauseCfg cause1
        let secondWsCfg = firstWsCfg |> WorkspaceCfg.addCauseCfg cause2

        let ws = Workspace.empty 
                    |> WorkspaceCfg.makeWorkspace secondWsCfg.history
                    |> Result.ExtractOrThrow

        Assert.AreEqual(ws |> Workspace.getId, secondWsCfg.id);
