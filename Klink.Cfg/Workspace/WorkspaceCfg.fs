namespace global

open System


//type IWorkspaceCfg = 
//    abstract member Id:Guid
//    abstract member History:ICauseCfg list
//and ICauseCfg = 
//    abstract member Id:Guid
//    abstract member Updater:IWorkspaceCfg->IWorkspaceCfg


//type workspaceCfg(hist:ICauseCfg list) =
//    member this.hist = hist
//    member this.id =   
//        hist |> Seq.map(fun h -> h:>obj)
//             |> GuidUtils.guidFromObjs
//             |> WorkspaceId.create
//    interface IWorkspaceCfg with
//        member this.History = this.hist
//        member this.Id = this.id


//type causeCfg(hist:ICauseCfg list) =
//    member this.hist = hist
//    member this.id =   
//        hist |> Seq.map(fun h -> h:>obj)
//             |> GuidUtils.guidFromObjs
//             |> WorkspaceId.create
//    interface IWorkspaceCfg with
//        member this.History = this.hist
//        member this.Id = this.id




module WorkspaceCfg = 

    ()



module CauseCfg = 

    ()