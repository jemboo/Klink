﻿namespace global
open System
open System.IO
open System.Threading

module ScriptFileRun =

    let getNextScript (projectFolderPath:string) =
            use mutex = new Mutex(false, "FileMoveMutex")
            let mutable fileNameAndContents = ("", "") |> Ok
            if mutex.WaitOne() then
                  try
                        let scriptToDoFolder = FolderParams.toDoFolder projectFolderPath
                        let scriptRunningFolder = FolderParams.runningFolder projectFolderPath
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


    let getNextKlinkScript (projectFolderPath:string) =
        result {
            let! (filePath, contents) = 
                    getNextScript projectFolderPath
            
            let! cfgSet = contents |> KlinkScriptDto.fromJson

            return (filePath, cfgSet)
        }


    let finishScript (projectFolderPath:string) (scriptFileName:string)  =
        let scriptRunningFolder = FolderParams.runningFolder projectFolderPath
        let scriptRunningPath = Path.Combine(scriptRunningFolder, scriptFileName)
        let scriptCompletedFolder = FolderParams.completedFolder projectFolderPath
        let scriptCompletedPath = Path.Combine(scriptCompletedFolder, scriptFileName)
        try
            Directory.CreateDirectory(scriptCompletedFolder) |> ignore
            File.Move(scriptRunningPath, scriptCompletedPath)
            () |> Ok

        with ex ->
            $"error in finishScript: { ex.Message}" |> Result.Error
            

