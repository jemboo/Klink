namespace global
open System
open System.IO
open System.Threading

module ScriptFileMake =

    let writeScript 
            (baseDir:string)
            (klinkScript:klinkScript) 
        =
        TextIO.writeToFileOverwrite 
                "txt" 
                (baseDir |> Some) 
                (FolderParams.toDoFolder (klinkScript.projectFolder |> ProjectFolder.value))
                (klinkScript.scriptName |> ScriptName.value)
                (klinkScript |> KlinkScriptDto.toJson)


            

