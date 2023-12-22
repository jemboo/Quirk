namespace Quirk.Project


[<Measure>] type quirkRunId
[<Measure>] type projectName


[<Measure>] type replicaNumber
[<Measure>] type modelParamName

[<Measure>] type modelParamSetId
[<Measure>] type reportName
[<Measure>] type reportParamName
[<Measure>] type reportParamSetName


[<Measure>] type workspaceId
[<Measure>] type workspaceComponentId


type workspaceComponentType =
    | WorkspaceDescription = 0
    | SortableSet = 10
    | SorterSet = 20
    | SorterSetAncestry = 21
    | SorterSetConcatMap = 22
    | SorterSetEval = 23
    | SorterSetMutator = 24
    | SorterSetParentMap = 25
    | SorterSpeedBinSet = 30
    | SorterSetPruner = 40
    | WorkspaceParams = 50



//type IWorkspaceStore = 
//    abstract member SaveWorkSpace: workspace -> (wsComponentName -> bool) -> Result<string,string>
//    abstract member LoadWorkSpace: workspaceId -> Result<workspace,string>
//    abstract member WorkSpaceExists: workspaceId -> Result<bool,string>
//    abstract member GetLastWorkspaceId: unit -> Result<workspaceId,string>
//    abstract member WriteLinesEnsureHeader: (workspaceComponentType option) -> (string) -> (seq<string>) -> (string seq) -> Result<bool,string>
//    abstract member GetAllSorterSetEvalsWithParams: wsComponentName -> (workspaceParams -> bool) -> Result<(sorterSetEval*workspaceParams) list, string>
//    abstract member GetAllSpeedSetBinsWithParams: wsComponentName -> (workspaceParams -> bool) -> Result<(sorterSpeedBinSet*workspaceParams) list, string>
//    abstract member GetAllWorkspaceDescriptionsWithParams: unit -> Result<(workspaceDescription*workspaceParams) list, string>
//    abstract member GetComponent: wsComponentName -> workspaceDescription -> Result<workspaceComponent, string>