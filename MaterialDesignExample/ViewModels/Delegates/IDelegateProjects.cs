namespace SealWatch.Wpf.View.Delegates;

public interface IDelegateProjects
{
    void CreateOrUpdate(bool update = false);

    void MarkDone();

    void MarkDeleted();

    void Refresh();

    void ShowDoneItems();

    void ShowDeletedItems();

    void ShowHistory();

    void ShowDetails();
}