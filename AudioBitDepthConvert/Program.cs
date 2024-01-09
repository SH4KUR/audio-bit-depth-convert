using System.Diagnostics;
using NAudio.Wave;

namespace AudioBitDepthConvert;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Enter the path of the directory: ");
        var directoryPath = Console.ReadLine();

        if (Directory.Exists(directoryPath))
        {
            ConvertFiles(directoryPath);

            Console.WriteLine("Converting completed.");
        }
        else
        {
            Console.WriteLine("Directory not found.");
        }
    }
    
    private static void ConvertFiles(string directoryPath) 
    {
        var audioFiles = Directory.GetFiles(directoryPath, "*.wav", SearchOption.AllDirectories);

        foreach (var filePath in audioFiles)
        {
            ConvertBitDepth(filePath);
        }
    }

    private static void ConvertBitDepth(string filePath)
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
                ExecuteSoxUtility(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }   
        }
    }

    private static void ExecuteSoxUtility(string filePath) 
    {
        var convertedFilePath = CreateConvertedFilePath(filePath);

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

        Console.WriteLine($">>> {convertedFilePath}");
    }

    private static string CreateConvertedFilePath(string filePath) 
    {
        var directoryPath = Path.GetDirectoryName(filePath);
        var fileName = Path.GetFileName(filePath);
        var outputDirectoryPath = directoryPath + "\\converted";

        if (!Directory.Exists(outputDirectoryPath))
        {
            Directory.CreateDirectory(outputDirectoryPath);
        }

        return $"{outputDirectoryPath}\\{fileName}";
    }
}