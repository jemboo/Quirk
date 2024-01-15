
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Quirk.Project;

namespace Quirk.UI.W.ComponentVms;
public partial class ProjectVm : ObservableObject
{
    public ProjectVm(quirkProject quirkProject)
    {
        _quirkProject = quirkProject;
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

        SetHeaders();
    }

    private quirkProject _quirkProject;

    private void SetHeaders()
    {
        var variableParams = QuirkProject.getVariableParamNames(_quirkProject);

        if (variableParams.Length > 0)
        {
            Hdr2 = variableParams[0];
            Hdr2Vis = Visibility.Visible;
        }
        else { Hdr2Vis = Visibility.Collapsed; }
        if (variableParams.Length > 1)
        {
            Hdr3 = variableParams[1];
            Hdr3Vis = Visibility.Visible;
        }
        else { Hdr3Vis = Visibility.Collapsed; }
        if (variableParams.Length > 2)
        {
            Hdr4 = variableParams[2];
            Hdr4Vis = Visibility.Visible;
        }
        else { Hdr4Vis = Visibility.Collapsed; }
        if (variableParams.Length > 3)
        {
            Hdr5 = variableParams[3];
            Hdr5Vis = Visibility.Visible;
        }
        else { Hdr5Vis = Visibility.Collapsed; }
        if (variableParams.Length > 4)
        {
            Hdr6 = variableParams[4];
            Hdr6Vis = Visibility.Visible;
        }
        else { Hdr6Vis = Visibility.Collapsed; }
        if (variableParams.Length > 5)
        {
            Hdr7 = variableParams[5];
            Hdr7Vis = Visibility.Visible;
        }
        else { Hdr7Vis = Visibility.Collapsed; }
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


    [ObservableProperty]
    private string _hdr2;

    [ObservableProperty]
    private string _hdr3;

    [ObservableProperty]
    private string _hdr4;

    [ObservableProperty]
    private string _hdr5;

    [ObservableProperty]
    private string _hdr6;

    [ObservableProperty]
    private string _hdr7;


    [ObservableProperty]
    private Visibility _hdr2Vis;

    [ObservableProperty]
    private Visibility _hdr3Vis;

    [ObservableProperty]
    private Visibility _hdr4Vis;

    [ObservableProperty]
    private Visibility _hdr5Vis;

    [ObservableProperty]
    private Visibility _hdr6Vis;

    [ObservableProperty]
    private Visibility _hdr7Vis;


}
