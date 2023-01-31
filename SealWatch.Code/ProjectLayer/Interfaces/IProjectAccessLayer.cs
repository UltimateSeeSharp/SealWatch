using SealWatch.Code.CutterLayer;
using SealWatch.Code.Enums;
using SealWatch.Data.Model;

namespace SealWatch.Code.ProjectLayer.Intefaces;

public interface IProjectAccessLayer
{
    void CreateOrUpdate(ProjectEditDto dtoItem);
    bool CutterExists(string serialNumber);
    void Done(int id);
    List<Cutter> GetCutterByProjectId(int id, bool sealOrdered = false, string? search = null);
    ProjectDetailDto GetDetails(int id);
    ProjectEditDto GetEditData(int id);
    List<CutterAnalyseDto> GetAnalyticData();
    Guid GetGuid();
    List<ProjectListDto> GetList(string? search = null, Visibility visibility = Visibility.All);
    bool ProjectExists(int id);
    void Remove(int id);
}
