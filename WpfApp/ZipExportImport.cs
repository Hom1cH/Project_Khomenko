using System.IO;
using System.IO.Compression;

namespace WpfApp;

public static class ZipExportImport
{
    public static void Export(string dataDirectory, string exportDirectory, string zipPath)
    {
        if (File.Exists(zipPath))
            File.Delete(zipPath);

        using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);

        string jsonPath = Path.Combine(exportDirectory, "courses.json");
        if (File.Exists(jsonPath))
            zip.CreateEntryFromFile(jsonPath, "courses.json");

        string xmlPath = Path.Combine(exportDirectory, "courses.xml");
        if (File.Exists(xmlPath))
            zip.CreateEntryFromFile(xmlPath, "courses.xml");

        AddFolder(zip, dataDirectory, "task_images");
        AddFolder(zip, dataDirectory, "task_files");
    }

    public static void Import(string zipPath, string dataDirectory, string exportDirectory)
    {
        using var zip = ZipFile.OpenRead(zipPath);

        foreach (var entry in zip.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name)) continue;

            string destDirectory = entry.FullName.StartsWith("task_")
                ? dataDirectory
                : exportDirectory;

            string destPath = Path.Combine(destDirectory, entry.FullName);

            string? destDir = Path.GetDirectoryName(destPath);
            if (destDir != null)
                Directory.CreateDirectory(destDir);

            entry.ExtractToFile(destPath, overwrite: true);
        }
    }

    private static void AddFolder(ZipArchive zip, string dataDirectory, string folderName)
    {
        string folderPath = Path.Combine(dataDirectory, folderName);

        if (!Directory.Exists(folderPath)) return;

        foreach (string filePath in Directory.GetFiles(folderPath))
        {
            string entryName = $"{folderName}/{Path.GetFileName(filePath)}";
            zip.CreateEntryFromFile(filePath, entryName);
        }
    }
}