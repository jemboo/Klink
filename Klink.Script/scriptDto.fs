namespace global
open System


type runScriptDto = 
    private 
        { 
            scriptName:scriptName
            runCfgDtos:runCfgDto
        }


type reportScriptDto = 
    private 
        { 
            scriptName:scriptName
           // reportCfgDto:reportCfgDto
        }


type scriptDto =
    | Run of runScriptDto
    | Report of reportScriptDto

