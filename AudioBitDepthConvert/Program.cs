using System.Diagnostics;
using NAudio.Wave;

namespace AudioBitDepthConvert;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Enter the path of the directory: ");
        var directoryPath = Console.ReadLine();

        if (!string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
        {
            ConvertFiles(RemoveDashesFromEndPath(directoryPath));

            Console.WriteLine("Converting completed.");
        }
        else
        {
            Console.WriteLine($"Directory '{directoryPath}' not found.");
        }
    }
    
    private static void ConvertFiles(string directoryPath) 
    {
        var audioFiles = Directory.GetFiles(directoryPath, "*.wav", SearchOption.AllDirectories);

        foreach (var filePath in audioFiles)
        {
            ConvertBitDepth(filePath, directoryPath);
        }
    }

    private static void ConvertBitDepth(string filePath, string parentDirectoryPath)
    {
        using (var reader = new WaveFileReader(filePath))
        {
            if (reader.WaveFormat.BitsPerSample != 32)
            {
                return;
            }

            Console.WriteLine(filePath);

            try
            {
                ExecuteSoxUtility(filePath, CreateConvertedFilePath(filePath, parentDirectoryPath));
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }   
        }
    }

    private static void ExecuteSoxUtility(string filePath, string convertedFilePath) 
    {
        var startInfo = new ProcessStartInfo() 
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,

            FileName = "sox",
            Arguments = $"\"{filePath}\" -b 24 \"{convertedFilePath}\""
        };

        var process = Process.Start(startInfo) ?? throw new NullReferenceException("Process is null");
        process.WaitForExit();

        Console.WriteLine($"> {convertedFilePath}");
    }

    private static string CreateConvertedFilePath(string filePath, string parentDirectoryPath) 
    {
        var fileDirectoryPath = Path.GetDirectoryName(filePath) ?? throw new NullReferenceException($"Directory path for '{filePath}' is null");
        var outputDirectoryPath = fileDirectoryPath.Replace(parentDirectoryPath, $"{parentDirectoryPath}\\converted");

        if (!Directory.Exists(outputDirectoryPath))
        {
            Directory.CreateDirectory(outputDirectoryPath);
        }

        var fileName = Path.GetFileName(filePath);
        return $"{outputDirectoryPath}\\{fileName}";
    }

    private static string RemoveDashesFromEndPath(string directoryPath) 
    {
        return directoryPath.EndsWith("\\") 
            ? directoryPath.Remove(directoryPath.Length - 1)
            : directoryPath;
    }
}