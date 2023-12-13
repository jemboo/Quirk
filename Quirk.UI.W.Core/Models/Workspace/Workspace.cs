namespace Quirk.UI.W.Core.Models.Workspace;

// Model for the SampleDataService. Replace with your own model.
public class Workspace
{
    public string Name
    {
        get; set;
    }

    public ICollection<CfgPlexVm> CfgPlexes
    {
        get; set;
    }
}
