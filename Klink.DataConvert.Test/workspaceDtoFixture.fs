namespace Klink.DataConvert.Test
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type workspaceDtoFixture() =

    [<TestMethod>]
    member this.workSpaceDto() =
        let wsIn = Workspace.empty
        let wsCereal = wsIn |> WorkspaceDto.toJson
        let wsBack = wsCereal 
                        |> WorkspaceDto.fromJson
                        |> Result.ExtractOrThrow

        Assert.AreEqual(wsIn, wsBack)