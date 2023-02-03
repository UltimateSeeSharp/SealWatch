using SealWatch.Code.HistotyLayer;

namespace SealWatch.Code.HistoryLayer.Interfaces
{
    public interface IHistoryAccessLayer
    {
        List<HistoryListDto> GetList(Guid refGuid, string refId);
    }
}