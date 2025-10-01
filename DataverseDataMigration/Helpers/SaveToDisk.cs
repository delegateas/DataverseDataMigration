using System.IO;

namespace Export.Helpers
{
    class SaveToDisk
    {
        public static void Save(string json, string fileName)
        {
            if(fileName.StartsWith("ExportedData/"))
            {
                fileName = fileName.Substring("ExportedData/".Length);
            }
            var path = $"ExportedData/";
            var filePath = Path.Combine(path, fileName);
            // Ensure the directory exists
            Directory.CreateDirectory(path);
            // Write the JSON to a file
            File.WriteAllText(filePath, json);
        }
    }
}
