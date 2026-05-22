using System.Collections;
using System.Linq;

namespace DotNet_Lab01_Core;

public class TaskManager : IEnumerable<ParacTask>
{
    private readonly List<ParacTask> _tasks = new();
    private readonly Dictionary<int, ParacTask> _taskDictionary = new();
    private readonly CourseManager _courseManager;
    private readonly ResourceManager? _logger;
    private bool _suspendAutoSave;

    public TaskManager(CourseManager courseManager, ResourceManager? logger = null)
    {
        _courseManager = courseManager;
        _logger = logger;
        Log("TaskManager initialized.");
    }

    private void Log(string message)
    {
        _logger?.WriteLog($"TaskManager: {message}");
    }

    internal void LoadTasksFromStorage(IEnumerable<ParacTask> tasks)
    {
        Log($"LoadTasksFromStorage start count={tasks.Count()}");
        _suspendAutoSave = true;
        _tasks.Clear();
        _taskDictionary.Clear();

        foreach (ParacTask task in tasks)
        {
            _tasks.Add(task);
            _taskDictionary[task.Id] = task;
        }

        _suspendAutoSave = false;
        Log($"LoadTasksFromStorage completed count={_tasks.Count}");
    }

    private void SaveChanges()
    {
        Log("SaveChanges called.");
        if (!_suspendAutoSave)
            _courseManager.SaveChanges();
    }

    public ParacTask CreateTask(
        string taskName,
        DateTime deadline,
        int difficulty,
        int credits,
        string description)
    {
        Log($"CreateTask start name={taskName} deadline={deadline:O} difficulty={difficulty} credits={credits}");
        ParacTask task = new ParacTask(taskName, deadline, difficulty, credits, description);
        AddTask(task);
        Log($"CreateTask completed id={task.Id} name={task.TaskName}");

        return task;
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
        ParacTask task = CreateTask(taskName, deadline, difficulty, credits, description);

        if (!_courseManager.AddTaskToCourseById(courseId, task))
        {
            Log($"CreateTaskForCourse failed course not found courseId={courseId} taskId={task.Id}");
            RemoveTask(task.Id);
            throw new InvalidOperationException($"Course with ID {courseId} was not found.");
        }

        Log($"CreateTaskForCourse completed courseId={courseId} taskId={task.Id}");
        return task;
    }

    public void AddTask(ParacTask task)
    {
        Log($"AddTask start taskId={task.Id} taskName={task.TaskName}");

        if (_taskDictionary.ContainsKey(task.Id))
        {
            Log($"AddTask skipped duplicate taskId={task.Id}");
            return;
        }

        _tasks.Add(task);
        _taskDictionary[task.Id] = task;
        SaveChanges();
        Log($"AddTask completed taskId={task.Id}");
    }

    public bool RemoveTask(int taskId)
    {
        Log($"RemoveTask start taskId={taskId}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"RemoveTask failed not found taskId={taskId}");
            return false;
        }

        if (task.CourseId.HasValue)
        {
            Course? course = _courseManager.FindById(task.CourseId.Value);
            course?.RemoveTask(taskId);
            Log($"RemoveTask removed task from courseId={task.CourseId.Value}");
        }

        _taskDictionary.Remove(taskId);
        bool removed = _tasks.Remove(task);

        if (removed)
        {
            SaveChanges();
            Log($"RemoveTask completed taskId={taskId}");
        }
        else
        {
            Log($"RemoveTask failed remove returned false taskId={taskId}");
        }

        return removed;
    }

    public ParacTask? FindById(int taskId)
    {
        _taskDictionary.TryGetValue(taskId, out ParacTask? task);
        Log(task == null ? $"FindById not found taskId={taskId}" : $"FindById found taskId={taskId}");
        return task;
    }

    public ParacTask? FindByName(string name)
    {
        ParacTask? task = _taskDictionary.Values.FirstOrDefault(task => task.TaskName == name);
        Log(task == null ? $"FindByName not found name={name}" : $"FindByName found taskId={task.Id} name={name}");
        return task;
    }

    public List<ParacTask> GetTasks()
    {
        Log($"GetTasks returned {_tasks.Count} tasks");
        return _tasks;
    }

    public bool RenameTask(int taskId, string name)
    {
        Log($"RenameTask start taskId={taskId} name={name}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"RenameTask failed not found taskId={taskId}");
            return false;
        }

        task.Rename(name);
        SaveChanges();
        Log($"RenameTask completed taskId={taskId} name={name}");
        return true;
    }

