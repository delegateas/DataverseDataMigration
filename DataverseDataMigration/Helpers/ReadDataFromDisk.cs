using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Export.Helpers
{
    public class ReadDataFromDisk
    {
        public static List<string> ListFiles(string prefix = "Account_")
        {
            var path = $"ExportedData/";
            // Ensure the directory exists
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The directory {path} does not exist.");
            }
            // Get all files in the directory
            return Directory.GetFiles(path, $"{prefix}*.json").ToList();
        }

        public static string Read(string fileName)
        {
            // Check if the file exists
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"The file {fileName} does not exist.");
            }

            // Read the JSON from the file
            return File.ReadAllText(fileName);
        }
    }
}
