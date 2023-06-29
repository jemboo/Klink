namespace global


type sorterSetParentMapCfg
            (name:wsComponentName,
             parentSorterSetId:sorterSetId,
             parentSorterSetCount:sorterCount,
             childSorterSetId:sorterSetId,
             childSorterSetCount:sorterCount
            ) =

    member this.sorterSetParentMapId = 
                    SorterSetParentMap.makeId
                            parentSorterSetId
                            childSorterSetId
    member this.name = name
    member this.parentSorterSetId = parentSorterSetId
    member this.parentSorterSetCount = parentSorterSetCount
    member this.childSorterSetId = childSorterSetId
    member this.childSorterSetCount = childSorterSetCount
    interface IWorkspaceComponentCfg with
        member this.Id = this.sorterSetParentMapId |> SorterSetParentMapId.value
        member this.WsComponentName = name
        member this.WorkspaceComponentType =
                workspaceComponentType.SorterSetMutator


module SorterSetParentMapCfg =

    let getFileName
            (cfg: sorterSetParentMapCfg) 
        =
        cfg.sorterSetParentMapId |> string


    let makeParentMap 
            (cfg: sorterSetParentMapCfg) = 

        SorterSetParentMap.create
            cfg.childSorterSetId
            cfg.parentSorterSetId
            cfg.childSorterSetCount
            cfg.parentSorterSetCount
  