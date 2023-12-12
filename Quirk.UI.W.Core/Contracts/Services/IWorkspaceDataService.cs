using Quirk.UI.W.Core.Models.Workspace;

namespace Quirk.UI.W.Core.Contracts.Services;

// Remove this class once your pages/features are using your data.
public interface IWorkspaceDataService
{
    Task<IEnumerable<CfgPlex>> GetGridDataAsync();

    Task<IEnumerable<CfgPlex>> GetListDetailsDataAsync();

    Task<IEnumerable<CfgPlex>> GetCfgPlexesInWorkspace(string workspacePath);
}
