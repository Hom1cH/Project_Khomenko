using System.Linq;
using System.Text.Json;

namespace DotNet_Lab01_Core;

public static class CourseJsonStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static void SaveCourses(IEnumerable<Course> courses, string filePath, ResourceManager? logger = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            logger?.WriteLog("CourseJsonStorage: SaveCourses failed because file path is empty.");
            Console.WriteLine("Cannot save courses: file path is empty.");
            return;
        }

        List<Course> courseList = courses?.ToList() ?? new List<Course>();
        logger?.WriteLog($"CourseJsonStorage: SaveCourses start filePath={filePath} count={courseList.Count}");

        try
        {
            EnsureDirectoryExists(filePath);

            using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(stream, courseList, Options);

            logger?.WriteLog($"CourseJsonStorage: SaveCourses completed filePath={filePath}");
        }
        catch (UnauthorizedAccessException exception)
        {
            logger?.WriteLog($"CourseJsonStorage: save access denied filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot save courses: access denied to '{filePath}'.");
        }
        catch (IOException exception)
        {
            logger?.WriteLog($"CourseJsonStorage: save IO error filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot save courses: file '{filePath}' cannot be written.");
        }
        catch (NotSupportedException exception)
        {
            logger?.WriteLog($"CourseJsonStorage: save invalid path filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot save courses: path '{filePath}' is invalid.");
        }
    }

    public static List<Course> LoadCourses(string filePath, ResourceManager? logger = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            logger?.WriteLog("CourseJsonStorage: LoadCourses failed because file path is empty.");
            Console.WriteLine("Cannot load courses: file path is empty.");
            return new List<Course>();
        }

        logger?.WriteLog($"CourseJsonStorage: LoadCourses start filePath={filePath}");

        if (!File.Exists(filePath))
        {
            logger?.WriteLog($"CourseJsonStorage: LoadCourses file not found filePath={filePath}");
            return new List<Course>();
        }

        try
        {
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Length == 0)
            {
                logger?.WriteLog($"CourseJsonStorage: LoadCourses empty file filePath={filePath}");
                return new List<Course>();
            }

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            List<Course> courses = JsonSerializer.Deserialize<List<Course>>(stream, Options) ?? new List<Course>();
            courses = ValidateCourses(courses, logger);

            logger?.WriteLog($"CourseJsonStorage: LoadCourses completed filePath={filePath} count={courses.Count}");
            return courses;
        }
        catch (JsonException exception)
        {
            logger?.WriteLog($"CourseJsonStorage: invalid JSON filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot load courses: JSON file '{filePath}' is corrupted.");
            return new List<Course>();
        }
        catch (IOException exception)
        {
            logger?.WriteLog($"CourseJsonStorage: file read error filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot load courses: file '{filePath}' cannot be read.");
            return new List<Course>();
        }
        catch (UnauthorizedAccessException exception)
        {
            logger?.WriteLog($"CourseJsonStorage: file read access denied filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot load courses: access denied to '{filePath}'.");
            return new List<Course>();
        }
        catch (NotSupportedException exception)
        {
            logger?.WriteLog($"CourseJsonStorage: invalid load path filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot load courses: path '{filePath}' is invalid.");
            return new List<Course>();
        }
    }

    public static void LoadCourses(string filePath, CourseManager manager, ResourceManager? logger = null)
    {
        List<Course> courses = LoadCourses(filePath, logger);
        manager.LoadCoursesFromStorage(courses);
    }

    public static void LoadCourses(string filePath, CourseManager courseManager, TaskManager taskManager, ResourceManager? logger = null)
    {
        List<Course> courses = LoadCourses(filePath, logger);
        courseManager.LoadCoursesFromStorage(courses);

        List<ParacTask> tasks = new List<ParacTask>();

        foreach (Course course in courseManager)
        {
            foreach (ParacTask task in course.Tasks)
            {
                task.CourseId = course.Id;
                tasks.Add(task);
            }
        }

        taskManager.LoadTasksFromStorage(tasks);
        logger?.WriteLog($"CourseJsonStorage: LoadCourses load completed with {tasks.Count} tasks from json.");
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);
    }

    private static List<Course> ValidateCourses(List<Course> courses, ResourceManager? logger)
    {
        List<Course> validCourses = new List<Course>();
        HashSet<int> courseIds = new HashSet<int>();

        foreach (Course course in courses)
        {
            if (course == null)
            {
                logger?.WriteLog("CourseJsonStorage: skipped null course from JSON.");
                continue;
            }

            if (course.Id <= 0 || string.IsNullOrWhiteSpace(course.CourseName))
            {
                logger?.WriteLog($"CourseJsonStorage: skipped invalid course id={course.Id} title={course.CourseName}");
                continue;
            }

            if (!courseIds.Add(course.Id))
            {
                logger?.WriteLog($"CourseJsonStorage: skipped duplicate course id={course.Id}");
                continue;
            }

            course.Tasks ??= new List<ParacTask>();
            course.Tasks = ValidateTasks(course.Tasks, course.Id, logger);
            validCourses.Add(course);
        }

        return validCourses;
    }

    private static List<ParacTask> ValidateTasks(List<ParacTask> tasks, int courseId, ResourceManager? logger)
    {
        List<ParacTask> validTasks = new List<ParacTask>();
        HashSet<int> taskIds = new HashSet<int>();

        foreach (ParacTask task in tasks)
        {
            if (task == null)
            {
                logger?.WriteLog($"CourseJsonStorage: skipped null task for courseId={courseId}");
                continue;
            }

            if (task.Id <= 0 || string.IsNullOrWhiteSpace(task.TaskName))
            {
                logger?.WriteLog($"CourseJsonStorage: skipped invalid task id={task.Id} title={task.TaskName}");
                continue;
            }

            if (!taskIds.Add(task.Id))
            {
                logger?.WriteLog($"CourseJsonStorage: skipped duplicate task id={task.Id} for courseId={courseId}");
                continue;
            }

            task.CourseId = courseId;
            validTasks.Add(task);
        }

        return validTasks;
    }
}
