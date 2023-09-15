namespace Klink.History.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type HistoryFixture () =

    [<TestMethod>]
    member this.AddAndRemoveWorkspaceCfg () =
        let order = 16 |> Order.createNr
        let wsCompName1 = "test1" |> WsComponentName.create
        let wsCompName2 = "test2" |> WsComponentName.create
        let ssCfg = sortableSetCertainCfg.All_Bits order
                    |> sortableSetCfg.Certain
        let cause1 =  new causeAddSortableSet(wsCompName1, ssCfg)
        let cause2 =  new causeAddSortableSet(wsCompName2, ssCfg)

        let emptyWsCfg = History.Empty
        let firstWsCfg = emptyWsCfg |> History.addCause cause1
        let secondWsCfg = firstWsCfg |> History.addCause cause2
        let reFirstWsCfg, future = secondWsCfg |> History.removeLastCause
        let emptied, futureNo = firstWsCfg |> History.removeLastCause

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

        let emptyWsCfg = History.Empty
        let firstWsCfg = emptyWsCfg |> History.addCause cause1
        let secondWsCfg = firstWsCfg |> History.addCause cause2

        let ws = Workspace.empty 
                    |> History.makeWorkspace secondWsCfg.causes (fun s-> ())
                    |> Result.ExtractOrThrow

        Assert.AreEqual(ws |> Workspace.getId, secondWsCfg.id);
