using System.Collections;
using System.Linq;

namespace DotNet_Lab01_Core;

public class CourseManager : IEnumerable<Course>
{
    private List<Course> _courses = new();
    private Dictionary<int, Course> _courseDictionary = new();

    public void AddCourse(Course course)
    {
        _courses.Add(course);
        _courseDictionary[course.Id] = course;
    }

    public List<Course> GetCourses()
    {
        return _courses;
    }

    public IEnumerator<Course> GetEnumerator()
    {
        foreach (var course in _courses)
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

}