using Newtonsoft.Json.Linq;
using Quirk.UI.W.Core.Contracts.Services;
using Quirk.UI.W.Core.Models;
using Quirk.UI.W.Core.Models.Workspace;

namespace Quirk.UI.W.Core.Services;


public class WorkspaceDataService : IWorkspaceDataService
{
    private List<CfgPlex> _allOrders;

    public WorkspaceDataService(IFileService fileService)
    {
        _fileService = fileService;
    }

    private IFileService _fileService;

    private static IEnumerable<CfgPlex> AllOrders()
    {
        // The following is order summary data
        var companies = AllCompanies();
        return companies.SelectMany(c => c.CfgPlexes);
    }

    private static IEnumerable<Workspace> AllCompanies()
    {
        return new List<Workspace>()
        {
            new Workspace()
            {
                Name = "Company A",

                CfgPlexes = new List<CfgPlex>()
                {
                    new CfgPlex()
                    {
                        Name = "Speedy Fugazi",
                        SymbolCode = 57643,
                        SymbolName = "Globe",
                        CfgPlexItems = new List<CfgPlexItem>()
                        {
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15

                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            }
                        }
                    },
                    new CfgPlex()
                    {
                        Name = "Federal Shipping",
                        SymbolCode = 57737,
                        SymbolName = "Audio",
                        CfgPlexItems = new List<CfgPlexItem>()
                        {
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            }
                        }
                    },
                    new CfgPlex()
                    {
                        Name = "Speedy Express",
                        SymbolCode = 57699,
                        SymbolName = "Calendar",
                        CfgPlexItems = new List<CfgPlexItem>()
                        {
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            }
                        }
                    }
                }
            },
            new Workspace()
            {
                Name = "Company F",

                CfgPlexes = new List<CfgPlex>()
                {
                    new CfgPlex()
                    {
                        Name = "Speedy Express",
                        SymbolCode = 57620,
                        SymbolName = "Camera",
                        CfgPlexItems = new List<CfgPlexItem>()
                        {
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            }
                        }
                    },
                    new CfgPlex()
                    {
                        Name = "Federal Shipping",
                        SymbolCode = 57633,
                        SymbolName = "Clock",
                        CfgPlexItems = new List<CfgPlexItem>()
                        {
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            }
                        }
                    }
                }
            },
            new Workspace()
            {
                Name = "Company Z",

                CfgPlexes = new List<CfgPlex>()
                {
                    new CfgPlex()
                    {
                        Name = "Speedy Express",
                        SymbolCode = 57661,
                        SymbolName = "Contact",
                        CfgPlexItems = new List<CfgPlexItem>()
                        {
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            }
                        }
                    },
                    new CfgPlex()
                    {
                        Name = "Federal Shipping",
                        SymbolCode = 57619,
                        SymbolName = "Favorite",
                        CfgPlexItems = new List<CfgPlexItem>()
                        {
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            }
                        }
                    },
                    new CfgPlex()
                    {
                        Name = "United Package",
                        SymbolCode = 57615,
                        SymbolName = "Home",
                        CfgPlexItems = new List<CfgPlexItem>()
                        {
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            },
                            new CfgPlexItem()
                            {
                                Name = "Rössle Sauerkraut",
                                Rank = 15
                            }
                        }
                    }
                }
            }
        };
    }

    public async Task<IEnumerable<CfgPlex>> GetGridDataAsync()
    {
        _allOrders ??= new List<CfgPlex>(); // AllOrders());

        await Task.CompletedTask;
        return _allOrders;
    }

    public async Task<IEnumerable<CfgPlex>> GetListDetailsDataAsync()
    {
        _allOrders ??= new List<CfgPlex>(); // AllOrders());

        await Task.CompletedTask;
        return _allOrders;
    }
    public async Task<IEnumerable<CfgPlex>> GetCfgPlexesInWorkspace(string workspacePath)
    {
        var subfolders = _fileService.GetFolders(workspacePath)
                            .Select(path => Path.GetRelativePath(workspacePath, path));
        var cfgPlexesInWorkspace =
            subfolders.Select(subfolder => new CfgPlex() { Name = subfolder });

        await Task.CompletedTask;
        return cfgPlexesInWorkspace;
    }

    public async Task<CfgPlex> GetCfgPlexDetails(string workspacePath, CfgPlex cfgPlex)
    {
        var cfgPlexPath = $"{Path.Combine(workspacePath, cfgPlex.Name)}\\{(cfgPlex.Name)}.txt";

        cfgPlex.CfgPlexType = CfgPlexType.None;


        await Task.CompletedTask;
        return cfgPlex;
    }


}
