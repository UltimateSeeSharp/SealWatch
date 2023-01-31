using SealWatch.Data.Model;

namespace SealWatch.Code.CutterLayer.Interfaces;

public interface ICutterAccessLayer
{
    Cutter GetCutterById(int id);
    CutterEditDto? GetEditData(int id);
    List<Cutter> GetList();
    void Remove(int id);
    void Order(int id);
    void CreateOrUpdate(CutterEditDto cutterDto);
    List<CutterAnalyseDto> GetAnalyticData(string? search = null, int? daysLeftFilter = null);
}