namespace Klink.Runner.Test

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type KlinkScriptFixture () =

    let baseDir = $"c:\Klink"
    let projectFolder  = $"o64\StagePhenoPrune" |> ProjectFolder.create
    let runCfgPlex = 
        O64_Stage_PhenoPrune.runCfgPlex
            projectFolder


    let freqReportFilter = CommonParams.modulusFilter 10
    let fullReportFilter = CommonParams.modulusFilter 500
    
    let reportGenStart = 0 |> Generation.create
    let initGenerationCount = 10000 |> Generation.create
    let continueGenerationCount = 49500 |> Generation.create
    let totalGenerationCount = Generation.addG initGenerationCount continueGenerationCount
    
    let maxRunsPerScript = 1
    let selectedIndexes = [|0;|] |> Option.Some


    [<TestMethod>]
    member this.shcInitRunCfgDtos () =

        KlinkScript.createInitRunScriptsFromRunCfgPlex 
            initGenerationCount
            freqReportFilter
            fullReportFilter
            maxRunsPerScript
            selectedIndexes
            runCfgPlex
        |> Array.map(ScriptFileMake.writeScript baseDir)
        |> ignore

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcContinueRunCfgDtos () =

        KlinkScript.createContinueRunScriptsFromRunCfgPlex 
            continueGenerationCount
            freqReportFilter
            fullReportFilter
            maxRunsPerScript
            selectedIndexes
            runCfgPlex
        |> Array.map(ScriptFileMake.writeScript baseDir)
        |> ignore

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcReportEvalCfgDtos () =


        let evalScriptComponent = ("sorterSetEvalParent" |> WsComponentName.create)

        KlinkScript.createReportEvalsScriptFromRunCfgPlex 
            reportGenStart
            totalGenerationCount
            evalScriptComponent
            fullReportFilter
            selectedIndexes
            runCfgPlex
        |> ScriptFileMake.writeScript baseDir
        |> ignore

        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.shcReportBinCfgDtos () =
        KlinkScript.createReportBinsScriptFromRunCfgPlex 
            reportGenStart
            totalGenerationCount
            selectedIndexes
            runCfgPlex
        |> ScriptFileMake.writeScript baseDir
        |> ignore
        Assert.AreEqual (1, 1);




    [<TestMethod>]
    member this.writeCfgPlex () =

        let cereal = runCfgPlex |> RunCfgPlexDto.toJson

        TextIO.writeToFileOverwrite 
                "txt" 
                (baseDir |> Some) 
                (projectFolder |> ProjectFolder.value)
                (runCfgPlex |> RunCfgPlex.name |> CfgPlexName.value)
                (cereal)
        |> ignore

        Assert.AreEqual (1, 1);


    [<TestMethod>]
    member this.readCfgPlex () =
        let cereal = 
            TextIO.readAllText 
                    "txt" 
                    (baseDir |> Some) 
                    (projectFolder |> ProjectFolder.value)
                    (runCfgPlex |> RunCfgPlex.name |> CfgPlexName.value)
            |> Result.ExtractOrThrow

        let cfg = cereal 
                    |> RunCfgPlexDto.fromJson
                    |> Result.ExtractOrThrow

        Assert.AreEqual (1, 1);