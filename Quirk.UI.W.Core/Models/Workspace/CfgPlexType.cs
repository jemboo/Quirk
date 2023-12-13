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