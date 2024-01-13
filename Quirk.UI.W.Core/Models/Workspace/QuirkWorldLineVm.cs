using CommunityToolkit.Mvvm.ComponentModel;
namespace Quirk.UI.W.Core.Models.Workspace;

public partial class QuirkWorldLineVm : ObservableObject
{
    public QuirkWorldLineVm()
    {
        WorldlineType = WorldlineType.Unknown;
        SymbolCode = WorldlineType.ToSymbolCode();
    }

    [ObservableProperty]
    private WorldlineType _worldlineType;


    [ObservableProperty]
    private int _symbolCode;

    public char Symbol => (char)SymbolCode;

}
