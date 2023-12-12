using System.Reflection.Metadata.Ecma335;

namespace Quirk.UI.W.Core.Models.Workspace;


public enum CfgPlexType
{
    Sample,
    Shc,
    Ga,
    None,
    Unknown
}

public static class CfgPlexTypeExt
{
    public static int ToSymbolCode(this CfgPlexType cfgPlexType)
    {
        switch (cfgPlexType)
        {
            case CfgPlexType.Sample:
                return 57643;  //Globe
            case CfgPlexType.Shc:
                return 57699;  //Calendar
            case CfgPlexType.Ga:
                return 57661;  //Contact
            case CfgPlexType.None:
                return 57665;
            case CfgPlexType.Unknown:
                return 57666;
            default:
                return 0;
        }
    }
}

// Model for the SampleDataService. Replace with your own model.
public class CfgPlex
{
    public CfgPlex()
    {
        CfgPlexType = CfgPlexType.Unknown;
    }
    public int ItemCount => CfgPlexItems?.Count ?? 0;

    public string Name
    {
        get; set;
    }

    public CfgPlexType CfgPlexType 
    {
        get; set; 
    }

    public int SymbolCode
    {
        get; set;
    }

    public string SymbolName
    {
        get; set;
    }

    public char Symbol => (char)SymbolCode;

    public ICollection<CfgPlexItem> CfgPlexItems
    {
        get; set;
    }

    public string ShortDescription => $"Name: {Name}";

    public override string ToString() => $"{Name}";
}
