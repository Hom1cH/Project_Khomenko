using System.Windows;
using System.Windows.Media;
using DotNet_Lab01_Core;
using MaterialDesignThemes.Wpf;

namespace WpfApp;

public static class ThemeManager
{
    public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    public static void ApplyTheme(AppTheme theme)
    {
        CurrentTheme = theme;
        ViewModelVisuals.SetTheme(theme);
        ApplyMaterialDesignTheme(theme);
        ResourceDictionary resources = System.Windows.Application.Current.Resources;

        if (theme == AppTheme.Dark)
        {
            Set(resources, "AppBackgroundBrush", "#1E1F26");
            Set(resources, "SurfaceBrush", "#1F2937");
            Set(resources, "PanelBrush", "#3E4559");
            Set(resources, "SubtleSurfaceBrush", "#4C556E");
            Set(resources, "PrimaryTextBrush", "#FFFFFF");
            Set(resources, "SecondaryTextBrush", "#B8C1CC");
            Set(resources, "BorderBrush", "#374151");
            Set(resources, "AppBarBrush", "#263351");
            Set(resources, "AppBarTitleBrush", "#FFFFFF");
            Set(resources, "ButtonTitle", "#000000");
            Set(resources, "AppBarSubtitleBrush", "#D7E2F1");
            Set(resources, "TransparentBrush", "#00FFFFFF");
            Set(resources, "StatusBadgeBackgroundBrush", "#14532D");
            Set(resources, "StatusBadgeTextBrush", "#BBF7D0");
            Set(resources, "FabBackgroundBrush", "#22C55E");
            Set(resources, "FabForegroundBrush", "#FFFFFF");
            Set(resources, "ButtonBackground", "#416DED");
            Set(resources, "ProgresBackgroundBrush", "#abc0ff");
            Set(resources, "ProgresForegroundBrush", "#416DED");
            Set(resources, "GamblingFill", "#E8D441");
            SetColor(resources, "ShadowColor", "#000000");
            return;
        }

        Set(resources, "AppBackgroundBrush", "#F4F7FB");
        Set(resources, "SurfaceBrush", "#FFFFFF");
        Set(resources, "PanelBrush", "#FFFFFF");
        Set(resources, "SubtleSurfaceBrush", "#ededf7");
        Set(resources, "PrimaryTextBrush", "#111827");
        Set(resources, "SecondaryTextBrush", "#667085");
        Set(resources, "BorderBrush", "#E5E7EB");
        Set(resources, "AppBarBrush", "#263351");
        Set(resources, "AppBarTitleBrush", "#FFFFFF");
        Set(resources, "ButtonTitle", "#000000");
        Set(resources, "AppBarSubtitleBrush", "#B8C7DA");
        Set(resources, "TransparentBrush", "#00FFFFFF");
        Set(resources, "StatusBadgeBackgroundBrush", "#ECFDF3");
        Set(resources, "StatusBadgeTextBrush", "#027A48");
        Set(resources, "FabBackgroundBrush", "#32CD32");
        Set(resources, "FabForegroundBrush", "#FFFFFF");
        Set(resources, "ButtonBackground", "#6689ed");
        Set(resources, "ProgresBackgroundBrush", "#c7d5ff");
        Set(resources, "ProgresForegroundBrush", "#6689ed");
        Set(resources, "GamblingFill", "#E8D441");
        SetColor(resources, "ShadowColor", "#000000");
    }

    public static System.Windows.Media.Brush GetStatusHeaderBrush(UnitStatus status)
    {
        if (CurrentTheme == AppTheme.Dark)
        {
            return status switch
            {
                UnitStatus.InProgress => BrushFromHex("#E8D441"),
                UnitStatus.Completed => BrushFromHex("#249E52"),
                UnitStatus.Paused => BrushFromHex("#696F78"),
                _ => BrushFromHex("#9FB1C9")
            };
        }

        return status switch
        {
            UnitStatus.InProgress => BrushFromHex("#FEF7C3"),
            UnitStatus.Completed => BrushFromHex("#DCFCE7"),
            UnitStatus.Paused => BrushFromHex("#F3F4F6"),
            _ => BrushFromHex("#FFFFFF")
        };
    }

