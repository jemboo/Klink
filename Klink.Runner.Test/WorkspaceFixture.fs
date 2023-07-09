namespace Klink.Runner.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type WorkspaceFixture () =

    [<TestMethod>]
    member this.makeAndSaveWorkspace () =
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

        let rootDir = "C:\\Klinkster"
        let fs = new WorkspaceFileStore(rootDir)
        let res = fs.saveWorkSpace ws |> Result.ExtractOrThrow

        Assert.AreEqual(ws |> Workspace.getId, secondWsCfg.id);



    [<TestMethod>]
    member this.updateWorkspace () =
        let order = 16 |> Order.createNr
        let orderN = 18 |> Order.createNr
        let wsCompName1 = "test1" |> WsComponentName.create
        let wsCompName2 = "test2" |> WsComponentName.create
        let wsCompName3 = "test3" |> WsComponentName.create
        let wsCompName4 = "test4" |> WsComponentName.create

        let ssCfg1 = sortableSetCertainCfg.All_Bits order
                    |> sortableSetCfg.Certain
        let ssCfg2 = SortableSetCertainCfg.makeAllBitsReducedOneStage orderN
                    |> sortableSetCfg.Certain
        let ssCfg3 = sortableSetCertainCfg.All_Bits order
                    |> sortableSetCfg.Certain
        let ssCfg4 = SortableSetCertainCfg.makeAllBitsReducedOneStage orderN
                    |> sortableSetCfg.Certain

        let cause1 =  new causeAddSortableSet(wsCompName1, ssCfg1)
        let cause2 =  new causeAddSortableSet(wsCompName2, ssCfg2)
        let cause3 =  new causeAddSortableSet(wsCompName3, ssCfg3)
        let cause4 =  new causeAddSortableSet(wsCompName4, ssCfg4)


        let emptyWsCfg = WorkspaceCfg.Empty
        let firstWsCfg = emptyWsCfg |> WorkspaceCfg.addCauseCfg cause1
        let secondWsCfg = firstWsCfg |> WorkspaceCfg.addCauseCfg cause2
        let thirdWsCfg = secondWsCfg |> WorkspaceCfg.addCauseCfg cause3
        let fourthWsCfg = thirdWsCfg |> WorkspaceCfg.addCauseCfg cause4


        let firstWs = Workspace.empty 
                    |> WorkspaceCfg.makeWorkspace firstWsCfg.history
                    |> Result.ExtractOrThrow

        Assert.AreEqual(firstWs |> Workspace.getId, firstWsCfg.id);

        let rootDir = "C:\\Pinkster"
        let fileStore = new WorkspaceFileStore(rootDir)


        let res = fileStore.saveWorkSpace firstWs |> Result.ExtractOrThrow
        let fourthWs =  WorkspaceCfg.updateWorkspace fourthWsCfg fileStore
                        |> Result.ExtractOrThrow

        let resR = fileStore.saveWorkSpace fourthWs
        let res = resR |> Result.ExtractOrThrow

        Assert.AreEqual(fourthWs |> Workspace.getId, fourthWsCfg.id);
