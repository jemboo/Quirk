
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Quirk.UI.W.Core.Models.Workspace;

namespace Quirk.UI.W.ComponentVms;
public partial class CfgPlexVm : ObservableObject
{
    public CfgPlexVm()
    {
        _cfgPlexType = WorldlineType.Unknown;
    }

    public void CopyValuesFrom(CfgPlexVm cfgPlexVmSrc)
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

    public ObservableCollection<CfgPlexItem> CfgPlexItems { get; private set; } = new ObservableCollection<CfgPlexItem>();

    public string ShortDescription => $"Name: {Name}";

    public override string ToString() => $"{Name}";
}
