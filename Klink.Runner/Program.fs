open System
open Argu


module Program = 

    let [<EntryPoint>] main argv =
        let parser = ArgumentParser.Create<CliArguments>(programName = "Klink.Runner.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse argv
        //let all = argResults.GetAllResults()
        //Console.WriteLine(parser.PrintUsage())

        let workingDirectoryArg = argResults.GetResults Working_Directory |> List.head
        let projectFolderArg = argResults.GetResults Project_Folder |> List.head

        let projectFolder  = projectFolderArg |> ProjectFolder.create
        let projectFolderPath = IO.Path.Combine(workingDirectoryArg, projectFolder |> ProjectFolder.value)

        let (scriptFileName, klinkScript) = 
                ScriptFileRun.getNextKlinkScript projectFolderPath
                |> Result.ExtractOrThrow

        let _makeFileStore (path:string) = 
            new WorkspaceFileStore(path) :> IWorkspaceStore

        let useParallel = true |> UseParallel.create

        Console.WriteLine($"Running script: {scriptFileName}")
        let yow = klinkScript |> KlinkScript.procScript projectFolderPath useParallel _makeFileStore

        let fr = scriptFileName |> ScriptFileRun.finishScript projectFolderPath
        match fr with
        | Ok _ -> Console.WriteLine($"Script finished: {scriptFileName}")
        | Error m -> Console.WriteLine($"Error finishing {scriptFileName} : {m}")


        0
