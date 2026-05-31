using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace WpfApp;

public partial class CourseDialog : Window
{
    public CourseDialog()
    {
        InitializeComponent();
        DeadlinePicker.SelectedDate = DateTime.Now.AddDays(90);
    }

    public CourseDialog(DotNet_Lab01_Core.Course course) : this()
    {
        Title = "Edit course";
        TitleTextBox.Text = course.CourseName;
        DescriptionTextBox.Text = course.CourseDescription ?? "";
        DifficultyTextBox.Text = course.Difficulty.ToString();
        CreditsTextBox.Text = course.Credits.ToString();
        ProgressTextBox.Text = course.Progress.ToString();
        DeadlinePicker.SelectedDate = course.Deadline;
        TagsTextBox.Text = string.Join(", ", course.Tags);
    }

    public string CourseTitle => TitleTextBox.Text.Trim();
    public string CourseDescription => DescriptionTextBox.Text.Trim();
    public List<string> Tags => ParseTags(TagsTextBox.Text);
    public int Difficulty { get; private set; }
    public int Credits { get; private set; }
    public int Progress { get; private set; }
    public DateTime Deadline => DeadlinePicker.SelectedDate ?? DateTime.Now.AddDays(90);

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput())
            return;

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(CourseTitle))
        {
            ShowValidation("Enter course title.");
            return false;
        }

        if (!TryReadNumber(DifficultyTextBox.Text, "Difficulty", out int difficulty))
            return false;

        if (!TryReadNumber(CreditsTextBox.Text, "Credits", out int credits))
            return false;

        if (!TryReadProgress(ProgressTextBox.Text, out int progress))
            return false;

        if (Deadline <= DateTime.Now)
        {
            ShowValidation("Deadline must be in the future.");
            return false;
        }

        Difficulty = difficulty;
        Credits = credits;
        Progress = progress;
        return true;
    }

    private static bool TryReadNumber(string value, string fieldName, out int number)
    {
        if (!int.TryParse(value, out number) || number < 1 || number > 100)
        {
            MessageBox.Show($"{fieldName} must be a number from 1 to 100.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private static bool TryReadProgress(string value, out int progress)
    {
        if (!int.TryParse(value, out progress) || progress < 0 || progress > 100)
        {
            MessageBox.Show("Progress must be a number from 0 to 100.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private static void ShowValidation(string message)
    {
        MessageBox.Show(message, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private static List<string> ParseTags(string value)
    {
        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
