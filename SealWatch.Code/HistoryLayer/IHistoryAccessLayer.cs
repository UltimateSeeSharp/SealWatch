using SealWatch.Code.HistotyLayer;

namespace SealWatch.Code.HistoryLayer
{
    public interface IHistoryAccessLayer
    {
        List<HistoryListDto> GetList(Guid refGuid, string refId);
    }
}