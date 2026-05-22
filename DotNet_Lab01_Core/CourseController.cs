namespace DotNet_Lab01_Core;

public class CourseControllerConfig
{
    public int DefaultDifficulty { get; set; }
    public int DefaultCredits { get; set; }
    public int DefaultCourseDurationDays { get; set; }
    public bool AutoStartCourse { get; set; }

    public CourseControllerConfig()
    {
        DefaultDifficulty = 50;
        DefaultCredits = 1;
        DefaultCourseDurationDays = 90;
        AutoStartCourse = false;
    }
}

public class CourseController
{
    private readonly CourseControllerConfig _config;
    private readonly CourseManager _courseManager;
    private readonly TaskManager _taskManager;
    private readonly ResourceManager? _logger;

    public CourseController(CourseManager courseManager, TaskManager taskManager, ResourceManager? logger = null)
    {
        _config = new CourseControllerConfig();
        _courseManager = courseManager;
        _taskManager = taskManager;
        _logger = logger;
        Log("CourseController initialized.");
    }

    private void Log(string message)
    {
        _logger?.WriteLog($"CourseController: {message}");
    }

    public Course CreateCourse(string name)
    {
        Log($"CreateCourse start name={name}");

        Course course = new Course(
            name,
            _config.DefaultCredits,
            DateTime.Now.AddDays(_config.DefaultCourseDurationDays)
        );

        ConfigureCourse(course);
        _courseManager.AddCourse(course);

        Log($"CreateCourse completed id={course.Id} name={name}");
        return course;
    }

    public ParacTask CreateTaskForCourse(
        int courseId,
        string taskName,
        DateTime deadline,
        int difficulty,
        int credits,
        string description)
    {
        Log($"CreateTaskForCourse start courseId={courseId} taskName={taskName}");
        ParacTask task = _taskManager.CreateTaskForCourse(courseId, taskName, deadline, difficulty, credits, description);
        Log($"CreateTaskForCourse completed taskId={task.Id} courseId={courseId}");
        return task;
    }

    public ParacTask CreateTaskForCourse(
        string courseName,
        string taskName,
        DateTime deadline,
        int difficulty,
        int credits,
        string description)
    {
        Log($"CreateTaskForCourse by name start courseName={courseName} taskName={taskName}");
        Course? course = _courseManager.FindByName(courseName);

        if (course == null)
        {
            Log($"CreateTaskForCourse failed: courseName={courseName} not found");
            throw new InvalidOperationException($"Course with Name \"{courseName}\" was not found.");
        }

        ParacTask task = _taskManager.CreateTaskForCourse(course.Id, taskName, deadline, difficulty, credits, description);
        Log($"CreateTaskForCourse by name completed taskId={task.Id} courseId={course.Id}");
        return task;
    }

    public void ConfigureCourse(Course course)
    {
        Log($"ConfigureCourse start courseId={course.Id} name={course.CourseName}");

        if (course.Difficulty == 0)
            course.Difficulty = _config.DefaultDifficulty;

        if (_config.AutoStartCourse)
        {
            course.Start();
            Log($"ConfigureCourse auto-started courseId={course.Id}");
        }

        Log($"ConfigureCourse completed courseId={course.Id} difficulty={course.Difficulty}");
    }

    public void ShowConfig()
    {
        Log("ShowConfig called.");
        Console.WriteLine(
            $"Default difficulty: {_config.DefaultDifficulty}\n" +
            $"Default credits: {_config.DefaultCredits}\n" +
            $"Default duration: {_config.DefaultCourseDurationDays} days\n" +
            $"Auto start course: {(_config.AutoStartCourse ? "Yes" : "No")}"
        );
    }
}
