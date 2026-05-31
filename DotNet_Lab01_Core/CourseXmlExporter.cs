using System.Xml.Linq;

namespace DotNet_Lab01_Core;

public static class CourseXmlExporter
{
    public static void ExportCurrentCourses(IEnumerable<Course> courses, string filePath, ResourceManager? logger = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            logger?.WriteLog("CourseXmlExporter: ExportCurrentCourses failed because file path is empty.");
            Console.WriteLine("Cannot export courses to XML: file path is empty.");
            return;
        }

        logger?.WriteLog($"CourseXmlExporter: ExportCurrentCourses start filePath={filePath}");

        try
        {
            List<Course> courseList = courses?.ToList() ?? new List<Course>();

            XDocument document = new XDocument(
                new XElement("Courses",
                    courseList
                        .Where(course => course.Status != UnitStatus.Completed && course.Status != UnitStatus.Archived)
                        .Select(course =>
                            new XElement("Course",
                                new XAttribute("Id", course.Id),
                                new XElement("Title", course.CourseName),
                                new XElement("Description", course.CourseDescription),
                                new XElement("Credits", course.Credits),
                                new XElement("Difficulty", course.Difficulty),
                                new XElement("Progress", course.Progress),
                                new XElement("Status", course.Status),
                                new XElement("Deadline", course.Deadline),
                                new XElement("Workload", course.ComputeWorkload())
                            )
                        )
                )
            );

            string? directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            document.Save(stream);

            logger?.WriteLog($"CourseXmlExporter: ExportCurrentCourses completed filePath={filePath}");
        }
        catch (UnauthorizedAccessException exception)
        {
            logger?.WriteLog($"CourseXmlExporter: access denied filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot export courses to XML: access denied to '{filePath}'.");
        }
        catch (IOException exception)
        {
            logger?.WriteLog($"CourseXmlExporter: IO error filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot export courses to XML: file '{filePath}' cannot be written.");
        }
        catch (NotSupportedException exception)
        {
            logger?.WriteLog($"CourseXmlExporter: invalid path filePath={filePath}. Error: {exception.Message}");
            Console.WriteLine($"Cannot export courses to XML: path '{filePath}' is invalid.");
        }
    }
}
