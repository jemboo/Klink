open System
open Argu


module Program = 
    let [<EntryPoint>] main _ =

        let parser = ArgumentParser.Create<CliArguments>(programName = "gadget.exe")
        let usage = parser.PrintUsage()

        let argResults = parser.Parse [||] // [| "--detach" ; "--listener" ; "localhost" ; "8080" |]

        let all = argResults.GetAllResults()
        let rootDir = argResults.GetResults Working_Directory
        let logLevel = argResults.GetResults Log_Level
        let ears = argResults.GetResults Listener

        Console.WriteLine(usage)

        //WsOps.makeEm() |> ignore
        Console.WriteLine("done ...")
        Console.Read() |> ignore
        0
