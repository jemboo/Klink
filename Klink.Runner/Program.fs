open System
open Argu


module Program = 

    let [<EntryPoint>] main argv =
        
        let projectFolderPath = IO.Path.Combine(O64_Stage_PhenoPrune.baseDir, O64_Stage_PhenoPrune.projectFolder |> ProjectFolder.value)

        let (scriptFileName, klinkScript) = 
                ScriptFileRun.getNextKlinkScript projectFolderPath
                |> Result.ExtractOrThrow

        let _makeFileStore (path:string) = 
            new WorkspaceFileStore(path) :> IWorkspaceStore

        let useParallel = true |> UseParallel.create


        let yow = klinkScript |> KlinkScript.procScript projectFolderPath useParallel _makeFileStore

        let fr = scriptFileName |> ScriptFileRun.finishScript projectFolderPath
        match fr with
        | Ok _ -> Console.WriteLine($"Script finished: {scriptFileName}")
        | Error m -> Console.WriteLine($"Error finishing {scriptFileName} : {m}")


        0
