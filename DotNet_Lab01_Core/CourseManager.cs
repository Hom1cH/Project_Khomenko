using System.Collections;
using System.Linq;

namespace DotNet_Lab01_Core;

public class CourseManager : IEnumerable<Course>
{
    private readonly List<Course> _courses = new();
    private readonly Dictionary<int, Course> _courseDictionary = new();
    private readonly ResourceManager? _logger;
    private string? _jsonFilePath;
    private bool _suspendAutoSave;

    public CourseManager(ResourceManager? logger = null)
    {
        _logger = logger;
        Log("CourseManager initialized.");
    }

    private void Log(string message)
    {
        _logger?.WriteLog($"CourseManager: {message}");
    }

    public void EnableJsonAutoSave(string filePath)
    {
        _jsonFilePath = filePath;
        Log($"EnableJsonAutoSave filePath={filePath}");
    }

    internal void SaveChanges()
    {
        if (_suspendAutoSave || string.IsNullOrWhiteSpace(_jsonFilePath))
        {
            Log("SaveChanges skipped because auto-save is disabled or path is not set.");
            return;
        }

        Log($"SaveChanges writing {_courses.Count} courses to {_jsonFilePath}");
        CourseJsonStorage.SaveCourses(_courses, _jsonFilePath);
    }

    internal void LoadCoursesFromStorage(IEnumerable<Course> courses)
    {
        Log("LoadCoursesFromStorage start.");
        _suspendAutoSave = true;
        _courses.Clear();
        _courseDictionary.Clear();

        foreach (Course course in courses)
        {
            _courses.Add(course);
            _courseDictionary[course.Id] = course;
        }

        _suspendAutoSave = false;
        Log($"LoadCoursesFromStorage completed count={_courses.Count}");
    }

    public void AddCourse(Course course)
    {
        Log($"AddCourse start id={course.Id} name={course.CourseName}");

        if (_courseDictionary.ContainsKey(course.Id))
        {
            Log($"AddCourse skipped duplicate id={course.Id}");
            return;
        }

        _courses.Add(course);
        _courseDictionary[course.Id] = course;
        SaveChanges();
        Log($"AddCourse completed id={course.Id}");
    }

    public bool RemoveCourse(int id)
    {
        Log($"RemoveCourse start id={id}");
        Course? course = FindById(id);

        if (course == null)
        {
            Log($"RemoveCourse failed not found id={id}");
            return false;
        }

        course.ClearTasks();
        _courseDictionary.Remove(id);
        bool removed = _courses.Remove(course);

        if (removed)
        {
            SaveChanges();
            Log($"RemoveCourse completed id={id}");
        }
        else
        {
            Log($"RemoveCourse failed remove returned false id={id}");
        }

        return removed;
    }

    public bool AddTaskToCourseById(int courseId, ParacTask task)
    {
        Log($"AddTaskToCourseById start courseId={courseId} taskId={task.Id}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"AddTaskToCourseById failed course not found courseId={courseId}");
            return false;
        }

        course.AddTask(task);
        SaveChanges();
        Log($"AddTaskToCourseById completed courseId={courseId} taskId={task.Id}");
        return true;
    }

    public bool AddTaskToCourseByName(string courseName, ParacTask task)
    {
        Log($"AddTaskToCourseByName start courseName={courseName} taskId={task.Id}");
        Course? course = FindByName(courseName);

        if (course == null)
        {
            Log($"AddTaskToCourseByName failed course not found courseName={courseName}");
            return false;
        }

        course.AddTask(task);
        SaveChanges();
        Log($"AddTaskToCourseByName completed courseName={courseName} taskId={task.Id}");
        return true;
    }

    public List<ParacTask> GetTasksByCourseId(int courseId)
    {
        Log($"GetTasksByCourseId courseId={courseId}");
        Course? course = FindById(courseId);
        return course?.Tasks ?? new List<ParacTask>();
    }

    public bool RenameCourse(int courseId, string name)
    {
        Log($"RenameCourse start courseId={courseId} name={name}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"RenameCourse failed not found courseId={courseId}");
            return false;
        }

        course.Rename(name);
        SaveChanges();
        Log($"RenameCourse completed courseId={courseId} name={name}");
        return true;
    }

    public bool RenameCourse(string oldname, string name)
    {
        Log($"RenameCourse by name start oldname={oldname} name={name}");
        Course? course = FindByName(oldname);

        if (course == null)
        {
            Log($"RenameCourse by name failed not found oldname={oldname}");
            return false;
        }

        course.Rename(name);
        SaveChanges();
        Log($"RenameCourse by name completed oldname={oldname} name={name}");
        return true;
    }

    public bool ChangeCourseDescription(int courseId, string description)
    {
        Log($"ChangeCourseDescription start courseId={courseId} description={description}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"ChangeCourseDescription failed not found courseId={courseId}");
            return false;
        }

        course.ChangeDescription(description);
        SaveChanges();
        Log($"ChangeCourseDescription completed courseId={courseId}");
        return true;
    }

    public bool ChangeCourseDeadline(int courseId, DateTime deadline)
    {
        Log($"ChangeCourseDeadline start courseId={courseId} deadline={deadline:O}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"ChangeCourseDeadline failed not found courseId={courseId}");
            return false;
        }

        course.ChangeDeadline(deadline);
        SaveChanges();
        Log($"ChangeCourseDeadline completed courseId={courseId} deadline={deadline:O}");
        return true;
    }

