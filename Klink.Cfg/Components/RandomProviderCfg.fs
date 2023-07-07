namespace global

type randomProviderCfg
            (name:wsComponentName,
             rngGen:rngGen
            ) =
    let _id =         
        [|
          "randomProviderCfg" :> obj;
           rngGen :> obj;
        |] 
        |> GuidUtils.guidFromObjs

    member this.id = _id
    member this.name = name
    member this.rngGen = rngGen
    member this.randy = rngGen |> Rando.fromRngGen

    interface IWorkspaceComponentCfg with
        member this.Id = _id
        member this.WsComponentName = name
        member this.WorkspaceComponentType =
                workspaceComponentType.RandomProvider


module RandomProviderCfg =

    let getNextRngGen
            (cfg:randomProviderCfg) 
        = cfg.randy |> Rando.nextRngGen
        
