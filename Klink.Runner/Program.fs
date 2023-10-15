open System
open Argu


module Program = 

    let [<EntryPoint>] main argv =
        
        let projectFolderPath = IO.Path.Combine(O16_StageRflCfg.baseDir, O16_StageRflCfg.projectFolder)

        let (scriptFileName, klinkScript) = 
                ScriptFileRun.getNextKlinkScript projectFolderPath
                |> Result.ExtractOrThrow

        let _makeFileStore (path:string) = 
            new WorkspaceFileStore(path) :> IWorkspaceStore

        let useParallel = true |> UseParallel.create


        let yow = klinkScript |> KlinkScript.procScript projectFolderPath useParallel _makeFileStore

        0
