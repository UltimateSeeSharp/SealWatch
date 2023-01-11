namespace SealWatch.Code.HistotyLayer;

public class HistoryListDto
{
    public string Property { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public string ChangeUser { get; set; }
    public DateTime ChangeDate { get; set; }
}