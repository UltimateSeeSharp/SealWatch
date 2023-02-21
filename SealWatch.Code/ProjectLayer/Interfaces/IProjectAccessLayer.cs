using SealWatch.Code.CutterLayer;
using SealWatch.Code.Enums;
using SealWatch.Data.Model;

namespace SealWatch.Code.ProjectLayer.Intefaces;

public interface IProjectAccessLayer
{
    IEnumerable<ProjectListDto> GetList(string? search = null, Visibility visibility = Visibility.All);
    ProjectEditDto GetEditData(int id);
    ProjectDetailDto GetDetails(int id);
    void AddOrEdit(ProjectEditDto dtoItem);
    void Remove(int id);
    void Done(int id);
    bool ProjectExists(int id);
    Guid GetGuid();
}
