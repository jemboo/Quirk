
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Quirk.UI.W.Core.Models.Workspace;
public partial class ProjectVm : ObservableObject
{
    public ProjectVm()
    {
        _cfgPlexType = WorldlineType.Unknown;
    }

    public void CopyValuesFrom(ProjectVm cfgPlexVmSrc)
    {
        Name = cfgPlexVmSrc.Name;
        CfgPlexType = cfgPlexVmSrc.CfgPlexType;
        SymbolCode = cfgPlexVmSrc.SymbolCode;
        SymbolName = cfgPlexVmSrc.SymbolName;
    }

    public int ItemCount => CfgPlexItems?.Count ?? 0;


    [ObservableProperty]
    private string _name;


    [ObservableProperty]
    private WorldlineType _cfgPlexType;


    [ObservableProperty]
    private int _symbolCode;


    [ObservableProperty]
    private string _symbolName;


    public char Symbol => (char)SymbolCode;

    public ObservableCollection<QuirkWordline> CfgPlexItems { get; private set; } = new ObservableCollection<QuirkWordline>();

    public string ShortDescription => $"Name: {Name}";

    public override string ToString() => $"{Name}";
}
