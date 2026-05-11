namespace DotNet_Lab01_Core;

public static class CollectionExtensions
{
    public static int GetCourseDuration(this Course course)
    {
        return (course.EndDate - course.StartDate).Days;
    }
}