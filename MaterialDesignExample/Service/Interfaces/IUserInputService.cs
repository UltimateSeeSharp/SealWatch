using SealWatch.Code.CutterLayer;
using SealWatch.Code.ProjectLayer;
using System.Collections.Generic;

namespace SealWatch.Wpf.Service.Interfaces;

public interface IUserInputService
{
    List<ValidationError> GetInvalidCutterInputs(CutterEditDto cutter);
    List<ValidationError> GetInvalidProjectInputs(ProjectEditDto project);
    bool UserPopupConfirmed(string tool);
}