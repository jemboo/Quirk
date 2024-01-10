namespace Quirk.UI.W.Core.Models.Workspace;

public enum WorldlineType
{
    Sample,
    Shc,
    Ga,
    None,
    Unknown
}

public static class CfgPlexTypeExt
{
    public static int ToSymbolCode(this WorldlineType cfgPlexType)
    {
        switch (cfgPlexType)
        {
            case WorldlineType.Sample:
                return 57643;  //Globe
            case WorldlineType.Shc:
                return 57699;  //Calendar
            case WorldlineType.Ga:
                return 57661;  //Contact
            case WorldlineType.None:
                return 57665;
            case WorldlineType.Unknown:
                return 57666;
            default:
                return 0;
        }
    }
}