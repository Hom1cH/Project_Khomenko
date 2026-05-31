using DotNet_Lab01_Core;

class Program
{
    static void Main()
    {
        string dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(dataDirectory);

        string filePathJSON = Path.Combine(dataDirectory, "courses.json");
        string filePathXML = Path.Combine(dataDirectory, "courses.xml");
        string logFilePath = Path.Combine(dataDirectory, "logs.log");

        using ResourceManager logger = new ResourceManager(logFilePath);

        CourseManager manager = new CourseManager(logger);
        TaskManager taskManager = new TaskManager(manager, logger);

        CourseJsonStorage.LoadCourses(filePathJSON, manager, taskManager, logger);
        manager.EnableJsonAutoSave(filePathJSON);

        CourseController controller = new CourseController(manager, taskManager, logger);
        
        CourseXmlExporter.ExportCurrentCourses(manager.GetCourses(), filePathXML, logger);



    }
}