    public bool UpdateCourseDifficulty(int courseId, int difficulty)
    {
        Log($"UpdateCourseDifficulty start courseId={courseId} difficulty={difficulty}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"UpdateCourseDifficulty failed not found courseId={courseId}");
            return false;
        }

        course.Difficulty = difficulty;
        SaveChanges();
        Log($"UpdateCourseDifficulty completed courseId={courseId} difficulty={difficulty}");
        return true;
    }

    public bool UpdateCourseCredits(int courseId, int credits)
    {
        Log($"UpdateCourseCredits start courseId={courseId} credits={credits}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"UpdateCourseCredits failed not found courseId={courseId}");
            return false;
        }

        course.Credits = credits;
        SaveChanges();
        Log($"UpdateCourseCredits completed courseId={courseId} credits={credits}");
        return true;
    }

    public bool UpdateCourseReceivedCredits(int courseId, int receivedCredits)
    {
        Log($"UpdateCourseReceivedCredits start courseId={courseId} receivedCredits={receivedCredits}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"UpdateCourseReceivedCredits failed not found courseId={courseId}");
            return false;
        }

        course.UpdateReceivedCredits(receivedCredits);
        SaveChanges();
        Log($"UpdateCourseReceivedCredits completed courseId={courseId} receivedCredits={receivedCredits}");
        return true;
    }

    public bool UpdateCourseProgress(int courseId, int progress)
    {
        Log($"UpdateCourseProgress start courseId={courseId} progress={progress}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"UpdateCourseProgress failed not found courseId={courseId}");
            return false;
        }

        course.UpdateProgress(progress);
        SaveChanges();
        Log($"UpdateCourseProgress completed courseId={courseId} progress={progress}");
        return true;
    }

    public bool StartCourse(int courseId)
    {
        Log($"StartCourse start courseId={courseId}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"StartCourse failed not found courseId={courseId}");
            return false;
        }

        course.Start();
        SaveChanges();
        Log($"StartCourse completed courseId={courseId}");
        return true;
    }

    public bool CompleteCourse(int courseId)
    {
        Log($"CompleteCourse start courseId={courseId}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"CompleteCourse failed not found courseId={courseId}");
            return false;
        }

        course.Complete();
        SaveChanges();
        Log($"CompleteCourse completed courseId={courseId}");
        return true;
    }

    public bool ActivateCourse(int courseId)
    {
        Log($"ActivateCourse start courseId={courseId}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"ActivateCourse failed not found courseId={courseId}");
            return false;
        }

        course.Activate();
        SaveChanges();
        Log($"ActivateCourse completed courseId={courseId}");
        return true;
    }

    public bool DeactivateCourse(int courseId)
    {
        Log($"DeactivateCourse start courseId={courseId}");
        Course? course = FindById(courseId);

        if (course == null)
        {
            Log($"DeactivateCourse failed not found courseId={courseId}");
            return false;
        }

        course.Deactivate();
        SaveChanges();
        Log($"DeactivateCourse completed courseId={courseId}");
        return true;
    }

    public bool AddCourseTag(int courseId, string tag)
    {
        Course? course = FindById(courseId);

        if (course == null)
            return false;

        course.AddTag(tag);
        SaveChanges();
        return true;
    }

    public bool RemoveCourseTag(int courseId, string tag)
    {
        Course? course = FindById(courseId);

        if (course == null)
            return false;

        course.RemoveTag(tag);
        SaveChanges();
        return true;
    }

    public void ShowTasksByCourseName(string courseName)
    {
        Course? course = FindByName(courseName);

        if (course == null)
        {
            Console.WriteLine($"Course with Name {courseName} not found.");
            return;
        }

        ShowTasksByCourse(course);
    }

    public void ShowTasksByCourseId(int courseId)
    {
        Course? course = FindById(courseId);

        if (course == null)
        {
            Console.WriteLine($"Course with ID {courseId} not found.");
            return;
        }

        ShowTasksByCourse(course);
    }

    public void ShowTasksByCourse(Course course)
    {
        Console.WriteLine($"\nTasks for course ID {course.Id} ({course.CourseName}):\n");

        foreach (ParacTask task in course.Tasks)
        {
            Console.WriteLine(task);
        }
    }

    public List<Course> GetCourses()
    {
        return _courses;
    }

    public IEnumerator<Course> GetEnumerator()
    {
        foreach (Course course in _courses)
        {
            yield return course;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Course? FindById(int id)
    {
        _courseDictionary.TryGetValue(id, out Course? course);

        return course;
    }

    public Course? FindByName(string name)
    {
        return _courses.FirstOrDefault(course => course.CourseName == name);
    }

    public List<Course> GetUncompletedCourses()
    {
        return _courseDictionary
            .Where(pair => pair.Value.Status != UnitStatus.Completed)
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<Course> GetCompletedCourses()
    {
        return _courseDictionary
            .Where(pair => pair.Value.Status == UnitStatus.Completed)
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<Course> GetCoursesSortedByName()
    {
        return _courses
            .OrderBy(course => course.CourseName)
            .ToList();
    }

    public List<Course> GetCoursesSortedByDeadline()
    {
        return _courses
            .OrderBy(course => course.Deadline)
            .ToList();
    }

    public List<Course> GetCoursesSortedByDifficulty()
    {
        return _courses
            .OrderByDescending(course => course.Difficulty)
            .ToList();
    }

    public List<Course> GetCoursesSortedByTasksCount()
    {
        return _courses
            .OrderByDescending(course => course.Tasks.Count)
            .ToList();
    }
}
