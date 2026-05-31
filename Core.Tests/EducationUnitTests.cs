using DotNet_Lab01_Core;

namespace DotNet_Lab01_Core.Tests;

public class EducationUnitTests
{
    [Fact]
    public void Difficulty_IgnoresValuesOutsideAllowedRange()
    {
        Course course = new Course("C# Basics", "Intro course", 4, DateTime.Now.AddDays(30));

        course.Difficulty = 60;
        course.Difficulty = 0;
        course.Difficulty = 101;

        Assert.Equal(60, course.Difficulty);
    }

    [Fact]
    public void Progress_IsClampedAndUpdatesStatus()
    {
        Course course = new Course("WPF", "Desktop UI", 3, DateTime.Now.AddDays(20));

        course.UpdateProgress(-10);
        Assert.Equal(0, course.Progress);
        Assert.Equal(UnitStatus.NotStarted, course.Status);

        course.UpdateProgress(50);
        Assert.Equal(50, course.Progress);
        Assert.Equal(UnitStatus.InProgress, course.Status);

        course.UpdateProgress(150);
        Assert.Equal(100, course.Progress);
        Assert.Equal(UnitStatus.Completed, course.Status);
    }

    [Fact]
    public void CourseWorkload_UsesCreditsDifficultyAndUrgency()
    {
        Course course = new Course("Algorithms", "Practice", 6, DateTime.Now.AddDays(30))
        {
            CreatedAt = new DateTime(2026, 1, 1),
            Deadline = new DateTime(2026, 1, 31),
            Difficulty = 72
        };

        double workload = course.ComputeWorkload();

        Assert.Equal(34.26, workload);
    }

    [Fact]
    public void TaskWorkload_GrowsWhenDeadlineIsCloser()
    {
        ParacTask urgentTask = new ParacTask("Urgent", DateTime.Now.AddDays(1), 70, 20, "Close deadline");
        ParacTask laterTask = new ParacTask("Later", DateTime.Now.AddDays(30), 70, 20, "Longer deadline");

        double urgentWorkload = urgentTask.ComputeWorkload();
        double laterWorkload = laterTask.ComputeWorkload();

        Assert.True(urgentWorkload > laterWorkload);
    }

    [Fact]
    public void CourseManager_RemoveCourse_RemovesAttachedTasksFromCourse()
    {
        CourseManager courseManager = new CourseManager();
        TaskManager taskManager = new TaskManager(courseManager);
        Course course = new Course("Databases", "SQL and LINQ", 5, DateTime.Now.AddDays(60));

        courseManager.AddCourse(course);
        ParacTask firstTask = taskManager.CreateTaskForCourse(course.Id, "SQL queries", DateTime.Now.AddDays(10), 40, 2, "Write SQL queries");
        ParacTask secondTask = taskManager.CreateTaskForCourse(course.Id, "LINQ filter", DateTime.Now.AddDays(12), 45, 2, "Filter data with LINQ");

        bool removed = courseManager.RemoveCourse(course.Id);

        Assert.True(removed);
        Assert.Empty(course.Tasks);
        Assert.Null(firstTask.CourseId);
        Assert.Null(secondTask.CourseId);
        Assert.Null(courseManager.FindById(course.Id));
    }

    [Fact]
    public void LinqFiltering_SelectsOnlyCurrentCourses()
    {
        List<Course> courses = new()
        {
            new Course("Active", "Visible", 3, DateTime.Now.AddDays(20)),
            new Course("Done", "Hidden", 3, DateTime.Now.AddDays(20)),
            new Course("Archived", "Hidden", 3, DateTime.Now.AddDays(20))
        };

        courses[1].Status = UnitStatus.Completed;
        courses[2].Status = UnitStatus.Archived;

        List<Course> currentCourses = courses
            .Where(course => course.Status != UnitStatus.Completed && course.Status != UnitStatus.Archived)
            .ToList();

        Assert.Single(currentCourses);
        Assert.Equal("Active", currentCourses[0].CourseName);
    }
}
