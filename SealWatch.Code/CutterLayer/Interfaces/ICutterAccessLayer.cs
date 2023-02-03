using SealWatch.Data.Model;

namespace SealWatch.Code.CutterLayer.Interfaces;

public interface ICutterAccessLayer
{
    List<Cutter> GetList();
    CutterEditDto GetEditData(int id);
    List<AnalysedCutterDto> GetAnalysedCutters(string? search = null, int? daysLeftFilter = null, int? projectId = null);
    void CreateOrUpdate(CutterEditDto cutterDto);
    void Remove(int id);
    void Order(int id);
}