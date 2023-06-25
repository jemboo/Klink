namespace global

open System



type causeCfgAddSortableSet 
            (name:wsComponentName,
            ssCfg:sortableSetCfg) 
    = 
    member this.name = name
    member this.updater = 
            fun (w : workspace) -> Workspace.empty |> Ok
    member this.id =   
        [
            name :> obj
            ssCfg :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create
    interface ICauseCfg with
        member this.Id = this.id
        member this.Updater = this.updater



module CauseCfg = 

    ()