    public static System.Windows.Media.Brush GetStatusBadgeBackgroundBrush(UnitStatus status)
    {
        if (CurrentTheme == AppTheme.Dark)
        {
            return status switch
            {
                UnitStatus.InProgress => BrushFromHex("#713F12"),
                UnitStatus.Completed => BrushFromHex("#14532D"),
                UnitStatus.Paused => BrushFromHex("#374151"),
                _ => BrushFromHex("#334155")
            };
        }

        return status switch
        {
            UnitStatus.InProgress => BrushFromHex("#FEF9C3"),
            UnitStatus.Completed => BrushFromHex("#ECFDF3"),
            UnitStatus.Paused => BrushFromHex("#F3F4F6"),
            _ => BrushFromHex("#F8FAFC")
        };
    }

    public static System.Windows.Media.Brush GetStatusBadgeForegroundBrush(UnitStatus status)
    {
        if (CurrentTheme == AppTheme.Dark)
        {
            return status switch
            {
                UnitStatus.InProgress => BrushFromHex("#FEF08A"),
                UnitStatus.Completed => BrushFromHex("#BBF7D0"),
                UnitStatus.Paused => BrushFromHex("#E5E7EB"),
                _ => BrushFromHex("#E0F2FE")
            };
        }

        return status switch
        {
            UnitStatus.InProgress => BrushFromHex("#A16207"),
            UnitStatus.Completed => BrushFromHex("#027A48"),
            UnitStatus.Paused => BrushFromHex("#4B5563"),
            _ => BrushFromHex("#475569")
        };
    }

    public static System.Windows.Media.Brush GetStatusBadgeBorderBrush(UnitStatus status)
    {
        if (CurrentTheme == AppTheme.Dark)
        {
            return status switch
            {
                UnitStatus.InProgress => BrushFromHex("#E8D441"),
                UnitStatus.Completed => BrushFromHex("#249E52"),
                UnitStatus.Paused => BrushFromHex("#696F78"),
                _ => BrushFromHex("#9FB1C9")
            };
        }

        return status switch
        {
            UnitStatus.InProgress => BrushFromHex("#FDE68A"),
            UnitStatus.Completed => BrushFromHex("#ABEFC6"),
            UnitStatus.Paused => BrushFromHex("#D1D5DB"),
            _ => BrushFromHex("#CBD5E1")
        };
    }

    public static System.Windows.Media.Brush GetTagBackgroundBrush(int index)
    {
        string[] colors = CurrentTheme == AppTheme.Dark
            ? new[] { "#1E3A8A", "#14532D", "#7C2D12", "#831843", "#4C1D95", "#134E4A" }
            : new[] { "#EFF6FF", "#ECFDF3", "#FFF7ED", "#FDF2F8", "#F5F3FF", "#F0FDFA" };

        return BrushFromHex(colors[index % colors.Length]);
    }

    public static System.Windows.Media.Brush GetTagBorderBrush(int index)
    {
        string[] colors = CurrentTheme == AppTheme.Dark
            ? new[] { "#60A5FA", "#4ADE80", "#FB923C", "#F472B6", "#A78BFA", "#2DD4BF" }
            : new[] { "#BFDBFE", "#ABEFC6", "#FED7AA", "#FBCFE8", "#DDD6FE", "#99F6E4" };

        return BrushFromHex(colors[index % colors.Length]);
    }

    public static System.Windows.Media.Brush GetTagForegroundBrush(int index)
    {
        string[] colors = CurrentTheme == AppTheme.Dark
            ? new[] { "#DBEAFE", "#DCFCE7", "#FFEDD5", "#FCE7F3", "#EDE9FE", "#CCFBF1" }
            : new[] { "#1D4ED8", "#027A48", "#C2410C", "#BE185D", "#6D28D9", "#0F766E" };

        return BrushFromHex(colors[index % colors.Length]);
    }

    private static void Set(ResourceDictionary resources, string key, string color)
    {
        resources[key] = BrushFromHex(color);
    }

    private static void SetColor(ResourceDictionary resources, string key, string color)
    {
        resources[key] = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color);
    }

    private static SolidColorBrush BrushFromHex(string color)
    {
        SolidColorBrush brush = new((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
        brush.Freeze();
        return brush;
    }

    private static void ApplyMaterialDesignTheme(AppTheme theme)
    {
        PaletteHelper paletteHelper = new();
        var materialTheme = paletteHelper.GetTheme();
        materialTheme.SetBaseTheme(theme == AppTheme.Dark ? BaseTheme.Dark : BaseTheme.Light);
        materialTheme.SetPrimaryColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#416DED"));
        materialTheme.SetSecondaryColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#22C55E"));
        paletteHelper.SetTheme(materialTheme);
    }
}
