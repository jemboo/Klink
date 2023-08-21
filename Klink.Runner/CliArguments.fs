﻿namespace global

open Argu


type CliArguments =
    | Working_Directory of path:string
    | Project_Folder of string
    | Report_File_Name of string
    | Procedure of string
    | Starting_Config_Index of int
    | Config_Count of int
    | Iteration_Count of int
    | Log_level of int
    | Use_Parallel of bool

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Working_Directory _ -> "specify the working directory."
            | Project_Folder _ -> "a subfolder of the working directory"
            | Report_File_Name _ -> "the report file name"
            | Procedure _ -> "which procedure to run"
            | Starting_Config_Index _ -> "set the index of the first subfolder of the run folder to process"
            | Config_Count _ -> "set the number of consecutive folders to process"
            | Iteration_Count _ -> "set the number of iterations to do for each folder"
            | Log_level _ -> "set the log level (0, 1, or 2)"
            | Use_Parallel _ -> "run the sorterEval loop in parallel"

