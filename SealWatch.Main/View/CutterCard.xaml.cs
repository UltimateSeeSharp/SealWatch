using SealWatch.Data.Model;
using System.Windows;
using System.Windows.Controls;

namespace SealWatch.Main.CustomControls;

public partial class CutterCard : UserControl
{
    public CutterCard()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty CutterProperty = DependencyProperty.Register("Cutter", typeof(Cutter), typeof(CutterCard));

    public Cutter Cutter
    {
        get { return (Cutter)GetValue(CutterProperty); }
        set { SetValue(CutterProperty, value); }
    }
}