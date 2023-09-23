namespace global
open System
open System.IO


type initScriptSet = 
        { setNamePfx:string;
          generations:generation;
          reportFilter:generationFilter
          runCfgPlex:runCfgPlex  }


module InitScriptSet =

    let make 
            (setNamePfx:string) 
            (generations:generation) 
            (reportFilter:generationFilter) 
            (runCfgPlex:runCfgPlex ) 
        =
            { setNamePfx = setNamePfx;
              generations = generations;
              reportFilter = reportFilter
              runCfgPlex = runCfgPlex  }


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

   
    let createScriptFiles 
            (maxRunsPerScript:int)
            (initScriptSet:initScriptSet)
        =
         makeRunConfigSets initScriptSet maxRunsPerScript
         |> Array.map(fun dto ->(dto.setName, dto |> RunCfgSetDto.toJson))




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

   
    let createScriptFile
            (continueScriptSet:continueScriptSet)
        =
         let yab  = procRunCfg continueScriptSet
         (yab.setName, yab |> RunCfgSetDto.toJson)
