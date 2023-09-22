namespace global
open System
open System.IO


type initScriptSet = 
        { setNamePfx:string;
          generations:generation;
          reportFilter:generationFilter
          runCfgPlex:runCfgPlex  }


module InitScriptSet =

    let makeRunConfigSets 
            (initScriptSet:initScriptSet)
            (maxRunsPerScript:int)
        =
         RunCfgSet.initRunsFromPlex
            initScriptSet.generations
            initScriptSet.reportFilter
            initScriptSet.setNamePfx
            maxRunsPerScript
            initScriptSet.runCfgPlex



type continueScriptSet = 
        { 
          setName:string; 
          runCfgPlex:runCfgPlex  
          generationsToAdd:generation
        }


module ContinueScriptSet =

    let procRunCfg 
            (continueScriptSet:continueScriptSet)

        =
           RunCfgSet.continueRunFromPlex
                continueScriptSet.generationsToAdd
                continueScriptSet.setName
                continueScriptSet.runCfgPlex