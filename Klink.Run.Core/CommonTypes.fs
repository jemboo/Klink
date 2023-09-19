namespace global
open System



type scriptName = private ScriptName of string
module ScriptName =
    let value (ScriptName v) = v

    let create (value: string) =
        value |> ScriptName

