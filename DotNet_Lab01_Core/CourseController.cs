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

    public CourseController(CourseManager courseManager)
    {
        _config = new CourseControllerConfig();
        _courseManager = courseManager;
    }

    public Course CreateCourse(string name)
    {
        Course course = new Course(
            name,
            _config.DefaultCredits,
            DateTime.Now.AddDays(_config.DefaultCourseDurationDays)
        );
        ConfigureCourse(course);
        _courseManager.AddCourse(course);
        return course;
    }

    public void ConfigureCourse(Course course)
    {
        if (course.Difficulty == 0)
            course.Difficulty = _config.DefaultDifficulty;

        if (_config.AutoStartCourse)
            course.Start();
    }

    public void ShowConfig()
    {
        Console.WriteLine(
            $"Default difficulty: {_config.DefaultDifficulty}\n" +
            $"Default credits: {_config.DefaultCredits}\n" +
            $"Default duration: {_config.DefaultCourseDurationDays} days\n" +
            $"Auto start course: {(_config.AutoStartCourse ? "Yes" : "No")}"
        );
    }
}
