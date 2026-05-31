using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DotNet_Lab01_Core;
using Button = System.Windows.Controls.Button;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WpfApp;

public partial class MainWindow : Window
{
    private readonly Reminder _reminder;
    private readonly System.Windows.Threading.DispatcherTimer _deadlineTimer;
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = (MainViewModel)DataContext;
        _viewModel.AddObjectRequested += AddObject;
        _viewModel.ImportRequested += ImportCourses;
        _viewModel.CoursesSaved += ShowSaveMessage;
        _viewModel.BackToCoursesRequested += ShowCoursesPanel;
        _viewModel.CurrentViewChanged += ShowCurrentPanel;
        _viewModel.ConfirmDeleteCourse = ConfirmDeleteCourse;
        _viewModel.ConfirmDeleteTask = ConfirmDeleteTask;

        _reminder = new Reminder(new WindowsReminderNotifier());
        _deadlineTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(30)
        };
        _deadlineTimer.Tick += (_, _) => CheckDeadlineReminders();

        _viewModel.LoadData();
        ShowCourses();
        CheckDeadlineReminders();
        _deadlineTimer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _deadlineTimer.Stop();
        _viewModel.AddObjectRequested -= AddObject;
        _viewModel.ImportRequested -= ImportCourses;
        _viewModel.CoursesSaved -= ShowSaveMessage;
        _viewModel.BackToCoursesRequested -= ShowCoursesPanel;
        _viewModel.CurrentViewChanged -= ShowCurrentPanel;
        _viewModel.ConfirmDeleteCourse = null;
        _viewModel.ConfirmDeleteTask = null;
        _reminder.Dispose();
        _viewModel.Dispose();
        base.OnClosed(e);
    }

    private void ShowCourses()
    {
        _viewModel.ShowCourses();
        CoursesView.Visibility = Visibility.Visible;
        TasksView.Visibility = Visibility.Collapsed;
        ShowSummarySidebar();
    }

    private void ShowTasks(Course course)
    {
        _viewModel.ShowTasks(course);
        CoursesView.Visibility = Visibility.Collapsed;
        TasksView.Visibility = Visibility.Visible;
        ShowCourseSidebar(course);
    }

    private void ShowSummarySidebar()
    {
        SummaryPanel.Visibility = Visibility.Visible;
        SelectedCoursePanel.Visibility = Visibility.Collapsed;
    }

    private void ShowCourseSidebar(Course course)
    {
        SummaryPanel.Visibility = Visibility.Collapsed;
        SelectedCoursePanel.Visibility = Visibility.Visible;

        _viewModel.ShowSelectedCourse(course);
    }

    private void CourseCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: WpfApp.CourseCardViewModel viewModel })
            ShowTasks(viewModel.Course);
    }

    private void ImportCourses()
    {
        OpenFileDialog dialog = new OpenFileDialog
        {
            Title = "Import courses from JSON",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = _viewModel.DataDirectory
        };

        if (dialog.ShowDialog(this) != true)
            return;

        _viewModel.ImportCoursesAndRefresh(dialog.FileName);
        ShowCoursesPanel();
        MessageBox.Show($"Courses imported from:\n{dialog.FileName}", "Import", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"\t\tComing soon\nCurrent JSON file:\n{_viewModel.JsonFilePath}", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void AddObject()
    {
        if (TasksView.Visibility == Visibility.Visible && _viewModel.SelectedCourse != null)
        {
            AddTaskToSelectedCourse();
            return;
        }

        AddCourse();
    }

    private void AddCourse()
    {
        CourseDialog dialog = new CourseDialog
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.CreateCourse(
            dialog.CourseTitle,
            dialog.CourseDescription,
            dialog.Credits,
            dialog.Difficulty,
            dialog.Deadline,
            dialog.Progress,
            dialog.Tags);

        ShowCourses();
    }

    private void AddTaskToSelectedCourse()
    {
        if (_viewModel.SelectedCourse == null)
            return;

        TaskDialog dialog = new TaskDialog
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.CreateTaskForSelectedCourse(
            dialog.TaskTitle,
            dialog.Deadline,
            dialog.Difficulty,
            dialog.Credits,
            dialog.TaskDescription,
            dialog.Progress,
            dialog.Tags);

        ShowCourseViewIfSelected();
        CheckDeadlineReminders();
    }

    private void CourseCardSettings_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void TaskCardSettings_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void EditCourseMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Parent: ContextMenu { PlacementTarget: Button { Tag: WpfApp.CourseCardViewModel viewModel } } })
            return;

        CourseDialog dialog = new CourseDialog(viewModel.Course)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.UpdateCourse(
            viewModel.Course,
            dialog.CourseTitle,
            dialog.CourseDescription,
            dialog.Credits,
            dialog.Difficulty,
            dialog.Progress,
            dialog.Deadline,
            dialog.Tags);

        ShowCourses();
        CheckDeadlineReminders();
    }

    private void EditTaskMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Parent: ContextMenu { PlacementTarget: Button { Tag: WpfApp.TaskCardViewModel viewModel } } })
            return;

        TaskDialog dialog = new TaskDialog(viewModel.Task)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        Course? course = _viewModel.UpdateTask(
            viewModel.Task,
            dialog.TaskTitle,
            dialog.TaskDescription,
            dialog.Credits,
            dialog.Difficulty,
            dialog.Progress,
            dialog.Deadline,
            dialog.Tags);

        ShowCurrentView(course);

        CheckDeadlineReminders();
    }

    private bool ConfirmDeleteCourse(CourseCardViewModel viewModel)
    {
        MessageBoxResult result = MessageBox.Show(
            $"Delete course \"{viewModel.Title}\"?",
            "Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        return result == MessageBoxResult.Yes;
    }

    private bool ConfirmDeleteTask(TaskCardViewModel viewModel)
    {
        MessageBoxResult result = MessageBox.Show(
            $"Delete task \"{viewModel.Title}\"?",
            "Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        return result == MessageBoxResult.Yes;
    }

    private void CheckDeadlineReminders()
    {
        _reminder.CheckDeadlines(_viewModel.CourseManager.GetCourses());
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void Status_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;

        if (e.ClickCount != 2)
            return;

        if (sender is TextBlock { ContextMenu: not null } statusText)
        {
            statusText.ContextMenu.PlacementTarget = statusText;
            statusText.ContextMenu.IsOpen = true;
        }
    }

    private void ShowCurrentView(Course? course)
    {
        if (course != null)
            ShowTasks(course);
        else
            ShowCourses();
    }

    private void ShowCurrentPanel()
    {
        if (_viewModel.SelectedCourse != null)
            ShowTasksPanel(_viewModel.SelectedCourse);
        else
            ShowCoursesPanel();

        CheckDeadlineReminders();
    }

    private void ShowCoursesPanel()
    {
        CoursesView.Visibility = Visibility.Visible;
        TasksView.Visibility = Visibility.Collapsed;
        ShowSummarySidebar();
        CheckDeadlineReminders();
    }

    private void ShowTasksPanel(Course course)
    {
        CoursesView.Visibility = Visibility.Collapsed;
        TasksView.Visibility = Visibility.Visible;
        ShowCourseSidebar(course);
    }

    private void ShowSaveMessage()
    {
        MessageBox.Show($"\t\tCourses saved.\nCurrent JSON file:\n{_viewModel.JsonFilePath}", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ShowCourseViewIfSelected()
    {
        if (_viewModel.SelectedCourse != null)
            ShowTasks(_viewModel.SelectedCourse);
        else
            ShowCourses();
    }
}
