using SealWatch.Code.CutterLayer;
using SealWatch.Code.ProjectLayer;
using System.Collections.Generic;

namespace SealWatch.Wpf.Service.Interfaces;

public interface IUserInputService
{
    List<ValidationError> GetCutterValidationErrors(CutterEditDto cutter);
    List<ValidationError> GetProjectValidationErrors(ProjectEditDto project);
    bool UserConfirmPopUp(string tool);
}