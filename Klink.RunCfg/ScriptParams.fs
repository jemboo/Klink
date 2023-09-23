namespace global
open System

module ScriptParams =

    let scriptFolder = "scripts"

    let toDoFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, "toDo")

    let runningFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, "running")

    let completedFolder (projectPath:string) =
        IO.Path.Combine(projectPath, scriptFolder, "completed")

    let modulusFilter (modulus:int) = 
            {modGenerationFilter.modulus = modulus } |> generationFilter.ModF

    let expFilter (exper:float) =
            {expGenerationFilter.exp = exper } |> generationFilter.ExpF



