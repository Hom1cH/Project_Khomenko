using System.IO;
using System.Windows;
using System.ComponentModel;
using DotNet_Lab01_Core;

namespace WpfApp;

public partial class MainWindow : Window
{
    private readonly string _dataDirectory;
    private readonly string _jsonFilePath;
    private readonly string _logFilePath;
    private readonly ResourceManager _logger;
    private readonly CourseManager _courseManager;
    private readonly TaskManager _taskManager;
    private readonly CourseController _courseController;

    public MainWindow()
    {
        InitializeComponent();

        _dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(_dataDirectory);

        _jsonFilePath = Path.Combine(_dataDirectory, "courses.json");
        _logFilePath = Path.Combine(_dataDirectory, "logs.log");

        _logger = new ResourceManager(_logFilePath);
        _courseManager = new CourseManager(_logger);
        _taskManager = new TaskManager(_courseManager, _logger);
        _courseController = new CourseController(_courseManager, _taskManager, _logger);

        LoadData();
        RenderCourses();
    }

    protected override void OnClosed(EventArgs e)
    {
        _logger.Dispose();
        base.OnClosed(e);
    }

    private void LoadData()
    {
        CourseJsonStorage.LoadCourses(_jsonFilePath, _courseManager, _taskManager, _logger);
        _courseManager.EnableJsonAutoSave(_jsonFilePath);

        if (_courseManager.GetCourses().Count > 0)
            return;

        Course csharp = _courseController.CreateCourse("C# Fundamentals", "Base syntax, classes, LINQ and files.", 5, 55, DateTime.Now.AddMonths(3));
        _courseController.CreateTaskForCourse(csharp.Id, "Console CRUD", DateTime.Now.AddDays(9), 45, 2, "Create a console manager for courses.");
        _courseController.CreateTaskForCourse(csharp.Id, "JSON persistence", DateTime.Now.AddDays(16), 70, 3, "Save and load data with JsonSerializer.");

        Course oop = _courseController.CreateCourse("Object-Oriented Design", "Inheritance, interfaces and composition.", 4, 68, DateTime.Now.AddMonths(4));
        _courseController.CreateTaskForCourse(oop.Id, "Polymorphism demo", DateTime.Now.AddDays(12), 65, 2, "Use a common interface for several models.");
        _courseController.CreateTaskForCourse(oop.Id, "Course-task relation", DateTime.Now.AddDays(20), 75, 3, "Bind tasks to courses and cascade delete.");

        Course ui = _courseController.CreateCourse("Desktop UI", "WinForms and WPF interfaces for the model.", 3, 60, DateTime.Now.AddMonths(2));
        _courseController.CreateTaskForCourse(ui.Id, "WinForms grid", DateTime.Now.AddDays(7), 50, 2, "Display courses in a DataGridView.");
    }

    private void RenderCourses()
    {
        List<Course> courses = _courseManager.GetCourses();
        CourseCards.ItemsSource = courses.Select(course => new CourseCardViewModel(course)).ToList();
        UpdateSummary(courses);

        HeaderSubtitle.Text = "Courses overview";
        HeaderCounter.Text = $"{courses.Count} courses";
        CoursesView.Visibility = Visibility.Visible;
        TasksView.Visibility = Visibility.Collapsed;
    }

    private void RenderTasks(Course course)
    {
        TaskCards.ItemsSource = course.Tasks.Select(task => new TaskCardViewModel(task)).ToList();
        SelectedCourseTitle.Text = course.CourseName;
        SelectedCourseSubtitle.Text = $"{course.Tasks.Count} tasks attached to this course";
        HeaderSubtitle.Text = "Task details";
        HeaderCounter.Text = $"{course.Tasks.Count} tasks";
        CoursesView.Visibility = Visibility.Collapsed;
        TasksView.Visibility = Visibility.Visible;
    }

    private void UpdateSummary(List<Course> courses)
    {
        int totalTasks = courses.Sum(course => course.Tasks.Count);
        int activeCourses = courses.Count(course => course.IsActive);
        double averageProgress = courses.Count == 0 ? 0 : courses.Average(course => course.Progress);

        TotalCoursesText.Text = courses.Count.ToString();
        TotalTasksText.Text = totalTasks.ToString();
        ActiveCoursesText.Text = activeCourses.ToString();
        AverageProgressText.Text = $"{Math.Round(averageProgress)}%";
    }

    private void CourseCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: CourseCardViewModel viewModel })
            RenderTasks(viewModel.Course);
    }

    private void BackToCourses_Click(object sender, RoutedEventArgs e)
    {
        RenderCourses();
    }

    private sealed class CourseCardViewModel : INotifyPropertyChanged
    {
        public CourseCardViewModel(Course course)
        {
            Course = course;
        }

        public Course Course { get; }
        public string Title => Course.CourseName;
        public string Description => Course.CourseDescription ?? "";
        public int Progress
        {
            get => Course.Progress;
            set
            {
                if (Course.Progress == value) return;
                Course.Progress = value;
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressText));
            }
        }
        public string ProgressText => $"{Course.Progress}%";
        public int TaskCount => Course.Tasks.Count;
        public int Difficulty => Course.Difficulty;
        public int Credits => Course.Credits;
        public string Status => Course.Status.ToString();
        public double Workload => Course.ComputeWorkload();

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class TaskCardViewModel
    {
        public TaskCardViewModel(ParacTask task)
        {
            Task = task;
        }

        public ParacTask Task { get; }
        public string Title => Task.TaskName;
        public string Description => Task.TaskDescription ?? "";
        public int Difficulty => Task.Difficulty;
        public int Credits => Task.Credits;
        public string DeadlineText => Task.Deadline.ToString("dd.MM.yyyy");
    }
}
