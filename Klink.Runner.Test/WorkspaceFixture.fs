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

        let emptyWsCfg = History.Empty
        let firstWsCfg = emptyWsCfg |> History.addCause cause1
        let secondWsCfg = firstWsCfg |> History.addCause cause2

        let ws = Workspace.empty 
                    |> History.makeWorkspace secondWsCfg.causes (fun s->())
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
        let stageReduction = 1 |> StageCount.create

        let ssCfg1 = sortableSetCertainCfg.All_Bits order
                    |> sortableSetCfg.Certain
        let ssCfg2 = SortableSetCertainCfg.makeAllBitsReducedOneStage  orderN
                    |> sortableSetCfg.Certain
        let ssCfg3 = sortableSetCertainCfg.All_Bits order
                    |> sortableSetCfg.Certain
        let ssCfg4 = SortableSetCertainCfg.makeAllBitsReducedOneStage  orderN
                    |> sortableSetCfg.Certain

        let cause1 =  new causeAddSortableSet(wsCompName1, ssCfg1)
        let cause2 =  new causeAddSortableSet(wsCompName2, ssCfg2)
        let cause3 =  new causeAddSortableSet(wsCompName3, ssCfg3)
        let cause4 =  new causeAddSortableSet(wsCompName4, ssCfg4)


        let emptyWsCfg = History.Empty
        let firstWsCfg = emptyWsCfg |> History.addCause cause1
        let secondWsCfg = firstWsCfg |> History.addCause cause2
        let thirdWsCfg = secondWsCfg |> History.addCause cause3
        let fourthWsCfg = thirdWsCfg |> History.addCause cause4


        let firstWs = Workspace.empty 
                    |> History.makeWorkspace firstWsCfg.causes (fun s->())
                    |> Result.ExtractOrThrow

        Assert.AreEqual(firstWs |> Workspace.getId, firstWsCfg.id);

        let rootDir = "C:\\Pinkster"
        let fileStore = new WorkspaceFileStore(rootDir)


        let res = fileStore.saveWorkSpace firstWs |> Result.ExtractOrThrow
        let fourthWs = fourthWsCfg |> History.runWorkspaceCfg fileStore (fun s->())
                        |> Result.ExtractOrThrow

        let resR = fileStore.saveWorkSpace fourthWs
        let res = resR |> Result.ExtractOrThrow

        Assert.AreEqual(fourthWs |> Workspace.getId, fourthWsCfg.id);
