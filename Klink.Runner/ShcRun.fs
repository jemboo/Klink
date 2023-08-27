﻿namespace global
open System
open System.IO
open System.Threading

module ShcRun =

    let scriptFolder = "scripts"
    let toDoFolder = "toDo"
    let runningFolder = "running"
    let completedFolder = "completed"

    let scriptToDoFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, toDoFolder)

    let scriptRunningFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, runningFolder)

    let scriptCompletedFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, completedFolder)


    let getNextScript (projectFolderPath:string) =
            use mutex = new Mutex(false, "FileMoveMutex")
            let mutable fileNameAndContents = ("", "") |> Ok
            if mutex.WaitOne() then
                  try
                        let scriptToDoFolder = scriptToDoFolder projectFolderPath
                        let scriptRunningFolder = scriptRunningFolder projectFolderPath
                        Directory.CreateDirectory(scriptRunningFolder) |> ignore
                        let currentScriptToDoPath = 
                            IO.Directory.EnumerateFiles scriptToDoFolder
                                |> Seq.head
                        let currentScriptFileName = Path.GetFileName(currentScriptToDoPath)
                        let currentScriptRunningPath = Path.Combine(scriptRunningFolder, currentScriptFileName)
                        File.Move(currentScriptToDoPath, currentScriptRunningPath)
                        fileNameAndContents <- (currentScriptFileName, (File.ReadAllText currentScriptRunningPath)) |> Ok
                        mutex.ReleaseMutex()

                  with ex ->
                         fileNameAndContents <- $"error in getNextScript: { ex.Message }" |> Result.Error
            fileNameAndContents



    let getNextRunCfgSet (projectFolderPath:string) =
        result {
            let! (filePath, contents) = 
                    getNextScript projectFolderPath
            
            let! cfgSet = contents |> ShcRunCfgSetDto.fromJson

            return (filePath, cfgSet)
        }


    let finishScript (scriptFileName:string) (projectFolderPath:string) =
        let scriptRunningFolder = scriptRunningFolder projectFolderPath
        let scriptRunningPath = Path.Combine(scriptRunningFolder, scriptFileName)
        let scriptCompletedFolder = scriptCompletedFolder projectFolderPath
        let scriptCompletedPath = Path.Combine(scriptCompletedFolder, scriptFileName)
        try
            Directory.CreateDirectory(scriptCompletedFolder) |> ignore
            File.Move(scriptRunningPath, scriptCompletedPath)
            () |> Ok

        with ex ->
            $"error in finishScript: { ex.Message}" |> Result.Error
            

