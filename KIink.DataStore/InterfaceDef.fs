namespace global

type IWorkspaceStore = 
    abstract member SaveWorkSpace: workspace -> Result<string,string>
    abstract member LoadWorkSpace: workspaceId -> Result<workspace,string>
