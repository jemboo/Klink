namespace global

open Argu


type CliArguments =
    | Working_Directory of path:string
    | Project_Folder of string
    | Report_File_Name of string
    | Log_level of int
    | Use_Parallel of bool

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Working_Directory _ -> "specify the working directory."
            | Project_Folder _ -> "a subfolder of the working directory"
            | Report_File_Name _ -> "the report file name"
            | Log_level _ -> "set the log level (0, 1, or 2)"
            | Use_Parallel _ -> "run the sorterEval loop in parallel"