    public bool RenameTask(string oldname, string name)
    {
        Log($"RenameTask by name start oldname={oldname} name={name}");
        ParacTask? task = FindByName(oldname);

        if (task == null)
        {
            Log($"RenameTask by name failed not found oldname={oldname}");
            return false;
        }

        task.Rename(name);
        SaveChanges();
        Log($"RenameTask by name completed taskId={task.Id} name={name}");
        return true;
    }

    public bool ChangeTaskDescription(int taskId, string description)
    {
        Log($"ChangeTaskDescription start taskId={taskId} description={description}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"ChangeTaskDescription failed not found taskId={taskId}");
            return false;
        }

        task.ChangeDescription(description);
        SaveChanges();
        Log($"ChangeTaskDescription completed taskId={taskId}");
        return true;
    }

    public bool ChangeTaskDeadline(int taskId, DateTime deadline)
    {
        Log($"ChangeTaskDeadline start taskId={taskId} deadline={deadline:O}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"ChangeTaskDeadline failed not found taskId={taskId}");
            return false;
        }

        task.ChangeDeadline(deadline);
        SaveChanges();
        Log($"ChangeTaskDeadline completed taskId={taskId} deadline={deadline:O}");
        return true;
    }

    public bool UpdateTaskDifficulty(int taskId, int difficulty)
    {
        Log($"UpdateTaskDifficulty start taskId={taskId} difficulty={difficulty}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"UpdateTaskDifficulty failed not found taskId={taskId}");
            return false;
        }

        task.Difficulty = difficulty;
        SaveChanges();
        Log($"UpdateTaskDifficulty completed taskId={taskId} difficulty={difficulty}");
        return true;
    }

    public bool UpdateTaskCredits(int taskId, int credits)
    {
        Log($"UpdateTaskCredits start taskId={taskId} credits={credits}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"UpdateTaskCredits failed not found taskId={taskId}");
            return false;
        }

        task.Credits = credits;
        SaveChanges();
        Log($"UpdateTaskCredits completed taskId={taskId} credits={credits}");
        return true;
    }

    public bool UpdateTaskProgress(int taskId, int progress)
    {
        Log($"UpdateTaskProgress start taskId={taskId} progress={progress}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"UpdateTaskProgress failed not found taskId={taskId}");
            return false;
        }

        task.UpdateProgress(progress);
        SaveChanges();
        Log($"UpdateTaskProgress completed taskId={taskId} progress={progress}");
        return true;
    }

    public bool StartTask(int taskId)
    {
        Log($"StartTask start taskId={taskId}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"StartTask failed not found taskId={taskId}");
            return false;
        }

        task.Start();
        SaveChanges();
        Log($"StartTask completed taskId={taskId}");
        return true;
    }

    public bool CompleteTask(int taskId)
    {
        Log($"CompleteTask start taskId={taskId}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"CompleteTask failed not found taskId={taskId}");
            return false;
        }

        task.Complete();
        task.IsCompleted = true;
        SaveChanges();
        Log($"CompleteTask completed taskId={taskId}");
        return true;
    }

    public bool ActivateTask(int taskId)
    {
        Log($"ActivateTask start taskId={taskId}");
        ParacTask? task = FindById(taskId);

        if (task == null)
        {
            Log($"ActivateTask failed not found taskId={taskId}");
            return false;
        }

        task.Activate();
        SaveChanges();
        Log($"ActivateTask completed taskId={taskId}");
        return true;
    }

    public bool DeactivateTask(int taskId)
    {
        ParacTask? task = FindById(taskId);

        if (task == null)
            return false;

        task.Deactivate();
        SaveChanges();
        return true;
    }

    public bool AddTaskTag(int taskId, string tag)
    {
        ParacTask? task = FindById(taskId);

        if (task == null)
            return false;

        task.AddTag(tag);
        SaveChanges();
        return true;
    }

    public bool RemoveTaskTag(int taskId, string tag)
    {
        ParacTask? task = FindById(taskId);

        if (task == null)
            return false;

        task.RemoveTag(tag);
        SaveChanges();
        return true;
    }

    public List<ParacTask> GetTasksSortedByDeadline()
    {
        return _tasks
            .OrderBy(task => task.Deadline)
            .ToList();
    }

    public List<ParacTask> GetTasksSortedByDifficulty()
    {
        return _tasks
            .OrderByDescending(task => task.Difficulty)
            .ToList();
    }

    public List<ParacTask> GetTasksByCourseId(int courseId)
    {
        return _tasks
            .Where(task => task.CourseId == courseId)
            .ToList();
    }

    public IEnumerator<ParacTask> GetEnumerator()
    {
        foreach (ParacTask task in _tasks)
        {
            yield return task;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
