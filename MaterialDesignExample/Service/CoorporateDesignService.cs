﻿using SealWatch.Wpf.Service.Interfaces;
using System.Windows.Media;

namespace SealWatch.Wpf.Service;

public class CoorporateDesignService : ICoorporateDesignService
{
    public SolidColorBrush colGrid => new() { Color = Color.FromArgb(20, 21, 23, 27) };
    public SolidColorBrush colGold60 => new() { Color = Color.FromArgb(60, 255, 215, 0) };
    public SolidColorBrush colGold160 => new() { Color = Color.FromArgb(160, 255, 215, 0) };
}