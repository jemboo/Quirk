using Newtonsoft.Json.Linq;
using Quirk.UI.W.Core.Contracts.Services;
using Quirk.UI.W.Core.Models;
using Quirk.UI.W.Core.Models.Workspace;

namespace Quirk.UI.W.Core.Services;


public class WorkspaceDataService : IWorkspaceDataService
{
    private List<CfgPlexVm> _allOrders;

    public WorkspaceDataService(IFileService fileService)
    {
        _fileService = fileService;
    }

    private readonly IFileService _fileService;

    public async Task<IEnumerable<CfgPlexVm>> GetGridDataAsync()
    {
        _allOrders ??= new List<CfgPlexVm>(); // AllOrders());

        await Task.CompletedTask;
        return _allOrders;
    }

    public async Task<IEnumerable<CfgPlexVm>> GetListDetailsDataAsync()
    {
        _allOrders ??= new List<CfgPlexVm>(); // AllOrders());

        await Task.CompletedTask;
        return _allOrders;
    }

    public async Task<IEnumerable<CfgPlexVm>> GetCfgPlexesInWorkspace(string workspacePath)
    {
        var subfolders = _fileService.GetFolders(workspacePath)
                            .Select(path => Path.GetRelativePath(workspacePath, path));
        var cfgPlexesInWorkspace =
            subfolders.Select(subfolder => new CfgPlexVm() { Name = subfolder });

        await Task.CompletedTask;
        return cfgPlexesInWorkspace;
    }

    public async Task<CfgPlexVm> GetCfgPlexDetails(string workspacePath, CfgPlexVm cfgPlex)
    {
        var cfgPlexRet = new CfgPlexVm();
        cfgPlexRet.CopyValuesFrom(cfgPlex);

        var cfgPlexPath = Path.Combine(workspacePath, $"{cfgPlex.Name}\\{cfgPlex.Name}.txt");

        if (! Path.Exists(cfgPlexPath))
        {
            cfgPlexRet.CfgPlexType = CfgPlexType.None;
        }
        else
        {

        }

        await Task.CompletedTask;
        return cfgPlexRet;
    }


}
