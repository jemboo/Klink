namespace global


type projectFolder = private ProjectFolder of string
module ProjectFolder =

    let value (ProjectFolder v) = v

    let create (value: string) =
        value |> ProjectFolder



type IWorkspaceStore = 
    abstract member SaveWorkSpace: workspace -> (wsComponentName -> bool) -> Result<string,string>
    abstract member LoadWorkSpace: workspaceId -> Result<workspace,string>
    abstract member WorkSpaceExists: workspaceId -> Result<bool,string>
    abstract member GetLastWorkspaceId: unit -> Result<workspaceId,string>
    abstract member WriteLinesEnsureHeader: (workspaceComponentType option) -> (string) -> (seq<string>) -> (string seq) -> Result<bool,string>
    abstract member GetAllSorterSetEvalsWithParams: wsComponentName -> (workspaceParams -> bool) -> Result<(sorterSetEval*workspaceParams) list, string>
    abstract member GetAllSpeedSetBinsWithParams: wsComponentName -> (workspaceParams -> bool) -> Result<(sorterSpeedBinSet*workspaceParams) list, string>
    abstract member GetAllWorkspaceDescriptionsWithParams: unit -> Result<(workspaceDescription*workspaceParams) list, string>
    abstract member GetComponent: wsComponentName -> workspaceDescription -> Result<workspaceComponent, string>