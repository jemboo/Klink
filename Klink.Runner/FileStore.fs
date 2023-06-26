namespace global
open System

module FileStore =

    let compStore:(workspaceComponentDescr -> IWorkspaceComponent option) = 
        fun wx -> None

