
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Quirk.Project;

namespace Quirk.UI.W.Core.Models.Workspace;
public partial class ProjectVm : ObservableObject
{
    public ProjectVm(quirkProject quirkProject)
    {
        ProjectName = QuirkProject.getProjectName(quirkProject);

        foreach (var quirkWorldLine in QuirkProject.getQuirkWorldLines(quirkProject))
        {
            QuirkWorldLineVms.Add(new QuirkWorldLineVm());
        }


        SymbolCode = WorldlineType.Shc.ToSymbolCode();
    }

    public void CopyValuesFrom(ProjectVm projectVm)
    {
        ProjectName = projectVm.ProjectName;
        QuirkWorldLineVms = projectVm.QuirkWorldLineVms;
        SymbolCode = projectVm.SymbolCode;
        SymbolName = projectVm.SymbolName;
    }

    public int ItemCount => QuirkWorldLineVms?.Count ?? 0;

    [ObservableProperty]
    private string _projectName;


    [ObservableProperty]
    private int _symbolCode;


    [ObservableProperty]
    private string _symbolName;


    public char Symbol => (char)SymbolCode;

    public ObservableCollection<QuirkWorldLineVm> QuirkWorldLineVms { get; private set; } 
        = new ObservableCollection<QuirkWorldLineVm>();

    public string ShortDescription => $"Name: {ProjectName}";

    public override string ToString() => $"{ProjectName}";
}
