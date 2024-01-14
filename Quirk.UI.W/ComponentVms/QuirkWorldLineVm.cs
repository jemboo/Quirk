using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Quirk.Project;
namespace Quirk.UI.W.ComponentVms;

public partial class QuirkWorldLineVm : ObservableObject
{
    public QuirkWorldLineVm(quirkWorldLine quirkWorldLine, string[] variableParamNames)
    {
        _quirkWorldLine = quirkWorldLine;
        QuirkModelType = QuirkWorldLine.getQuirkModelType(quirkWorldLine);
        QuirkWorldLineId = QuirkWorldLine.getId(quirkWorldLine);
        ModelParamSet = QuirkWorldLine.getModelParamSet(quirkWorldLine);
        _replicaNum = Project.ModelParamSet.getReplicaNumber(ModelParamSet);
        var mpvs =
            Project.ModelParamSet.getModelParamValues(variableParamNames, ModelParamSet)
            .Select(x => ModelParamValue.toArrayOfStrings(x))
            .Select(x => x[2])
            .ToArray();

        setupColumns(mpvs);
    }

    private readonly quirkWorldLine _quirkWorldLine;

    [ObservableProperty]
    private modelParamSet _modelParamSet;

    [ObservableProperty]
    private quirkModelType _quirkModelType;

    [ObservableProperty]
    private Guid _quirkWorldLineId;

    [ObservableProperty]
    private int _replicaNum;

    [ObservableProperty]
    private string _val2;

    [ObservableProperty]
    private string _val3;

    [ObservableProperty]
    private string _val4;

    [ObservableProperty]
    private string _val5;

    [ObservableProperty]
    private string _val6;

    [ObservableProperty]
    private string _val7;

    private void setupColumns(string[] colVals)
    {
        if (colVals.Length > 0) 
        {
            Val2 = colVals[0];
        }
        else
        {
           // _val2Vis = V
        }
        if (colVals.Length > 1)
        {
            Val3 = colVals[1];
        }
        if (colVals.Length > 2) 
        {
            Val4 = colVals[2];
        }
        if (colVals.Length > 3) 
        {
            Val5 = colVals[3];
        }
        if (colVals.Length > 4) 
        {
            Val6 = colVals[4];
        }
        if (colVals.Length > 5)
        {
            Val7 = colVals[5];
        }
    }
}
