namespace Klink.DataConvert.Test
open Microsoft.VisualStudio.TestTools.UnitTesting
open System

[<TestClass>]
type workspaceDtoFixture() =

    [<TestMethod>]
    member this.Empty_Workspace_to_workSpaceDto() =
        let fs = new WorkspaceFileStore("c:\\Boinky")
        let wsIn = Workspace.empty
        let wsCereal = wsIn
                        |> Workspace.toWorkspaceDescription
                        |> WorkspaceDescriptionDto.toJson

        let wsBack = wsCereal
                        |> WorkspaceDescriptionDto.fromJson
                        |> Result.bind(Workspace.ofWorkspaceDescription fs.compRetreive)
                        |> Result.ExtractOrThrow

        Assert.AreEqual(wsIn |> Workspace.getId, wsBack|> Workspace.getId)
        Assert.IsTrue(
            CollectionProps.areEqual 
                (wsIn |> Workspace.getWsComponents)
                (wsBack |> Workspace.getWsComponents)
                )


    [<TestMethod>]
    member this.Workspace_to_WorkspaceDto() =
        let wsEmpty = Workspace.empty

        let order = Order.createNr 10
        let ssRecId = Guid.NewGuid() |> SortableSetId.create
        let wcSS = SortableSet.makeAllBits ssRecId rolloutFormat.RfU8 order 
                    |> Result.ExtractOrThrow
                    |> workspaceComponent.SortableSet

        let wsCompName = "sortableSet" |> WsComponentName.create
        let ssWsId = Guid.NewGuid() |> WorkspaceId.create

        let wsIn = wsEmpty 
                   |> Workspace.addComponents ssWsId [(wsCompName, wcSS)]

        let fs = new WorkspaceFileStore("c:\\Boinky")

        let _ = fs.saveWorkSpace wsIn
                |> Result.ExtractOrThrow

        let wsBack = fs.loadWorkSpace ssWsId
                     |> Result.ExtractOrThrow

        Assert.AreEqual(wsIn |> Workspace.getId, wsBack|> Workspace.getId)
        Assert.IsTrue(
            CollectionProps.areEqual 
                (wsIn |> Workspace.getWsComponents |> Map.keys)
                (wsBack |> Workspace.getWsComponents |> Map.keys)
                )