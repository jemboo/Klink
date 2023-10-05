namespace global
open System


type scriptName = private ScriptName of string
module ScriptName =
    let value (ScriptName v) = v

    let create (value: string) =
        value |> ScriptName


type runScript = 
    private 
        { 
            scriptName:scriptName
            runCfgs:runCfg
        }


type reportScript = 
    private 
        { 
            scriptName:scriptName
            reportCfg:reportCfg
        }


type script =
    | Run of runScript
    | Report of reportScript








type scriptItem = 
     | Run of runCfg
     | Report of reportCfg


module ScriptItem =
    let yab = ()


type klinkScript = 
    private 
        {
            name: scriptName;
            items: scriptItem[]
        }


module KlinkScript =
    let yab = ()