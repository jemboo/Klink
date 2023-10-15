namespace global
open System
open System.IO
open System.Threading

module ScriptFileMake =

    let writeScript 
            (baseDir:string) 
            (projectPath:string) 
            (klinkScript:klinkScript) 
        =
        TextIO.writeToFileOverwrite 
                    "txt" 
                    (baseDir |> Some) 
                    (FolderParams.toDoFolder projectPath)
                    (klinkScript.name |> ScriptName.value)
                    (klinkScript |> KlinkScriptDto.toJson)


            

