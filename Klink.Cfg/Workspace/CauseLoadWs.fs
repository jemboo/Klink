namespace global

type causeLoadWorkspace
            (workspaceId:workspaceId) 
    = 
    member this.workspaceId = workspaceId

    member this.updater =

        fun (w: workspace) _ ->

            result {

                return w
            }

    member this.id =
        [
            "causeLoadWorkspace" :> obj
            this.workspaceId |> WorkspaceId.value  :> obj
        ]
             |> GuidUtils.guidFromObjs
             |> CauseId.create

    member this.makeTruncatedWorkspaceCfg() 
            =
            WorkspaceCfg.makeWorkspaceCfg [this]

    interface ICause with
        member this.Id = this.id
        member this.ResetId = Some this.workspaceId
        member this.Name = $"causeLoadWorkspace"
        member this.Updater = this.updater
        member this.UseInWorkspaceId = true
