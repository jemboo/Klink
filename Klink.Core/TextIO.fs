namespace global

open System.IO
open System

module TextIO =

    let readAllLines 
            (ext:string)
            (folder:string) 
            (fileName:string)
        =
        try
            let fne = sprintf "%s.%s" fileName ext
            let fp = Path.Combine(folder, fne)
            if File.Exists(fp) then
                File.ReadAllLines fp |> Ok
            else
                sprintf "not found (401): %s" fp |> Error
        with ex ->
            ("error in TextIO.readAllLines: " + ex.Message) |> Result.Error


    let fileExists
            (ext:string) 
            (folder:string) 
            (fileName:string)
        =
        try
            let fne = sprintf "%s.%s" fileName ext
            let fp = Path.Combine(folder, fne)
            File.Exists(fp) |> Ok
        with ex ->
            ("error in TextIO.fileExists: " + ex.Message) |> Result.Error


    let readAllText
            (ext:string)
            (folder:string) 
            (fileName:string)
        =
        try
            let fne = sprintf "%s.%s" fileName ext
            let fp = Path.Combine(folder, fne)
            if File.Exists(fp) then
                File.ReadAllText fp |> Ok
            else
                sprintf "not found (402): %s" fp |> Error
        with ex ->
            ("error in TextIO.readAllText: " + ex.Message) |> Result.Error


    let appendLines
            (ext:string)
            (folder:string) 
            (file:string)
            (data: seq<string>)
        =
        try
            let fne = sprintf "%s.%s" file ext
            let fp = Path.Combine(folder, fne)
            Directory.CreateDirectory(folder) |> ignore
            File.AppendAllLines(fp, data)
            true |> Ok
        with ex ->
            ("error in TextIO.appendLines: " + ex.Message) |> Result.Error


    let writeLinesEnsureHeader
            (ext:string)
            (folder:string) 
            (file:string)
            (hdr: seq<string>)
            (data: seq<string>)
        =
        try
            let fne = sprintf "%s.%s" file ext
            let fp = Path.Combine(folder, fne)
            Directory.CreateDirectory(folder) |> ignore
            if File.Exists(fp) then
                File.AppendAllLines(fp, data)
                true |> Ok
            else
                File.AppendAllLines(fp, hdr)
                File.AppendAllLines(fp, data)
                true |> Ok
        with ex ->
            ("error in TextIO.writeLinesIfNew: " + ex.Message) |> Result.Error


    let writeToFileIfMissing
            (ext:string)
            (folder:string) 
            (file:string)
            (data:string)
        =
        try
            let fne = sprintf "%s.%s" file ext
            let fp = Path.Combine(folder, fne)
            Directory.CreateDirectory(folder) |> ignore
            if File.Exists(fp) then
                true |> Ok
            else
                File.WriteAllText(fp, data)
                true |> Ok
        with ex ->
            ("error in TextIO.writeToFile: " + ex.Message) |> Result.Error


    let writeToFileOverwrite
            (ext:string)
            (folder:string) 
            (file:string)
            (data:string)
        =
        try
            let fne = sprintf "%s.%s" file ext
            let fp = Path.Combine(folder, fne)
            Directory.CreateDirectory(folder) |> ignore
            File.WriteAllText(fp, data)
            () |> Ok
        with ex ->
            ("error in TextIO.writeToFile: " + ex.Message) |> Result.Error

