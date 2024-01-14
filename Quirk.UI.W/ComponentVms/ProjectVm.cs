
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Quirk.Project;

namespace Quirk.UI.W.ComponentVms;
public partial class ProjectVm : ObservableObject
{
    public ProjectVm(quirkProject quirkProject)
    {
        ProjectName = QuirkProject.getProjectName(quirkProject);
        var variableParamNames = QuirkProject.getVariableParamNames(quirkProject);

        foreach (var quirkWorldLine in QuirkProject.getQuirkWorldLines(quirkProject))
        {
            QuirkWorldLineVms.Add(new QuirkWorldLineVm(quirkWorldLine, variableParamNames));
        }
        ConstantParams =
            QuirkWorldLineVms.Any() ?
            QuirkProject.getSingletonParams(quirkProject)
            .Select(x => ModelParamValue.toReportString(x))
            .Aggregate((x,cum) => $"{cum}, {x}")
            :
            String.Empty;
    }

    public void CopyValuesFrom(ProjectVm projectVm)
    {
        ProjectName = projectVm.ProjectName;
        QuirkWorldLineVms = projectVm.QuirkWorldLineVms;
        SingetonModelParamValues = projectVm.SingetonModelParamValues;
    }

    public int ItemCount => QuirkWorldLineVms?.Count ?? 0;

    [ObservableProperty]
    private string _projectName;


    [ObservableProperty]
    private string _singetonModelParamValues;

    public ObservableCollection<QuirkWorldLineVm> QuirkWorldLineVms { get; private set; } 
        = new ObservableCollection<QuirkWorldLineVm>();

    [ObservableProperty]
    private string _constantParams;

    public override string ToString() => $"{ProjectName}";

    //[ObservableProperty]
    //private Visibility _val2Vis;

    //[ObservableProperty]
    //private Visibility _val3Vis;

    //[ObservableProperty]
    //private Visibility _val4Vis;

    //[ObservableProperty]
    //private Visibility _val5Vis;

    //[ObservableProperty]
    //private Visibility _val6Vis;

    //[ObservableProperty]
    //private Visibility _val7Vis;


}
