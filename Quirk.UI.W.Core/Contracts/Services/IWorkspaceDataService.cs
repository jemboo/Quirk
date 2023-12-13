using Quirk.UI.W.Core.Models.Workspace;

namespace Quirk.UI.W.Core.Contracts.Services;

// Remove this class once your pages/features are using your data.
public interface IWorkspaceDataService
{
    Task<IEnumerable<CfgPlexVm>> GetGridDataAsync();

    Task<IEnumerable<CfgPlexVm>> GetListDetailsDataAsync();

    Task<IEnumerable<CfgPlexVm>> GetCfgPlexesInWorkspace(string workspacePath);

    Task<CfgPlexVm> GetCfgPlexDetails(string workspacePath, CfgPlexVm cfgPlexVm);
}
