namespace global

type IWorkspaceStore = 
    abstract member SaveWorkSpace: workspace -> Result<string,string>
    abstract member LoadWorkSpace: workspaceId -> Result<workspace,string>
    abstract member WorkSpaceExists: workspaceId -> Result<bool,string>
