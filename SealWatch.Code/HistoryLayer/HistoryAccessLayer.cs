using SealWatch.Code.HistotyLayer;
using SealWatch.Data.Database;
using SealWatch.Data.Model;

namespace SealWatch.Code.HistoryLayer;

public class HistoryAccessLayer : IHistoryAccessLayer
{
    public List<HistoryListDto> GetList(Guid refGuid, string refId)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            return context.Set<History>()
                .Where(item => item.ReferenceGuid == refGuid && item.ReferenceId == refId)
                .Select(item => new HistoryListDto
                {
                    Property = item.Property,
                    ChangeDate = item.ChangeDate,
                    ChangeUser = item.ChangeUser,
                    NewValue = item.NewValue,
                    OldValue = item.OldValue
                }).OrderByDescending(item => item.ChangeDate)
                .ToList();
        }
    }
}