namespace global
open System


module Exp1Run =

    let scP0 = 1     |> SorterCount.create
    let scP1 = 2     |> SorterCount.create
    let scP2 = 4     |> SorterCount.create
    let scP3 = 8     |> SorterCount.create
    let scP4 = 16    |> SorterCount.create
    let scP5 = 32    |> SorterCount.create
    let scP6 = 64    |> SorterCount.create
    let scP7 = 128   |> SorterCount.create
    let scP8 = 256   |> SorterCount.create
    let scP9 = 512   |> SorterCount.create
    let scP10 = 1024 |> SorterCount.create


    let scM0 = 1     |> SorterCount.create
    let scM1 = 2     |> SorterCount.create
    let scM2 = 4     |> SorterCount.create
    let scM3 = 8     |> SorterCount.create
    let scM4 = 16    |> SorterCount.create
    let scM5 = 32    |> SorterCount.create
    let scM6 = 64    |> SorterCount.create
    let scM7 = 128   |> SorterCount.create
    let scM8 = 256   |> SorterCount.create
    let scM9 = 512   |> SorterCount.create
    let scM10 = 1024 |> SorterCount.create


    let nf0 = 0.001 |> NoiseFraction.create
    let nf1 = 0.025 |> NoiseFraction.create
    let nf2 = 0.050 |> NoiseFraction.create
    let nf3 = 0.100 |> NoiseFraction.create
    let nf4 = 0.250 |> NoiseFraction.create
    let nf5 = 0.500 |> NoiseFraction.create
    let nf6 = 1.000 |> NoiseFraction.create
    let nf7 = 2.000 |> NoiseFraction.create

        
    let sw0 = 0.05 |> StageWeight.create
    let sw1 = 0.10 |> StageWeight.create
    let sw2 = 0.25 |> StageWeight.create
    let sw3 = 0.50 |> StageWeight.create
    let sw4 = 0.75 |> StageWeight.create
    let sw5 = 1.00 |> StageWeight.create
    let sw6 = 2.00 |> StageWeight.create
    let sw7 = 4.00 |> StageWeight.create


    
    let mr000 =  0.0075 |> MutationRate.create
    let mr00 =   0.0100 |> MutationRate.create
    let mr0 =    0.0125 |> MutationRate.create
    let mr1 =    0.0150 |> MutationRate.create
    let mr2 =    0.0200 |> MutationRate.create
    let mr3 =    0.0250 |> MutationRate.create
    let mr4 =    0.0300 |> MutationRate.create
    let mr5 =    0.0350 |> MutationRate.create
    let mr6 =    0.0400 |> MutationRate.create

    let sspm1 = sorterSetPruneMethod.Whole
    let sspm2 = sorterSetPruneMethod.Shc

    //let sorterSetSizes = [(scP0, scM0); (scP0, scM1); (scP1, scM1); (scP1, scM2)]
    let sorterSetSizes = [(scP9, scM9);]

    let stageWeights = [sw0; sw1; sw2;]

    let noiseFractions = [nf0; nf2; nf3; nf4; ]

    //let mutationRates = [mr1; mr2; mr3; mr4; mr5; mr6;]
    let mutationRates = [mr00; mr00; mr0; mr1]
        
    let sorterSetPruneMethods = [sspm2;] // sspm2]

    let switchGenModes = [switchGenMode.Switch; switchGenMode.Stage; switchGenMode.StageSymmetric]

    let cfgsForTestRun (iterationCt:int) = 
        Exp1CfgOld.enumerate 
                (Exp1CfgOld.rndGens)
                [(scP8, scM8)]
                [switchGenMode.Stage; switchGenMode.Switch; switchGenMode.StageSymmetric]
                [sw0; sw2; sw4; sw6]
                [nf0; nf3;]
                mutationRates
                [sspm2]
                (iterationCt |> Generation.create)
         


    let cfgsForCompleteRun () = 
        Exp1CfgOld.enumerate 
                (Exp1CfgOld.rndGens)
                sorterSetSizes
                switchGenModes
                stageWeights 
                noiseFractions
                mutationRates 
                sorterSetPruneMethods
                (50 |> Generation.create)


    let wnSortableSet = "sortableSet" |> WsComponentName.create
    let wnSorterSetParent = "sorterSetParent" |> WsComponentName.create
    let wnSorterSetMutator = "sorterSetMutator" |> WsComponentName.create
    let wnSorterSetMutated = "sorterSetMutated" |> WsComponentName.create
    let wnSorterSetPruned = "sorterSetPruned" |> WsComponentName.create
    let wnParentMap = "parentMap" |> WsComponentName.create
    let wnSorterSetEvalParent = "sorterSetEvalParent" |> WsComponentName.create
    let wnSorterSetEvalMutated = "sorterSetEvalMutated" |> WsComponentName.create
    let wnSorterSetEvalPruned = "sorterSetEvalPruned" |> WsComponentName.create


    let doRun
            (projectDir:string)
            (wsParamsS: workspaceParams seq)
            (maxGen:generation)
        = 
        result {
            for wsParams in wsParamsS do
                let! runId = wsParams |> WorkspaceParams.getRunId "runId"

                let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
                let fs = new WorkspaceFileStore(runDir)

                let! wsCfg_params, _ = 
                        Exp1WsOps.setupWorkspace
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))

                let mutable curGen = 0
                let mutable curCfg = wsCfg_params |> fst
                let mutable curParams = wsCfg_params |> snd

                while curGen < (maxGen |> Generation.value) do
                    let! wsCfgN, wsPramsN = 
                        Exp1WsOps.doGen
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetMutator
                            wnSorterSetMutated
                            wnSorterSetPruned
                            wnParentMap
                            wnSorterSetEvalParent
                            wnSorterSetEvalMutated
                            wnSorterSetEvalPruned
                            fs
                            (fun s-> Console.WriteLine(s))
                            curParams
                            curCfg

                    curCfg <- wsCfgN
                    curParams <- wsPramsN
                    curGen <- curGen + 1

            return "success"
        }


    let doRunRun
            (projectDir:string)
            (wsParamsS: workspaceParams seq)
            (maxGen:generation)
        = 
        result {
            for wsParams in wsParamsS do

                let! runId = wsParams |> WorkspaceParams.getRunId "runId"
                let runDir = IO.Path.Combine(projectDir, runId |> RunId.value |> string)
                let fs = new WorkspaceFileStore(runDir)

                let! wsCfg_params, ws = 
                        Exp1WsOps.setupWorkspace
                                wnSortableSet
                                wnSorterSetParent
                                wnSorterSetEvalParent
                                wsParams
                                fs
                                (fun s-> Console.WriteLine(s))
                let! cg = wsParams |> WorkspaceParams.getGeneration "generation"
                                   |> Result.map(Generation.value)
                let mutable curGen = cg
                let mutable curParams = wsCfg_params |> snd
                let mutable curWorkspace = ws

                while curGen < (maxGen |> Generation.value) do
                    let! wsN, wsPramsN =
                        Exp1WsOps.doGenOnWorkspace
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetMutator
                            wnSorterSetMutated
                            wnSorterSetPruned
                            wnParentMap
                            wnSorterSetEvalParent
                            wnSorterSetEvalMutated
                            wnSorterSetEvalPruned
                            fs
                            (fun s-> Console.WriteLine(s))
                            curParams
                            curWorkspace

                    curWorkspace <- wsN
                    curParams <- wsPramsN
                    curGen <- curGen + 1

            return "success"
        }


    let continueUpdating
            (projectDir:string)
            (firstFolderIndex:int)
            (folderNum:int)
            (iterationCount:int)
        =
        let runDirs = IO.Directory.EnumerateDirectories(projectDir)
                        |> Seq.toArray
                        |> Array.sort
                        |> Array.skip firstFolderIndex
                        |> Array.take folderNum
        result {
            for runDir in runDirs do
                Console.WriteLine(runDir)

                let fs = new WorkspaceFileStore(runDir)

                let! workspaceId = fs.getLastWorkspaceId
                
                Console.WriteLine($" wsId: {workspaceId}")

                let! wsLoaded = fs.loadWorkSpace workspaceId
                let! paramsLoaded = wsLoaded 
                                    |> Workspace.getComponent ("workspaceParams" |> WsComponentName.create)
                                    |> Result.bind(WorkspaceComponent.asWorkspaceParams)
                let! genLoaded = paramsLoaded |> WorkspaceParams.getGeneration "generation"
                                 |> Result.map(Generation.value)

                let mutable curGen = genLoaded
                let mutable curParams = paramsLoaded
                let mutable curWorkspace = wsLoaded


                let maxGen = curGen + iterationCount
                while curGen < maxGen do
                    let! wsN, wsPramsN = 
                        Exp1WsOps.doGenOnWorkspace
                            wnSortableSet
                            wnSorterSetParent
                            wnSorterSetMutator
                            wnSorterSetMutated
                            wnSorterSetPruned
                            wnParentMap
                            wnSorterSetEvalParent
                            wnSorterSetEvalMutated
                            wnSorterSetEvalPruned
                            fs
                            (fun s-> Console.WriteLine(s))
                            curParams
                            curWorkspace

                    curWorkspace <- wsN
                    curParams <- wsPramsN
                    curGen <- curGen + 1

            return "success"
        }
