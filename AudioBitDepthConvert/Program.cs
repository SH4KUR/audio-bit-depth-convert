using System.Diagnostics;
using NAudio.Wave;

namespace AudioBitDepthConvert;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Enter the path of the directory: ");
        var inputPath = Console.ReadLine();

        if (!string.IsNullOrEmpty(inputPath) && Path.Exists(inputPath))
        {
            var targetDirectoryPath = Path.GetDirectoryName(inputPath) ?? throw new NullReferenceException("targetDirectoryPath is null");
            
            ConvertFiles(targetDirectoryPath);

            Console.WriteLine($"Converting in '{targetDirectoryPath}' completed.");
        }
        else
        {
            Console.WriteLine($"Directory '{inputPath}' not found.");
        }
    }
    
    private static void ConvertFiles(string targetDirectoryPath) 
    {
        var audioFiles = Directory.GetFiles(targetDirectoryPath, "*.wav", SearchOption.AllDirectories);

        foreach (var filePath in audioFiles)
        {
            ConvertBitDepth(filePath, targetDirectoryPath);
        }
    }

    private static void ConvertBitDepth(string filePath, string targetDirectoryPath)
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
                ExecuteSoxUtility(filePath, CreateConvertedFilePath(filePath, targetDirectoryPath));
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

    private static string CreateConvertedFilePath(string filePath, string targetDirectoryPath) 
    {
        var fileDirectoryPath = Path.GetDirectoryName(filePath) ?? throw new NullReferenceException($"Directory path for '{filePath}' is null");
        var folderWithConvertedFiles = Path.Combine(targetDirectoryPath, "converted");
        var convertedFileDirectoryPath = fileDirectoryPath.Replace(targetDirectoryPath, folderWithConvertedFiles);

        if (!Directory.Exists(convertedFileDirectoryPath))
        {
            Directory.CreateDirectory(convertedFileDirectoryPath);
        }

        return Path.Combine(convertedFileDirectoryPath, Path.GetFileName(filePath));
    }
}