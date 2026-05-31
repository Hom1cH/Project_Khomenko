using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using DotNet_Lab01_Core;

namespace WpfApp;

public class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly ResourceManager _logger;
    private readonly CourseManager _courseManager;
    private readonly TaskManager _taskManager;
    private readonly CourseController _courseController;
    private Course? _selectedCourse;
    private SortMode _sortMode = SortMode.Name;
    private string _headerSubtitle = "Courses overview";
    private int _totalCourses;
    private int _totalTasks;
    private int _currentCourses;
    private string _averageProgressText = "0%";
    private string _selectedCourseTitle = "Course tasks";
    private string _selectedCourseSubtitle = "Tasks overview";
    private string _sidebarCourseTitle = "Course title";
    private string _sidebarCourseStatus = "Status";
    private string _sidebarCourseTags = "Tags";
    private string _sidebarCourseProgress = "0%";
    private string _sidebarCourseTasks = "0";
    private string _sidebarCourseCredits = "0";
    private string _sidebarCourseDifficulty = "0";
    private string _sidebarCourseWorkload = "0";
    private string _sidebarCourseCreated = "-";
    private string _sidebarCourseDeadline = "-";
    private string _sidebarCourseDescription = "-";

    public ObservableCollection<CourseCardViewModel> Courses { get; } = new();
    public ObservableCollection<TaskCardViewModel> Tasks { get; } = new();

    public MainViewModel()
    {
        DataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(DataDirectory);

        JsonFilePath = Path.Combine(DataDirectory, "courses.json");
        LogFilePath = Path.Combine(DataDirectory, "logs.log");

        _logger = new ResourceManager(LogFilePath);
        _courseManager = new CourseManager(_logger);
        _taskManager = new TaskManager(_courseManager, _logger);
        _courseController = new CourseController(_courseManager, _taskManager, _logger);

        AddObjectCommand = new RelayCommand(_ => AddObjectRequested?.Invoke());
        DeleteCourseCommand = new RelayCommand(DeleteCourseFromCommand, parameter => parameter is CourseCardViewModel);
        DeleteTaskCommand = new RelayCommand(DeleteTaskFromCommand, parameter => parameter is TaskCardViewModel);
        SaveCommand = new RelayCommand(_ => SaveCoursesFromCommand());
        ImportCommand = new RelayCommand(_ => ImportRequested?.Invoke());
        BackCommand = new RelayCommand(_ => BackToCoursesFromCommand());
        SortCommand = new RelayCommand(SortFromCommand);
        SetCourseNotStartedCommand = new RelayCommand(parameter => UpdateCourseStatusFromCommand(parameter, UnitStatus.NotStarted), parameter => parameter is CourseCardViewModel);
        SetCourseInProgressCommand = new RelayCommand(parameter => UpdateCourseStatusFromCommand(parameter, UnitStatus.InProgress), parameter => parameter is CourseCardViewModel);
        SetCourseCompletedCommand = new RelayCommand(parameter => UpdateCourseStatusFromCommand(parameter, UnitStatus.Completed), parameter => parameter is CourseCardViewModel);
        SetCoursePausedCommand = new RelayCommand(parameter => UpdateCourseStatusFromCommand(parameter, UnitStatus.Paused), parameter => parameter is CourseCardViewModel);
        SetCourseArchivedCommand = new RelayCommand(parameter => UpdateCourseStatusFromCommand(parameter, UnitStatus.Archived), parameter => parameter is CourseCardViewModel);
        SetTaskNotStartedCommand = new RelayCommand(parameter => UpdateTaskStatusFromCommand(parameter, UnitStatus.NotStarted), parameter => parameter is TaskCardViewModel);
        SetTaskInProgressCommand = new RelayCommand(parameter => UpdateTaskStatusFromCommand(parameter, UnitStatus.InProgress), parameter => parameter is TaskCardViewModel);
        SetTaskCompletedCommand = new RelayCommand(parameter => UpdateTaskStatusFromCommand(parameter, UnitStatus.Completed), parameter => parameter is TaskCardViewModel);
        SetTaskPausedCommand = new RelayCommand(parameter => UpdateTaskStatusFromCommand(parameter, UnitStatus.Paused), parameter => parameter is TaskCardViewModel);
        SetTaskArchivedCommand = new RelayCommand(parameter => UpdateTaskStatusFromCommand(parameter, UnitStatus.Archived), parameter => parameter is TaskCardViewModel);
    }

    public ICommand AddObjectCommand { get; }
    public ICommand DeleteCourseCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand SortCommand { get; }
    public ICommand SetCourseNotStartedCommand { get; }
    public ICommand SetCourseInProgressCommand { get; }
    public ICommand SetCourseCompletedCommand { get; }
    public ICommand SetCoursePausedCommand { get; }
    public ICommand SetCourseArchivedCommand { get; }
    public ICommand SetTaskNotStartedCommand { get; }
    public ICommand SetTaskInProgressCommand { get; }
    public ICommand SetTaskCompletedCommand { get; }
    public ICommand SetTaskPausedCommand { get; }
    public ICommand SetTaskArchivedCommand { get; }
    public event Action? AddObjectRequested;
    public event Action? ImportRequested;
    public event Action? CoursesSaved;
    public event Action? BackToCoursesRequested;
    public event Action? CurrentViewChanged;
    public Func<CourseCardViewModel, bool>? ConfirmDeleteCourse { get; set; }
    public Func<TaskCardViewModel, bool>? ConfirmDeleteTask { get; set; }
    public string DataDirectory { get; }
    public string JsonFilePath { get; private set; }
    public string LogFilePath { get; }
    public ResourceManager Logger => _logger;
    public CourseManager CourseManager => _courseManager;
    public TaskManager TaskManager => _taskManager;
    public CourseController CourseController => _courseController;
    public SortMode CurrentSortMode => _sortMode;

    public Course? SelectedCourse
    {
        get => _selectedCourse;
        set
        {
            if (_selectedCourse == value)
                return;

            _selectedCourse = value;
            OnPropertyChanged(nameof(SelectedCourse));
        }
    }

    public string HeaderSubtitle
    {
        get => _headerSubtitle;
        set
        {
            if (_headerSubtitle == value)
                return;

            _headerSubtitle = value;
            OnPropertyChanged(nameof(HeaderSubtitle));
        }
    }

    public int TotalCourses
    {
        get => _totalCourses;
        set
        {
            if (_totalCourses == value)
                return;

            _totalCourses = value;
            OnPropertyChanged(nameof(TotalCourses));
        }
    }

    public int TotalTasks
    {
        get => _totalTasks;
        set
        {
            if (_totalTasks == value)
                return;

            _totalTasks = value;
            OnPropertyChanged(nameof(TotalTasks));
        }
    }

    public int CurrentCourses
    {
        get => _currentCourses;
        set
        {
            if (_currentCourses == value)
                return;

            _currentCourses = value;
            OnPropertyChanged(nameof(CurrentCourses));
        }
    }

    public string AverageProgressText
    {
        get => _averageProgressText;
        set
        {
            if (_averageProgressText == value)
                return;

            _averageProgressText = value;
            OnPropertyChanged(nameof(AverageProgressText));
        }
    }

    public string SelectedCourseTitle { get => _selectedCourseTitle; set => SetField(ref _selectedCourseTitle, value, nameof(SelectedCourseTitle)); }
    public string SelectedCourseSubtitle { get => _selectedCourseSubtitle; set => SetField(ref _selectedCourseSubtitle, value, nameof(SelectedCourseSubtitle)); }
    public string SidebarCourseTitle { get => _sidebarCourseTitle; set => SetField(ref _sidebarCourseTitle, value, nameof(SidebarCourseTitle)); }
    public string SidebarCourseStatus { get => _sidebarCourseStatus; set => SetField(ref _sidebarCourseStatus, value, nameof(SidebarCourseStatus)); }
    public string SidebarCourseTags { get => _sidebarCourseTags; set => SetField(ref _sidebarCourseTags, value, nameof(SidebarCourseTags)); }
    public string SidebarCourseProgress { get => _sidebarCourseProgress; set => SetField(ref _sidebarCourseProgress, value, nameof(SidebarCourseProgress)); }
    public string SidebarCourseTasks { get => _sidebarCourseTasks; set => SetField(ref _sidebarCourseTasks, value, nameof(SidebarCourseTasks)); }
    public string SidebarCourseCredits { get => _sidebarCourseCredits; set => SetField(ref _sidebarCourseCredits, value, nameof(SidebarCourseCredits)); }
    public string SidebarCourseDifficulty { get => _sidebarCourseDifficulty; set => SetField(ref _sidebarCourseDifficulty, value, nameof(SidebarCourseDifficulty)); }
    public string SidebarCourseWorkload { get => _sidebarCourseWorkload; set => SetField(ref _sidebarCourseWorkload, value, nameof(SidebarCourseWorkload)); }
    public string SidebarCourseCreated { get => _sidebarCourseCreated; set => SetField(ref _sidebarCourseCreated, value, nameof(SidebarCourseCreated)); }
    public string SidebarCourseDeadline { get => _sidebarCourseDeadline; set => SetField(ref _sidebarCourseDeadline, value, nameof(SidebarCourseDeadline)); }
    public string SidebarCourseDescription { get => _sidebarCourseDescription; set => SetField(ref _sidebarCourseDescription, value, nameof(SidebarCourseDescription)); }

    public void LoadData()
    {
        CourseJsonStorage.LoadCourses(JsonFilePath, _courseManager, _taskManager, _logger);
        _courseManager.EnableJsonAutoSave(JsonFilePath);
    }

    public void ImportCourses(string filePath)
    {
        CourseJsonStorage.LoadCourses(filePath, _courseManager, _taskManager, _logger);
        JsonFilePath = filePath;
        _courseManager.EnableJsonAutoSave(filePath);
        OnPropertyChanged(nameof(JsonFilePath));
    }

    public void SaveCourses()
    {
        CourseJsonStorage.SaveCourses(_courseManager.GetCourses(), JsonFilePath, _logger);
    }

    public void SetSortMode(SortMode sortMode)
    {
        _sortMode = sortMode;
        OnPropertyChanged(nameof(CurrentSortMode));
    }

    public void ImportCoursesAndRefresh(string filePath)
    {
        ImportCourses(filePath);
        ShowCourses();
        CurrentViewChanged?.Invoke();
    }

    public void ShowCourses()
    {
        List<Course> courses = GetVisibleCourses();

        SetCourses(courses);
        UpdateSummary(courses);
        HeaderSubtitle = "Courses overview";
        SelectedCourse = null;
    }

    public void ShowTasks(Course course)
    {
        List<ParacTask> tasks = GetVisibleTasks(course);

        SelectedCourse = course;
        ShowSelectedCourse(course);
        SetTasks(tasks);
        HeaderSubtitle = "Task details";
    }

    public void RefreshCurrentView()
    {
        if (SelectedCourse != null)
            ShowTasks(SelectedCourse);
        else
            ShowCourses();
    }

    public Course CreateCourse(string title, string description, int credits, int difficulty, DateTime deadline, int progress, IEnumerable<string> tags)
    {
        Course course = _courseController.CreateCourse(title, description, credits, difficulty, deadline);
        _courseManager.UpdateCourseProgress(course.Id, progress);
        _courseManager.UpdateCourseTags(course.Id, tags);
        ShowCourses();
        return course;
    }

    public ParacTask CreateTaskForSelectedCourse(string title, DateTime deadline, int difficulty, int credits, string description, int progress, IEnumerable<string> tags)
    {
        if (SelectedCourse == null)
            throw new InvalidOperationException("No selected course.");

        ParacTask task = _courseController.CreateTaskForCourse(SelectedCourse.Id, title, deadline, difficulty, credits, description);
        _taskManager.UpdateTaskProgress(task.Id, progress);
        _taskManager.UpdateTaskTags(task.Id, tags);
        ShowTasks(SelectedCourse);
        UpdateSummary(GetVisibleCourses());
        return task;
    }

    public void UpdateCourse(Course course, string title, string description, int credits, int difficulty, int progress, DateTime deadline, IEnumerable<string> tags)
    {
        _courseManager.RenameCourse(course.Id, title);
        _courseManager.ChangeCourseDescription(course.Id, description);
        _courseManager.UpdateCourseCredits(course.Id, credits);
        _courseManager.UpdateCourseDifficulty(course.Id, difficulty);
        _courseManager.UpdateCourseProgress(course.Id, progress);
        _courseManager.ChangeCourseDeadline(course.Id, deadline);
        _courseManager.UpdateCourseTags(course.Id, tags);
        ShowCourses();
    }

    public void DeleteCourse(int courseId)
    {
        _courseManager.RemoveCourse(courseId);
        ShowCourses();
    }

    public Course? UpdateTask(ParacTask task, string title, string description, int credits, int difficulty, int progress, DateTime deadline, IEnumerable<string> tags)
    {
        _taskManager.RenameTask(task.Id, title);
        _taskManager.ChangeTaskDescription(task.Id, description);
        _taskManager.UpdateTaskCredits(task.Id, credits);
        _taskManager.UpdateTaskDifficulty(task.Id, difficulty);
        _taskManager.UpdateTaskProgress(task.Id, progress);
        _taskManager.ChangeTaskDeadline(task.Id, deadline);
        _taskManager.UpdateTaskTags(task.Id, tags);

        Course? course = _courseManager.FindById(task.CourseId ?? 0);
        if (course != null)
            ShowTasks(course);
        else
            ShowCourses();

        return course;
    }

    public Course? DeleteTask(int taskId, int courseId)
    {
        _taskManager.RemoveTask(taskId);

        Course? course = _courseManager.FindById(courseId);
        if (course != null)
            ShowTasks(course);
        else
            ShowCourses();

        return course;
    }

    private void DeleteCourseFromCommand(object? parameter)
    {
        if (parameter is not CourseCardViewModel courseViewModel)
            return;

        if (ConfirmDeleteCourse?.Invoke(courseViewModel) == false)
            return;

        DeleteCourse(courseViewModel.Course.Id);
        CurrentViewChanged?.Invoke();
    }

    private void DeleteTaskFromCommand(object? parameter)
    {
        if (parameter is not TaskCardViewModel taskViewModel)
            return;

        if (ConfirmDeleteTask?.Invoke(taskViewModel) == false)
            return;

        DeleteTask(taskViewModel.Task.Id, taskViewModel.Task.CourseId ?? 0);
        CurrentViewChanged?.Invoke();
    }

    private void SaveCoursesFromCommand()
    {
        SaveCourses();
        CoursesSaved?.Invoke();
    }

    private void BackToCoursesFromCommand()
    {
        ShowCourses();
        BackToCoursesRequested?.Invoke();
    }

    private void SortFromCommand(object? parameter)
    {
        if (parameter is not string sortName)
            return;

        if (!Enum.TryParse(sortName, out SortMode sortMode))
            return;

        SetSortMode(sortMode);
        RefreshCurrentView();
        CurrentViewChanged?.Invoke();
    }

    private void UpdateCourseStatusFromCommand(object? parameter, UnitStatus status)
    {
        if (parameter is not CourseCardViewModel courseViewModel)
            return;

        UpdateCourseStatus(courseViewModel.Course.Id, status);
        CurrentViewChanged?.Invoke();
    }

    private void UpdateTaskStatusFromCommand(object? parameter, UnitStatus status)
    {
        if (parameter is not TaskCardViewModel taskViewModel)
            return;

        UpdateTaskStatus(taskViewModel.Task.Id, taskViewModel.Task.CourseId ?? 0, status);
        CurrentViewChanged?.Invoke();
    }

    public void UpdateCourseStatus(int courseId, UnitStatus status)
    {
        _courseManager.UpdateCourseStatus(courseId, status);
        ShowCourses();
    }

    public Course? UpdateTaskStatus(int taskId, int courseId, UnitStatus status)
    {
        _taskManager.UpdateTaskStatus(taskId, status);

        Course? course = _courseManager.FindById(courseId);
        if (course != null)
            ShowTasks(course);
        else
            ShowCourses();

        return course;
    }

    public void SetCourses(IEnumerable<Course> courses)
    {
        Courses.Clear();

        foreach (Course course in courses)
        {
            Courses.Add(new CourseCardViewModel(course));
        }

        OnPropertyChanged(nameof(Courses));
    }

    public void SetTasks(IEnumerable<ParacTask> tasks)
    {
        Tasks.Clear();

        foreach (ParacTask task in tasks)
        {
            Tasks.Add(new TaskCardViewModel(task));
        }

        OnPropertyChanged(nameof(Tasks));
    }

    public void UpdateSummary(IEnumerable<Course> courses)
    {
        List<Course> courseList = courses.ToList();

        TotalCourses = courseList.Count;
        TotalTasks = courseList.Sum(course => course.Tasks.Count);
        CurrentCourses = courseList.Count(course => course.Status != UnitStatus.Completed && course.Status != UnitStatus.Archived);
        AverageProgressText = courseList.Count == 0
            ? "0%"
            : $"{Math.Round(courseList.Average(course => course.Progress))}%";
    }

    public void ShowSelectedCourse(Course course)
    {
        SelectedCourse = course;
        SelectedCourseTitle = course.CourseName;
        SelectedCourseSubtitle = $"{course.Tasks.Count} tasks attached to this course";
        SidebarCourseTitle = course.CourseName;
        SidebarCourseStatus = course.Status.ToString();
        SidebarCourseTags = course.Tags.Count == 0 ? "No tags" : string.Join(", ", course.Tags);
        SidebarCourseProgress = $"{course.Progress}%";
        SidebarCourseTasks = course.Tasks.Count.ToString();
        SidebarCourseCredits = $"{course.ReceivedCredits}/{course.Credits}";
        SidebarCourseDifficulty = course.Difficulty.ToString();
        SidebarCourseWorkload = course.ComputeWorkload().ToString();
        SidebarCourseCreated = course.CreatedAt.ToString("dd.MM.yyyy");
        SidebarCourseDeadline = course.Deadline.ToString("dd.MM.yyyy");
        SidebarCourseDescription = string.IsNullOrWhiteSpace(course.CourseDescription) ? "-" : course.CourseDescription;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField(ref string field, string value, string propertyName)
    {
        if (field == value)
            return;

        field = value;
        OnPropertyChanged(propertyName);
    }

    private List<Course> GetVisibleCourses()
    {
        return SortCourses(_courseManager
            .GetCourses()
            .Where(course => course.Status != UnitStatus.Archived)
            .ToList());
    }

    private List<ParacTask> GetVisibleTasks(Course course)
    {
        return SortTasks(course
            .Tasks
            .Where(task => task.Status != UnitStatus.Archived)
            .ToList());
    }

    private List<Course> SortCourses(List<Course> courses)
    {
        IOrderedEnumerable<Course> sorted = _sortMode switch
        {
            SortMode.Status => courses.OrderBy(course => course.Status),
            SortMode.Progress => courses.OrderByDescending(course => course.Progress),
            SortMode.Tags => courses.OrderBy(course => GetTagsSortText(course.Tags)),
            SortMode.Deadline => courses.OrderBy(course => course.Deadline),
            SortMode.Created => courses.OrderBy(course => course.CreatedAt),
            SortMode.Credits => courses.OrderByDescending(course => course.Credits),
            SortMode.Workload => courses.OrderByDescending(course => course.ComputeWorkload()),
            SortMode.Difficulty => courses.OrderByDescending(course => course.Difficulty),
            _ => courses.OrderBy(course => course.CourseName)
        };

        return sorted
            .ThenBy(course => course.CourseName)
            .ToList();
    }

    private List<ParacTask> SortTasks(List<ParacTask> tasks)
    {
        IOrderedEnumerable<ParacTask> sorted = _sortMode switch
        {
            SortMode.Status => tasks.OrderBy(task => task.Status),
            SortMode.Progress => tasks.OrderByDescending(task => task.Progress),
            SortMode.Tags => tasks.OrderBy(task => GetTagsSortText(task.Tags)),
            SortMode.Deadline => tasks.OrderBy(task => task.Deadline),
            SortMode.Created => tasks.OrderBy(task => task.CreatedAt),
            SortMode.Credits => tasks.OrderByDescending(task => task.Credits),
            SortMode.Workload => tasks.OrderByDescending(task => task.ComputeWorkload()),
            SortMode.Difficulty => tasks.OrderByDescending(task => task.Difficulty),
            _ => tasks.OrderBy(task => task.TaskName)
        };

        return sorted
            .ThenBy(task => task.TaskName)
            .ToList();
    }

    private static string GetTagsSortText(IEnumerable<string> tags)
    {
        return string.Join(",",
            tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .OrderBy(tag => tag));
    }

    public void Dispose()
    {
        _logger.Dispose();
    }
}

public enum SortMode
{
    Status,
    Progress,
    Name,
    Tags,
    Deadline,
    Created,
    Credits,
    Workload,
    Difficulty
}
