using System.Windows;
using System.Windows.Media;
using DotNet_Lab01_Core;
using MediaBrush = System.Windows.Media.Brush;

namespace WpfApp;

public sealed class CourseCardViewModel
{
    public CourseCardViewModel(Course course)
    {
        Course = course;
    }

    public Course Course { get; }
    public string Title => Course.CourseName;
    public string Description => Course.CourseDescription ?? "";
    public int Progress => Course.Progress;
    public string ProgressText => $"{Course.Progress}%";
    public int TaskCount => Course.Tasks.Count;
    public int Difficulty => Course.Difficulty;
    public int Credits => Course.Credits;
    public string Status => Course.Status.ToString();
    public MediaBrush HeaderBackground => ViewModelVisuals.GetStatusBackground(Course.Status);
    public MediaBrush StatusBadgeBackground => ThemeManager.GetStatusBadgeBackgroundBrush(Course.Status);
    public MediaBrush StatusBadgeForeground => ThemeManager.GetStatusBadgeForegroundBrush(Course.Status);
    public MediaBrush StatusBadgeBorderBrush => ThemeManager.GetStatusBadgeBorderBrush(Course.Status);
    public double Workload => Course.ComputeWorkload();
    public List<TagChipViewModel> TagItems => ViewModelVisuals.CreateTagItems(Course.Tags);
    public string CreatedAtText => Course.CreatedAt.ToString("dd.MM.yyyy");
    public string DeadlineText => Course.Deadline.ToString("dd.MM.yyyy");
    public string EndedAtText => Course.EndedAt?.ToString("dd.MM.yyyy") ?? "-";
    public string StatusText => Course.Status.ToString();
    public string ReceivedCreditsText => $"{Course.ReceivedCredits}/{Course.Credits}";
}

public sealed class TaskCardViewModel
{
    public TaskCardViewModel(ParacTask task)
    {
        Task = task;
    }

    public ParacTask Task { get; }
    public int Id => Task.Id;
    public string Title => Task.TaskName;
    public string Status => Task.Status.ToString();
    public MediaBrush HeaderBackground => ViewModelVisuals.GetStatusBackground(Task.Status);
    public MediaBrush StatusBadgeBackground => ThemeManager.GetStatusBadgeBackgroundBrush(Task.Status);
    public MediaBrush StatusBadgeForeground => ThemeManager.GetStatusBadgeForegroundBrush(Task.Status);
    public MediaBrush StatusBadgeBorderBrush => ThemeManager.GetStatusBadgeBorderBrush(Task.Status);
    public string Description => Task.TaskDescription ?? "";
    public List<TagChipViewModel> TagItems => ViewModelVisuals.CreateTagItems(Task.Tags);
    public int Progress => Task.Progress;
    public string ProgressText => $"{Task.Progress}%";
    public int Difficulty => Task.Difficulty;
    public int Credits => Task.Credits;
    public double Workload => Task.ComputeWorkload();
    public string CreatedAtText => Task.CreatedAt.ToString("dd.MM.yyyy");
    public string DeadlineText => Task.Deadline.ToString("dd.MM.yyyy");
    public string EndedAtText => Task.EndedAt?.ToString("dd.MM.yyyy") ?? "-";
    public string StatusText => Task.Status.ToString();
}

public sealed class TagChipViewModel
{
    public TagChipViewModel(string text, MediaBrush background, MediaBrush borderBrush, MediaBrush foreground)
    {
        Text = text;
        Background = background;
        BorderBrush = borderBrush;
        Foreground = foreground;
    }

    public string Text { get; }
    public MediaBrush Background { get; }
    public MediaBrush BorderBrush { get; }
    public MediaBrush Foreground { get; }
}

public sealed class TaskImageViewModel
{
    public TaskImageViewModel(string path)
    {
        Path = path;
        FileName = System.IO.Path.GetFileName(path);
    }

    public string Path { get; }
    public string FileName { get; }
}

public sealed class TaskAttachmentViewModel
{
    public TaskAttachmentViewModel(string path)
    {
        Path = path;
        FileName = System.IO.Path.GetFileName(path);
    }

    public string Path { get; }
    public string FileName { get; }
}

internal static class ViewModelVisuals
{
    private static AppTheme _theme = AppTheme.Light;

    public static void SetTheme(AppTheme theme)
    {
        _theme = theme;
    }

    public static List<TagChipViewModel> CreateTagItems(IEnumerable<string> tags)
    {
        List<string> cleanTags = tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .ToList();

        if (cleanTags.Count == 0)
            cleanTags.Add("No tags");

        return cleanTags
            .Select((tag, index) => new TagChipViewModel(
                tag,
                ThemeManager.GetTagBackgroundBrush(index),
                ThemeManager.GetTagBorderBrush(index),
                ThemeManager.GetTagForegroundBrush(index)))
            .ToList();
    }

    public static MediaBrush GetStatusBackground(UnitStatus status)
    {
        return ThemeManager.GetStatusHeaderBrush(status);
    }
}
