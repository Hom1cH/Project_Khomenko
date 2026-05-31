using System.Windows.Media;
using DotNet_Lab01_Core;
using MediaBrush = System.Windows.Media.Brush;
using MediaBrushes = System.Windows.Media.Brushes;
using MediaBrushConverter = System.Windows.Media.BrushConverter;

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

internal static class ViewModelVisuals
{
    public static List<TagChipViewModel> CreateTagItems(IEnumerable<string> tags)
    {
        string[] backgrounds = { "#EFF6FF", "#ECFDF3", "#FFF7ED", "#FDF2F8", "#F5F3FF", "#F0FDFA" };
        string[] borders = { "#BFDBFE", "#ABEFC6", "#FED7AA", "#FBCFE8", "#DDD6FE", "#99F6E4" };
        string[] foregrounds = { "#1D4ED8", "#027A48", "#C2410C", "#BE185D", "#6D28D9", "#0F766E" };

        List<string> cleanTags = tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .ToList();

        if (cleanTags.Count == 0)
            cleanTags.Add("No tags");

        return cleanTags
            .Select((tag, index) => new TagChipViewModel(
                tag,
                BrushFromHex(backgrounds[index % backgrounds.Length]),
                BrushFromHex(borders[index % borders.Length]),
                BrushFromHex(foregrounds[index % foregrounds.Length])))
            .ToList();
    }

    public static MediaBrush GetStatusBackground(UnitStatus status)
    {
        return status switch
        {
            UnitStatus.InProgress => BrushFromHex("#FEF7C3"),
            UnitStatus.Completed => BrushFromHex("#DCFCE7"),
            UnitStatus.Paused => BrushFromHex("#F3F4F6"),
            _ => MediaBrushes.White
        };
    }

    private static MediaBrush BrushFromHex(string hex)
    {
        MediaBrush brush = (MediaBrush)new MediaBrushConverter().ConvertFromString(hex)!;
        brush.Freeze();
        return brush;
    }
}
