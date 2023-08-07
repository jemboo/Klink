﻿namespace global


type wsFile = 
    | SortableSet
    | SorterSet
    | SorterSetEval
    | SorterEvalReport
    | SorterEvalMergeReport
    | SorterSetConcatMap
    | SorterSetParentMap


module WsFile = 

    let wsRootDir = "c:\\GortFilesoo"
    let fileExt = "txt"


    let getFolder (fileType:wsFile) =
        match fileType with
        | SortableSet -> "SortableSets"
        | SorterSet -> "SorterSets"
        | SorterSetEval -> "SorterSetEvals"
        | SorterEvalReport -> "SorterEvalReports"
        | SorterEvalMergeReport -> "SorterEvalMergeReports"
        | SorterSetConcatMap -> "SorterSetConcatMaps"
        | SorterSetParentMap -> "SorterSetParentMaps"


    let writeToFile (fileType:wsFile) (fileName:string) (data: string) =
        TextIO.writeToFileIfMissing "txt" (Some wsRootDir) (getFolder fileType) fileName data

    let writeLinesIfNew (fileType:wsFile) (fileName:string) (data: string seq) =
        TextIO.writeLinesIfNew "txt" (Some wsRootDir) (getFolder fileType) fileName data

    let appendLines (fileType:wsFile) (fileName:string) (data: string seq) =
        TextIO.appendLines "txt" (Some wsRootDir) (getFolder fileType) fileName data

    let readAllText (fileType:wsFile) (fileName:string) =
        TextIO.readAllText "txt" (Some wsRootDir) (getFolder fileType) fileName

    let readAllLines (fileType:wsFile) (fileName:string) =
        TextIO.readAllLines "txt" (Some wsRootDir) (getFolder fileType) fileName

