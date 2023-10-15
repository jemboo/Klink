namespace global
open System

module FolderParams =

    let scriptFolder = "scripts"

    let toDoFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, "toDo")

    let runningFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, "running")

    let completedFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, "completed")
