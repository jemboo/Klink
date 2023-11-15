namespace global
open System
open System.IO

type cfgPlexName = private CfgPlexName of string
module CfgPlexName =

    let value (CfgPlexName v) = v

    let create (value: string) =
        value |> CfgPlexName
