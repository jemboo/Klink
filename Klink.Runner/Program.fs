open System
open Argu


module Program = 
    let [<EntryPoint>] main _ =

        let parser = ArgumentParser.Create<CliArguments>(programName = "gadget.exe")
        Console.WriteLine(parser.PrintUsage())
        let argResults = parser.Parse [||] // [| "--detach" ; "--listener" ; "localhost" ; "8080" |]

        let all = argResults.GetAllResults()
        let rootDir = argResults.GetResults Working_Directory
        let logLevel = argResults.GetResults Log_Level
        let ears = argResults.GetResults Listener




        let yow = WsOps.makeEm(rootDir |> List.head)
        match yow with
        | Ok msg -> Console.WriteLine($"done ... {msg}")
        | Error yow -> Console.WriteLine($"done ... {yow}")


        Console.Read() |> ignore
        0
