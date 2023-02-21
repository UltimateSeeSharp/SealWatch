using SealWatch.Code.Enums;
using SealWatch.Data.Model;
using System.Collections.Generic;

namespace SealWatch.Code.CutterLayer.Interfaces;

public interface ICutterAccessLayer
{
    IEnumerable<Cutter> GetList();
    IEnumerable<string> GetSoilTypes();
    CutterEditDto GetEditData(int id);
    IEnumerable<AnalysedCutterDto> GetAnalysedCutters(string? search = null, Timeframe? timeframe = null, int? projectId = null, int accuracy = 0);
    void CreateOrUpdate(CutterEditDto cutterDto);
    void Remove(int id);
    void Order(int id);
}