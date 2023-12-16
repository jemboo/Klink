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
                (Path.Combine(baseDir, FolderParams.toDoFolder (klinkScript.projectFolder |> ProjectFolder.value)))
                (klinkScript.scriptName |> ScriptName.value)
                (klinkScript |> KlinkScriptDto.toJson)

    
    let writeInitRunScriptsFromRunCfgPlex
            (baseDir:string)
            (generations:generation)
            (reportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (maxRunCountPerScript:int)
            (selectedIndexes: int[] option)
            (plex:runCfgPlex)
        =
        KlinkScript.createInitRunScriptsFromRunCfgPlex
                generations
                reportFilter
                fullReportFilter
                maxRunCountPerScript
                selectedIndexes
                plex
         |> Array.map(writeScript baseDir)


    
    let writeContinueRunScriptsFromRunCfgPlex
            (baseDir:string)
            (generations:generation)
            (reportFilter:generationFilter)
            (fullReportFilter:generationFilter)
            (maxRunCountPerScript:int)
            (selectedIndexes: int[] option)
            (plex:runCfgPlex)
        =
        KlinkScript.createContinueRunScriptsFromRunCfgPlex
                generations
                reportFilter
                fullReportFilter
                maxRunCountPerScript
                selectedIndexes
                plex
         |> Array.map(writeScript baseDir)